using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using SharpFAI.Util;

namespace SharpFAI.Framework;

public class Floor
{
    public static float length = 50;
    public static float width = 50 / 2f;
    public float outline = 2f;
    
    public readonly double entryAngle;
    public readonly double exitAngle;
    public float angle;
    public bool isMidspin;
    public bool isCW;

    public Floor lastFloor;
    public Floor nextFloor;

    public double bpm;
    public double entryTime;

    public Vector2 position;
    public Polygon floorPolygon;
    
    public Floor(double entryAngle, double exitAngle,Vector2 position)
    {
        this.entryAngle = entryAngle;
        this.exitAngle = exitAngle;
        this.position = position;
        isMidspin = false;
    }

    public Polygon GeneratePolygons()
    {
        if (!floorPolygon.IsEmpty()) return floorPolygon;
        this.floorPolygon = isMidspin ? CreateMidspinPolygons(entryAngle.ToFloat()) : CreateFloorPolygons();
        return this.floorPolygon;
    }
    
    private Polygon CreateFloorPolygons() 
    {
        float length = Floor.length;
        float width = Floor.width;
        List<Vector3> vertices = [];
        List<int> triangles = [];
        List<Color> colors = [];

        #region 基础处理
        float m11 = MathF.Cos(entryAngle / 180f * (float)MathF.PI);
        float m12 = MathF.Sin(entryAngle / 180f * (float)MathF.PI);
        float m21 = MathF.Cos(exitAngle / 180f * (float)MathF.PI);
        float m22 = MathF.Sin(exitAngle / 180f * (float)MathF.PI);
        float[] a = new float[2];

        if ((entryAngle - exitAngle).Fmod(360f) >= (exitAngle - entryAngle).Fmod(360f)) {
            a[0] = entryAngle.Fmod(360f).ToFloat() * (float)MathF.PI / 180f;
            a[1] = a[0] + (exitAngle - entryAngle).Fmod(360f).ToFloat() * (float)MathF.PI / 180f;
        } else {
            a[0] = exitAngle.Fmod(360f).ToFloat() * (float)MathF.PI / 180f;
            a[1] = a[0] + (entryAngle - exitAngle).Fmod(360f).ToFloat() * (float)MathF.PI / 180f;
        }
        float angle = a[1] - a[0];
        float mid = a[0] + angle / 2f;
        #endregion
        if (angle < 2.0943952f && angle > 0) 
        {
            #region 角度小于2.0943952
            float x;
            if (angle < 0.08726646f) {
                x = 1f;
            } else if (angle < 0.5235988f) {
                x = MathF.Lerp(1f, 0.83f, MathF.Pow((angle - 0.08726646f) / 0.43633235f, 0.5f));
            } else if (angle < 0.7853982f) {
                x = MathF.Lerp(0.83f, 0.77f, MathF.Pow((angle - 0.5235988f) / 0.2617994f, 1f));
            } else if (angle < 1.5707964f) {
                x = MathF.Lerp(0.77f, 0.15f, MathF.Pow((angle - 0.7853982f) / 0.7853982f, 0.7f));
            } else {
                x = MathF.Lerp(0.15f, 0f, MathF.Pow((angle - 1.5707964f) / 0.5235988f, 0.5f));
            }
            float distance;
            float radius;
            if (x == 1f) {
                distance = 0f;
                radius = width;
            } else {
                radius = MathF.Lerp(0f, width, x);
                distance = (width - radius) / MathF.Sin(angle / 2f);

            }
            float circlex = -distance * MathF.Cos(mid);
            float circley = -distance * MathF.Sin(mid);
            width += outline;
            length += outline;
            radius += outline;
            GraphicUtils.CreateCircle(new Vector3(circlex, circley, 0), radius, Color.Black, vertices, triangles, colors, 0);
            {
                int count = vertices.Count;
                vertices.Add(new Vector3(-radius * MathF.Sin(a[1]) + circlex, radius * MathF.Cos(a[1]) + circley, 0));
                vertices.Add(new Vector3(circlex, circley, 0));
                vertices.Add(new Vector3(radius * MathF.Sin(a[0]) + circlex, -radius * MathF.Cos(a[0]) + circley, 0));
                vertices.Add(new Vector3(width * MathF.Sin(a[0]), -width * MathF.Cos(a[0]), 0));
                vertices.Add(Vector3.Zero);
                vertices.Add(new Vector3(-width * MathF.Sin(a[1]), width * MathF.Cos(a[1]), 0));
                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 5);
                triangles.Add(count + 4);
                triangles.Add(count + 1);
                triangles.Add(count + 5);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count + 4);
                triangles.Add(count + 1);
                triangles.Add(count + 3);
                triangles.Add(count + 4);
                for (int i = 0; i < 6; i++) colors.Add(Color.Black);
            }
            {
                int count = vertices.Count;
                vertices.Add(new Vector3(length * m11 + width * m12, length * m12 - width * m11, 0));
                vertices.Add(new Vector3(length * m11 - width * m12, length * m12 + width * m11, 0));
                vertices.Add(new Vector3(-width * m12, width * m11, 0));
                vertices.Add(new Vector3(width * m12, -width * m11, 0));

                vertices.Add(new Vector3(length * m21 + width * m22, length * m22 - width * m21, 0));
                vertices.Add(new Vector3(length * m21 - width * m22, length * m22 + width * m21, 0));
                vertices.Add(new Vector3(-width * m22, width * m21, 0));
                vertices.Add(new Vector3(width * m22, -width * m21, 0));
                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 2);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count);
                triangles.Add(count + 4);
                triangles.Add(count + 5);
                triangles.Add(count + 6);
                triangles.Add(count + 6);
                triangles.Add(count + 7);
                triangles.Add(count + 4);
                for (int i = 0; i < 8; i++) colors.Add(Color.Black);
            }
            #endregion
            #region 边框
            width -= outline * 2f;
            length -= outline * 2f;
            radius -= outline * 2f;
            if (radius < 0) {
                radius = 0;
                circlex = -width / MathF.Sin(angle / 2f) * MathF.Cos(mid);
                circley = -width / MathF.Sin(angle / 2f) * MathF.Sin(mid);
            }
            GraphicUtils.CreateCircle(new Vector3(circlex, circley, 0), radius, Color.White, vertices, triangles, colors, 0);
            {
                int count = vertices.Count;
                vertices.Add(new Vector3(-radius * MathF.Sin(a[1]) + circlex, radius * MathF.Cos(a[1]) + circley, 0));
                vertices.Add(new Vector3(circlex, circley, 0));
                vertices.Add(new Vector3(radius * MathF.Sin(a[0]) + circlex, -radius * MathF.Cos(a[0]) + circley, 0));
                vertices.Add(new Vector3(width * MathF.Sin(a[0]), -width * MathF.Cos(a[0]), 0));
                vertices.Add(Vector3.Zero);
                vertices.Add(new Vector3(-width * MathF.Sin(a[1]), width * MathF.Cos(a[1]), 0));
                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 5);
                triangles.Add(count + 4);
                triangles.Add(count + 1);
                triangles.Add(count + 5);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count + 4);
                triangles.Add(count + 1);
                triangles.Add(count + 3);
                triangles.Add(count + 4);
                for (int i = 0; i < 6; i++) colors.Add(Color.White);
            }
            {
                int count = vertices.Count;
                vertices.Add(new Vector3(length * m11 + width * m12, length * m12 - width * m11, 0));
                vertices.Add(new Vector3(length * m11 - width * m12, length * m12 + width * m11, 0));
                vertices.Add(new Vector3(-width * m12, width * m11, 0));
                vertices.Add(new Vector3(width * m12, -width * m11, 0));

                vertices.Add(new Vector3(length * m21 + width * m22, length * m22 - width * m21, 0));
                vertices.Add(new Vector3(length * m21 - width * m22, length * m22 + width * m21, 0));
                vertices.Add(new Vector3(-width * m22, width * m21, 0));
                vertices.Add(new Vector3(width * m22, -width * m21, 0));
                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 2);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count);
                triangles.Add(count + 4);
                triangles.Add(count + 5);
                triangles.Add(count + 6);
                triangles.Add(count + 6);
                triangles.Add(count + 7);
                triangles.Add(count + 4);
                for (int i = 0; i < 8; i++) colors.Add(Color.White);
            }
            #endregion
        } else if (angle > 0) {
            #region 正常情况
            width += outline;
            length += outline;

            float circlex = -width / MathF.Sin(angle / 2f) * MathF.Cos(mid);
            float circley = -width / MathF.Sin(angle / 2f) * MathF.Sin(mid);

            {
                int count = 0;
                vertices.Add(new Vector3(circlex, circley, 0));
                vertices.Add(new Vector3(width * MathF.Sin(a[0]), -width * MathF.Cos(a[0]), 0));
                vertices.Add(Vector3.Zero);
                vertices.Add(new Vector3(-width * MathF.Sin(a[1]), width * MathF.Cos(a[1]), 0));
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);
                triangles.Add(2);
                triangles.Add(3);
                triangles.Add(count);
                for (int i = 0; i < 4; i++) colors.Add(Color.Black);
            }
            {
                int count = vertices.Count;
                vertices.Add(new Vector3(length * m11 + width * m12, length * m12 - width * m11, 0));
                vertices.Add(new Vector3(length * m11 - width * m12, length * m12 + width * m11, 0));
                vertices.Add(new Vector3(-width * m12, width * m11, 0));
                vertices.Add(new Vector3(width * m12, -width * m11, 0));

                vertices.Add(new Vector3(length * m21 + width * m22, length * m22 - width * m21, 0));
                vertices.Add(new Vector3(length * m21 - width * m22, length * m22 + width * m21, 0));
                vertices.Add(new Vector3(-width * m22, width * m21, 0));
                vertices.Add(new Vector3(width * m22, -width * m21, 0));
                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 2);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count);
                triangles.Add(count + 4);
                triangles.Add(count + 5);
                triangles.Add(count + 6);
                triangles.Add(count + 6);
                triangles.Add(count + 7);
                triangles.Add(count + 4);
                for (int i = 0; i < 8; i++) colors.Add(Color.Black);
            }
            #endregion
            #region 边框
            width -= outline * 2f;
            length -= outline * 2f;

            circlex = -width / MathF.Sin(angle / 2f) * MathF.Cos(mid);
            circley = -width / MathF.Sin(angle / 2f) * MathF.Sin(mid);

            {
                int count = vertices.Count;
                vertices.Add(new Vector3(circlex, circley, 0));
                vertices.Add(new Vector3(width * MathF.Sin(a[0]), -width * MathF.Cos(a[0]), 0));
                vertices.Add(Vector3.Zero);
                vertices.Add(new Vector3(-width * MathF.Sin(a[1]), width * MathF.Cos(a[1]), 0));
                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 2);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count);
                for (int i = 0; i < 4; i++) colors.Add(Color.White);
            }
            {
                int count = vertices.Count;
                vertices.Add(new Vector3(length * m11 + width * m12, length * m12 - width * m11, 0));
                vertices.Add(new Vector3(length * m11 - width * m12, length * m12 + width * m11, 0));
                vertices.Add(new Vector3(-width * m12, width * m11, 0));
                vertices.Add(new Vector3(width * m12, -width * m11, 0));

                vertices.Add(new Vector3(length * m21 + width * m22, length * m22 - width * m21, 0));
                vertices.Add(new Vector3(length * m21 - width * m22, length * m22 + width * m21, 0));
                vertices.Add(new Vector3(-width * m22, width * m21, 0));
                vertices.Add(new Vector3(width * m22, -width * m21, 0));
                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 2);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count);
                triangles.Add(count + 4);
                triangles.Add(count + 5);
                triangles.Add(count + 6);
                triangles.Add(count + 6);
                triangles.Add(count + 7);
                triangles.Add(count + 4);
                for (int i = 0; i < 8; i++) colors.Add(Color.White);
            }
            #endregion
        } else {
            #region 如果角度差为180度，则绘制一个半圆（主体）
            length = width;
            width += outline;
            length += outline;

            Vector3 midpoint = new Vector3(-m11 * 0.04f, -m12 * 0.04f, 0);
            GraphicUtils.CreateCircle(midpoint, width, Color.Black, vertices, triangles, colors, 0);

            {
                int count = vertices.Count;
                vertices.Add(Vector3.Add(midpoint,new Vector3(length * m11 + width * m12, length * m12 - width * m11, 0)));
                vertices.Add(Vector3.Add(midpoint,new Vector3(length * m11 - width * m12, length * m12 + width * m11, 0)));
                vertices.Add(Vector3.Add(midpoint,new Vector3(-width * m12, width * m11, 0)));
                vertices.Add(Vector3.Add(midpoint,new Vector3(width * m12, -width * m11, 0)));

                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 2);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count);
                for (int i = 0; i < 4; i++) colors.Add(Color.Black);
            }
            #endregion
            #region 边框
            width -= outline * 2f;
            length -= outline * 2f;
            GraphicUtils.CreateCircle(midpoint, width, Color.White, vertices, triangles, colors, 0);
            {
                int count = vertices.Count;
                vertices.Add(Vector3.Add(midpoint,new Vector3(length * m11 + width * m12, length * m12 - width * m11, 0)));
                vertices.Add(Vector3.Add(midpoint,new Vector3(length * m11 - width * m12, length * m12 + width * m11, 0)));
                vertices.Add(Vector3.Add(midpoint,new Vector3(-width * m12, width * m11, 0)));
                vertices.Add(Vector3.Add(midpoint,new Vector3(width * m12, -width * m11, 0)));

                triangles.Add(count);
                triangles.Add(count + 1);
                triangles.Add(count + 2);
                triangles.Add(count + 2);
                triangles.Add(count + 3);
                triangles.Add(count);
                for (int i = 0; i < 4; i++) colors.Add(Color.White);
            }
            #endregion
        }
        
        Polygon polygon = new Polygon()
        {
            vertice = vertices.ToArray(),
            triangle = triangles.Select(x => (short) x).ToArray(),
            color = colors.ToArray()
        };
        return polygon;
    }
    
    private Polygon CreateMidspinPolygons(float a1) {

        float width = Floor.width;
        float length = Floor.width;
        float m1 = MathF.Cos(a1 / 180f * MathF.PI);
        float m2 = MathF.Sin(a1 / 180f * MathF.PI);

        #region 主体
        List<Vector3> vertices = [];
        List<int> triangles = [];
        List<Color> colors = [];
        Vector3 midpoint = new Vector3(-m1 * 0.04f, -m2 * 0.04f, 0);
        width += outline;
        length += outline;
        {
            int count = 0;
            vertices.Add(Vector3.Add(midpoint,new Vector3((length) * m1 + (width) * m2, (length) * m2 - (width) * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3((length) * m1 - (width) * m2, (length) * m2 + (width) * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3(-(width) * m2, (width) * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3((width) * m2, -(width) * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3(-width * m1, -width * m2, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3(width * m2, -width * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3(-width * m2, width * m1, 0)));
            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
            triangles.Add(count + 2);
            triangles.Add(count + 3);
            triangles.Add(count);
            triangles.Add(count + 4);
            triangles.Add(count + 5);
            triangles.Add(count + 6);
            for (int i = 0; i < 7; i++) colors.Add(Color.Black);
        }
        #endregion
        #region 边框
        width -= outline * 2;
        length -= outline * 2;
        {
            int count = vertices.Count;
            vertices.Add(Vector3.Add(midpoint,new Vector3((length) * m1 + (width) * m2, (length) * m2 - (width) * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3((length) * m1 - (width) * m2, (length) * m2 + (width) * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3(-(width) * m2, (width) * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3((width) * m2, -(width) * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3(-width * m1, -width * m2, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3(width * m2, -width * m1, 0)));
            vertices.Add(Vector3.Add(midpoint,new Vector3(-width * m2, width * m1, 0)));
            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
            triangles.Add(count + 2);
            triangles.Add(count + 3);
            triangles.Add(count);
            triangles.Add(count + 4);
            triangles.Add(count + 5);
            triangles.Add(count + 6);
            for (int i = 0; i < 7; i++) colors.Add(Color.White);
        }
        #endregion
     
        Polygon polygon = new Polygon()
        {
            vertice = vertices.ToArray(),
            triangle = triangles.Select(x => (short) x).ToArray(),
            color = colors.ToArray()
        };
        return polygon;
    }
    
    public struct Polygon
    {
        public Vector3[] vertice;
        public Color[] color;
        public short[] triangle;

        public bool IsEmpty()
        {
            if (vertice != null && triangle != null && color != null)
            {
                return vertice.Length == 0 && triangle.Length == 0 && color.Length == 0;
            }
            return true;
        }
        public override string ToString()
        {
            return this.AsString();
        }
    }

    public override string ToString()
    {
        return this.AsString();
    }
}