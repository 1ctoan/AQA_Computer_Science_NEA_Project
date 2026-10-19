using NEA_2Body_Prototype;
using NEA_2Body_Prototype_GUI;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace NEA_2Body_Prototype
{
    internal class Program // class to run the application and coordinate the main loop. console app
    {
        static void Main(string[] args)
        {
            bool showMenu = true;

            while (showMenu)
            {
                showMenu = ConsoleMenu();
            }
        }

        private static bool ConsoleMenu()
        {
            Console.WriteLine("(1) - N body console app\n" +
                "(2) - Simulation w/ GUI");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ConsoleVersion.ConsoleLoop();
                    return true;

                case "2":
                    GUI.GraphicalMenu();
                    return true;

                default:
                    Console.WriteLine("bubye");
                    return false;
            }

        }

    }

}