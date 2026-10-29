using Raylib_cs;

namespace NEA_NBody_3D_2D_GUI_Protoype
{
    internal class Body // this class establishes the blueprints for each body
    {
        // fields and properties for each body
        public string Name { get; }
        public int BodyID { get; }
        private static int NextBodyID = 0;
        public double Mass { get; set; }
        public float Radius { get; }
        public Color BodyColour { get; set; }

        public double CurrentXPos { get; set; }
        public double CurrentYPos { get; set; }
        public double CurrentZPos { get; set; }
        public double CurrentXVel { get; set; }
        public double CurrentYVel { get; set; }
        public double CurrentZVel { get; set; }
        public double Speed { get; set; }

        public Body(string name, double mass, float radius, Color bodyColour,
            double currentXPos, double currentYPos, double currentZPos,
            double currentXVel, double currentYVel, double currentZVel, double speed)

        {
            Name = name;

            BodyID = NextBodyID;
            NextBodyID++;

            Mass = mass;
            Radius = radius;
            BodyColour = bodyColour;

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
