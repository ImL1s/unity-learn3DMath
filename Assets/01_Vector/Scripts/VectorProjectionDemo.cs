using UnityEngine;

/// <summary>
/// 向量投影演示
/// 投影可以用来：
/// 1. 计算一个向量在另一个向量方向上的分量
/// 2. 找到点到线的最近点
/// 3. 计算光照强度
/// 4. 斜坡上的速度分解
/// </summary>
public class VectorProjectionDemo : MonoBehaviour
{
    [Header("投影设置")]
    public Transform pointA;        // 被投影的点
    public Transform lineStart;     // 投影线起点
    public Transform lineEnd;       // 投影线终点

    [Header("显示选项")]
    public bool showVectors = true;
    public bool showProjection = true;
    public bool showRejection = false;      // 投影的垂直分量
    public bool showClosestPoint = true;
    public bool showDistance = true;

    [Header("应用示例")]
    public bool showLightingExample = false;
    public Transform lightDirection;        // 光源方向
    public Transform surfaceNormal;         // 表面法线

    [Header("颜色设置")]
    public Color vectorColor = Color.red;
    public Color lineColor = Color.blue;
    public Color projectionColor = Color.green;
    public Color rejectionColor = Color.yellow;

    void OnDrawGizmos()
    {
        if (pointA == null || lineStart == null || lineEnd == null) return;

        Vector3 start = lineStart.position;
        Vector3 end = lineEnd.position;
        Vector3 point = pointA.position;

        // 线的方向向量
        Vector3 lineDirection = (end - start).normalized;
        Vector3 lineVector = end - start;

        // 从线起点到目标点的向量
        Vector3 startToPoint = point - start;

        // 计算投影
        // 方法1: 使用点积
        float projectionLength = Vector3.Dot(startToPoint, lineDirection);
        Vector3 projection = lineDirection * projectionLength;
        Vector3 projectionPoint = start + projection;

        // 计算rejection (垂直分量)
        Vector3 rejection = point - projectionPoint;

        // 点到线的距离
        float distance = rejection.magnitude;

        // 显示线
        Gizmos.color = lineColor;
        DrawArrow(start, end, 0.3f);
        DrawLabel((start + end) / 2, "投影线");

        // 显示从起点到目标点的向量
        if (showVectors)
        {
            Gizmos.color = vectorColor;
            DrawArrow(start, point, 0.3f);
            DrawLabel((start + point) / 2, $"原始向量\n长度: {startToPoint.magnitude:F2}");
        }

        // 显示投影
        if (showProjection)
        {
            Gizmos.color = projectionColor;
            DrawArrow(start, projectionPoint, 0.35f);
            DrawLabel((start + projectionPoint) / 2,
                $"投影\n长度: {projectionLength:F2}");

            // 投影点
            Gizmos.DrawWireSphere(projectionPoint, 0.15f);
        }

        // 显示rejection (垂直分量)
        if (showRejection)
        {
            Gizmos.color = rejectionColor;
            DrawArrow(projectionPoint, point, 0.3f);
            DrawLabel((projectionPoint + point) / 2,
                $"垂直分量\n长度: {rejection.magnitude:F2}");
        }

        // 显示最近点
        if (showClosestPoint)
        {
            // 处理线段范围限制
            float t = Vector3.Dot(startToPoint, lineVector) / lineVector.sqrMagnitude;
            t = Mathf.Clamp01(t); // 限制在线段范围内
            Vector3 closestPoint = start + lineVector * t;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(closestPoint, 0.2f);

            // 从最近点到目标点的连线
            Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
            Gizmos.DrawLine(closestPoint, point);

            DrawLabel(closestPoint + Vector3.up * 0.5f,
                $"最近点\nt = {t:F2}");
        }

        // 显示距离
        if (showDistance)
        {
            DrawLabel(point + Vector3.up,
                $"到线的距离: {distance:F2}");
        }

        // 光照示例
        if (showLightingExample && lightDirection != null && surfaceNormal != null)
        {
            Vector3 surfacePos = transform.position;
            Vector3 normal = surfaceNormal.forward.normalized;
            Vector3 light = -lightDirection.forward.normalized; // 光线方向

            // 计算光照强度 (兰伯特余弦定律)
            float lightIntensity = Mathf.Max(0, Vector3.Dot(normal, light));

            // 绘制表面法线
            Gizmos.color = Color.blue;
            DrawArrow(surfacePos, surfacePos + normal * 2f, 0.3f);
            DrawLabel(surfacePos + normal, "表面法线");

            // 绘制光线方向
            Gizmos.color = Color.yellow;
            DrawArrow(surfacePos, surfacePos + light * 2f, 0.3f);
            DrawLabel(surfacePos + light, "光线方向");

            // 绘制光照投影
            Vector3 lightProjection = normal * lightIntensity;
            Gizmos.color = Color.red;
            DrawArrow(surfacePos, surfacePos + lightProjection, 0.25f);

            DrawLabel(surfacePos + Vector3.down,
                $"光照强度: {lightIntensity:F2}\n" +
                $"夹角: {Mathf.Acos(lightIntensity) * Mathf.Rad2Deg:F1}°");

            // 绘制表面（半透明平面）
            DrawSurface(surfacePos, normal, 2f, new Color(1f, 1f, 1f, lightIntensity * 0.5f));
        }

        DrawCoordinateSystem();
    }

