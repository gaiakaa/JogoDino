#version 330 core

out vec4 FragColor;
uniform vec3 color;
uniform float brightness;

void main()
{
    vec3 finalColor = color * brightness;
    FragColor = vec4(finalColor, 1.0);
}