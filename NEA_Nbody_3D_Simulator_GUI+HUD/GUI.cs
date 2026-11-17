using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static NEA_Nbody_3D_Simulator_GUI_HUD.UIMethods;

namespace NEA_Nbody_3D_Simulator_GUI_HUD
{

    internal class GUI
    {
        // creates the camera and simulation objects as fields
        FreeCamera freeCamera = new FreeCamera();
        Simulation simulation = new Simulation();
        AddBodyPanel addBodyPanel = new AddBodyPanel();
        UIBodyEditor editor = new UIBodyEditor();
        ReverseSimCircularStack stack = new ReverseSimCircularStack(10800); // an array with enough storage to rewind the simulator for 3 minutes.

        private bool simIsPaused = true;
        private bool menuIsOpen = false;

        private double simTime = 0;

        HashSet<int> ExpandedBodyIDs = new HashSet<int>(); // i used a hashset purely because it prevents duplicates.
        // I dont need an order of data in this either so having a data set that could just tell me if something was stored or not was best

        private bool removeMode = false;
        private int bodyToDelete = -1;

        private bool cursorIsFree = false;

        private double radius;
        private const double EarthRadiusMetres = 6.371e6;
        private const double EarthSceneRadius = 2.5;
        private const double SmallBodyExponent = 0.877; // calculated through ln(0.8/2.5) / ln(1.7374e6/6.371e6) = −1.139 / −1.299
        private const double LargeBodyExponent = 0.334; // calculated through ln(12/2.5) ÷ ln(6.957e8/6.371e6) = 1.569 ÷ 4.693
        private const double RadiusDisplayScale = 10.0;

        private const int HUDFontSize = 30;
        private const int Buttonheight = 30;
        private const int ButtonGap = 5;
        private const int LineSpacing = 30;
        private const int ScreenMargin = 15;

        private int calculationsPerFrame = 8;
        private const int MaxSimSpeed = 1024;
        private const int MinSimSpeed = 1;

        private const float TextBoxOffset = 170;
        private const float MaxFontSize = 100;
        private const float MinFontSize = 20;
        private const float FontSizeConstant = 10000;

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

        private void UpdateCursor()
        {
            bool cursorShouldBeFree = menuIsOpen || addBodyPanel.IsOpen;

            if (cursorShouldBeFree == cursorIsFree)
            {
                return;
            }
            if (cursorShouldBeFree)
            {
                EnableCursor();
            }
            else
            {
                DisableCursor();
            }
            cursorIsFree = cursorShouldBeFree;
        }

        private void HandleInputs()
        {
            if (IsKeyPressed(KeyboardKey.F11))
            {
                ToggleFullscreen();
            }

            if (addBodyPanel.IsOpen)
            {
                addBodyPanel.HandleAddBodyPanelInput();
                return;
            }

            if (editor.IsAnythingFocused()) // blocks keys from interacting with movement and camera when typing
            {
                return;
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
                simIsPaused = true;
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
            UpdateCursor();

            if (addBodyPanel.BodyReady)
            {
                CreateBody();
                addBodyPanel.ClearBodyReady();
                addBodyPanel.CloseAddBodyPanel();
            }

            if (!menuIsOpen && !addBodyPanel.IsOpen)
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
        }

        private void DrawMainScene()
        {
            BeginDrawing();

            ClearBackground(Color.Black); // aestheritc

            Draw3D();
            DrawBodyLabels();
            if (menuIsOpen) { SideBar(); } // wrote the if statement in one line to keep this method as a clean block of code
            DrawHUD();
            if (addBodyPanel.IsOpen) { addBodyPanel.DrawAddBodyPanel(); }

            EndDrawing();
        }

        private void Draw3D()
        {
            BeginMode3D(freeCamera.Camera);

            Rlgl.SetClipPlanes(1, 100000); //  is raylibs lower level Open Gl abstraction layer.
            // With this line im essentially sending a direct command to the lower level architecture of raylib.
            // Letting me tell it to increase the render distance to 10000 instead of the default 1000s

            // loop to cycle through all bodies and draw all of them
            foreach (Body body in simulation.bodies)
            {
                Vector3 bodyPosition = GetBodyPositionInScene(body);
                float scaledBodyRadius = (float)GetBodyRadiusInScene(body);

                DrawSphere(bodyPosition, scaledBodyRadius, body.BodyColour);
            }

            DrawGrid(1000, 224);

            EndMode3D();
        }

        private double GetBodyRadiusInScene(Body body)
        {
            double ratio = body.Radius / EarthRadiusMetres; // a ratio for bodies smaller and greater than earth

            if (ratio <= 1)
            {
                radius = RadiusDisplayScale * EarthSceneRadius * Math.Pow(ratio, SmallBodyExponent);
            }
            radius = RadiusDisplayScale * EarthSceneRadius * Math.Pow(ratio, LargeBodyExponent);

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
                float bodyRadius = (float)GetBodyRadiusInScene(body);

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
                    float drawXPositionOfLabel = screenPosition.X - (labelWidth / 2);
                    float drawYPositionOfLabel = screenPosition.Y - labelFontSize;

                    DrawText(body.Name, (int)drawXPositionOfLabel, (int)drawYPositionOfLabel, labelFontSize, Color.White);
                }
            }
        }

        private int LabelFontSize(Vector3 labelPosition) // return an appropriate font size respective of the distance of the body to the camera
        {
            float distanceBetweenCamAndLabel = Vector3.Distance(freeCamera.Camera.Position, labelPosition);

            float fontSize = FontSizeConstant / distanceBetweenCamAndLabel;
            fontSize = Math.Clamp(fontSize, MinFontSize, MaxFontSize);
            return (int)fontSize;
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
        }

