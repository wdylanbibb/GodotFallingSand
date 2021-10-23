using Godot;
using System;
using Godot.Collections;


// Straight ripped from http://www.tomgibara.com/computer-vision/marching-squares

namespace MarchingSquares
{
    public class Direction
    {
        public static Direction East = new Direction(Vector2.Right);
        public static Direction North = new Direction(Vector2.Up);
        public static Direction West = new Direction(Vector2.Left);
        public static Direction South = new Direction(Vector2.Down);

        public Vector2 Screen;

        public float Length;

        private Direction(int x, int y)
        {
            Screen = new Vector2(x, y);
            Length = x != 0 && y != 0 ? Mathf.Sqrt(2) / 2 : 1;
        }

        private Direction(Vector2 v)
        {
            Screen = new Vector2(v.x, v.y);
            Length = v.x != 0 && v.y != 0 ? Mathf.Sqrt(2) / 2 : 1;
        }

        private Direction()
        {
            Screen = Vector2.Zero;
            Length = 1;
        }

        public static implicit operator Vector2(Direction d) => new Vector2(d.Screen.x, d.Screen.y);
        public static implicit operator Direction(Vector2 v) => new Direction(v);
    }

    public class MarchingSquares : Reference
    {
        

        public int[] Data;

        public int Width;

        public int Height;

        public MarchingSquares(int width, int height, int[] data)
        {
            this.Width = width;
            this.Height = height;
            this.Data = data;
        }

        public MarchingSquares() : this(0, 0, new int[] { }) { }

        public MarchingSquaresPath IdentifyPerimeter(int initialX, int initialY)
        {
            if (initialX < 0) initialX = 0;
            if (initialX > Width) initialX = Width;
            if (initialY < 0) initialY = 0;
            if (initialY > Height) initialY = Height;

            int initialValue = value(initialX, initialY);
            if (initialValue == 0 || initialValue == 15)
                GD.PrintErr($"MarchingSquares: Supplied initial coordinates ({initialX}, {initialY}) do not lie on a perimeter.");

            Array<Vector2> directions = new Array<Vector2>();

            int x = initialX;
            int y = initialY;
            Direction previous = null;

            do {
                Direction direction = Direction.South;
                switch (value(x, y)) {
                    case  1: direction = Direction.North; break;
                    case  2: direction = Direction.East; break;
                    case  3: direction = Direction.East; break;
                    case  4: direction = Direction.West; break;
                    case  5: direction = Direction.North; break;
                    case  6: direction = previous == Direction.North ? Direction.West : Direction.East; break;
                    case  7: direction = Direction.East; break;
                    case  8: direction = Direction.South; break;
                    case  9: direction = previous == Direction.East ? Direction.North : Direction.South; break;
                    case 10: direction = Direction.South; break;
                    case 11: direction = Direction.South; break;
                    case 12: direction = Direction.West; break;
                    case 13: direction = Direction.North; break;
                    case 14: direction = Direction.West; break;
                }
                Vector2 v = direction;
                directions.Add(v);
                x += (int)direction.Screen.x;
                y += (int)direction.Screen.y; // accomodate change of basis
                previous = direction;
            } while (x != initialX || y != initialY);

            return new MarchingSquaresPath(new Vector2(initialX, initialY), directions);
        }

        public MarchingSquaresPath IdentifyPerimeter()
        {
            int size = Width * Height;
            for (int i = 0; i < size; i++) {
                if (Data[i] != 0) {
                    return IdentifyPerimeter(i % Width, i / Width);
                }
            }
            return null;
        }

        private int value(int x, int y)
        {
            int sum = 0;
            if (isSet(x, y)) sum |= 1;
            if (isSet(x + 1, y)) sum |= 2;
            if (isSet(x, y + 1)) sum |= 4;
            if (isSet(x + 1, y + 1)) sum |= 8;
            return sum;
        }

        private bool isSet(int x, int y)
        {
            return x <= 0 || x > Width || y <= 0 || y > Height ? false : Data[(y - 1) * Width + (x - 1)] != 0;
	    }
    }

    public class MarchingSquaresPath : Reference
    {
        public Array<Vector2> Directions;

        public float Length;

        public Vector2 Origin;

        public Vector2 Terminal;

        private MarchingSquaresPath(MarchingSquaresPath that, Vector2 delta)
        {
            this.Directions = that.Directions;
            this.Length = that.Length;
            this.Origin = that.Origin + delta;
            this.Terminal = that.Terminal + delta;
        }

        public MarchingSquaresPath(Vector2 start, Array<Vector2> directions)
        {
            this.Origin = start;
            this.Directions = new Array<Vector2>(directions);

            Vector2 end = start;
            int diagonals = 0;
            foreach (Vector2 direction in directions)
            {
                end += direction;
                if (direction.x != 0 && direction.y != 0) {
                    diagonals++;
                }
            }

            this.Terminal = end;

            this.Length = directions.Count + diagonals * (Mathf.Sqrt(2) / 2 - 1);
        }

        public MarchingSquaresPath() : this(Vector2.Zero, new Array<Vector2>()) { }

        public bool IsClosed()
        {
		    return Origin.x == Terminal.x && Origin.y == Terminal.y;
	    }

        public override int GetHashCode()
        {
            return (int)Origin.x ^ 7 * (int)Origin.y ^ Directions.GetHashCode();
        }

        public override string ToString()
        {
            return $"X: {Origin.x}, Y: {Origin.y} {Directions.ToString()}";
        }
    }
}
