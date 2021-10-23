using System;
using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class CellularMatrix : Node
{

    public static Vector2 WorldSize = new Vector2(320, 180);

    public static Vector2 ChunkSize = new Vector2(64, 64);

    public Array<Element.Element> ElementsToUpdate = new Array<Element.Element>();
    
    // private Element.Element[,] Elements = new Element.Element[(int)WorldSize.x, (int)WorldSize.y];

    private System.Collections.Generic.Dictionary<Vector2, Element.Element> Elements = new System.Collections.Generic.Dictionary<Vector2, Element.Element>();

    public CellularMatrix()
    {
    }

    public override void _PhysicsProcess(float delta)
    {
        // Process Elements
        // Process Particles
        // Draw Element Matrix
        foreach (Element.Element element in ElementsToUpdate)
        {
            Element.Element[,] r = new Element.Element[3,3];
            for (int x = Mathf.FloorToInt(element.Position.x)-1; x < Mathf.FloorToInt(element.Position.x)+2; x++)
            {
                for (int y = Mathf.FloorToInt(element.Position.y)-1; y < Mathf.FloorToInt(element.Position.y)+2; y++)
                {
                    if (x >= 0 && x < WorldSize.x && y >= 0 && y < WorldSize.y)
                    {
                        
                        r[x-Mathf.FloorToInt(element.Position.x)+1, y-Mathf.FloorToInt(element.Position.y)+1] = (Element.Element)GetElementOrDefault(new Vector2(x, y), new Element.Empty());
                    }
                    else
                    {
                        r[x-Mathf.FloorToInt(element.Position.x)+1, y-Mathf.FloorToInt(element.Position.y)+1] = null;
                    }
                }
            }
            // GD.Print((int)(Mathf.Min(Mathf.FloorToInt(element.Position.x)+2, WorldSize.x)-Mathf.Max(Mathf.FloorToInt(element.Position.x)-1, 0)), ", ", (int)(Mathf.Min(Mathf.FloorToInt(element.Position.y)+2, WorldSize.y)-Mathf.Max(Mathf.FloorToInt(element.Position.y)-1, 0)));
            element.Step(r, delta);

            GD.Print(Elements.ContainsKey(element.Position.Floor()));
            ElementPositionChange(element.Position + element.Velocity * delta, element);
            // element.Position += element.Velocity;
        }
    }

    // Adds Element to pool of available elements, used for neighbor checks as well as recieving updates
    // returns if addition was successful
    public bool RegisterElement(Element.Element element)
    {
        if (ElementsToUpdate.Contains(element)) return false;

        ElementsToUpdate.Add(element);
        Elements.Add(element.Position.Floor(), element);
        GD.Print(Elements.ContainsKey(element.Position.Floor()));   
        // Elements[Mathf.FloorToInt(element.Position.x), Mathf.FloorToInt(element.Position.y)] = element;
        // element.Connect("RequestPositionChange", this, "ElementPositionChange", new Godot.Collections.Array { element });
        return true;
    }

    private void ElementPositionChange(Vector2 NewPosition, Element.Element element)
    {
        if (NewPosition == element.Position) return;
        if (NewPosition.x >= 0 && NewPosition.x < WorldSize.x && NewPosition.y >= 0 && NewPosition.y < WorldSize.x)
        {
            GD.Print(Elements.ContainsKey(element.Position.Floor()));
            var temp = Elements[element.Position.Floor()];
            switch(element.OnInteraction((Element.Element)GetElementOrDefault(NewPosition, new Element.Empty()), Element.Direction.DirToV(NewPosition.Floor() - element.Position.Floor())))
            {
                case Element.MoveOption.REPLACE:
                    SetOrAdd(NewPosition.Floor(), element);
                    Elements.Remove(element.Position.Floor());
                    break;
                case Element.MoveOption.SWAP:
                    if (GetElementOrDefault(NewPosition, new Element.Empty()) is Element.Empty)
                    {
                        SetOrAdd(NewPosition.Floor(), element);
                        Elements.Remove(element.Position.Floor());
                    }
                    else
                    {
                        Elements[element.Position.Floor()] = Elements[NewPosition.Floor()];
                        Elements[NewPosition.Floor()] = temp;
                    }
                    break;
                case Element.MoveOption.STOP:
                    NewPosition = element.Position;
                    break;
                    
            }
            element.Position = NewPosition;
        }
    }

    private object GetElementOrDefault(Vector2 Position, object Default)
    {
        if (Elements.ContainsKey(Position))
        {
            return Elements[Position];
        }
        else
        {
            return Default;
        }
    }

    private void SetOrAdd(Vector2 Position, Element.Element element)
    {
        if (Elements.ContainsKey(Position))
        {
            Elements[Position] = element;
        }
        else
        {
            Elements.Add(Position, element);
        }
    }
}