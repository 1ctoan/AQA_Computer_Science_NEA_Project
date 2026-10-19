namespace NEA_NBody_Prototype_3D_ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double simTime = 0;
            Simulation simulation = new Simulation();

            Console.WriteLine($"------------------------------------------------------------\n");

            while (true) // main sim loop
            {
                simulation.update();
                simTime += simulation.dTime;

                Console.WriteLine($"Simulation time : {simTime}s\n");

                foreach (Body body in simulation.bodies)
                {
                    Console.WriteLine($"Body : {body.Name}\n" +
                        $"(X : {body.CurrentXPos}, Y : {body.CurrentYPos}, Z : {body.CurrentZPos})\n" +
                        $"Avg Velocity : {body.Speed} m/s^-2\n");
                }

                Console.WriteLine($"------------------------------------------------------------\n");

                Thread.Sleep(1000);
            }
        }
    }
}
                
