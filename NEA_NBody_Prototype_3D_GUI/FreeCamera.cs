using Raylib_cs;
using static Raylib_cs.Raylib;
using System;
using System.Numerics;

namespace RaylibTest
{
    internal class FreeCamera
    {
        // setting up constants and fields
        private Vector3 position;

        private float yaw; // looking left and right
        private float pitch; // looking up and down

        private float cameraSpeed;
        private float mouseSensitivity;

        private const float MaxPitch = 89.9f * MathF.PI / 180; // a value just below 90 degrees in radians
        private Vector3 worldUp = new Vector3(0, 1, 0); // to calculate left right strafing

        private const float MaxSpeed = 1000f;
        private const float MinSpeed = 0.1f;
        private const float SpeedChangeInterval = 1.2f;

        private Camera3D camera; // camera struct or objcect from raylib

        public FreeCamera()
        {
            position = new Vector3(50, 50, 50); // default spawn location

            cameraSpeed = 100.0f; // default speed
            // set it here as it needs to change
            mouseSensitivity = 0.003f; // default radians per pixel

            // initialised as empty values
            yaw = 0; 
            pitch = 0;

            // prepaing the camera object
            camera = new Camera3D(
                position: position, // the Camera's spawn location
                target: new Vector3(0, 0, 0), // where it should be looking when created
                up: new Vector3(0, 1, 0), // its tilt or rotation 
                fovY: 70.0f, // how much it can see in a way. High values spread the image out
                projection: CameraProjection.Perspective); // makes distant objects smaller
        }

        public void Update()
        {
            // initialising and preparing values

            // the distance that the mouse moves in one frame, as it is a vector2 type it has x and y values
            Vector2 changeInMousePos = GetMouseDelta();
            float changeInTime = GetFrameTime(); // self explanatory

            yaw += changeInMousePos.X * mouseSensitivity;
            pitch -= changeInMousePos.Y * mouseSensitivity;

            pitch = Math.Clamp(pitch, -MaxPitch, MaxPitch); // clamp is a function that lets me create boundaries
            // If the user looks straight down or up the pitch value becomes 90 whihc is pi/2.
            // As cos 90 is 0 this prevents the camera from deleting itself due to a NaN error

            Vector3 moveDirection = Vector3.Zero; // an empty 3d vector
            Vector3 forward = GetForwardDirection(); // calls the function that identifies where the camera is looking using pitch and yaw
            
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, worldUp)); 
            // cross product of the up vector and the forward vector provides the perpendicular vector which in this case is right
            Vector3 cameraUp = Vector3.Normalize(Vector3.Cross(right, forward)); 
            // cross product of the right vector and the forward vector provides the up vector respective to what angle the camera is facing
            // this way the camera always moves up and down no matter which way the camera is facing

            float mouseWheel = GetMouseWheelMove();

            if (mouseWheel < 0) // if scroll up
            {
                cameraSpeed *= SpeedChangeInterval; // increase speed
            }

            if (mouseWheel > 0) // if scroll down
            {
                cameraSpeed /= SpeedChangeInterval; // decrease speed
            }

            if (IsKeyPressed(KeyboardKey.R)) // speed reset
            {
                cameraSpeed = 100.0f; // back to original speed
            }

            cameraSpeed = Math.Clamp(cameraSpeed, MinSpeed, MaxSpeed); // same logic as before but to prevent user from flying at lightspeed

            if (IsKeyDown(KeyboardKey.W))
            {
                moveDirection += forward;
            }

            if (IsKeyDown(KeyboardKey.S))
            {
                moveDirection -= forward;
            }

            if (IsKeyDown(KeyboardKey.A))
            {
                moveDirection -= right;
            }

            if (IsKeyDown(KeyboardKey.D))
            {
                moveDirection += right;
            }

            if (IsKeyDown(KeyboardKey.E))
            {
                moveDirection += cameraUp;
            }

            if (IsKeyDown(KeyboardKey.Q))
            {
                moveDirection -= cameraUp;
            }

            if (moveDirection != Vector3.Zero)
            {
                moveDirection = Vector3.Normalize(moveDirection); // normalizing the vectors to keep their magnitudes at 1. makes it easier to calculate and mess around wiht things
                position += moveDirection * cameraSpeed * changeInTime; // update the position using the speed and time in the scene
                // this means that the simulation will play out the same no matter the performance of the computer its running on
            }

            camera.Target = position + forward; 
            camera.Position = position;
        }

        public Camera3D Camera => camera; // properties that other classes can access without writing over them
        // didnt use get set as camera is a struct and deosnt let me treat it that way
        public float CameraSpeed => cameraSpeed; // wanted to practice using properties more so avoided get set for this aswell

        private Vector3 GetForwardDirection() // the function that lets the camera move around
        {
            float x = MathF.Cos(pitch) * MathF.Cos(yaw); // resolving vectors gets you these equations
            float y = MathF.Sin(pitch);
            float z = MathF.Cos(pitch) * MathF.Sin(yaw);

            return new Vector3(x, y, z);
        }

    }

}
