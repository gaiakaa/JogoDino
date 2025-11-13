using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace DinoGame
{
    public class DinoGame : GameWindow
    {
        private Camera camera = null!;
        private Player player = null!;
        private Ground ground = null!;
        private Shader shader = null!;

        private List<Cactus> cactuses = new();
        private float cactusSpawnTimer = 0f;
        private float points = 0f; // fiz uma pontucao pelo tempo jogado, pra poder mudar o cenario d dia pra noitee75
        private bool isNight = false;
        private Background3D backgroundNuvens;
        private Background3D backgroundMontanhas;

        public DinoGame(GameWindowSettings gws, NativeWindowSettings nws)
            : base(gws, nws) { }


        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(Color4.SkyBlue);
            GL.Enable(EnableCap.DepthTest);

            shader = new Shader("assets/shaders/vertex.glsl", "assets/shaders/fragment.glsl");
            camera = new Camera(new Vector3(0, 2, 6), Vector3.Zero);
            player = new Player(shader, new Vector3(0, 0.5f, 0), new Vector3(0, 1, 0));
            ground = new Ground(shader);

            cactuses.Add(new Cactus(shader, new Vector3(5, 0.5f, 0)));
            //PARALLAAAAAAXXXX
            backgroundNuvens = new Background3D(shader, new Vector3(0f, 2.5f, -4f), new Vector3(1f, 1f, 1f), 1.0f, false);
            backgroundMontanhas = new Background3D(shader, new Vector3(0f, -1f, -5f), new Vector3(0.3f, 0.25f, 0.2f), 0.5f, true);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            Title = $"Dino Game - Pontos: {(int)points}"; //OS PONTOS TAO NA JANELA KJGKGJKGH

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            player.Update(args.Time, KeyboardState);

            cactusSpawnTimer += (float)args.Time;
            if (cactusSpawnTimer > 2.5f)
            {
                cactusSpawnTimer = 0;
                cactuses.Add(new Cactus(shader, new Vector3(5, 0.5f, 0)));
            }

            for (int i = cactuses.Count - 1; i >= 0; i--)
            {
                cactuses[i].Update(args.Time);
                if (cactuses[i].Position.X < -6)
                    cactuses.RemoveAt(i);
            }

            points += 10f * (float)args.Time; //Contagem d Pointes

            if (points % 1000 >= 500) //Troca Dia pra noite a cada 500 pointes
                isNight = true;
            else
                isNight = false;

            backgroundNuvens.Update((float)args.Time);
            backgroundMontanhas.Update((float)args.Time);

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            float cyclePoints = points % 1000;
            float t = cyclePoints < 500
                ? cyclePoints / 500f
                : 1f - ((cyclePoints - 500) / 500f);

            var dayColor = new Color4(0.53f, 0.81f, 0.92f, 1.0f); // Day
            var nightColor = new Color4(0.05f, 0.05f, 0.1f, 1.0f); // Night

            float Lerp(float a, float b, float t) => a + (b - a) * t;

            var currentColor = new Color4(
                Lerp(dayColor.R, nightColor.R, t),
                Lerp(dayColor.G, nightColor.G, t),
                Lerp(dayColor.B, nightColor.B, t),
                1.0f
            );

            GL.ClearColor(currentColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix(Size.X / (float)Size.Y);

            ground.Render(view, projection);
            player.Render(view, projection);

            foreach (var cactus in cactuses)
                cactus.Render(view, projection);

            backgroundMontanhas.Render(view, projection);
            backgroundNuvens.Render(view, projection);

            SwapBuffers();
        }
    }

}
