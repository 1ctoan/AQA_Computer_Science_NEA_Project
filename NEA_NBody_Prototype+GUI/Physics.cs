using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace NEA_2Body_Prototype
{
    internal class Physics // This class calculates gravitational interactions, accelerations and new positions
    {
        // constants i might need
        const double GravitationalConstant = 6.6743e-11;

        public void CalculateNextIteration(List<Body> bodies, double dTime) // goes through each body in list
        {
            // Making new temporary storage arrays to keep the updated positions of the bodies
            // each slot of the array would represent a bodies position in the loop and their updated values

            double[] newXVelocities = new double[bodies.Count];
            double[] newYVelocities = new double[bodies.Count];
            double[] newSpeed = new double[bodies.Count];

            double[] newXPositions = new double[bodies.Count];
            double[] newYPositions = new double[bodies.Count];

            for (int i = 0; i < bodies.Count; i++) // main physics sim loop
            {
                Body bodyA = bodies[i]; // first body to be processed

                double resultantXForce = 0;
                double resultantYForce = 0;

                // I can import all the values for Body A before nested loop as it doesnt change every iteration

                // X and Y positions of body A
                double xPosA = bodyA.CurrentXPos;
                double yPosA = bodyA.CurrentYPos;

                // X, Y and average velocities of body A
                double xVelA = bodyA.CurrentXVel;
                double yVelA = bodyA.CurrentYVel;

                // mass of body A
                double massA = bodyA.Mass;

                foreach (Body bodyB in bodies)
                {
                    if (bodyA == bodyB) // this checks to prevent calculating the influence of one body onto itself
                    {
                        continue;
                    }
                    // retrieve all relevant data for bodyB
                    // mass, radius, name, currentXPos, currentYPos, currentXVel, currentYVel)

                    double massB = bodyB.Mass;

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

                    double distanceSquared = dx * dx + dy * dy;
                    double distance = Math.Sqrt(distanceSquared);

                    // calculate GRAVITATIONAL FORCE
                    // Use newtons law on gravitation

                    double gravitationalForce = GravitationalConstant * (massA * massB) / distanceSquared;

                    // Resolve the gravitational force into X and Y components
                    // using the displacements and distance I can work out the x and y forces without needing an angle or other trig functions

                    double xForce = (gravitationalForce * dx) / distance;
                    double yForce = (gravitationalForce * dy) / distance;

                    // The resolved forces for this iteration is stored in a variable to be processed later on

                    resultantXForce += xForce;
                    resultantYForce += yForce;

                } // end of nested loop

                // using the resultant forces, the rest of the values are processed and completed here
                // Calculate ACCELERATION
                // F = ma
                
                double xAcceleration = resultantXForce / massA; 
                double yAcceleration = resultantYForce / massA;

                // Calculate the new velocities in the x and y plane
                // V = u + at

                double xVelocity = xVelA + (xAcceleration * dTime);
                double yVelocity = yVelA + (yAcceleration * dTime);

                double speed = Math.Sqrt((xVelocity * xVelocity) + (yVelocity * yVelocity)); // extra info for funsies

                // Use the new velocities to calculate the updated position of the body
                // s = vt

                double xPosition = xPosA + (xVelocity * dTime);
                double yPosition = yPosA + (yVelocity * dTime);

                // update temporary arrays with new data respective to the i body

                newXVelocities[i] = xVelocity;
                newYVelocities[i] = yVelocity;
                newSpeed[i] = speed;

                newXPositions[i] = xPosition;
                newYPositions[i] = yPosition;


            } // end of physics loop

            for (int i = 0; i < bodies.Count; i++)
            // loop to upload new positions and velocities into body data
            { // arrays to assign the updated values in the correct order (Array = updated value not vice verca)
                bodies[i].CurrentXVel = newXVelocities[i];
                bodies[i].CurrentYVel = newYVelocities[i];
                bodies[i].Speed = newSpeed[i];

                bodies[i].CurrentXPos = newXPositions[i];
                bodies[i].CurrentYPos = newYPositions[i];
            }

        }

    }

}