using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SharpFAI.Util;

internal static class GraphicUtils
{
     public static void CreateCircle(Vector3 center, float r, Color c, List<Vector3> vertices, List<int> triangles, List<Color> colors, int resolution) {
        if (resolution <= 0) {
            resolution = 32; // Default value if not provided
        }

        int centerIndex = vertices.Count;
        vertices.Add(center);
        colors.Add(c);

        for (int i = 0; i < resolution; i++) {
            float angle = 2f * (float)Math.PI * i / resolution;
            Vector3 vertex = Vector3.Add(new Vector3((float)Math.Cos(angle) * r, (float)Math.Sin(angle) * r, 0),center);
            vertices.Add(vertex);
            colors.Add(c);
        }

        for (int i = 1; i < resolution; i++) {
            triangles.Add(centerIndex);
            triangles.Add(centerIndex + i);
            triangles.Add(centerIndex + i + 1);
        }

        // CloSing the circle by connecting the last vertex to the first
        triangles.Add(centerIndex);
        triangles.Add(centerIndex + resolution);
        triangles.Add(centerIndex + 1);
    }
}