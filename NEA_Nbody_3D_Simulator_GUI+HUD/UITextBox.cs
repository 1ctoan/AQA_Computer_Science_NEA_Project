using Raylib_cs;
using static Raylib_cs.Raylib;

namespace NEA_Nbody_3D_Simulator_GUI_HUD
{
    internal class UITextBox
    {
        public string Text = "";
        public bool IsFocused {  get; private set; }
        public bool numbersOnly;

        public void Focus(string startingText)
        {
            IsFocused = true;
            Text = startingText;
        }

        public void UnFocus()
        {
            IsFocused = false;
        }

        public void Update(Rectangle buttonArea)
        {
            bool mouseHovered = CheckCollisionPointRec(GetMousePosition(), buttonArea);

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                if (mouseHovered && !IsFocused)
                {
                    Focus(Text);
                }
                else if (!mouseHovered)
                {
                    UnFocus();
                }
            }

            if (IsFocused)
            {
                int pressedCharacter;

                while ((pressedCharacter = GetCharPressed()) > 0)
                {
                    if (pressedCharacter >= 32)
                    {
                        char c = (char)pressedCharacter;

                        if (!numbersOnly || char.IsDigit(c) || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
                        {
                            Text += c;
                        }
                    }
                }

                if (IsKeyPressed(KeyboardKey.Backspace) && Text.Length > 0)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
            }
        }

        public void Draw(Rectangle area) // copied and made from the existing code in add body panel
        {
            bool mouseHovered = CheckCollisionPointRec(GetMousePosition(), area);
            Color inputBackground = (mouseHovered || IsFocused) ? Color.LightGray : Color.DarkGray;

            DrawRectangleRec(area, inputBackground);
            DrawRectangleLinesEx(area, IsFocused ? 2 : 1, IsFocused ? Color.SkyBlue : Color.DarkGray); // highlights selected input field

            BeginScissorMode((int)area.X + 5, (int)area.Y + 2, (int)area.Width - 10, (int)area.Height - 4); // inputbox swapped with area

            DrawText(Text, (int)area.X + 6, (int)area.Y + 5, 20, Color.Black);

            EndScissorMode();
        }
    }
}