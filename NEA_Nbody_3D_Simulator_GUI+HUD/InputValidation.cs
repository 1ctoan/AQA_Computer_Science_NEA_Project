using Raylib_cs;
using static Raylib_cs.Raylib;

namespace NEA_Nbody_3D_Simulator_GUI_HUD
{
    internal static class InputValidation
    {
        public static readonly Dictionary<string, Color> colourCatalogue = new Dictionary<string, Color>
        {
            { "black", Color.Black },
            { "darkgrey", Color.DarkGray },
            { "gray", Color.Gray },
            { "lightgrey", Color.LightGray },
            { "white", Color.White },
            { "maroon", Color.Maroon },
            { "red", Color.Red },
            { "gold", Color.Gold },
            { "orange", Color.Orange },
            { "yellow", Color.Yellow },
            { "darkgreen", Color.DarkGreen },
            { "green", Color.Green },
            { "lime", Color.Lime },
            { "darkblue", Color.DarkBlue },
            { "blue", Color.Blue },
            { "skyblue", Color.SkyBlue },
            { "darkpurple", Color.DarkPurple },
            { "purple", Color.Purple },
            { "violet", Color.Violet },
            { "darkbrown", Color.DarkBrown },
            { "brown", Color.Brown },
            { "beige", Color.Beige },
            { "pink", Color.Pink },
            { "magenta", Color.Magenta },
            { "blank", Color.Blank },
            { "raywhite", Color.RayWhite }
        };

        public static bool SanitiseInput(string input, out double output) // out essentially means that this function can return 2 values instead of 1
        {
            if (!double.TryParse(input, out output) || !double.IsFinite(output)) // if input isnt a double or infinity or NaN, then false
            {
                return false;
            }
            return true; // if its a value that should work, true 
        }

        public static bool SanitiseInput(string input, double minimum, out double output) // string input, duoble output with a minimum for ranged numbers like mass and radius
        {
            if (!SanitiseInput(input, out output)) // decided to use the sanitisation method above instead of just copy pasting the code
            {
                return false;
            }
            if (output <= minimum) // the value must be greater than a minimum to prevent negative or 0 masses 
            {
                return false;
            }
            return true; // if its a value that should work, true 
        }

        public static bool SanitiseInput(string input, out string output) // string input, string output for names
        {
            output = input.Trim();
            if (output.Length == 0)
            {
                return false; // ensuring that the user has actually typed something in
            }
            return true;
        }

        public static bool SanitiseInput(string input, out Color output) // string input and colour output for colour
        {
            input = input.Trim().ToLower();
            if (!colourCatalogue.TryGetValue(input, out output)) // checks through the dictionary to look for a value from the list of keys
            {
                output = Color.White;
                return false;
            }
            return true;
        }
    }
}