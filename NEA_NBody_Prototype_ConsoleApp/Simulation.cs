using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_2Body_Prototype
{
    internal class Simulation // class to calculate simulation time, and call other functions to run the sim
    {
        public List<Body> bodies { get; } = new List<Body>(); // empty public list to store any bodies to be made

        // Set up how much time passes in the sim per loop
        private double dTime = 60;

        Physics physics = new Physics();

        public Simulation()
        {
            // Create the predefined objects for the bodies
            // mass, radius, name, currentXPos, currentYPos, currentXVel, currentYVel, averageVel)

            bodies.Add(new Body(5.97e24, 6.37e6, "Earth", 1.496e11, 0, 0, 29780, 0)); // earth object

            bodies.Add(new Body(1.99e30, 6.96e8, "Sun", 0, 0, 0, 0, 0)); // sun object

            bodies.Add(new Body(3e20, 4e5, "body3", -1.5e9, 0, 100000, 0, 100000)); // 3rd test object
        }

        public void update()
        {
            physics.CalculateNextIteration(bodies, dTime);
        }

    }

}
