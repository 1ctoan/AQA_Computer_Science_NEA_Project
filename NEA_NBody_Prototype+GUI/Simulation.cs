using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace NEA_2Body_Prototype
{
    internal class Simulation // class to calculate simulation time, and call other functions to run the sim
    {
        public List<Body> bodies { get; } = new List<Body>(); // empty public list to store any bodies to be made

        // Set up how much time passes in the sim per loop
        public double dTime = 1.0f;

        Physics physics = new Physics();

        Random random = new Random();
        int red;
        int green;
        int blue;

        public Simulation()
        {
            // Get the random RGB values
            red = random.Next(50, 255);
            green = random.Next(50, 255);
            blue = random.Next(50, 255);

            Color randomColour = new Color(red, green, blue);

            // Create the predefined objects for the bodies
            // mass (kg), color, radius (m), name, currentXPos, currentYPos, currentXVel, currentYVel, averageVel)
            // bodies.Add(new Body(0, 0, "TEMPLATE", 0, 0, 0, 0, 0));

            bodies.Add(new Body(6, Color.Blue, 15, "Earth", 100, -200, -1, 0, 0)); // earth object

            bodies.Add(new Body(20, Color.White, 30, "Sun", 0, 0, 0, 0, 0)); // sun object

            bodies.Add(new Body(3, randomColour, 10, "body3", -300, 400, 0, 0, 0)); // 3rd test object

            bodies.Add(new Body(10, randomColour, 20, "body4", 200, 500, 0, 0, 0));
        }

        public void update()
        {
            physics.CalculateNextIteration(bodies, dTime);
        }

    }

}
