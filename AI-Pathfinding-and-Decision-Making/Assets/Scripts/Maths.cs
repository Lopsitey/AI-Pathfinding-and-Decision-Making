#region

using UnityEngine;

#endregion

public static class Maths
{
    public static float Magnitude(Vector2 a) => Mathf.Sqrt(a.x * a.x + a.y * a.y);
    //does this: a.magnitude;

    public static Vector2 Normalise(Vector2 a)
    {
        float mag = Magnitude(a);
        Vector2 norm = new Vector2(a.x / mag, a.y / mag);
        //squares them then adds the squared versions then roots, not the same as just doing x+y
        
        //the above does the same as a.normalized 
        return mag > Mathf.Epsilon ? norm : Vector2.zero;//Returns zero if the input vector is too small
    }

    public static float Dot(Vector2 lhs, Vector2 rhs)
    {
        lhs = Normalise(lhs);
        rhs = Normalise(rhs);
        float dot = (lhs.x * rhs.x) + (lhs.y * rhs.y);
        return dot;
    }

    /// <summary>
    /// Returns the radians of the angle between two vectors
    /// </summary>
    public static float Angle(Vector2 lhs, Vector2 rhs)
    {
        float dot = Mathf.Clamp(Dot(lhs, rhs), -1f, 1f);
        //needs to be clamped otherwise it can potentially return nan
        float ang = Mathf.Acos(dot);
        return ang;
    }

    /// <summary>
    /// Translates a vector by X angle in degrees
    /// </summary>
    public static Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float rad = degrees * (Mathf.PI * 2) / 360;
        //also works as mathf.deg2rad
        float newX = vector.x * Mathf.Cos(rad) - vector.y * Mathf.Sin(rad);
        float newY = vector.x * Mathf.Sin(rad) + vector.y * Mathf.Cos(rad);
        Vector2 newVector = new Vector2(newX, newY);
        return newVector;
    }
}