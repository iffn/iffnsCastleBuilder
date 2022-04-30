using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    public static readonly float FloatThreshold = 0.0001f;

    public static readonly float SmallFloat = 0.0005f;

    public static int ClampInt(int value, int max, int min)
    {
        if (value > max) return max;
        if (value < min) return min;
        return value;
    }

    public static int ClampIntMin(int value, int min)
    {
        if (value < min) return min;
        return value;
    }

    public static int ClampIntMax(int value, int max)
    {
        if (value > max) return max;
        return value;
    }

    public static Vector2Int ClampVector2Int(Vector2Int value, Vector2Int max, Vector2Int min)
    {
        Vector2Int returnValue = new Vector2Int(MathHelper.ClampInt(value: value.x, max: max.x, min: min.x),
                    MathHelper.ClampInt(value: value.y, max: max.y, min: min.y));

        return returnValue;
    }

    public static bool FloatIsZero(float value)
    {
        return Mathf.Abs(value) < Mathf.Abs(FloatThreshold);
    }

    public static Vector3 ConvertPointIntoOriginTransform(Transform baseObject, Transform originObject, Vector3 vector)
    {
        return (originObject.InverseTransformPoint(baseObject.TransformPoint(vector)));
    }

    public static Vector2 RotateVector2CW(Vector2 vector, float angleDeg)
    {
        float angleRad = angleDeg * Mathf.Deg2Rad;

        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        return new Vector2(vector.x * cos - vector.y * sin, vector.y * cos - vector.x * sin);
    }

    public static float AreaOf2DTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float area = Mathf.Abs(v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y)) * 0.5f;

        return area;
    }

    //From StackOverflow: https://stackoverflow.com/a/20824923/9473490
    public static int GreatestCommonFactor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    //From StackOverflow: https://stackoverflow.com/a/20824923/9473490
    public static int LeastCommonMultiple(int a, int b)
    {
        return (a / GreatestCommonFactor(a, b)) * b;
    }

    public static Vector3 MultiplyVectorElemetns(Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
    }
}