        private void SideBar()
        {
            int sideBarWidth = GetSideBarWidth();
            float sideBarPosition = GetSideBarPosition();

            int buttonYPosition = 10; // top of list

            DrawRectangle((int)sideBarPosition, 0, sideBarWidth, GetScreenHeight(), new Color(20, 20, 30, 200));

            // RECTANGLE AND IF STATEMENTS WRITTEN BY AI
            Rectangle AddBodyButton = new Rectangle(0, GetScreenHeight() - 45, 210, 43);

            if (!addBodyPanel.IsOpen)
            {
                if (DrawButton(AddBodyButton, "Add Body", 40))
                {
                    addBodyPanel.OpenAddBodyPanel();
                    simIsPaused = true;
                }
            }
            // END OF AI GENERATED CODE

            Rectangle RemoveBodyButton = new Rectangle((int)sideBarPosition - 270, GetScreenHeight() - 45, 270, 43);

            string removeBodyButtonText = removeMode ? "Cancel" : "Remove body";

            if (DrawButton(RemoveBodyButton, removeBodyButtonText, 40)) // remove body function
            {
                if (removeMode)
                {
                    removeMode = false; // pressing same button again exits remove mode, letting the user change their minds
                }
                else
                {
                    simIsPaused = true;
                    removeMode = true;
                    ExpandedBodyIDs.Clear(); // closes all expanded info boxes
                }
            }

            foreach (Body body in simulation.bodies) // to draw all bodies in the side bar
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

        private void CreateBody()
        {
            simulation.AddBody(addBodyPanel.name, addBodyPanel.mass, addBodyPanel.radius, addBodyPanel.bodyColour,
                addBodyPanel.currentXPos, addBodyPanel.currentYPos, addBodyPanel.currentZPos,
                addBodyPanel.currentXVel, addBodyPanel.currentYVel, addBodyPanel.currentZVel);
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

            DrawText($"Mass         :", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            Rectangle massInputBox = new Rectangle(sideBarPos + TextBoxOffset, yPosition, GetSideBarWidth() - TextBoxOffset, lineHeight); // creating a new text box for the mass value
            if (editor.TryEditRow(body.BodyID * 10, body.Mass, massInputBox, 0, out double newMass)) // textBoxID is calculated by the body id so that its a respective quantity
            {
                body.Mass = newMass;
            }
            yPosition += lineHeight;

            DrawText("X Position   :", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            Rectangle xPositionInputBox = new Rectangle(sideBarPos + TextBoxOffset, yPosition, GetSideBarWidth() - TextBoxOffset, lineHeight);
            if (editor.TryEditRow(body.BodyID * 10 + 1, body.CurrentXPos, xPositionInputBox, null, out double newXPosition))
            {
                body.CurrentXPos = newXPosition;
            }
            yPosition += lineHeight;

            DrawText("Y Position   :", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            Rectangle yPositionInputBox = new Rectangle(sideBarPos + TextBoxOffset, yPosition, GetSideBarWidth() - TextBoxOffset, lineHeight);
            if (editor.TryEditRow(body.BodyID * 10 + 2, body.CurrentYPos, yPositionInputBox, null, out double newYPosition))
            {
                body.CurrentYPos = newYPosition;
            }
            yPosition += lineHeight;

            DrawText("Z Position   :", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            Rectangle zPositionInputBox = new Rectangle(sideBarPos + TextBoxOffset, yPosition, GetSideBarWidth() - TextBoxOffset, lineHeight);
            if (editor.TryEditRow(body.BodyID * 10 + 3, body.CurrentZPos, zPositionInputBox, null, out double newZPosition))
            {
                body.CurrentZPos = newZPosition;
            }
            yPosition += lineHeight;

            DrawText("X velocity   :", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            Rectangle xVelocityInputBox = new Rectangle(sideBarPos + TextBoxOffset, yPosition, GetSideBarWidth() - TextBoxOffset, lineHeight);
            if (editor.TryEditRow(body.BodyID * 10 + 4, body.CurrentXVel, xVelocityInputBox, null, out double newXVelocity))
            {
                body.CurrentXVel = newXVelocity;
            }
            yPosition += lineHeight;

            DrawText("Y velocity   :", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            Rectangle yVelocityInputBox = new Rectangle(sideBarPos + TextBoxOffset, yPosition, GetSideBarWidth() - TextBoxOffset, lineHeight);
            if (editor.TryEditRow(body.BodyID * 10 + 5, body.CurrentYVel, yVelocityInputBox, null, out double newYVelocity))
            {
                body.CurrentYVel = newYVelocity;
            }
            yPosition += lineHeight;

            DrawText("Z velocity   :", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            Rectangle zVelocityInputBox = new Rectangle(sideBarPos + TextBoxOffset, yPosition, GetSideBarWidth() - TextBoxOffset, lineHeight);
            if (editor.TryEditRow(body.BodyID * 10 + 6, body.CurrentZVel, zVelocityInputBox, null, out double newZVelocity))
            {
                body.CurrentZVel = newZVelocity;
            }
            yPosition += lineHeight;

            double speed = Math.Sqrt(body.CurrentXVel * body.CurrentXVel + body.CurrentYVel * body.CurrentYVel + body.CurrentZVel * body.CurrentZVel);
            DrawText($"Speed        : {speed:E3}", sideBarPos + 15, yPosition + 2, fontSize, Color.SkyBlue);
            yPosition += lineHeight;

            return yPosition + 15;
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