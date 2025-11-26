using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DinoGame
{
    public class Player
    {
        private Shader shader;
        public Vector3 Position { get; set; }
        private Vector3 color;
        private float velocityY = 0f;
        private bool isGrounded = true;
        public float Scale { get; set; } = 0.7f; //TamanhodoDINOO
        public Vector3 Size => new Vector3(Scale, Scale, Scale); //Colisao

        public Player(Shader shader, Vector3 position, Vector3 color)
        {
            this.shader = shader;
            this.Position = position;
            this.color = color;
        }

        public void Update(double deltaTime, KeyboardState keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Space) && isGrounded)
            {
                velocityY = 4.5f;
                isGrounded = false;
            }

            velocityY -= 9.8f * (float)deltaTime;
            var pos = Position;
            pos.Y += velocityY * (float)deltaTime;
            Position = pos;

            if (Position.Y <= 0.5f)
            {
                Position = new Vector3(Position.X, 0.5f, Position.Z);
                velocityY = 0f;
                isGrounded = true;
            }
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            shader.Use();

            Matrix4 model = Matrix4.CreateScale(Scale) * Matrix4.CreateTranslation(Position + new Vector3(0, Scale / 2f - 0.5f, 0));
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);
            shader.SetVector3("color", color);

            Utils.DrawCube();
        }

        public void Reset()
        { 
            Position = new Vector3(0, 0.5f, 0);
            velocityY = 0f;
            isGrounded = true;
        }
    }
}
