using Godot;
using System;

public class Map : Node2D
{
    private CellularMatrix GlobalMatrix;

    public override void _Ready()
    {
        GlobalMatrix = GetNode<CellularMatrix>("/root/CellularMatrix");
    }

    public override void _PhysicsProcess(float delta)
    {
        Update();
    }

    public override void _Draw()
    {
        foreach (Element.Element element in GlobalMatrix.ElementsToUpdate)
        {
            DrawRect(new Rect2(element.Position.Floor(), Vector2.One), element.ElementColor);
        }
    }
}
