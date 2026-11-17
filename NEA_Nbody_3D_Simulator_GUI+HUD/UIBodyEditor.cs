using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static NEA_Nbody_3D_Simulator_GUI_HUD.InputValidation;

namespace NEA_Nbody_3D_Simulator_GUI_HUD
{
    internal class UIBodyEditor
    {
        private Dictionary <int, UITextBox> textBoxes = new Dictionary <int, UITextBox> (); // maps box id to boxes as several boxes could be expanded at once.

        public bool IsAnythingFocused() // making a property with extra steps in its initialisation
        {
            foreach (UITextBox box in textBoxes.Values) // list every box in the dictionary looking for 
            {
                if (box.IsFocused)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryEditRow(int textBoxID, double currentBodyValue, Rectangle area, double? minimum, out double newBodyValue) // the ? in double? means that the call could either have an additional minimum parameter or not
        { // that way i can choose to include a minimum bound or not depending on what i want to send over
            newBodyValue = 0;

            if (!textBoxes.TryGetValue(textBoxID, out UITextBox box)) // looks for a textbox with the respective ID in the dictionary
            { // if its missing, then false so the a new box is made, to only hold numbers and is uploaded to the dictionary
                box = new UITextBox ();
                box.numbersOnly = true;
                textBoxes.Add(textBoxID, box);
            }

            if (!box.IsFocused)
            {
                box.Text = currentBodyValue.ToString("E3"); // E3 is an exponential format specifier and writes the number in scientific notation to 3 decimal places.
            }
            box.Update(area);
            box.Draw(area);

            if (box.IsFocused && IsKeyPressed(KeyboardKey.Enter))
            {
                box.UnFocus();

                if (minimum.HasValue) // returns a bool depending on wheter or not minium is a null
                {
                    return SanitiseInput(box.Text, minimum.Value, out newBodyValue); // as minimum is a double?, i need .value to extract what its storing and be able to send it over
                }
                return SanitiseInput(box.Text, out newBodyValue);
            }
            return false;

        }   
    }
}

