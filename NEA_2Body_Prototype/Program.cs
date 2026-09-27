using NEA_2Body_Prototype;
using System;
using System.Threading;

namespace NEA_2Body_Prototype
{
    internal class Program
    {
        static void Main()
        {
            double simTime = 0;
            Simulation simulation = new Simulation();

            Console.WriteLine($"------------------------------------------------------------\n");

            while (true)
            {
                simulation.update();
                simTime += 60;

                Console.WriteLine($"Simulation time : {simTime}s\n");

                foreach (Body body in simulation.bodies)
                {
                    Console.WriteLine($"Body : {body.Name}\n(X : {body.CurrentXPos}, Y : {body.CurrentYPos})\nAvg Velocity : {body.AverageVel} m/s^-2\n");
                }

                Console.WriteLine($"------------------------------------------------------------\n");

                Thread.Sleep(1000);

            }
        }
    }
}