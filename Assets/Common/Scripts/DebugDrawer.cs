using UnityEngine;

/// <summary>
/// 调试绘制辅助工具
/// 提供常用的Gizmos绘制功能
/// </summary>
public static class DebugDrawer
{
    /// <summary>
    /// 绘制箭头
    /// </summary>
    public static void DrawArrow(Vector3 start, Vector3 end, Color color, float arrowHeadSize = 0.25f)
    {
        Gizmos.color = color;
        DrawArrow(start, end, arrowHeadSize);
    }

    /// <summary>
    /// 绘制箭头（使用当前Gizmos颜色）
    /// </summary>
    public static void DrawArrow(Vector3 start, Vector3 end, float arrowHeadSize = 0.25f)
    {
        Gizmos.DrawLine(start, end);

        Vector3 direction = (end - start).normalized;
        if (direction.magnitude < 0.001f) return;

        Vector3 right = Vector3.Cross(Vector3.up, direction);
        if (right.magnitude < 0.001f)
            right = Vector3.Cross(Vector3.right, direction);
        right = right.normalized;

        Vector3 up = Vector3.Cross(direction, right).normalized;

        Vector3 arrowTip = end - direction * arrowHeadSize;
        Gizmos.DrawLine(end, arrowTip + right * arrowHeadSize * 0.5f);
        Gizmos.DrawLine(end, arrowTip - right * arrowHeadSize * 0.5f);
        Gizmos.DrawLine(end, arrowTip + up * arrowHeadSize * 0.5f);
        Gizmos.DrawLine(end, arrowTip - up * arrowHeadSize * 0.5f);
    }

    /// <summary>
    /// 绘制坐标系
    /// </summary>
    public static void DrawCoordinateSystem(Vector3 origin, float size = 1f, float alpha = 1f)
    {
        // X轴 - 红色
        Gizmos.color = new Color(1f, 0f, 0f, alpha);
        DrawArrow(origin, origin + Vector3.right * size, size * 0.2f);

        // Y轴 - 绿色
        Gizmos.color = new Color(0f, 1f, 0f, alpha);
        DrawArrow(origin, origin + Vector3.up * size, size * 0.2f);

        // Z轴 - 蓝色
        Gizmos.color = new Color(0f, 0f, 1f, alpha);
        DrawArrow(origin, origin + Vector3.forward * size, size * 0.2f);
    }

    /// <summary>
    /// 绘制网格
    /// </summary>
    public static void DrawGrid(Vector3 center, Vector3 normal, float size, int divisions, Color color)
    {
        Gizmos.color = color;

        Vector3 right = Vector3.Cross(normal, Vector3.up);
        if (right.magnitude < 0.001f)
            right = Vector3.Cross(normal, Vector3.right);
        right = right.normalized;

        Vector3 forward = Vector3.Cross(right, normal).normalized;

        float halfSize = size / 2f;
        float step = size / divisions;

        // 绘制网格线
        for (int i = 0; i <= divisions; i++)
        {
            float offset = -halfSize + i * step;

            // 平行于right的线
            Vector3 start1 = center + forward * offset - right * halfSize;
            Vector3 end1 = center + forward * offset + right * halfSize;
            Gizmos.DrawLine(start1, end1);

            // 平行于forward的线
            Vector3 start2 = center + right * offset - forward * halfSize;
            Vector3 end2 = center + right * offset + forward * halfSize;
            Gizmos.DrawLine(start2, end2);
        }
    }

    /// <summary>
    /// 绘制圆形
    /// </summary>
    public static void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color, int segments = 32)
    {
        Gizmos.color = color;
        DrawCircle(center, normal, radius, segments);
    }

    /// <summary>
    /// 绘制圆形（使用当前Gizmos颜色）
    /// </summary>
    public static void DrawCircle(Vector3 center, Vector3 normal, float radius, int segments = 32)
    {
        Vector3 right = Vector3.Cross(normal, Vector3.up);
        if (right.magnitude < 0.001f)
            right = Vector3.Cross(normal, Vector3.right);
        right = right.normalized;

        Vector3 forward = Vector3.Cross(right, normal).normalized;

        Vector3 previousPoint = center + right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            Vector3 point = center + (right * Mathf.Cos(angle) + forward * Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }

    /// <summary>
    /// 绘制扇形
    /// </summary>
    public static void DrawArc(Vector3 center, Vector3 forward, Vector3 axis, float angle, float radius, Color color, int segments = 20)
    {
        Gizmos.color = color;

        Vector3 previousPoint = center + forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            Quaternion rotation = Quaternion.AngleAxis(angle * t, axis);
            Vector3 point = center + rotation * (forward * radius);
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        // 绘制边缘线
        Gizmos.DrawLine(center, center + forward * radius);
        Gizmos.DrawLine(center, previousPoint);
    }

    /// <summary>
    /// 绘制线框立方体
    /// </summary>
    public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
    }

    /// <summary>
    /// 绘制文本标签（仅在编辑器中可用）
    /// </summary>
    public static void DrawLabel(Vector3 position, string text, Color? color = null)
    {
#if UNITY_EDITOR
        if (color.HasValue)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color.Value;
            UnityEditor.Handles.Label(position, text, style);
        }
        else
        {
            UnityEditor.Handles.Label(position, text);
        }
#endif
    }

    /// <summary>
    /// 绘制带箭头的向量
    /// </summary>
    public static void DrawVector(Vector3 origin, Vector3 direction, Color color, string label = "", float arrowSize = 0.25f)
    {
        Gizmos.color = color;
        Vector3 end = origin + direction;
        DrawArrow(origin, end, arrowSize);

        if (!string.IsNullOrEmpty(label))
        {
            DrawLabel((origin + end) / 2f, label, color);
        }
    }

    /// <summary>
    /// 绘制平面
    /// </summary>
    public static void DrawPlane(Vector3 center, Vector3 normal, float size, Color color, bool filled = false)
    {
        Vector3 right = Vector3.Cross(normal, Vector3.up);
        if (right.magnitude < 0.001f)
            right = Vector3.Cross(normal, Vector3.right);
        right = right.normalized;

        Vector3 forward = Vector3.Cross(right, normal).normalized;

        Gizmos.color = color;

        Vector3 p1 = center + (right + forward) * size * 0.5f;
        Vector3 p2 = center + (-right + forward) * size * 0.5f;
        Vector3 p3 = center + (-right - forward) * size * 0.5f;
        Vector3 p4 = center + (right - forward) * size * 0.5f;

        // 绘制边框
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        // 填充效果
        if (filled)
        {
            for (float t = 0; t <= 1f; t += 0.05f)
            {
                Vector3 start = Vector3.Lerp(p1, p2, t);
                Vector3 end = Vector3.Lerp(p4, p3, t);
                Gizmos.DrawLine(start, end);
            }
        }
    }

    /// <summary>
    /// 绘制射线
    /// </summary>
    public static void DrawRay(Ray ray, float length, Color color)
    {
        Gizmos.color = color;
        DrawArrow(ray.origin, ray.origin + ray.direction * length, length * 0.1f);
    }

    /// <summary>
    /// 绘制点（小球体）
    /// </summary>
    public static void DrawPoint(Vector3 position, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(position, radius);
    }

    /// <summary>
    /// 绘制线框点（空心球体）
    /// </summary>
    public static void DrawWirePoint(Vector3 position, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, radius);
    }
}
