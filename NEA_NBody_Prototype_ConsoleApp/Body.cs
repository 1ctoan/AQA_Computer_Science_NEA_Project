namespace NEA_2Body_Prototype
{
    internal class Body // this class establishes the blueprints for each body
    {
        // fields and properties for each body
        public double Mass { get; }
        public double Radius { get; }
        public string Name { get; }

        public double CurrentXPos { get; set; }
        public double CurrentYPos { get; set; }
        public double CurrentXVel { get; set; }
        public double CurrentYVel { get; set; }
        public double Speed { get; set; }

        public Body(double mass, double radius, string name, double currentXPos, double currentYPos, double currentXVel, double currentYVel, double speed)
        {
            Mass = mass;
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
