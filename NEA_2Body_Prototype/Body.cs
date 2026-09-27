namespace NEA_2Body_Prototype
{
    internal class Body
    {
        // fields and properties for each body
        public double Mass { get; }
        public double Radius { get; }
        public string Name { get; }

        public double CurrentXPos { get; set; }
        public double CurrentYPos { get; set; }
        public double CurrentXVel { get; set; }
        public double CurrentYVel { get; set; }
        public double AverageVel { get; set; }

        public Body(double mass, double radius, string name, double currentXPos, double currentYPos, double currentXVel, double currentYVel, double averageVel)
        {
            Mass = mass;
            Radius = radius;
            Name = name;
            CurrentXPos = currentXPos;
            CurrentYPos = currentYPos;
            CurrentXVel = currentXVel;
            CurrentYVel = currentYVel;
            AverageVel = averageVel;
        }
    }
}
