using Godot;
using System;


namespace Element
{
    public class TestElement : Element
    {
        public TestElement (Vector2 Position) : base(Position) 
        {
            ElementColor = new Color("#00ff00");
            Density = 1;
        }

        public override void Step(Element[,] radius, float delta)
        {
            if (radius[1, 2] is Empty)
            {
                Velocity = (Gravity * Density);
            }
            else if (radius[0, 2] is Empty)
            {
                GD.Print("Left");
                Velocity = (Vector2.Down + Vector2.Left) * (Density);
            }
            else if (radius[2, 2] is Empty)
            {
                Velocity = (Vector2.Down + Vector2.Right) * (Density);
            }
        }

        public override MoveOption OnInteraction(Element element, int direction)
        {
            return element is Empty ? MoveOption.REPLACE : MoveOption.STOP;
        }
    }

    public class TestElementSolid : Element
    {
        public TestElementSolid (Vector2 Position) : base(Position)
        {
            ElementColor = new Color("#000000");
            Density = 1;
        }

        public override void Step(Element[,] radius, float delta) { }

        public override MoveOption OnInteraction(Element element, int direction)
        {
            return MoveOption.STOP;
        }
    }
}