using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace NEA_NBody_3D_2D_GUI_Protoype
{

    internal class GUI
    {
        // creates the camera and simulation objects as fields
        FreeCamera freeCamera = new FreeCamera();
        Simulation simulation = new Simulation();
        ReverseSimCircularStack stack = new ReverseSimCircularStack(10800); // an array with enough storage to rewind the simulator for 3 minutes.

        private bool simIsPaused = true;
        private bool menuIsOpen = false;

        private double simTime = 0;

        private int calculationsPerFrame = 8;
        private const int MaxSimSpeed = 1024;
        private const int MinSimSpeed = 1;

        private const float MaxFontSize = 40;
        private const float MinFontSize = 1;
        private const float FontSizeConstant = 4000;

        private float metresPerSceneUnit = 1e9f; // a scale factor to decrease the size of some values

        public void graphics3D() // creates a loop and calls major functions
        {
            SetConfigFlags(ConfigFlags.ResizableWindow);

            InitWindow(800, 450, "3D N_Body prototype");
            DisableCursor();

            SetTargetFPS(60);

            while (!WindowShouldClose()) // until the esc key is pressed
            {
                HandleInputs();

                Update();

                DrawMainScene(); // function to actually draw out simulation

            }
            CloseWindow(); // closes the window and the openGl context terminal
        }

        private void HandleInputs()
        {
            if (IsKeyPressed(KeyboardKey.F11))
            {
                ToggleFullscreen();
            }

            if (IsKeyPressed(KeyboardKey.Space))
            {
                PauseSim();
            }

            if (IsKeyPressed(KeyboardKey.Up))
            {
                calculationsPerFrame *= 2; // speed up simulation
            }

            if (IsKeyPressed(KeyboardKey.Down))
            {
                calculationsPerFrame /= 2; // slow down simulation
            }

            calculationsPerFrame = Math.Clamp(calculationsPerFrame, MinSimSpeed, MaxSimSpeed); // limit the sim speed to a certain number of values

            if (IsKeyPressed(KeyboardKey.Tab))
            {
                MenuOpen();
            }

            if (IsKeyDown(KeyboardKey.Left))
            {
                simIsPaused = true;
                if (!stack.CheckStackIsEmpty())
                {
                    simTime = simulation.RestoreSnapshot(stack.Pop());
                }

            }

        }

        private void Update()
        {
            if (!menuIsOpen)
            {
                freeCamera.Update();
            }

            if (simIsPaused)
            {
                return; // skips the rest of the method, so stops calling the simulator
            }

            stack.Push(simulation.MakeNewSnapshot(simTime));

            for (int i = 0; i < calculationsPerFrame; i++) // speeds up the sim so that it doesnt take ages for an orbit
            {
                simulation.Update();
            }
            simTime += simulation.dTime * calculationsPerFrame; // makes the time accurate with the new sped up version
        }

        private void PauseSim()
        {
            simIsPaused = !simIsPaused;
        }

        private void MenuOpen()
        {
            menuIsOpen = !menuIsOpen;

            if (menuIsOpen)
            {
                simIsPaused = true;
                EnableCursor();
            }
            else
            {
                DisableCursor();
            }
        }

        private void DrawMainScene()
        {
            BeginDrawing();

            ClearBackground(Color.Black); // aestheritc

            Draw3D();
            DrawBodyLabels();
            if (menuIsOpen) { DrawSideBar(); }
            DrawHUD();

            EndDrawing();
        }

        private void Draw3D()
        {
            BeginMode3D(freeCamera.Camera);

            // loop to cycle through all bodies and draw all of them
            foreach (Body body in simulation.bodies)
            {
                Vector3 bodyPosition = GetBodyPositionInScene(body);
                float scaledBodyRadius = GetBodyRadiusInScene(body);
                
                DrawSphere(bodyPosition, scaledBodyRadius, body.BodyColour);
            }

            // purely for visual purposes
            DrawGrid(100, 10); // will add a reactive space time fabric thingy later

            EndMode3D();
        }

        private float GetBodyRadiusInScene(Body body)
        {
            float radius = MathF.Log10(body.Radius);
            return radius;
        }

        private Vector3 GetBodyPositionInScene(Body body) // seperating small functions into methods lets me re-use them for other stuff
        {
            Vector3 bodyPosition = new Vector3(
                    (float)(body.CurrentXPos / metresPerSceneUnit), // this is to actually fit the bodies into the scene
                    (float)(body.CurrentYPos / metresPerSceneUnit),
                    (float)(body.CurrentZPos / metresPerSceneUnit));
            return bodyPosition;
        }

        private void DrawBodyLabels()
        {
            Camera3D camera = freeCamera.Camera;
            Vector3 cameraLookDirection = camera.Target - camera.Position;

            foreach (Body body in simulation.bodies) // second loop but this time for labels
            {
                Vector3 bodyPosition = GetBodyPositionInScene(body);
                float bodyRadius = GetBodyRadiusInScene(body);

                // creating a new vector for the labels position that has the same x and z values as the body its assigned to
                // The y value of the label is calculated by adding on the radius of the body in the scene (to prevent it from clipping)
                // and multipling it the radius by 1.5 (to keep it consistent with all sizes of bodies)
                Vector3 labelPosition = new Vector3(bodyPosition.X, bodyPosition.Y + (bodyRadius * 2f), bodyPosition.Z);
                
                // Finding the vector between the label and the camera
                Vector3 vectorToLabel = labelPosition - camera.Position;

                // Working out the dot product of the vector between the label and the camera
                float dotProduct = Vector3.Dot(vectorToLabel, cameraLookDirection);

                if (dotProduct > 0) // if the angle between the forward vector of the camera and the distance between the two points is less than 90, the value is positive and in sight of the user.
                {
                    Vector2 screenPosition = GetWorldToScreen(labelPosition, camera);
                    int labelFontSize = LabelFontSize(labelPosition);
                    // Draw text will write the label starting from the coordinates i set it to. So if I halve the width of the label and subtract it from the x position, it should be centred
                    int labelWidth = MeasureText(body.Name, labelFontSize);
                    float drawPositionOfLabel = screenPosition.X - (labelWidth / 2);

                    DrawText(body.Name, (int)drawPositionOfLabel, (int)screenPosition.Y, labelFontSize, Color.White);
                }
               
            }

        }

        private int LabelFontSize(Vector3 labelPosition) // return an appropriate font size respective of the distance of the body to the camera
        {
            float distanceBetweenCamAndLabel = Vector3.Distance(freeCamera.Camera.Position, labelPosition);

            float fontSize = FontSizeConstant / distanceBetweenCamAndLabel;
            fontSize = Math.Clamp(fontSize, MinFontSize, MaxFontSize);
            return (int) fontSize;
        }

        private void DrawHUD()
        {
            int HUDfontSize = GetScreenHeight() / 40;
            HUDfontSize = Math.Clamp(HUDfontSize, 15, 30);

            int screenMargin = GetScreenWidth() / 60;

            int lineSpacing = GetScreenHeight() / 28;
            int textYposition = 5;

            DrawText($"Speed : {freeCamera.CameraSpeed:F2}", screenMargin, textYposition, HUDfontSize, Color.SkyBlue);
            textYposition += lineSpacing;

            DrawText($"Simulation time : {simTime / 86400:F2} days", screenMargin, textYposition, HUDfontSize, Color.SkyBlue);
            textYposition += lineSpacing;

            DrawText($"Simulation Speed : {calculationsPerFrame}x", screenMargin, textYposition, HUDfontSize, Color.SkyBlue);

            if (simIsPaused)
            {
                FloatingCentreText("PAUSED", HUDfontSize);
            }

        }
        
        private void FloatingCentreText(string message, int fontSize)
        {
            int textFont = fontSize * 2;
            float textWidth = MeasureText(message, textFont);
            float pauseXPos = GetScreenWidth() / 2 - (textWidth / 2);
            float pauseYPos = GetScreenHeight() - GetScreenHeight() / 10;

            DrawText(message, (int)pauseXPos, (int)pauseYPos, textFont, Color.White);
        }

        private void DrawSideBar()
        {
            DrawRectangle(GetSideBarPosition(), 0, GetSideBarWidth(), GetScreenHeight(), new Color(20, 20, 30, 200));

           
        }

        private int GetSideBarWidth()
        {
            int sideBarWidth = GetScreenWidth() / 5;
            sideBarWidth = Math.Max(sideBarWidth, 200);
            return sideBarWidth;
        }

        private int GetSideBarPosition()
        {
            int sideBarWidth = GetSideBarWidth();
            int sideBarPosition = GetScreenWidth() - sideBarWidth;
            return sideBarPosition;
        }

    }

}