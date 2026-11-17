namespace NEA_Nbody_3D_Simulator_GUI_HUD
{
    internal class BodyStateData
    {   
        public int BodyID;
        public double Mass;

        public double CurrentXPos;
        public double CurrentYPos;
        public double CurrentZPos;

        public double CurrentXVel;
        public double CurrentYVel;
        public double CurrentZVel;

        public BodyStateData(int bodyID, double mass, 
            double currentXPos, double currentYPos, double currentZPos,
            double currentXVel, double currentYVel, double currentZVel)
        {
            BodyID = bodyID;
            Mass = mass;

            CurrentXPos = currentXPos;
            CurrentYPos = currentYPos;
            CurrentZPos = currentZPos;

            CurrentXVel = currentXVel;
            CurrentYVel = currentYVel;
            CurrentZVel = currentZVel;

        }

    }

}