    /// <summary>
    /// 绘制表面
    /// </summary>
    void DrawSurface(Vector3 center, Vector3 normal, float size, Color color)
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

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        // 填充效果
        for (float t = 0; t <= 1f; t += 0.1f)
        {
            Vector3 start = Vector3.Lerp(p1, p2, t);
            Vector3 end = Vector3.Lerp(p4, p3, t);
            Gizmos.DrawLine(start, end);
        }
    }

    /// <summary>
    /// 绘制箭头
    /// </summary>
    void DrawArrow(Vector3 start, Vector3 end, float arrowHeadSize = 0.25f)
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
    void DrawCoordinateSystem()
    {
        Vector3 origin = Vector3.zero;
        float axisLength = 1f;

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawLine(origin, origin + Vector3.right * axisLength);

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawLine(origin, origin + Vector3.up * axisLength);

        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawLine(origin, origin + Vector3.forward * axisLength);
    }

    /// <summary>
    /// 绘制文本标签
    /// </summary>
    void DrawLabel(Vector3 position, string text)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, text);
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 start = lineStart.position;
            Vector3 end = lineEnd.position;
            Vector3 point = pointA.position;

            Vector3 lineDirection = (end - start).normalized;
            Vector3 startToPoint = point - start;

            float projectionLength = Vector3.Dot(startToPoint, lineDirection);
            Vector3 projection = lineDirection * projectionLength;
            Vector3 rejection = startToPoint - projection;

            Debug.Log("=== 向量投影示例 ===");
            Debug.Log($"线方向: {lineDirection}");
            Debug.Log($"原始向量: {startToPoint}");
            Debug.Log($"投影长度: {projectionLength}");
            Debug.Log($"投影向量: {projection}");
            Debug.Log($"垂直分量: {rejection}");
            Debug.Log($"到线的距离: {rejection.magnitude}");

            // 验证: 原始向量 = 投影 + 垂直分量
            Vector3 reconstructed = projection + rejection;
            Debug.Log($"验证重构: {reconstructed} (应该等于 {startToPoint})");

            if (showLightingExample && lightDirection != null && surfaceNormal != null)
            {
                Vector3 normal = surfaceNormal.forward.normalized;
                Vector3 light = -lightDirection.forward.normalized;
                float intensity = Mathf.Max(0, Vector3.Dot(normal, light));
                Debug.Log($"光照强度: {intensity}");
            }
        }
    }
}
