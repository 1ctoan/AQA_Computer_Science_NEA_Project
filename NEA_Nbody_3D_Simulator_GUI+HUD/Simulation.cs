using Raylib_cs;

// Max double value in c# = 1.7976931348623157E+308

namespace NEA_Nbody_3D_Simulator_GUI_HUD
{
    internal class Simulation
    {
        public List<Body> bodies { get; } = new List<Body>(); // empty public list to store any bodies to be made
        public Dictionary<int, Body> bodyCatalogue = new Dictionary<int, Body>(); // a dictionary to correlate body objects with body IDs
        // Set up how much time passes in the sim per loop
        public double dTime = 3600;

        Physics physics = new Physics();
    
        public Simulation()
        {
            // Create the predefined objects for the bodies
            // name, mass (kg), radius (m), Colour, currentXPos, currentYPos, currentZPos, currentXVel, currentYVel, currentZVel)
           
            AddBody("Sun", 1.989e30, 6.957e8f, Color.White, 0, 0, 0, 0, 0, 0);

            AddBody("Mercury", 3.301e23, 2.4397e6f, Color.Gray, 5.791e10, 0, 0, 0, 0, 47870);
            AddBody("Venus", 4.867e24, 6.0518e6f, Color.Orange, 1.082e11, 0, 0, 0, 0, 35020);
            AddBody("Earth", 5.972e24, 6.371e6f, Color.Blue, 1.496e11, 0, 0, 0, 0, 29780);
            AddBody("Mars", 6.417e23, 3.3895e6f, Color.Red, 2.279e11, 0, 0, 0, 0, 24070);

            AddBody("Jupiter", 1.898e27, 6.9911e7f, Color.Brown, 7.785e11, 0, 0, 0, 0, 13070);
            AddBody("Saturn", 5.683e26, 5.8232e7f, Color.Gold, 1.434e12, 0, 0, 0, 0, 9680);
            AddBody("Uranus", 8.681e25, 2.5362e7f, Color.SkyBlue, 2.871e12, 0, 0, 0, 0, 6800);
            AddBody("Neptune", 1.024e26, 2.4622e7f, Color.DarkBlue, 4.495e12, 0, 0, 0, 0, 5430);
        }

        // add new bodies by calling functions and sending in values using a parameter
        public void AddBody(string name, double mass, double radius, Color bodyColour,
            double currentXPos, double currentYPos, double currentZPos,
            double currentXVel, double currentYVel, double currentZVel)
        {
            Body body = new Body(name, mass, radius, bodyColour,
            currentXPos, currentYPos, currentZPos,
            currentXVel, currentYVel, currentZVel, 0);

            bodies.Add(body);
            bodyCatalogue.Add(body.BodyID, body);
        }

        public void DeleteBody(int bodyID)
        {
            for (int i = bodies.Count - 1; i >= 0; --i) // looping backwards to prevent sim from crashing after removing
            { // after deleting all other items would have to shift, but when you loop backwards that isnt an issue
                if (bodies[i].BodyID == bodyID)
                {
                    bodies.RemoveAt(i); // removeAt is better than Remove(bodies[i]) as it just deletes the body at that index directly
                    break;
                }
            }
        }

        public SimulationSceneSnapshot MakeNewSnapshot(double simTime) // to store data on each body to be recalled later
        {
            BodyStateData[] bodyStates = new BodyStateData[bodies.Count]; // makes a new array on every iteration of the simulator

            for (int i = 0; i < bodies.Count; i++) // cycle through every body that exists
            {
                // and make a new array containing all important info that might change
                bodyStates[i] = new BodyStateData(bodies[i].BodyID, bodies[i].Mass, 
                    bodies[i].CurrentXPos, bodies[i].CurrentYPos, bodies[i].CurrentZPos,
                    bodies[i].CurrentXVel, bodies[i].CurrentYVel, bodies[i].CurrentZVel);
            }
            return new SimulationSceneSnapshot(simTime, bodyStates);
        }

        public double RestoreSnapshot(SimulationSceneSnapshot snapshot) // to unpack and recall the data stored on each body to be able to reverse the simulation
        {  // snapshot is an object with the two simtime and bodystates fields
            bodies.Clear(); // removes every body from the live list, not dictionary

            foreach (BodyStateData bodyState in snapshot.BodyStates)
            {
                Body body = bodyCatalogue[bodyState.BodyID];
                // update values for each body that was in the dictionary
                body.Mass = bodyState.Mass;

                body.CurrentXPos = bodyState.CurrentXPos;
                body.CurrentYPos = bodyState.CurrentYPos;
                body.CurrentZPos = bodyState.CurrentZPos;

                body.CurrentXVel = bodyState.CurrentXVel;
                body.CurrentYVel = bodyState.CurrentYVel;
                body.CurrentZVel = bodyState.CurrentZVel;

                bodies.Add(body);
            }
            return snapshot.SimTime;
        }

        public void Update()
        {
            physics.calculateNextIteration(bodies, dTime);
        }
    }
}
