using System;
using System.Diagnostics;
using System.IO;

class HelloWorld
{
    static void Main()
    {
        int numRepeat = 100; // Number of repetitions to average the time
        int max = 1000;      // Maximum number of drones
        int min = 100;       // Minimum number of drones
        int stepsize = 100;  // Step size for increasing the number of drones
        int numsteps = (max - min) / stepsize;

        string csvFilePath = "timing_results.csv";
        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            writer.WriteLine("Number of Drones,Average (ms),Max (ms),Min (ms),BubbleSort (ms),InsertionSort (ms)");

            float[] timeAverage = new float[numsteps];
            float[] timeMax = new float[numsteps];
            float[] timeMin = new float[numsteps];
            float[] timeBubbleSort = new float[numsteps];
            float[] timeInsertionSort = new float[numsteps];

            for (int i = 0; i < numsteps; i++)
            {
                int numdrones = i * stepsize + min;
                Console.WriteLine("Current number of drones = " + numdrones);

                Flock flock = new Flock(numdrones);
                flock.Init((int)(0.9 * numdrones)); 

                var watch = Stopwatch.StartNew();
                for (int rep = 0; rep < numRepeat; rep++)
                {
                    flock.average();
                }
                watch.Stop();
                timeAverage[i] = watch.ElapsedMilliseconds / (float)numRepeat;
                Console.WriteLine($"Average time for 'average()' with {numdrones} drones: {timeAverage[i]} ms");

                watch.Restart();
                for (int rep = 0; rep < numRepeat; rep++)
                {
                    flock.max();
                }
                watch.Stop();
                timeMax[i] = watch.ElapsedMilliseconds / (float)numRepeat;
                Console.WriteLine($"Average time for 'max()' with {numdrones} drones: {timeMax[i]} ms");

                watch.Restart();
                for (int rep = 0; rep < numRepeat; rep++)
                {
                    flock.min();
                }
                watch.Stop();
                timeMin[i] = watch.ElapsedMilliseconds / (float)numRepeat;
                Console.WriteLine($"Average time for 'min()' with {numdrones} drones: {timeMin[i]} ms");

                watch.Restart();
                for (int rep = 0; rep < numRepeat; rep++)
                {
                    flock.bubblesort();
                }
                watch.Stop();
                timeBubbleSort[i] = watch.ElapsedMilliseconds / (float)numRepeat;
                Console.WriteLine($"Average time for 'bubblesort()' with {numdrones} drones: {timeBubbleSort[i]} ms");

                watch.Restart();
                for (int rep = 0; rep < numRepeat; rep++)
                {
                    flock.insertionsort();
                }
                watch.Stop();
                timeInsertionSort[i] = watch.ElapsedMilliseconds / (float)numRepeat;
                Console.WriteLine($"Average time for 'insertionsort()' with {numdrones} drones: {timeInsertionSort[i]} ms");

                writer.WriteLine($"{numdrones},{timeAverage[i]},{timeMax[i]},{timeMin[i]},{timeBubbleSort[i]},{timeInsertionSort[i]}");
            }
        }

        Console.WriteLine("Timing results saved to CSV.");
    }
}