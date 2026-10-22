using NEA_NBody_Prototype_3D_ConsoleApp;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace NEA_NBody_Prototype_3D_ConsoleApp
{
    internal class Body // this class establishes the blueprints for each body
    {
        // fields and properties for each body
        public string Name { get; }
        public double Mass { get; }
        public float Radius { get; }
        public Color BodyColour { get; set; }

        public double CurrentXPos { get; set; }
        public double CurrentYPos { get; set; }
        public double CurrentZPos { get; set; }
        public double CurrentXVel { get; set; }
        public double CurrentYVel { get; set; }
        public double CurrentZVel { get; set; }
        public double Speed { get; set; }


        public Body(string name, double mass, Color bodyColour, float radius,
            double currentXPos, double currentYPos, double currentZPos,
            double currentXVel, double currentYVel, double currentZVel, double speed)

        {
            Name = name;
            Mass = mass;
            BodyColour = bodyColour;
            Radius = radius;

            CurrentXPos = currentXPos;
            CurrentYPos = currentYPos;
            CurrentZPos = currentZPos;

            CurrentXVel = currentXVel;
            CurrentYVel = currentYVel;
            CurrentZVel = currentZVel;
            Speed = speed;
        }
    }
}
