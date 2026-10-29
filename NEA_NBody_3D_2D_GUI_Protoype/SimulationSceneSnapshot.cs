using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_NBody_3D_2D_GUI_Protoype
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
