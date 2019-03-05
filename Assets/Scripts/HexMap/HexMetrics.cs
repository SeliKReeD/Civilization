using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirections
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionextention
{
    public static HexDirections Opposide(this HexDirections direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirections Previous(this HexDirections direction)
    {
        return direction == HexDirections.NE ? HexDirections.NW : (direction - 1);
    }

    public static HexDirections Next(this HexDirections direction)
    {
        return direction == HexDirections.NW ? HexDirections.NE : (direction + 1);
    }
}

public static class HexMetrics 
{
    public const float outerRadius = 10f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public const float solidFactor = 0.75f;

    public const float blendFactor = 1 - solidFactor;

    public const float elevationStep = 3f;

    public const int terracesPerSlope = 4;

    public const int terracesSteps = terracesPerSlope * 2 + 1;

    public const float horizontalTerraseStep = 1f / terracesSteps;

    public const float verticalTerraceStep = 1f / (terracesPerSlope + 1);

    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * horizontalTerraseStep;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * verticalTerraceStep;
        a.y += (b.y - a.y) * v;
        return a;
    }

    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * horizontalTerraseStep;
        return Color.Lerp(a, b, h);
    }

    public static Vector3[] corners =
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f*outerRadius),
        new Vector3(innerRadius, 0f, -0.5f*outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f*outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f*outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static Vector3 GetFirstCorner(HexDirections direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirections direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirections direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirections direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirections direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }
}
