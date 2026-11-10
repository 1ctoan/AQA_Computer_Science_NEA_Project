using Raylib_cs;
using System.Numerics;
using System.Transactions;
using static Raylib_cs.Raylib;


namespace NEA_Nbody_3D_Simulator_GUI_HUD
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

        HashSet<int> ExpandedBodyIDs = new HashSet<int>();

        private bool removeMode = false;
        private int bodyToDelete = -1;

        private const int HUDFontSize = 30;
        private const int Buttonheight = 30;
        private const int ButtonGap = 5;
        private const int LineSpacing = 30;
        private const int ScreenMargin = 15;
         
        private int calculationsPerFrame = 8;
        private const int MaxSimSpeed = 1024;
        private const int MinSimSpeed = 1;

        private int selectedBodyId;
        private const float MaxFontSize = 40;
        private const float MinFontSize = 1;
        private const float FontSizeConstant = 4000;

        private float metresPerSceneUnit = 1e8f; // a scale factor to decrease the size of some values


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
            if (menuIsOpen) { SideBar(); }
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
            DrawGrid(1000, 50); // will add a reactive space time fabric thingy later

            EndMode3D();
        }

        private float GetBodyRadiusInScene(Body body)
        {
            float radius = MathF.Log10(body.Radius) * 3;
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
                // and multipling it the radius by 2 (to keep it consistent with all sizes of bodies)
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
            int textYposition = 5;

            DrawText($"Speed : {freeCamera.CameraSpeed:F2}", ScreenMargin, textYposition, HUDFontSize, Color.SkyBlue);
            textYposition += LineSpacing;

            DrawText($"Simulation time : {simTime / 86400:F2} days", ScreenMargin, textYposition, HUDFontSize, Color.SkyBlue);
            textYposition += LineSpacing;

            DrawText($"Simulation Speed : {calculationsPerFrame}x", ScreenMargin, textYposition, HUDFontSize, Color.SkyBlue);

            if (simIsPaused)
            {
                FloatingCentreText("PAUSED", HUDFontSize);
            }

            Rectangle AddBodyButton = new Rectangle(0, GetScreenHeight() - 45, 210, 45);
            Rectangle RemoveBodyButton = new Rectangle(300, 500, 210, 45);

            if (DrawButton(AddBodyButton, "Add Body", 40))
            {
                // Add body method called here.
                // Random value generator with clamps for fandom body method - Method overloading??
                // UI Textboxes for retrieving inputs for each value of the body from the user.

            }

            if (DrawButton(RemoveBodyButton, "Remove body", 40)) // remove body function to, i mean its pretty self explanatory
            {
                PauseSim();
                removeMode = !removeMode;
                ExpandedBodyIDs.Clear(); // closes all expanded info boxes
            }
            // edit body function - Make all draw body info text boxes into instances of draw button. Link them with a text box that does the edit body values function
        }
        
        private void FloatingCentreText(string message, int fontSize)
        {
            int textFont = fontSize * 2;
            float textWidth = MeasureText(message, textFont);
            float pauseXPos = GetScreenWidth() / 2 - (textWidth / 2);
            float pauseYPos = GetScreenHeight() - GetScreenHeight() / 10;

            DrawText(message, (int)pauseXPos, (int)pauseYPos, textFont, Color.White);
        }

        private void SideBar()
        {
            int sideBarWidth = GetSideBarWidth();
            float sideBarPosition = GetSideBarPosition();

            int buttonYPosition = 10; // top of list

            DrawRectangle((int)sideBarPosition, 0, sideBarWidth, GetScreenHeight(), new Color(20, 20, 30, 200));

            foreach (Body body in simulation.bodies)
            {
                Rectangle button = new Rectangle(sideBarPosition, buttonYPosition, sideBarWidth, Buttonheight);

                if (DrawButton(button, body.Name, 24))
                {
                    if (removeMode)
                    {
                        bodyToDelete = body.BodyID;
                        removeMode = false;
                    }

                    if (ExpandedBodyIDs.Contains(body.BodyID))
                    {
                        ExpandedBodyIDs.Remove(body.BodyID);
                    }
                    else
                    {
                        ExpandedBodyIDs.Add(body.BodyID);
                    }
                }
                buttonYPosition += Buttonheight + ButtonGap;

                if (ExpandedBodyIDs.Contains(body.BodyID))
                {
                    buttonYPosition = DrawBodyInfo(body, buttonYPosition);
                }
            }
            if (bodyToDelete != -1)
            {
                simulation.DeleteBody(bodyToDelete);
                bodyToDelete = -1;
            }

        }

        private int DrawBodyInfo(Body body, int yPosition)
        {
            int lineHeight = 25;
            int lineCount = 8;
            Color dataColour = new Color(100, 100, 100, 200);

            float hudFontSizeMultiplied = HUDFontSize * 0.8f;
            int fontSize = (int)hudFontSizeMultiplied;

            int sideBarPos = GetSideBarPosition();

            DrawRectangle(sideBarPos, yPosition, GetSideBarWidth(), lineHeight * lineCount + 5, dataColour);

            DrawText($"Mass         : {body.Mass:E1}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            DrawText($"X Position   : {body.CurrentXPos:E3}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            DrawText($"Y Position   : {body.CurrentYPos:E3}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            DrawText($"Z Position   : {body.CurrentZPos:E3}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            DrawText($"X velocity   : {body.CurrentXVel:E3}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            DrawText($"Y velocity   : {body.CurrentYVel:E3}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            DrawText($"Z velocity   : {body.CurrentZVel:E3}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            DrawText($"Speed        : {body.Speed:E3}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            return yPosition + 15;
        }

        private bool DrawButton(Rectangle buttonArea, string buttonContents, float fontSize) // button area is a rectangle struct that will be made every frame
        {
            bool mouseHovered = CheckCollisionPointRec(GetMousePosition(), buttonArea);

            DrawRectangleRec(buttonArea, (mouseHovered)? Color.LightGray : Color.DarkGray ); // because im using a rectangle struct, i dont need to handle 4 parameters at once and instead can use methods with rec in their names
            // the ? is a conditional operator which is just a shortened if statement
            DrawText(buttonContents, (int)buttonArea.X + 5, (int)buttonArea.Y, (int)fontSize, Color.SkyBlue);

            return mouseHovered && IsMouseButtonPressed(MouseButton.Left);
        }

        private int GetSideBarWidth()
        {
            int sideBarWidth = GetScreenWidth() / 4;
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