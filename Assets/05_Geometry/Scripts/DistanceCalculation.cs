using UnityEngine;

/// <summary>
/// 距离计算示例
/// 演示点到点、点到线、点到平面等各种距离计算
/// </summary>
public class DistanceCalculation : MonoBehaviour
{
    [Header("测试点")]
    public Transform testPoint;

    [Header("目标对象")]
    public Transform targetPoint;
    public Transform lineStart;
    public Transform lineEnd;
    public Transform planePoint;
    public Vector3 planeNormal = Vector3.up;

    [Header("计算模式")]
    public DistanceMode mode = DistanceMode.PointToPoint;

    [Header("显示选项")]
    public bool showDistance = true;
    public bool showClosestPoint = true;
    public bool showProjection = true;
    public bool showBounds = false;

    [Header("颜色设置")]
    public Color distanceLineColor = Color.yellow;
    public Color closestPointColor = Color.green;
    public Color projectionColor = Color.cyan;

    private float calculatedDistance;
    private Vector3 closestPoint;

    public enum DistanceMode
    {
        PointToPoint,       // 点到点
        PointToLine,        // 点到线段
        PointToPlane,       // 点到平面
        PointToBounds,      // 点到边界框
        LinesToLines        // 两条线段最近距离
    }

    void Update()
    {
        CalculateDistance();

        // 按空格键输出信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogDistanceInfo();
        }
    }

    void CalculateDistance()
    {
        if (testPoint == null) return;

        Vector3 point = testPoint.position;

        switch (mode)
        {
            case DistanceMode.PointToPoint:
                if (targetPoint != null)
                {
                    calculatedDistance = Vector3.Distance(point, targetPoint.position);
                    closestPoint = targetPoint.position;
                }
                break;

            case DistanceMode.PointToLine:
                if (lineStart != null && lineEnd != null)
                {
                    closestPoint = MathHelper.ClosestPointOnLineSegment(point, lineStart.position, lineEnd.position);
                    calculatedDistance = Vector3.Distance(point, closestPoint);
                }
                break;

            case DistanceMode.PointToPlane:
                if (planePoint != null)
                {
                    float signedDistance = MathHelper.DistanceToPlane(point, planePoint.position, planeNormal.normalized);
                    calculatedDistance = Mathf.Abs(signedDistance);
                    closestPoint = MathHelper.ProjectPointOnPlane(point, planePoint.position, planeNormal.normalized);
                }
                break;

            case DistanceMode.PointToBounds:
                if (targetPoint != null && targetPoint.GetComponent<Collider>() != null)
                {
                    Bounds bounds = targetPoint.GetComponent<Collider>().bounds;
                    closestPoint = bounds.ClosestPoint(point);
                    calculatedDistance = Vector3.Distance(point, closestPoint);
                }
                break;

            case DistanceMode.LinesToLines:
                if (lineStart != null && lineEnd != null && targetPoint != null && planePoint != null)
                {
                    // 计算两条线段之间的最近距离
                    CalculateLineToLineDistance();
                }
                break;
        }
    }

    void CalculateLineToLineDistance()
    {
        Vector3 p1 = lineStart.position;
        Vector3 p2 = lineEnd.position;
        Vector3 p3 = targetPoint.position;
        Vector3 p4 = planePoint.position;

        Vector3 closest1, closest2;
        ClosestPointsBetweenLineSegments(p1, p2, p3, p4, out closest1, out closest2);

        closestPoint = closest1;
        Vector3 closestPoint2 = closest2;
        calculatedDistance = Vector3.Distance(closest1, closest2);

        // 绘制两个最近点
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(closest2, 0.15f);
    }

    void ClosestPointsBetweenLineSegments(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4,
        out Vector3 closestPoint1, out Vector3 closestPoint2)
    {
        Vector3 d1 = p2 - p1;
        Vector3 d2 = p4 - p3;
        Vector3 r = p1 - p3;

        float a = Vector3.Dot(d1, d1);
        float e = Vector3.Dot(d2, d2);
        float f = Vector3.Dot(d2, r);

        float s, t;

        if (a <= MathHelper.EPSILON && e <= MathHelper.EPSILON)
        {
            // 两条线段都退化为点
            closestPoint1 = p1;
            closestPoint2 = p3;
            return;
        }

        if (a <= MathHelper.EPSILON)
        {
            // 第一条线段退化为点
            s = 0f;
            t = Mathf.Clamp01(f / e);
        }
        else
        {
            float c = Vector3.Dot(d1, r);

            if (e <= MathHelper.EPSILON)
            {
                // 第二条线段退化为点
                t = 0f;
                s = Mathf.Clamp01(-c / a);
            }
            else
            {
                // 一般情况
                float b = Vector3.Dot(d1, d2);
                float denom = a * e - b * b;

                if (denom != 0f)
                {
                    s = Mathf.Clamp01((b * f - c * e) / denom);
                }
                else
                {
                    s = 0f;
                }

                t = (b * s + f) / e;

                if (t < 0f)
                {
                    t = 0f;
                    s = Mathf.Clamp01(-c / a);
                }
                else if (t > 1f)
                {
                    t = 1f;
                    s = Mathf.Clamp01((b - c) / a);
                }
            }
        }

        closestPoint1 = p1 + d1 * s;
        closestPoint2 = p3 + d2 * t;
    }

    void OnDrawGizmos()
    {
        if (testPoint == null) return;

        Vector3 point = testPoint.position;

        // 绘制测试点
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point, 0.2f);
        DrawLabel(point + Vector3.up * 0.5f, "测试点");

        // 根据模式绘制
        switch (mode)
        {
            case DistanceMode.PointToPoint:
                DrawPointToPoint(point);
                break;

            case DistanceMode.PointToLine:
                DrawPointToLine(point);
                break;

            case DistanceMode.PointToPlane:
                DrawPointToPlane(point);
                break;

            case DistanceMode.PointToBounds:
                DrawPointToBounds(point);
                break;

            case DistanceMode.LinesToLines:
                DrawLinesToLines();
                break;
        }

        // 显示距离
        if (showDistance)
        {
            DrawDistanceInfo(point);
        }
    }

    void DrawPointToPoint(Vector3 point)
    {
        if (targetPoint == null) return;

        Vector3 target = targetPoint.position;

        // 绘制目标点
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(target, 0.2f);

        // 绘制连线
        Gizmos.color = distanceLineColor;
        Gizmos.DrawLine(point, target);

        DrawLabel(target + Vector3.up * 0.5f, "目标点");
    }

    void DrawPointToLine(Vector3 point)
    {
        if (lineStart == null || lineEnd == null) return;

        Vector3 start = lineStart.position;
        Vector3 end = lineEnd.position;

        // 绘制线段
        Gizmos.color = Color.blue;
        DebugDrawer.DrawArrow(start, end, 0.2f);
        Gizmos.DrawWireSphere(start, 0.15f);
        Gizmos.DrawWireSphere(end, 0.15f);

        // 绘制最近点
        if (showClosestPoint)
        {
            Gizmos.color = closestPointColor;
            Gizmos.DrawWireSphere(closestPoint, 0.18f);
            DrawLabel(closestPoint + Vector3.down * 0.3f, $"最近点");
        }

        // 绘制垂直线
        Gizmos.color = distanceLineColor;
        Gizmos.DrawLine(point, closestPoint);

        // 绘制投影信息
        if (showProjection)
        {
            Vector3 lineDir = (end - start).normalized;
            float projLength = Vector3.Dot(point - start, lineDir);
            Vector3 projPoint = start + lineDir * projLength;

            if (projLength > 0 && projLength < Vector3.Distance(start, end))
            {
                Gizmos.color = projectionColor;
                Gizmos.DrawWireSphere(projPoint, 0.12f);
            }
        }
    }

    void DrawPointToPlane(Vector3 point)
    {
        if (planePoint == null) return;

        Vector3 planeCenter = planePoint.position;
        Vector3 normal = planeNormal.normalized;

        // 绘制平面
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        DebugDrawer.DrawPlane(planeCenter, normal, 4f, Gizmos.color, true);

        // 绘制法线
        Gizmos.color = Color.blue;
        DebugDrawer.DrawArrow(planeCenter, planeCenter + normal * 2f, 0.3f);

        // 绘制投影点
        if (showClosestPoint)
        {
            Gizmos.color = closestPointColor;
            Gizmos.DrawWireSphere(closestPoint, 0.18f);
            DrawLabel(closestPoint, "投影点");
        }

        // 绘制垂直线
        Gizmos.color = distanceLineColor;
        Gizmos.DrawLine(point, closestPoint);

        // 计算有符号距离
        float signedDist = MathHelper.DistanceToPlane(point, planeCenter, normal);
        string side = signedDist > 0 ? "正面" : "背面";
        DrawLabel(point + Vector3.right * 0.5f, $"{side}\n距离: {Mathf.Abs(signedDist):F2}");
    }

    void DrawPointToBounds(Vector3 point)
    {
        if (targetPoint == null) return;

        Collider col = targetPoint.GetComponent<Collider>();
        if (col == null) return;

        Bounds bounds = col.bounds;

        // 绘制边界框
        if (showBounds)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        // 绘制最近点
        if (showClosestPoint)
        {
            Gizmos.color = closestPointColor;
            Gizmos.DrawWireSphere(closestPoint, 0.18f);
        }

        // 绘制连线
        Gizmos.color = distanceLineColor;
        Gizmos.DrawLine(point, closestPoint);

        // 判断点是否在边界框内
        bool inside = bounds.Contains(point);
        DrawLabel(point + Vector3.left * 0.5f, inside ? "在内部" : "在外部");
    }

    void DrawLinesToLines()
    {
        if (lineStart == null || lineEnd == null) return;
        if (targetPoint == null || planePoint == null) return;

        // 绘制第一条线段
        Gizmos.color = Color.blue;
        DebugDrawer.DrawArrow(lineStart.position, lineEnd.position, 0.2f);

        // 绘制第二条线段
        Gizmos.color = Color.red;
        DebugDrawer.DrawArrow(targetPoint.position, planePoint.position, 0.2f);

        // 绘制最近点连线
        if (showClosestPoint)
        {
            Gizmos.color = closestPointColor;
            Gizmos.DrawWireSphere(closestPoint, 0.15f);

            Gizmos.color = distanceLineColor;
            // closestPoint2在CalculateLineToLineDistance中已绘制
        }
    }

    void DrawDistanceInfo(Vector3 point)
    {
        string info = $"距离: {calculatedDistance:F3}";
        DrawLabel(point + Vector3.up * 1.2f, info);
    }

    void LogDistanceInfo()
    {
        Debug.Log("=== 距离计算信息 ===");
        Debug.Log($"模式: {mode}");
        Debug.Log($"测试点: {testPoint.position}");
        Debug.Log($"计算距离: {calculatedDistance:F4}");

        switch (mode)
        {
            case DistanceMode.PointToPoint:
                if (targetPoint != null)
                {
                    Debug.Log($"目标点: {targetPoint.position}");
                    Debug.Log($"使用方法: Vector3.Distance()");
                }
                break;

            case DistanceMode.PointToLine:
                if (lineStart != null && lineEnd != null)
                {
                    Debug.Log($"线段起点: {lineStart.position}");
                    Debug.Log($"线段终点: {lineEnd.position}");
                    Debug.Log($"最近点: {closestPoint}");
                    Debug.Log($"使用方法: MathHelper.ClosestPointOnLineSegment()");
                }
                break;

            case DistanceMode.PointToPlane:
                if (planePoint != null)
                {
                    float signedDist = MathHelper.DistanceToPlane(
                        testPoint.position, planePoint.position, planeNormal.normalized);
                    Debug.Log($"平面中心: {planePoint.position}");
                    Debug.Log($"平面法线: {planeNormal.normalized}");
                    Debug.Log($"有符号距离: {signedDist:F4}");
                    Debug.Log($"投影点: {closestPoint}");
                    Debug.Log($"使用方法: MathHelper.DistanceToPlane()");
                }
                break;

            case DistanceMode.PointToBounds:
                if (targetPoint != null)
                {
                    Collider col = targetPoint.GetComponent<Collider>();
                    if (col != null)
                    {
                        Bounds bounds = col.bounds;
                        Debug.Log($"边界框中心: {bounds.center}");
                        Debug.Log($"边界框大小: {bounds.size}");
                        Debug.Log($"最近点: {closestPoint}");
                        Debug.Log($"点是否在内部: {bounds.Contains(testPoint.position)}");
                        Debug.Log($"使用方法: Bounds.ClosestPoint()");
                    }
                }
                break;

            case DistanceMode.LinesToLines:
                Debug.Log($"线段1: {lineStart.position} -> {lineEnd.position}");
                Debug.Log($"线段2: {targetPoint.position} -> {planePoint.position}");
                Debug.Log($"使用方法: 自定义线段间距离算法");
                break;
        }

        // 性能提示
        Debug.Log($"\n性能提示:");
        Debug.Log($"- sqrMagnitude 比 magnitude 快（避免开方）");
        Debug.Log($"- 尽可能使用简单的距离检查");
        Debug.Log($"- 对于大量计算，考虑缓存结果");
    }

    void DrawLabel(Vector3 position, string text)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, text);
#endif
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Box("距离计算示例");

        GUILayout.Label($"模式: {mode}");
        GUILayout.Label($"距离: {calculatedDistance:F3}");

        switch (mode)
        {
            case DistanceMode.PointToPlane:
                if (planePoint != null && testPoint != null)
                {
                    float signed = MathHelper.DistanceToPlane(
                        testPoint.position, planePoint.position, planeNormal.normalized);
                    GUILayout.Label($"有符号距离: {signed:F3}");
                    GUILayout.Label($"位置: {(signed > 0 ? "正面" : "背面")}");
                }
                break;

            case DistanceMode.PointToBounds:
                if (targetPoint != null && testPoint != null)
                {
                    Collider col = targetPoint.GetComponent<Collider>();
                    if (col != null)
                    {
                        bool inside = col.bounds.Contains(testPoint.position);
                        GUILayout.Label($"在边界框{(inside ? "内部" : "外部")}");
                    }
                }
                break;
        }

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }
}
