using UnityEngine;
using System.Collections.Generic;

// Theory and logic adapted from https://www.redblobgames.com/grids/hexagons/
namespace RR.Utility.Hex
{
    [System.Serializable]
    public struct HexCoords
    {
        public static HexCoords zero { get { return new HexCoords(); } }
        public int x;
        public int y;
        public int z;

        // public HexCoords() => new HexCoords(0, 0, 0);
        public HexCoords(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public HexCoords Neighbour(int direction)
        {
            return this + Hex.Direction(direction);
        }

        public HexCoords Scale(int radius)
        {
            this.x *= radius;
            this.y *= radius;
            this.z *= radius;

            return this;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static HexCoords operator+(HexCoords a, HexCoords b)
        {
            return new HexCoords(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static HexCoords operator-(HexCoords a, HexCoords b)
        {
            return new HexCoords(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static HexCoords operator*(HexCoords a, int scale)
        {
            return new HexCoords(a.x * scale, a.y * scale, a.z * scale);
        }

        public static HexCoords operator*(HexCoords a, HexCoords b)
        {
            return new HexCoords(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public override string ToString() 
        {
            return $"({x}, {y}, {z})";
        }
    }

    public static class Hex
    {
        private static HexCoords[] _directionTable = new HexCoords[6]
        {
            new HexCoords(1, -1, 0),
            new HexCoords(1, 0, -1),
            new HexCoords(0, 1, -1),
            new HexCoords(-1, 1, 0),
            new HexCoords(-1, 0, 1),
            new HexCoords(0, -1, 1),
        }; 

        public static HexCoords Direction(int direction)
        {
            if (direction < 0 || direction >= _directionTable.Length)
            {
                return new HexCoords();
            }

            return _directionTable[direction];
        }

        public static List<HexCoords> MakeRing(int radius)
        {
            var results = new List<HexCoords>();

            if (radius == 0) 
            {
                results.Add(HexCoords.zero); 
                return results;
            }

            var coords = Hex.Direction(4).Scale(radius);

            for (var side = 0; side < 6; side++) {
                for (var step = 0; step < radius; step++) {
                    results.Add(coords);
                    coords = coords.Neighbour(side);
                }
            }

            return results;
        }
    }
}