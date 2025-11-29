using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace DinoGame
{
    public class Shader
    {
        public int Handle { get; private set; } 

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, OpenTK.Mathematics.Matrix4 data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref data);
        }

        public void SetVector3(string name, OpenTK.Mathematics.Vector3 data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(location, data);
        }
        public void SetFloat(string name, float data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, data);
        }
    }
}