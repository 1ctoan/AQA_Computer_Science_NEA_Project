using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace NEA_2Body_Prototype
{

    internal class Physics {
        // sim to use the physics equations to calculate the positions
       
        // constants i might need
        const double GravitationalConstant = 6.6743e-11;
        const double Pi = 3.1415926535897931;

        public void bodyCalculator(List<Body> bodies, double dTime) // goes through each body in list
        {
            // Making new temporary storage arrays to keep the updated positions of the bodies
            // each slot of the array would represent a bodies position in the loop and their updated values

            double[] newXVelocities = new double[bodies.Count];
            double[] newYVelocities = new double[bodies.Count];
            double[] newAvgVelocity = new double[bodies.Count];

            double[] newXPositions = new double[bodies.Count];
            double[] newYPositions = new double[bodies.Count];

            for (int i = 0; i < bodies.Count; i++) // main physics sim loop
            {

                Body bodyA = bodies[i];

                foreach (Body bodyB in bodies)
                {
                    if (bodyA == bodyB)
                    {
                        continue;
                    }

                    // retrieve all relevant data
                    // mass, radius, name, currentXPos, currentYPos, currentXVel, currentYVel)
                    double massA = bodyA.Mass;
                    double massB = bodyB.Mass;

                    // X and Y positions of body A
                    double xPosA = bodyA.CurrentXPos;
                    double yPosA = bodyA.CurrentYPos;

                    // X, Y and average velocities of body A
                    double xVelA = bodyA.CurrentXVel;
                    double yVelA = bodyA.CurrentYVel;

                    // X and y positions of body B
                    double xPosB = bodyB.CurrentXPos;
                    double yPosB = bodyB.CurrentYPos;

                    // Dont need x and y velocities of body B as they're unecessary in my calculations

                    // calculate DISPLACEMENT between the two bodies
                    // change in y and change in x between two bodies

                    double dx = xPosB - xPosA;
                    double dy = yPosB - yPosA;

                    // Calculate DISTANCE between the two bodies
                    // change in x squared plus change in y squared

                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    // calculate GRAVITATIONAL FORCE
                    // Use newtons law on gravitation

                    double gravitationalForce = GravitationalConstant * (massA * massB) / (distance * distance);

                    // Resolve the gravitational force into X and Y components
                    // using the displacements and distance I can work out the x and y forces without needing an angle or other trig functions

                    double xForce = (gravitationalForce * dx) / distance;
                    double yForce = (gravitationalForce * dy) / distance;

                    // translate forces into acceleration
                    // F = ma

                    double xAcceleration = xForce / massA;
                    double yAcceleration = yForce / massA;

                    // Use these new accelerations to calculate a new velocity and position
                    // v = u + at

                    double xVelocity = xVelA + (xAcceleration * dTime);
                    double yVelocity = yVelA + (yAcceleration * dTime);

                    double averageVelocity = Math.Sqrt((xVelocity * xVelocity) + (yVelocity * yVelocity));

                    // Use the new velocities to calculate the updated position of the body
                    // s = vt

                    double xPosition = xPosA + (xVelocity * dTime);
                    double yPosition = xPosB + (yVelocity * dTime);

                    // update temporary arrays with new data respective to the i body

                    newXVelocities[i] = xVelocity;
                    newYVelocities[i] = yVelocity;
                    newAvgVelocity[i] = averageVelocity;

                    newXPositions[i] = xPosition;
                    newYPositions[i] = yPosition;

                } // end of nested loop

            } // end of physics loop

            for (int i = 0; i < bodies.Count; i++)
            // loop to upload new positions and velocities into body data
            { // arrays to assign the updated values in the correct order (Array = updated value not vice verca)
                bodies[i].CurrentXVel = newXVelocities[i];
                bodies[i].CurrentYVel = newYVelocities[i];
                bodies[i].AverageVel = newAvgVelocity[i];

                bodies[i].CurrentXPos = newXPositions[i];
                bodies[i].CurrentYPos = newYPositions[i];
            }

        }

    }

}
