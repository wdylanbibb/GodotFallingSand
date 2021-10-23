using Godot;
using System;

public class Chunk : Node2D
{

    public int Width;

    public int Height;

    public Chunk(int Width, int Height)
    {
        this.Width = Width;
        this.Height = Height;
    }
}