using UnityEngine;

/// <summary>
/// 数学辅助工具类
/// 提供常用的数学计算功能
/// </summary>
public static class MathHelper
{
    public const float EPSILON = 0.0001f;

    #region 向量运算

    /// <summary>
    /// 安全的向量归一化（避免零向量）
    /// </summary>
    public static Vector3 SafeNormalize(Vector3 vector, Vector3 fallback = default)
    {
        if (vector.sqrMagnitude > EPSILON)
            return vector.normalized;
        return fallback;
    }

    /// <summary>
    /// 计算向量A在向量B上的投影
    /// </summary>
    public static Vector3 ProjectOnVector(Vector3 vector, Vector3 onVector)
    {
        float sqrMag = onVector.sqrMagnitude;
        if (sqrMag < EPSILON)
            return Vector3.zero;

        float dot = Vector3.Dot(vector, onVector);
        return onVector * (dot / sqrMag);
    }

    /// <summary>
    /// 计算向量A在向量B上的投影长度（标量）
    /// </summary>
    public static float ProjectOnVectorScalar(Vector3 vector, Vector3 onVector)
    {
        Vector3 normalized = SafeNormalize(onVector);
        return Vector3.Dot(vector, normalized);
    }

    /// <summary>
    /// 计算向量的rejection（垂直分量）
    /// </summary>
    public static Vector3 Rejection(Vector3 vector, Vector3 onVector)
    {
        return vector - ProjectOnVector(vector, onVector);
    }

    /// <summary>
    /// 判断两个向量是否近似相等
    /// </summary>
    public static bool Approximately(Vector3 a, Vector3 b, float epsilon = EPSILON)
    {
        return (a - b).sqrMagnitude < epsilon * epsilon;
    }

    /// <summary>
    /// 判断两个浮点数是否近似相等
    /// </summary>
    public static bool Approximately(float a, float b, float epsilon = EPSILON)
    {
        return Mathf.Abs(a - b) < epsilon;
    }

    #endregion

    #region 角度计算

    /// <summary>
    /// 计算两个向量的夹角（度）
    /// </summary>
    public static float AngleBetween(Vector3 from, Vector3 to)
    {
        from = SafeNormalize(from);
        to = SafeNormalize(to);

        float dot = Mathf.Clamp(Vector3.Dot(from, to), -1f, 1f);
        return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 计算两个向量的有符号夹角（度）
    /// </summary>
    public static float SignedAngleBetween(Vector3 from, Vector3 to, Vector3 axis)
    {
        float unsignedAngle = AngleBetween(from, to);
        float sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
        return unsignedAngle * sign;
    }

    /// <summary>
    /// 将角度限制在 -180 到 180 之间
    /// </summary>
    public static float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }

    #endregion

    #region 几何计算

    /// <summary>
    /// 计算点到线段的最近点
    /// </summary>
    public static Vector3 ClosestPointOnLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;

        if (lineLength < EPSILON)
            return lineStart;

        lineDirection /= lineLength;

        float projectionLength = Vector3.Dot(point - lineStart, lineDirection);
        projectionLength = Mathf.Clamp(projectionLength, 0f, lineLength);

