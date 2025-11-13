using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoGame
{
    public class Background3D
    {
        private readonly Shader shader;
        private readonly int vao;
        private readonly int vbo;
        private readonly Vector3 color;
        private readonly float speed;
        private float offset;

        public Vector3 Position { get; private set; }

        public Background3D(Shader shader, Vector3 position, Vector3 color, float speed, bool isMountain = false)
        {
            this.shader = shader;
            this.Position = position;
            this.color = color;
            this.speed = speed;
            //chekagem mucho loka e loopp
            float[] vertices = isMountain ? CreateMountainVertices() : CreateFlatVertices();

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
        public void Update(float deltaTime)
        {
            offset += speed * deltaTime;
            Position = new Vector3(Position.X - speed * deltaTime, Position.Y, Position.Z);

            if (Position.X < -20f)
                Position = new Vector3(20f, Position.Y, Position.Z);
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            shader.Use();
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            Matrix4 model = Matrix4.CreateTranslation(Position);
            shader.SetMatrix4("model", model);
            shader.SetVector3("color", color);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 9);
        }
        //fazer montanha pontuda
        private float[] CreateMountainVertices()
        {
            return new float[]
            {
                -15f,  0f, 0f,
                -10f,  3f, 0f,
                 -5f,  0f, 0f,

                 -5f,  0f, 0f,
                 0f,  5f, 0f,
                 5f,  0f, 0f,

                 5f,  0f, 0f,
                 10f,  3f, 0f,
                 15f,  0f, 0f
            };
        }
        //Fundo (Pra nuvem)
        private float[] CreateFlatVertices()
        {
            return new float[]
                {
                    -15f, 0f, 0f,
                    15f, 0f, 0f,
                    15f, 5f, 0f,

                    -15f, 0f, 0f,
                    15f, 5f, 0f,
                    -15f, 5f, 0f
                };
        }
    }
}
