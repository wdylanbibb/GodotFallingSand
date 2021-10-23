using Godot;
using System;
using Godot.Collections;

public class Canvas : Sprite
{

    [Export]
    private int Width;

    [Export]
    private int Height;

    public Canvas (int Width, int Height)
    {
        this.Width = Width;
        this.Height = Height;
    }

    public Canvas () : this(0, 0) { }

    public override void _Ready()
    {
        Centered = false;

        Image image = new Image();
        image.Create(Width, Height, false, Image.Format.Rgba8);

        ImageTexture itex = new ImageTexture();
        itex.CreateFromImage(image, 0);

        Texture = itex;
    }

    public override void _PhysicsProcess(float delta)
    {
    }

    public void SetPixel(int x, int y, Color color)
    {
        Image image = Texture.GetData();
        image.Lock();
        image.SetPixel(x, y, color);
        image.Unlock();
        ((ImageTexture)Texture).CreateFromImage(image, 0);
    }

    public void SetPixel(Vector2 v, Color color)
    {
        Image image = Texture.GetData();
        image.Lock();
        image.SetPixelv(v, color);
        image.Unlock();
        ((ImageTexture)Texture).CreateFromImage(image, 0);
    }

    public void SetSize(Vector2 size)
    {
        Width = (int)size.x;
        Height = (int)size.y;
    }
}