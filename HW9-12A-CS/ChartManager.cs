using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyHomework
{
    public class ChartManager
    {
        #region Members

        private ggPictureBox ggPictBox;
        private Bitmap bmp;
        private Rectangle viewPort;
        private Graphics G;

        private int nbPoints;         // number of points for each path
        private int tPoint;           // point in time

        private Random R = new Random();
        private Distribution D;

        private Pen blackPen = new Pen(Color.Black);
        private Pen whitePen = new Pen(Color.White);

        #endregion

        #region Constructor

        public ChartManager(Distribution rn, ggPictureBox pictureBox, int T)
        {
            ggPictBox = pictureBox;
            bmp = new Bitmap(ggPictBox.Width, ggPictBox.Height);
            G = Graphics.FromImage(bmp);

            D = rn;

            nbPoints = D.Paths[0].Points.Count;
            tPoint = T;
        }

        #endregion
        
        #region Public

        public void DrawChart(int nbClusters = 0)
        {
            var box = new Rectangle(0, 0, ggPictBox.Width, ggPictBox.Height);
            G.FillRectangle(Brushes.White, box);
            viewPort = new Rectangle(0, 0, (ggPictBox.Width * 3 / 4) - 1, ggPictBox.Height - 1);
            G.FillRectangle(Brushes.Black, viewPort);

            double minX = 0;
            double maxX = nbPoints;
            double minY = -0.1;
            double maxY = 0.1;

            double rangeX = maxX - minX;
            double rangeY = maxY - minY;

            DrawPaths(minX, minY, rangeX, rangeY);

            int noCluster = nbClusters == 0 ? 10 : nbClusters;
            DrawHistogram(noCluster, tPoint, minX, minY, rangeX, rangeY);
            DrawHistogram(noCluster, nbPoints, minX, minY, rangeX, rangeY);

            ggPictBox.Image = bmp;
        }

        #endregion

        #region Private

        private void DrawPaths(double startX, double startY, double rangeX, double rangeY)
        {
            PointF origin = AdjustPoint(new RandomPoint(0, 0), startX, startY, rangeX, rangeY);

            // Points Adjustment and drawing
            for (int i = 0; i < D.Paths.Count(); i++)
            {
                var path = D.Paths[i];

                SolidBrush randomBrush = new SolidBrush(Color.FromArgb(R.Next(255), R.Next(255), R.Next(255)));
                Pen randomPen = new Pen(randomBrush, 1.0f);

                var adjustedPoints = GetAdjustedPoints(path.Points, startX, startY, rangeX, rangeY);
                for (int j = 0; j < adjustedPoints.Count - 1; j++)
                {
                    if (j == 0)
                        G.DrawLine(randomPen, (float)origin.X, (float)origin.Y, (float)adjustedPoints[j + 1].X, (float)adjustedPoints[j + 1].Y);
                    else
                        G.DrawLine(randomPen, (float)adjustedPoints[j].X, (float)adjustedPoints[j].Y, (float)adjustedPoints[j + 1].X, (float)adjustedPoints[j + 1].Y);
                }
            }

            //foreach (var path in D.Paths)
            //{
            //    PointF origin = AdjustPoint(new RandomPoint(0, 0), startX, startY, rangeX, rangeY);

            //    SolidBrush randomBrush = new SolidBrush(Color.FromArgb(R.Next(255), R.Next(255), R.Next(255)));
            //    Pen randomPen = new Pen(randomBrush, 1.0f);

            //    for (int i = 1; i < path.Points.Count; i++)
            //    {
            //        PointF lastPoint;
            //        PointF currPoint = AdjustPoint(new RandomPoint(path.Points[i].X, path.Points[i].Y), startX, startY, rangeX, rangeY);
            //        if (i == 1)
            //            lastPoint = origin;
            //        else
            //            lastPoint = AdjustPoint(new RandomPoint(path.Points[i - 1].X, path.Points[i - 1].Y), startX, startY, rangeX, rangeY);

            //        G.DrawLine(randomPen, currPoint.X, currPoint.Y, lastPoint.X, lastPoint.Y);
            //    }
            //}

            var font = new Font("Calibri", 10.0f);
            PointF max_x = AdjustPoint(new RandomPoint(0, 0), startX, startY, rangeX, rangeY);
            G.DrawString(tPoint.ToString(), font, Brushes.White, new PointF(origin.X, origin.Y + 10));
        }

        private void DrawHistogram(int nbClusters, int T, double startX, double startY, double rangeX, double rangeY)
        {
            int x = (int)AdjustX(T, startX, rangeX);

            int w = 0;
            int y;
            int h;
            int maxOccurs = 0;

            // Find min and max value in T
            var tPoints = new List<double>();
            foreach (var path in D.Paths)
            {
                var tPoint = path.Points.Where(f => f.X == T).FirstOrDefault();
                if (tPoint != null)
                    tPoints.Add(tPoint.Y);
            }
            tPoints.Sort();
            var mint = tPoints.First();
            var maxt = tPoints.Last();
            var rng = maxt - mint > 0 ? maxt - mint : maxt;

            // Adjusts coordinates to viewport
            var y1 = (int)AdjustY(mint, startY, rangeY);
            var y2 = (int)AdjustY(maxt, startY, rangeY);

            // Creates clusters for histogram 
            var clusters = new List<double>();
            //nbClusters = (int)((y1 - y2) * 0.1);
            double clusterSize = rng / nbClusters;
            for (int i = 0; i < nbClusters; i++)
            {
                // valori positivi
                var min = mint;
                var max = min + clusterSize;
                var occurs = tPoints.Where(d => d >= min && d < max).Count();
                clusters.Add(occurs);
                mint = max;
            }

            // Calculates height of bars
            h = (y1 - y2) / clusters.Count;

            // Draws bars
            y = (int)y2;
            for (int i = 0; i < clusters.Count; i++)
            {
                w = (int)clusters[i];
                //w = (int)(w * (1d/viewPort.Width) * 100) + 100;

                if (w > maxOccurs)
                    maxOccurs = w;

                if (i == clusters.Count - 1)
                {
                    h = (int)(y1 - y);
                }

                Rectangle rectangle = new Rectangle(x, y - 1, w + 1, h);
                G.DrawRectangle(Pens.Black, rectangle);

                rectangle = new Rectangle(x, y, w + 1, h - 1);
                G.FillRectangle(Brushes.Gold, rectangle);

                G.DrawString(clusters[i].ToString(), new Font(FontFamily.GenericSansSerif,6, FontStyle.Bold), Brushes.Black, new Point(x + 5, y));
                y = y + h;
            }

            // Draws frames
            w = maxOccurs;

            //var frame = new Rectangle(x, y2, maxOccurs, y1 - y2);
            //G.DrawRectangle(Pens.Gold, frame);

            var frame = new Rectangle(x, 0, w + 1, ggPictBox.Height - 2);
            var pen = T == nbPoints ? Pens.Red : Pens.Blue;
            G.DrawRectangle(pen, frame);
        }

        private List<PointF> GetAdjustedPoints(List<RandomPoint> points, double startX, double startY, double rangeX, double rangeY)
        {
            // Adjusts all points to viewport area
            List<PointF> adjustedPoints = new List<PointF>();

            foreach (RandomPoint point in points)
            {
                var adjPoint = AdjustPoint(point, startX, startY, rangeX, rangeY);
                adjustedPoints.Add(adjPoint);
            }

            return adjustedPoints;
        }

        private PointF AdjustPoint(RandomPoint point, double startX, double startY, double rangeX, double rangeY)
        {
            // Adjusts the point to viewport area
            PointF adjustedPoint = new PointF();

            var X = AdjustX(point.X, startX, rangeX);
            var Y = AdjustY(point.Y, startY, rangeY);
            adjustedPoint = new PointF((float)X, (float)Y);

            return adjustedPoint;
        }

        private float AdjustX(double x, double startX, double rangeX)
        {
            return (float)(viewPort.Left + viewPort.Width * ((x - startX) / rangeX));
        }

        private float AdjustY(double y, double startY, double rangeY)
        {
            return (float)(viewPort.Top + viewPort.Height - (viewPort.Height * ((y - startY) / rangeY)));
        }

        #endregion
    }
}
