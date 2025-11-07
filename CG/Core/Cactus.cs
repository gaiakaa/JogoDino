using OpenTK.Mathematics;

namespace DinoGame
{
    public class Cactus
    {
        private Shader shader;
        public Vector3 Position { get; set; }
        private Vector3 color = new(1f, 0f, 0f);
        public float Scale { get; set; } = 0.5f; // tamano do cacto

        public Cactus(Shader shader, Vector3 position)
        {
            this.shader = shader;
            this.Position = position;
        }

        public void Update(double deltaTime)
        {
            
            var pos = Position;
            pos.X -= 3f * (float)deltaTime;
            Position = pos;
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
    }
}