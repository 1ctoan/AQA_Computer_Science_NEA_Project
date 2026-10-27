using Raylib_cs;

namespace NEA_NBody_3D_2D_GUI_Protoype
{
    internal class Simulation
    {
        public List<Body> bodies { get; } = new List<Body>(); // empty public list to store any bodies to be made

        // Set up how much time passes in the sim per loop
        public double dTime = 3600;

        Physics physics = new Physics();

        public Simulation()
        {
            // Create the predefined objects for the bodies
            // name, mass (kg), color, radius (m), currentXPos, currentYPos, currentZPos, currentXVel, currentYVel, currentZVel, averageVel)
            // bodies.Add(new Body(0, 0, "TEMPLATE", 0, 0, 0, 0, 0, 0, 0));

            bodies.Add(new Body("Earth", 5.972e24, Color.Blue, 6.371e6f, 1.496e11, 0, 0, 0, 0, 29800, 0)); // earth object

            bodies.Add(new Body("Sun", 1.989e30, Color.White, 6.957e8f, 0, 0, 0, 0, 0, 0, 0)); // sun object

        }

        public void update()
        {
            physics.calculateNextIteration(bodies, dTime);
        }
    }
}
