using Godot;
using System;

public class World : Node2D
{
    private Canvas[,] canvases;
    private CellularMatrix GlobalMatrix;
    private Node2D map;
    private Camera2D camera;

    public override void _Ready()
    {
        GlobalMatrix = GetNode<CellularMatrix>("/root/CellularMatrix");
        map = GetNode<Node2D>("Map");
        camera = GetNode<Camera2D>("RTSCamera2D");

        Node2D grid = GetNode<Node2D>("Grid");
        grid.Set("size", CellularMatrix.WorldSize);
        grid.Set("chunk_size", CellularMatrix.ChunkSize);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            if (((InputEventMouseButton)@event).Pressed)
            {
                if (((InputEventMouseButton)@event).ButtonIndex == (int)ButtonList.Left)
                {
                    Element.Element e = new Element.TestElement(GetGlobalMousePosition().Floor());
                    GlobalMatrix.RegisterElement(e);
                }
                else if (((InputEventMouseButton)@event).ButtonIndex == (int)ButtonList.Right)
                {
                    Element.Element e = new Element.TestElementSolid(GetGlobalMousePosition().Floor());
                    GlobalMatrix.RegisterElement(e);
                }
            }
        }
    }
}
