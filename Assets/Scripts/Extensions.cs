using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static float RandomRange(Vector2 range) => Random.Range(range.x, range.y);
    public static bool IsValid(this Quaternion quat) => !quat.IsZero() && !quat.IsNaN();
    public static bool IsZero(this Quaternion quat) => quat.x == 0 && quat.y == 0 && quat.z == 0 & quat.w == 0;
    public static bool IsNaN(this Quaternion quat) => float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsNaN(quat.w);
}
