namespace NEA_Nbody_3D_Simulator_GUI_HUD
{
    internal class SimulationSceneSnapshot
    {
        public double SimTime;
        public BodyStateData[] BodyStates;

        public SimulationSceneSnapshot(double simTime, BodyStateData[] bodyStates)
        {
            SimTime = simTime;
            BodyStates = bodyStates;
        }
    }
}
