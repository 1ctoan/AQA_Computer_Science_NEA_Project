using NEA_NBody_Prototype_3D_ConsoleApp;
using Raylib_cs;
using RaylibTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;

namespace NEA_NBody_Prototype_3D_GUI
{
 
    // side bar to add bodies
    // space time fabric visualiser
    // space to pause time
    // arrow keys to rewind or speed up
    // custom ray tracing?

    internal class GUI
    {
        public void graphics3D()
        {
            SetConfigFlags(ConfigFlags.ResizableWindow);

            InitWindow(800, 450, "3D N_Body prototype");
            DisableCursor();

            // creates the camera and simulation objects
            FreeCamera freeCamera = new FreeCamera(); 
            Simulation simulation = new Simulation();
            double simTime = 0;

            SetTargetFPS(60);

            while (!WindowShouldClose()) // until the esc key is pressed
            {
                simTime += simulation.dTime;

                freeCamera.Update();

                Draw3D(freeCamera, simulation, simTime); // function to actually draw out simulation

                if (IsKeyPressed(KeyboardKey.F11))
                {
                    ToggleFullscreen();
                }
            }
            CloseWindow(); // closes the window and the openGl context terminal
        }

        public static void Draw3D(FreeCamera freeCamera, Simulation simulation, double simTime)
        {
            BeginDrawing();
            simulation.update(); // need to call this here or nothing shows up

            ClearBackground(Color.Black); // aestheritc

            BeginMode3D(freeCamera.Camera);

            // loop to cycle through all bodies and draw all of them
            foreach (Body body in simulation.bodies) 
            {
                Vector3 bodyPosition = new Vector3(
                    (float)body.CurrentXPos,
                    (float)body.CurrentYPos,
                    (float)body.CurrentZPos);

                DrawSphere(bodyPosition, body.Radius, body.BodyColour);
            }

            // purely for visual purposes
            DrawGrid(100, 10); // will add a reactive space time fabric thingy later

            EndMode3D();

            DrawText($"Speed : {freeCamera.CameraSpeed:F1}\n" +
                $"Simulation time : {simTime:F1}s", 20, 20, 20, Color.SkyBlue);

            EndDrawing();
        }
        
    }

}

