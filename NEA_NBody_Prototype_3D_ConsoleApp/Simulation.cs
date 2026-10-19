using NEA_NBody_Prototype_3D_ConsoleApp;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;

namespace NEA_NBody_Prototype_3D_ConsoleApp
{
    internal class Simulation // class to calculate simulation time, and call other functions to run the sim
    { // need to make an add body function
        public List<Body> bodies { get; } = new List<Body>(); // empty public list to store any bodies to be made

        // Set up how much time passes in the sim per loop
        public double dTime = 1.0f;

        Physics physics = new Physics();

        public Simulation()
        {
            // Create the predefined objects for the bodies
            // mass (kg), color, radius (m), name, currentXPos, currentYPos, currentZPos, currentXVel, currentYVel, currentZVel, averageVel)
            // bodies.Add(new Body(0, 0, "TEMPLATE", 0, 0, 0, 0, 0));

            bodies.Add(new Body("Earth", 6, Color.Blue, 15, 0, 1000, 100, 0, 10, 3, 0)); // earth object

            bodies.Add(new Body( "Sun",20, Color.White, 30, 0, 0, 0, 0, 0, 0, 0)); // sun object

        }

        public void update()
        {
            physics.calculateNextIteration(bodies, dTime);
        }

    }

}
