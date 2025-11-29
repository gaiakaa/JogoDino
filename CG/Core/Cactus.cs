using OpenTK.Mathematics;
using System.Collections.Generic;

namespace DinoGame
{
    public class Cactus
    {
        private Shader shader;
        public Vector3 Position { get; set; }

        // cores
        private Vector3 greenColor = new(0.1f, 0.6f, 0.1f); // cacto
        private Vector3 sandColor = new(0.3f, 0.3f, 0.2f);  // monte de terra
        private Vector3 spineColor = new(0.9f, 0.9f, 0.7f); // espinhos

        public float Scale { get; set; } = 0.4f; // Scale geral do cacto
        public Vector3 Size => new Vector3(Scale, Scale, Scale); //colisoa

        // modelo dele
        private List<Vector3> cactusVoxels; // O corpo verde
        private List<Vector3> baseVoxels;   // A terra embaixo
        private List<Vector3> spineVoxels;  // Os espinhos

        public Cactus(Shader shader, Vector3 position)
        {
            this.shader = shader;
            this.Position = position;
            DefineCactusModel();
        }

        private void DefineCactusModel()
        {
            float v = 0.15f;
            float s = 0.02f;

            // base de terra
            baseVoxels = new List<Vector3> { new Vector3(0, 0, 0) };

            // cacto
            cactusVoxels = new List<Vector3>
            {
                // tronco
                new Vector3(0, v, 0),
                new Vector3(0, v*2, 0),
                new Vector3(0, v*3, 0),
                new Vector3(0, v*4, 0),
                new Vector3(0, v*5, 0), 

                // Braco Esquerdo
                new Vector3(-v, v*2, 0),
                new Vector3(-v, v*3, 0), 

                // Braco Direito
                new Vector3(v, v*4, 0),
                new Vector3(v, v*5, 0)
            };

            // espinhos
            spineVoxels = new List<Vector3>
            {
                // tronco
                new Vector3(v/2+s, v*1.5f, v/2+s),
                new Vector3(-v/2-s, v*2.5f, -v/2-s),
                new Vector3(0, v*4.5f, v/2+s),

                // braço esquerdo
                new Vector3(-v*1.5f-s, v*2.5f, 0),
                new Vector3(-v-s, v*3.5f, v/2+s),

                // braço direito
                new Vector3(v*1.5f+s, v*4.5f, 0),
                new Vector3(v+s, v*5.5f, -v/2-s)
            };
        }

        public void Update(double deltaTime)
        {
            var pos = Position;
            pos.X -= 5f * (float)deltaTime;
            Position = pos;
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            shader.Use();
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            float baseHeight = Scale * 0.5f;
            float groundFixY = -0.5f + (baseHeight / 2f);
            Vector3 offsetFix = new Vector3(0, groundFixY, 0);

            void DrawPart(List<Vector3> voxels, Vector3 partColor, Vector3 individualScale)
            {
                shader.SetVector3("color", partColor);
                foreach (var v in voxels)
                {
                    Vector3 blockPos = Position + (v * (Scale * 2.5f)) + offsetFix;

                    Matrix4 model = Matrix4.CreateScale(individualScale) * Matrix4.CreateTranslation(blockPos);
                    shader.SetMatrix4("model", model);
                    Utils.DrawCube();
                }
            }
            DrawPart(baseVoxels, sandColor, new Vector3(Scale * 1.5f, Scale * 0.5f, Scale * 1.5f));
            DrawPart(cactusVoxels, greenColor, new Vector3(Scale * 0.5f));
            DrawPart(spineVoxels, spineColor, new Vector3(Scale * 0.1f));
        }
    }
}