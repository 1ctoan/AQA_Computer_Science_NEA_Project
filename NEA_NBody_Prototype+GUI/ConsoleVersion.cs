using NEA_2Body_Prototype;
using System;
using System.Threading;

namespace NEA_2Body_Prototype
{
    internal class ConsoleVersion // class to run the application and coordinate the main loop. console app
    {
        public static void ConsoleLoop()
        {
            double simTime = 0;
            Simulation simulation = new Simulation();

            Console.WriteLine($"------------------------------------------------------------\n");
            try
            {
                // try to implement a clean way of catching keyboard interruptions to go back to menu
                
                while (true) // main sim loop
                {
                    simulation.update();
                    simTime += simulation.dTime;

                    Console.WriteLine($"Simulation time : {simTime}s\n");

                    foreach (Body body in simulation.bodies)
                    {
                        Console.WriteLine($"Body : {body.Name}\n(X : {body.CurrentXPos}, Y : {body.CurrentYPos})\nAvg Velocity : {body.Speed} m/s^-2\n");
                    }

                    Console.WriteLine($"------------------------------------------------------------\n");

                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
        
            }
        }
    }
}