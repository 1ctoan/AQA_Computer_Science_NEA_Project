using Raylib_cs;
using static Raylib_cs.Raylib;

namespace NEA_Nbody_3D_Simulator_GUI_HUD
{
    internal static class UIMethods
    {
        public static bool DrawButton(Rectangle buttonArea, string buttonContents, float fontSize) // button area is a rectangle struct that will be made every frame
        {
            bool mouseHovered = CheckCollisionPointRec(GetMousePosition(), buttonArea);

            DrawRectangleRec(buttonArea, (mouseHovered) ? Color.LightGray : Color.DarkGray); // because im using a rectangle struct, i dont need to handle 4 parameters at once and instead can use methods with rec in their names
            // the ? is a conditional operator which is just a shortened if statement
            DrawText(buttonContents, (int)buttonArea.X + 5, (int)buttonArea.Y, (int)fontSize, Color.SkyBlue);

            return mouseHovered && IsMouseButtonPressed(MouseButton.Left);
        }
        public static void FloatingCentreText(string message, int fontSize)
        {
            int textFont = fontSize * 2;
            float textWidth = MeasureText(message, textFont);
            float pauseXPos = GetScreenWidth() / 2 - (textWidth / 2);
            float pauseYPos = GetScreenHeight() - GetScreenHeight() / 10;

            DrawText(message, (int)pauseXPos, (int)pauseYPos, textFont, Color.White);
        }
    }
}
