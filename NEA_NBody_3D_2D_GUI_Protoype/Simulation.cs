using Raylib_cs;

namespace NEA_NBody_3D_2D_GUI_Protoype
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
            // name, mass (kg), color, radius (m), currentXPos, currentYPos, currentZPos, currentXVel, currentYVel, currentZVel)

            AddBody("Earth", 5.972e24, 6.371e6f, Color.Blue, 1.496e11, 0, 0, 0, 0, 29800); // earth object
            AddBody("Sun", 1.989e30, 6.957e8f, Color.White, 0, 0, 0, 0, 0, 0); // sun object

        }

        // add new bodies by calling functions and sending in values using a parameter
        public void AddBody(string name, double mass, float radius, Color bodyColour,
            double currentXPos, double currentYPos, double currentZPos,
            double currentXVel, double currentYVel, double currentZVel)
        {
            Body body = new Body(name, mass, radius, bodyColour,
            currentXPos, currentYPos, currentZPos,
            currentXVel, currentYVel, currentZVel, 0);

            bodies.Add(body);
            bodyCatalogue.Add(body.BodyID, body);
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
