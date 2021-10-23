using Godot;
using System;

namespace Element
{
    public class Direction 
    {
        public static int DOWN = 1;
        public static int UP = 2;
        public static int RIGHT = 4;
        public static int LEFT = 8;

        public static int DirToV(Vector2 v)
        {
            if (v == Vector2.Down)                  return DOWN;
            if (v == Vector2.Up)                    return UP;
            if (v == Vector2.Right)                 return RIGHT;
            if (v == Vector2.Left)                  return LEFT;
            if (v == Vector2.Down + Vector2.Right)  return DOWN | RIGHT;
            if (v == Vector2.Down + Vector2.Left)   return DOWN | LEFT;
            if (v == Vector2.Up + Vector2.Right)    return UP   | RIGHT;
            if (v == Vector2.Up + Vector2.Left)     return UP   | LEFT;
            return 0;
        }

        public static Vector2 VToDir(int d)
        {
            if (d == DOWN)              return Vector2.Down;
            if (d == UP)                return Vector2.Up;
            if (d == RIGHT)             return Vector2.Right;
            if (d == LEFT)              return Vector2.Left;
            if (d == (DOWN | RIGHT))    return Vector2.Down + Vector2.Right;
            if (d == (DOWN | LEFT))     return Vector2.Down + Vector2.Left;
            if (d == (UP | RIGHT))      return Vector2.Up   + Vector2.Right;
            if (d == (UP | LEFT))       return Vector2.Up   + Vector2.Left;
            return Vector2.Zero;
        }
    }

    public enum MoveOption
    {
        STOP,
        REPLACE,
        SWAP,
        SKIP
    }

    public abstract class Element : Resource
    {
        // ============= Field Declarations =============
        // A float position in the world (rounded when drawn)
        public Vector2 Position;
        
        // Color to render when drawn
        public Color ElementColor = new Color("#ff0000");

        // How much gravity affects element (Density * Gravity)
        // Also affects how it interacts with liquids
        public float Density = 1;

        // How element falls (in pixels per second)
        // NOTE: Since pixels move in screen space (y-axis goes down) gravity that would normally be
        //      positive is negative. 
        public Vector2 Gravity = new Vector2(0, 9.8f);

        // Velocity in Pixels/Second in screen space (Velocity of (1, 0) goes 1 pixel per second down)
        public Vector2 Velocity = Vector2.Zero;

        // ============= Signal Declarations =============
        // Emits when a position change is requested, which is then handled by the CellularMatrix
        [Signal]
        delegate void RequestPositionChange(Vector2 NewPosition);

        public Element(Vector2 Position)
        {
            this.Position = Position;
        }

        public Element() : this(Vector2.Zero) { }

        // Ran once a frame, delta is 1/FPS, so 10 * delta would accumulate to 10 every second independent of framerate
        public abstract void Step(Element[,] radius, float delta);

        // Outputs rules for element based on Direction and element. Runs when an element is trying to move into an already populated position.
        public abstract MoveOption OnInteraction(Element element, int direction);
    }

    public class Empty : Element
    {
        public Empty() : base(new Vector2(-1, -1)) { }

        public override void Step(Element[,] radius, float delta) { }

        public override MoveOption OnInteraction(Element element, int direction)
        {
            return MoveOption.STOP;
        }
    }
}