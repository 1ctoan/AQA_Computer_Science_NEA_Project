using NEA_NBody_Prototype_3D_ConsoleApp;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace NEA_NBody_Prototype_3D_ConsoleApp
{
    internal class Simulation // class to calculate simulation time, and call other functions to run the sim
    { // need to make an add body function
        public List<Body> bodies { get; } = new List<Body>(); // empty public list to store any bodies to be made

        // Set up how much time passes in the sim per loop
        public double dTime = 0.1;

        Physics physics = new Physics();

        public Simulation()
        {
            // Create the predefined objects for the bodies
            // name, mass (kg), color, radius (m), currentXPos, currentYPos, currentZPos, currentXVel, currentYVel, currentZVel, averageVel)
            // bodies.Add(new Body(0, 0, "TEMPLATE", 0, 0, 0, 0, 0, 0, 0));

            bodies.Add(new Body("Earth", 10, Color.Blue, 10, 300, 0, 0, 0, 0, 3, 0)); // earth object

            bodies.Add(new Body("Sun", 500, Color.White, 20, 0, 0, 0, 0, 0, 0, 0)); // sun object

            bodies.Add(new Body("Mars", 30, Color.Orange, 8, 200, 0, 0, 0, 0, -2, 0));

        }

        public void update()
        {
            physics.calculateNextIteration(bodies, dTime);
        }

    }

}
