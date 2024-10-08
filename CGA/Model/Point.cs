﻿using System.Numerics;

namespace CGA.Model
{
    public class Point
    {
        public Point(int x, int y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            NW = 1;
            Normal = Vector3.Zero;
            Texel = Vector3.Zero;
        }

        public Point(int x, int y, float z, float nW, Vector3 normal, Vector3 texel)
        {
            X = x;
            Y = y;
            Z = z;
            NW = nW;
            Normal = Vector3.Normalize(normal);
            Texel = texel;
        }

        public int X { get; }

        public int Y { get; }

        public float Z { get; }

        public float NW { get; }

        public Vector3 Normal { get; }

        public Vector3 Texel { get; }
    }
}
