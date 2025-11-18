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
        private readonly Vector3 scale;
        private readonly int vertexCount;
        private readonly int tileCount;
        private const float baseWidth = 30f;
        public Vector3 Position { get; private set; }

        public Background3D(Shader shader, Vector3 position, Vector3 color, float speed, Vector3 scale, int tileCount = 3, bool isMountain = false)
        {
            this.shader = shader;
            this.Position = position;
            this.color = color;
            this.speed = speed;
            this.scale = scale;
            this.tileCount = Math.Max(1, tileCount);

            //chekagem mucho loka e loopp
            float[] vertices = isMountain ? CreateMountainVertices() : CreateCloudVertices();
            vertexCount = vertices.Length / 3;

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

            Position += new Vector3(-speed * deltaTime, 0f, 0f);

            float tileWidth = baseWidth * scale.X;
            float fullWidth = tileWidth * tileCount;
            // se saiu à esquerda, aparece  na direita (assim nao explode)

            if (Position.X <= -fullWidth)
                Position = new Vector3(Position.X + fullWidth, Position.Y, Position.Z);
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            shader.Use();
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            float tileWidth = baseWidth * scale.X;
            
            // desenha cada tile em sequência (tileCount cópias)
            for (int i = 0; i < tileCount; i++)
            {
                var worldPos = new Vector3(Position.X + i * tileWidth, Position.Y, Position.Z);
                Matrix4 model = Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(worldPos);
                shader.SetMatrix4("model", model);
                shader.SetVector3("color", color);
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
            }
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
                 0f,  2f, 0f,
                 5f,  0f, 0f,

                 5f,  0f, 0f,
                 10f,  3f, 0f,
                 15f,  0f, 0f
            };
        }
        //Fundo (Pra nuvem)
        private float[] CreateCloudVertices()
        {
            return new float[]
                {
                    -2f, 0f, 0f,
                     2f, 0f, 0f,
                     2f, 1f, 0f,

                    -2f, 0f, 0f,
                     2f, 1f, 0f,
                    -2f, 1f, 0f
                };
        }
    }
}