        return lineStart + lineDirection * projectionLength;
    }

    /// <summary>
    /// 计算点到无限直线的最近点
    /// </summary>
    public static Vector3 ClosestPointOnLine(Vector3 point, Vector3 linePoint, Vector3 lineDirection)
    {
        lineDirection = SafeNormalize(lineDirection);
        Vector3 pointToLine = point - linePoint;
        float projectionLength = Vector3.Dot(pointToLine, lineDirection);
        return linePoint + lineDirection * projectionLength;
    }

    /// <summary>
    /// 计算点到线段的距离
    /// </summary>
    public static float DistanceToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 closest = ClosestPointOnLineSegment(point, lineStart, lineEnd);
        return Vector3.Distance(point, closest);
    }

    /// <summary>
    /// 计算点到平面的距离（有符号）
    /// </summary>
    public static float DistanceToPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
    {
        planeNormal = SafeNormalize(planeNormal, Vector3.up);
        return Vector3.Dot(point - planePoint, planeNormal);
    }

    /// <summary>
    /// 将点投影到平面上
    /// </summary>
    public static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
    {
        float distance = DistanceToPlane(point, planePoint, planeNormal);
        return point - planeNormal * distance;
    }

    /// <summary>
    /// 计算三角形面积
    /// </summary>
    public static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        return Vector3.Cross(ab, ac).magnitude * 0.5f;
    }

    #endregion

    #region 方向判断

    /// <summary>
    /// 判断点是否在向量的左侧（2D，使用Y轴作为向上方向）
    /// </summary>
    public static bool IsLeft(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        Vector3 toPoint = point - lineStart;
        float cross = Vector3.Cross(lineDirection, toPoint).y;
        return cross > 0;
    }

    /// <summary>
    /// 判断目标是否在前方（使用点积）
    /// </summary>
    public static bool IsInFront(Vector3 observerPosition, Vector3 forwardDirection, Vector3 targetPosition, float threshold = 0f)
    {
        Vector3 toTarget = targetPosition - observerPosition;
        float dot = Vector3.Dot(forwardDirection.normalized, toTarget.normalized);
        return dot > threshold;
    }

    /// <summary>
    /// 判断目标是否在视野内
    /// </summary>
    public static bool IsInFieldOfView(Vector3 observerPosition, Vector3 forwardDirection, Vector3 targetPosition, float fovAngle)
    {
        Vector3 toTarget = (targetPosition - observerPosition).normalized;
        float angle = AngleBetween(forwardDirection, toTarget);
        return angle < fovAngle / 2f;
    }

    #endregion

    #region 插值和平滑

    /// <summary>
    /// 平滑阻尼（类似SmoothDamp但更简单）
    /// </summary>
    public static float SmoothApproach(float current, float target, float smoothTime, float deltaTime)
    {
        if (smoothTime <= 0f) return target;
        float t = Mathf.Clamp01(deltaTime / smoothTime);
        return Mathf.Lerp(current, target, t);
    }

    /// <summary>
    /// 向量平滑接近
    /// </summary>
    public static Vector3 SmoothApproach(Vector3 current, Vector3 target, float smoothTime, float deltaTime)
    {
        if (smoothTime <= 0f) return target;
        float t = Mathf.Clamp01(deltaTime / smoothTime);
        return Vector3.Lerp(current, target, t);
    }

    /// <summary>
    /// 弹簧平滑
    /// </summary>
    public static float Spring(float current, float target, ref float velocity, float smoothTime, float deltaTime)
    {
        return Mathf.SmoothDamp(current, target, ref velocity, smoothTime, Mathf.Infinity, deltaTime);
    }

    #endregion

    #region 射线

    /// <summary>
    /// 计算射线与平面的交点
    /// </summary>
    public static bool RayPlaneIntersection(Ray ray, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection, out float distance)
    {
        intersection = Vector3.zero;
        distance = 0f;

        float denominator = Vector3.Dot(ray.direction, planeNormal);

        // 射线平行于平面
        if (Mathf.Abs(denominator) < EPSILON)
            return false;

        distance = Vector3.Dot(planePoint - ray.origin, planeNormal) / denominator;

        // 交点在射线反方向
        if (distance < 0f)
            return false;

        intersection = ray.origin + ray.direction * distance;
        return true;
    }

    /// <summary>
    /// 计算两条射线的最近点
    /// </summary>
    public static void ClosestPointsOnTwoRays(Ray ray1, Ray ray2, out Vector3 closestPoint1, out Vector3 closestPoint2)
    {
        Vector3 p1 = ray1.origin;
        Vector3 p2 = ray2.origin;
        Vector3 d1 = ray1.direction.normalized;
        Vector3 d2 = ray2.direction.normalized;

        Vector3 w0 = p1 - p2;
        float a = Vector3.Dot(d1, d1);
        float b = Vector3.Dot(d1, d2);
        float c = Vector3.Dot(d2, d2);
        float d = Vector3.Dot(d1, w0);
        float e = Vector3.Dot(d2, w0);

        float denominator = a * c - b * b;

        float t1, t2;
        if (denominator < EPSILON)
        {
            // 平行
            t1 = 0f;
            t2 = (Mathf.Abs(c) < EPSILON) ? 0f : (e / c);  // 安全检查，避免除零
        }
        else
        {
            t1 = (b * e - c * d) / denominator;
            t2 = (a * e - b * d) / denominator;
        }

        closestPoint1 = p1 + d1 * t1;
        closestPoint2 = p2 + d2 * t2;
    }

    #endregion

    #region 坐标变换

    /// <summary>
    /// 世界坐标转本地坐标（手动实现）
    /// </summary>
    public static Vector3 WorldToLocal(Vector3 worldPoint, Vector3 origin, Quaternion rotation)
    {
        return Quaternion.Inverse(rotation) * (worldPoint - origin);
    }

    /// <summary>
    /// 本地坐标转世界坐标（手动实现）
    /// </summary>
    public static Vector3 LocalToWorld(Vector3 localPoint, Vector3 origin, Quaternion rotation)
    {
        return origin + rotation * localPoint;
    }

    #endregion

    #region 其他实用功能

    /// <summary>
    /// 将值重新映射到新范围
    /// </summary>
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float t = Mathf.InverseLerp(fromMin, fromMax, value);
        return Mathf.Lerp(toMin, toMax, t);
    }

    /// <summary>
    /// 平方根（安全版本，避免负数）
    /// </summary>
    public static float SafeSqrt(float value)
    {
        return Mathf.Sqrt(Mathf.Max(0f, value));
    }

    /// <summary>
    /// 符号函数（返回 -1, 0, 或 1）
    /// </summary>
    public static int Sign(float value, float epsilon = EPSILON)
    {
        if (value > epsilon) return 1;
        if (value < -epsilon) return -1;
        return 0;
    }

    #endregion
}
