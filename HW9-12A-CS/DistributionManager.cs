﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace MyHomework
{
    public class RandomPoint
    {
        public int X { get; set; }
        public double Y { get; set; }

        public RandomPoint()
        {
            X = 0;
            Y = 0;
        }

        public RandomPoint(int x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class RandomPath
    {
        public List<RandomPoint> Points { get; set; }

        public RandomPath()
        {
            Points = new List<RandomPoint>();
        }
    }

    public class Distribution
    {
        #region MEMBERS

        public List<RandomPath> Paths { get; set; }

        private Random R;

        private int noPoints { get; set; }
        private int noPaths { get; set; }
        private double sigma = 0;
        private double variance = 1;
        private double mu = 0;

        #endregion

        #region CONSTRUCTOR

        public Distribution(int nbPoints, int nbPaths, double sigma = 0.03)
        {
            this.sigma = sigma;

            noPoints = nbPoints;
            noPaths = nbPaths;

            this.Paths = new List<RandomPath>();
            R = new Random();
        }

        #endregion

        #region PUBLIC

        public List<RandomPath> GenerateDistribution()
        {
            double lastValue = 0;
            double randomVal = 0;
            double randomStep = 0;
            double y;

            List<RandomPath> paths = new List<RandomPath>();

            for (int i = 0; i < noPaths; i++)
            {
                var path = new RandomPath();

                lastValue = 0;
                for (int x = 1; x <= noPoints; x++)
                {
                    randomVal = GetRandomNormalVariable();
                    randomStep = (float)(sigma * Math.Sqrt(1d / noPoints) * randomVal);
                    y = lastValue + randomStep;
                    lastValue = y;

                    RandomPoint p = new RandomPoint() { X = x, Y = y };
                    path.Points.Add(p);
                }

                paths.Add(path);
            }

            return paths;
        }

        #endregion

        #region PRIVATE

        private double GetRandomNormalVariable()
        {
            double u1;
            double u2;
            double rand_normal;

            // //< Method 1 >
            //double rSquared;

            //do
            //{
            //    u1 = 2.0 * R.NextDouble() - 1.0;
            //    u2 = 2.0 * R.NextDouble() - 1.0;
            //    rSquared = (u1 * u1) + (u2 * u2);
            //}
            //while (rSquared >= 1.0);
            //// </Method 1>

            ////polar tranformation 
            //double p = Math.Sqrt(-2.0 * Math.Log(rSquared) / rSquared);
            //rand_normal = u1 * p; //* sigma + mu;

            // <Method 2>
            u1 = R.NextDouble();
            u2 = R.NextDouble();
            var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            rand_normal = rand_std_normal * variance + mu;
            // </Method 2>

            // result
            return rand_normal;
        }

        #endregion
    }
}
