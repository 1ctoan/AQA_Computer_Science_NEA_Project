using Raylib_cs;
using static Raylib_cs.Raylib;

namespace NEA_2Body_Prototype
{
    internal class Body // this class establishes the blueprints for each body
    {
        // fields and properties for each body
        public double Mass { get; }
        public float Radius { get; }
        public string Name { get; }
        public Color BodyColour { get; set; }

        public double CurrentXPos { get; set; }
        public double CurrentYPos { get; set; }
        public double CurrentXVel { get; set; }
        public double CurrentYVel { get; set; }
        public double Speed { get; set; }

   
        public Body(double mass, Color bodyColour, float radius, string name, double currentXPos, double currentYPos, double currentXVel, double currentYVel, double speed)
        {
            Mass = mass;
            BodyColour = bodyColour;
            Radius = radius;
            Name = name;
            CurrentXPos = currentXPos;
            CurrentYPos = currentYPos;
            CurrentXVel = currentXVel;
            CurrentYVel = currentYVel;
            Speed = speed;
        }
    }
}
