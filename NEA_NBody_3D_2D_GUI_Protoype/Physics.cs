namespace NEA_NBody_3D_2D_GUI_Protoype
{
    internal class Physics
    {
        const double GravitationalConstant = 6.6743e-11;

        public void calculateNextIteration(List<Body> bodies, double dTime)
        {
            // arrays to store the updated values of each body
            double[] newXVelocities = new double[bodies.Count];
            double[] newYVelocities = new double[bodies.Count];
            double[] newZVelocities = new double[bodies.Count];

            double[] newSpeed = new double[bodies.Count];

            double[] newXPositions = new double[bodies.Count];
            double[] newYPositions = new double[bodies.Count];
            double[] newZPositions = new double[bodies.Count];

            // outer loop
            for (int i = 0; i < bodies.Count; i++)
            {
                // The body who's values are to be calculated
                Body bodyA = bodies[i];

                // not using vector3 for all variables as its simpler to handle them
                // resultant force variables that will be processed at end of loop
                double xResultantAcceleration = 0;
                double yResultantAcceleration = 0;
                double zResultantAcceleration = 0;

                // x, y, z position of A
                double xPosA = bodyA.CurrentXPos;
                double yPosA = bodyA.CurrentYPos;
                double zPosA = bodyA.CurrentZPos;

                // x, y, z velocities of A
                double xVelA = bodyA.CurrentXVel;
                double yVelA = bodyA.CurrentYVel;
                double zVelA = bodyA.CurrentZVel;

                // Mass of body A
                double massA = bodyA.Mass;

                // inner loop
                for (int j = 0; j < bodies.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    Body bodyB = bodies[j];

                    // x, y, z positions of body B
                    double xPosB = bodyB.CurrentXPos;
                    double yPosB = bodyB.CurrentYPos;
                    double zPosB = bodyB.CurrentZPos;

                    // mass of BodyB
                    double massB = bodyB.Mass;

                    // 1) Calculate DISPLACEMENT
                    double dX = xPosB - xPosA;
                    double dY = yPosB - yPosA;
                    double dZ = zPosB - zPosA;

                    // 2) Calculate DISTANCE^2 and DISTANCE^3
                    double distanceSquared = (dX * dX) + (dY * dY) + (dZ * dZ);
                    double distanceCubed = distanceSquared * Math.Sqrt(distanceSquared);

                    // 3) Calculate GRAVITATIONAL FORCE in X, Y, and Z plane and add it to acceleration
                    double xAcceleration = (GravitationalConstant * massB * dX) / distanceCubed;
                    xResultantAcceleration += xAcceleration;

                    double yAcceleration = (GravitationalConstant * massB * dY) / distanceCubed;
                    yResultantAcceleration += yAcceleration;

                    double zAcceleration = (GravitationalConstant * massB * dZ) / distanceCubed;
                    zResultantAcceleration += zAcceleration;
                }

                // 4) Calculate VELOCITIES in all planes using v = u + at
                double xNewVelocity = xVelA + (xResultantAcceleration * dTime);
                double yNewVelocity = yVelA + (yResultantAcceleration * dTime);
                double zNewVelocity = zVelA + (zResultantAcceleration * dTime);

                // Bonus - Calculate MAGNITUDE of the vector to find speed
                double speed = Math.Sqrt((xNewVelocity * xNewVelocity) + (yNewVelocity * yNewVelocity) + (zNewVelocity * zNewVelocity));

                // 5) Calculate the new POSITIONS of the body using updated velocity and d = d_old + st
                double xNewPosition = xPosA + (xNewVelocity * dTime);
                double yNewPosition = yPosA + (yNewVelocity * dTime);
                double zNewPosition = zPosA + (zNewVelocity * dTime);

                // 6) UPDATE all temporary arrays from the start of the class with the new data
                newXVelocities[i] = xNewVelocity;
                newYVelocities[i] = yNewVelocity;
                newZVelocities[i] = zNewVelocity;

                newSpeed[i] = speed;

                newXPositions[i] = xNewPosition;
                newYPositions[i] = yNewPosition;
                newZPositions[i] = zNewPosition;
            }

            // Loop to upload all updated data back into the bodies
            for (int i = 0; i < bodies.Count; i++)
            {
                // x, y, and z velocities
                bodies[i].CurrentXVel = newXVelocities[i];
                bodies[i].CurrentYVel = newYVelocities[i];
                bodies[i].CurrentZVel = newZVelocities[i];

                // speed
                bodies[i].Speed = newSpeed[i];

                // x, y, and z positions
                bodies[i].CurrentXPos = newXPositions[i];
                bodies[i].CurrentYPos = newYPositions[i];
                bodies[i].CurrentZPos = newZPositions[i];
            }

        }

    }

}