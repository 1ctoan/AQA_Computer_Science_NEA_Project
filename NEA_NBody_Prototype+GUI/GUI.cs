using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NEA_2Body_Prototype;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace NEA_2Body_Prototype_GUI
{
    internal class GUI
    {
        public static void GraphicalMenu()
        {
            // flags to let the user resize the window and enable fullscreen mode
            SetConfigFlags(ConfigFlags.ResizableWindow);

            InitWindow(800, 600, "N-Body gravitational simulator"); // this makes the window
            SetTargetFPS(60); // pretty self explanatory

            Camera2D camera2D = new Camera2D(); // a 2 dimensional camera object
            camera2D.Zoom = 1;

            Simulation simulation = new Simulation(); // simulation body
            double simTime = 0;

            while (!WindowShouldClose())
            {
                Update(ref camera2D, simulation);
                simTime += simulation.dTime;
            }
            CloseWindow();
        }

        public static void Update(ref Camera2D camera2D, Simulation simulation)
        {

            if (IsKeyPressed(KeyboardKey.F11))
            {
                ToggleFullscreen();
            }

            // camera panning and movement
            if (IsMouseButtonDown(MouseButton.Left))
            {
                Vector2 changeInPos = GetMouseDelta(); // this function gets the change in distance of the camera from the position it was to the position it moved to by the mouse
                changeInPos *= -1.0f / camera2D.Zoom; // reverses the change to make it go in the right direction. Drag up -> go down
                // multiplies existing movement vector by the scale factor of the -1 and the zoom, then stores result back into pos
                camera2D.Target += changeInPos;
                // then we target the camera in the right position by adding the value to the pos.
            }

            // camera zooming 
            float mouseWheel = GetMouseWheelMove();
            if (mouseWheel != 0) // if the wheel is moving or isnt still
            {
                // this gets the exact point on the screen or world where the cursor is
                Vector2 cursorPosition = GetScreenToWorld2D(GetMousePosition(), camera2D); // Get world space position for a 2d camera screen space position

                // Set the offset to where the mouse is
                camera2D.Offset = GetMousePosition();

                // Set the target to match, so that the camera maps the world space point
                // under the cursor to the screen space point under the cursor at any zoom
                camera2D.Target = cursorPosition;

                // zoom increment which uses logarithmic scaling to provide a consistent zoom speed
                float zoomScale = 0.2f * mouseWheel;
                // clamp provides upper (64.0f) and lower (0.125f) boundaries to the zoom in order to prevent zooming out too much or zooming in too much
                // MathF is just the same as Math but operates entirely using float data type, and was made for unity
                camera2D.Zoom = Math.Clamp(MathF.Exp(MathF.Log(camera2D.Zoom) + zoomScale), 0.125f, 64.0f);
            }

            BeginDrawing();

            simulation.update();

            ClearBackground(Color.Black);

            // apply the camerea configs to the grid
            BeginMode2D(camera2D);

            // CODE A SPACE TIME VISUALISER HERE INSTEAD
            /* Vertical lines
            for (int x = -10000; x <= 10000; x += 50)
            {
                DrawLine(x, -10000, x, 10000, Color.DarkGray);
            }

            // Horizontal lines
            for (int y = -10000; y <= 10000; y += 50)
            {
                DrawLine(-10000, y, 10000, y, Color.DarkGray);
            }
            */

            foreach (Body body in simulation.bodies)
            {
                Vector2 bodyPosition = new Vector2((float)body.CurrentXPos, (float)body.CurrentYPos);

                DrawCircleV(bodyPosition, body.Radius, body.BodyColour);

            }

            EndMode2D();

            EndDrawing();
        }

    }

}
