using System;
using System.Diagnostics;

namespace ShortestDistancePerformanceTest
{
    class MainClass
    {
        private Random random = new Random();

        /// <summary>
        /// Constructor
        /// </summary>
        public MainClass ()
        {
            //Generate test data
            int testCount = 1000;
            Result[] results = new Result[testCount];
            for (int i = 0; i < testCount; i++)
            {
                results[i] = runTest(new Vector3(0, 0, 0), 1000, -1000000, 1000000);
            }

            double averageStandard = 0;
            double averageRevised = 0;

            //Format and output
            foreach (Result r in results)
            {
                averageStandard += r.standardTime;
                averageRevised += r.revisedTime;
            }
            averageRevised /= 100;
            averageStandard /= 100;

            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("Average Standard Time : " + averageStandard);
            Console.WriteLine("Average Revised Time  : " + averageRevised);
            Console.WriteLine("Improvement Ratio: " + (averageStandard / averageRevised));

            //Pause for reading output
            Console.ReadLine();
        }

        /// <summary>
        /// Primary method for running a timed calculation check. Takes parameters for test and conducts test with return of results. 
        /// </summary>
        /// <param name="sourcePoint">Source Vector3 to find closest point to</param>
        /// <param name="arraySize">Number of randomized points to generate for test</param>
        /// <param name="rangeMin">Minimum value for randomized point</param>
        /// <param name="rangeMax">Max value for randomized point</param>
        /// <returns>Result struct holding outcome of single test</returns>
        private Result runTest (Vector3 sourcePoint, int arraySize, double rangeMin, double rangeMax)
        {
            //Build an array of 1000 random Vector3's
            Vector3[] randomizedPointsArray = new Vector3[arraySize];
            randomizedPointsArray = generateRandomPosition(randomizedPointsArray, rangeMin, rangeMax);
            Vector3 source = sourcePoint;

            Vector3 closestVectorbyStandard;
            Vector3 closestVectorbyRevised;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            closestVectorbyStandard = getClosestPointFromArray_Brute(source, randomizedPointsArray);

            sw.Stop();
            double standardTime = sw.Elapsed.TotalMilliseconds;

            sw = new Stopwatch();
            sw.Start();

            closestVectorbyRevised = getClosestPointFromArray_Fast(source, randomizedPointsArray);

            sw.Stop();
            double revisedTime = sw.Elapsed.TotalMilliseconds;

            return new Result(standardTime, revisedTime);
        }

        /// <summary>
        /// Hopefully faster method for accomplishing same goal
        /// </summary>
        /// <param name="source">Constant point to check against</param>
        /// <param name="points">Array of points to check against</param>
        /// <returns>Closest Point to source</returns>
        private Vector3 getClosestPointFromArray_Fast (Vector3 source, Vector3[] points)
        {
            double sum = Double.MaxValue;
            double temp;

            Vector3 bestVector = new Vector3(0,0,0);
            double bestDistance = Double.MaxValue;


            //For every other Vector3 - Find smallest vector sum
            for (int i = 0; i < points.Length; i++)
            {
                temp = Math.Abs(points[i].x) + Math.Abs(points[i].y) + Math.Abs(points[i].z);
                if (temp < sum)
                {
                    sum = temp;
                }
            }


            //Take the new check volume and find closest Vector3
            for (int i = 0; i < points.Length; i++)
            {
                if (Math.Abs(points[i].x) <= sum && Math.Abs(points[i].y) <= sum && Math.Abs(points[i].z) <= sum)
                {
                    temp = getRoughPytharusDistance(source, points[i]);
                    if (temp < bestDistance)
                    {
                        bestVector = points[i];
                        bestDistance = temp;
                    }
                }
            }

            return bestVector;
        }

        /// <summary>
        /// Given a source point and an array of vectors, the method returns the Vector3 closest to the source
        /// </summary>
        /// <param name="source">Constant Vector3 to be checked against</param>
        /// <param name="points">Array of Vector3 to check against for distance</param>
        /// <returns>Closest Vector3 to source</returns>
        private Vector3 getClosestPointFromArray_Brute (Vector3 source, Vector3[] points)
        {
            Vector3 bestVector = new Vector3(Double.MaxValue, Double.MaxValue, Double.MaxValue);
            double bestDistance = Double.MaxValue;

            for (int i = 0; i < points.Length; i++)
            {
                double current = getPythagorusDistance(source, points[i]);
                if (current < bestDistance)
                {
                    bestVector = points[i]; bestDistance = current;
                }
            }

            return bestVector;
        }

        /// <summary>
        /// Calculate Distance between two vectors in 3-space using Pythagoras' Theorum
        /// </summary>
        /// <param name="start">Starting Vector</param>
        /// <param name="end">Ending Vector</param>
        /// <returns>Distance between two vectors</returns>
        private double getPythagorusDistance (Vector3 start, Vector3 end)
        {
            return Math.Sqrt( Math.Pow((end.x - start.x), 2) + Math.Pow((end.y - start.y), 2) + Math.Pow((end.z - start.z), 2) );
        }

        /// <summary>
        /// Calculates a rough distance between two vector3's
        /// </summary>
        /// <param name="start">Starting Vector3</param>
        /// <param name="end">Ending Vector3</param>
        /// <returns>Distance between the start and end points</returns>
        private double getRoughPytharusDistance (Vector3 start, Vector3 end)
        {
            return Math.Pow((end.x - start.x), 2) + Math.Pow((end.y - start.y), 2) + Math.Pow((end.z - start.z), 2);
        }

   
        /// <summary>
        /// Adds randomized Vector3's to provided array within the min and max range provided
        /// </summary>
        /// <param name="input">Array to add randomized Vector3's to</param>
        /// <param name="min">Min axis value</param>
        /// <param name="max">Max axis value</param>
        /// <returns>Array with randomized Vector3 entries at every index</returns>
        private Vector3[] generateRandomPosition (Vector3[] input, double min, double max)
        {
            for (int i = 0; i < input.Length; i++)
            {
                
                input[i] = new Vector3(getRandomDouble(min, max), getRandomDouble(min, max), getRandomDouble(min, max));
            }

            return input;
        }

        /// <summary>
        /// Returns a random double between a max and min value
        /// </summary>
        /// <param name="min">Min Value</param>
        /// <param name="max">Max Value</param>
        /// <returns>Random Double</returns>
        private double getRandomDouble (double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Static Start for run
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            MainClass m = new MainClass();
        }
    }

    /// <summary>
    /// Simple Vector for 3-Space
    /// </summary>
    public struct Vector3
    {
        public double x, y, z;

        public Vector3 (double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return "(" + x.ToString() + "/" + y.ToString() + "/" + z.ToString() + ")";
        }
    }

    /// <summary>
    /// Container to hold the outcome of each test conducted
    /// </summary>
    public struct Result
    {
        public double standardTime;
        public double revisedTime;

        /// <summary>
        /// Constructor for building container.
        /// </summary>
        /// <param name="standardTime">Time taken this cycle using standard Pythagorean distance</param>
        /// <param name="revisedTime">Time taken this cycle using the revised pythagorean distance</param>
        public Result (double standardTime, double revisedTime)
        {
            this.standardTime = standardTime;
            this.revisedTime = revisedTime;
        }

        /// <summary>
        /// ToString Override for printing this struct
        /// </summary>
        /// <returns>String representation of String</returns>
        public override String ToString ()
        {
            return ("(" + standardTime.ToString() + " / " + revisedTime.ToString() + ")");
        }
    }
}
