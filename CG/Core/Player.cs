using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic; // Adicionei para usar Listas... ou alguma merda do tipo...

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

        public Vector3 Size => new Vector3(Scale, Scale, Scale);

        private List<Vector3> bodyVoxels;
        private List<Vector3> legsFrame1;
        private List<Vector3> legsFrame2;
        private List<Vector3> eyeVoxels; // zóio

        private int currentFrame = 0;
        private float animationTimer = 0f;
        private const float voxelSize = 0.1f; // Tamanho de cada quadradinho

        // Variaveis do PISCAR
        private float blinkTimer = 0f;
        private bool isBlinking = false;

        public Player(Shader shader, Vector3 position, Vector3 color)
        {
            this.shader = shader;
            this.Position = position;
            this.color = color;

            DefineDinoModel(); // Chama a função que cria o treco
        }

        private void DefineDinoModel()
        {
            float v = voxelSize;
            // Desenho do Dino (Corpo estatico)
            bodyVoxels = new List<Vector3>
            {
                new Vector3(-v, v, 0), new Vector3(0, v, 0), new Vector3(v, v, 0), // Base corpo
                new Vector3(-v, v*2, 0), new Vector3(0, v*2, 0), new Vector3(v, v*2, 0), new Vector3(v*2, v*2, 0), // Pescoço
                new Vector3(v, v*3, 0), new Vector3(v*2, v*3, 0), new Vector3(v*3, v*3, 0), // Cabeça
                new Vector3(v*3, v*2, 0), // Focinho
                new Vector3(-v*2, v, 0) // Rabinho
            };

            // Perna Frame 1
            legsFrame1 = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(v * 1.5f, v * 0.5f, 0) };
            // Perna Frame 2
            legsFrame2 = new List<Vector3> { new Vector3(-v * 0.5f, v * 0.5f, 0), new Vector3(v, 0, 0) };

            // O L H O (posicionado na cabeça e levemente pra fora em Z)
            eyeVoxels = new List<Vector3> { new Vector3(v * 2.0f, v * 3.2f, v * 0.6f) };
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

            // Logica da Perna
            if (isGrounded)
            {
                animationTimer += (float)deltaTime;
                if (animationTimer > 0.1f) // Velocidade da perna
                {
                    animationTimer = 0;
                    currentFrame = (currentFrame == 0) ? 1 : 0;
                }
            }

            // PISCAR 
            blinkTimer += (float)deltaTime;

            if (!isBlinking)
            {
                if (blinkTimer >= 2.0f)
                {
                    isBlinking = true;
                    blinkTimer = 0f;
                }
            }
            else
            {
                if (blinkTimer >= 0.15f)
                {
                    isBlinking = false;
                    blinkTimer = 0f;
                }
            }
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            shader.Use();
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            // Seleciona a perna que vai desenhar no fiadaputinha
            List<Vector3> currentLegs = (currentFrame == 0) ? legsFrame1 : legsFrame2;

            void DrawList(List<Vector3> voxels, float sizeMultiplier)
            {
                foreach (var v in voxels)
                {
                    Vector3 blockPos = Position + (v * (Scale * 3.5f)) + new Vector3(0, -0.3f, 0);

                    // Aqui entra o sizeMultiplier pra controlar o tamanho do bloco individual
                    Matrix4 model = Matrix4.CreateScale(Scale * voxelSize * 3.5f * sizeMultiplier) * Matrix4.CreateTranslation(blockPos);
                    shader.SetMatrix4("model", model);
                    Utils.DrawCube();
                }
            }

            // 1. Desenha o corpo
            shader.SetVector3("color", color);
            DrawList(bodyVoxels, 1.0f);
            DrawList(currentLegs, 1.0f);

            // 2. Desenha o Olho
            if (!isBlinking)
            {
                shader.SetVector3("color", new Vector3(0f, 0f, 0f)); // PRETO
                DrawList(eyeVoxels, 0.4f);
            }
        }

        public void Reset()
        {
            Position = new Vector3(0, 0.5f, 0);
            velocityY = 0f;
            isGrounded = true;
            currentFrame = 0;
            blinkTimer = 0f;
            isBlinking = false;
        }
    }
}