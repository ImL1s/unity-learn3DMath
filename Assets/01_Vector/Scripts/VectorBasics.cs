using UnityEngine;

/// <summary>
/// 向量基础示例
/// 演示向量的基本运算：加法、减法、缩放、归一化、长度计算
/// </summary>
public class VectorBasics : MonoBehaviour
{
    [Header("向量A和B")]
    public Transform pointA;
    public Transform pointB;

    [Header("可视化设置")]
    public bool showVectorA = true;
    public bool showVectorB = true;
    public bool showAddition = false;      // A + B
    public bool showSubtraction = false;   // B - A
    public bool showNormalized = false;    // 归一化向量
    public bool showScaled = false;        // 缩放向量
    public float scaleMultiplier = 2f;

    [Header("显示设置")]
    public Color colorA = Color.red;
    public Color colorB = Color.blue;
    public Color colorResult = Color.green;
    public Color colorNormalized = Color.yellow;

    void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;

        Vector3 vecA = pointA.position;
        Vector3 vecB = pointB.position;

        // 显示向量A (从原点到A点)
        if (showVectorA)
        {
            Gizmos.color = colorA;
            DrawArrow(Vector3.zero, vecA, 0.3f);
            DrawLabel(vecA / 2, $"向量A\n长度: {vecA.magnitude:F2}");
        }

        // 显示向量B (从原点到B点)
        if (showVectorB)
        {
            Gizmos.color = colorB;
            DrawArrow(Vector3.zero, vecB, 0.3f);
            DrawLabel(vecB / 2, $"向量B\n长度: {vecB.magnitude:F2}");
        }

        // 向量加法: A + B
        if (showAddition)
        {
            Vector3 sum = vecA + vecB;
            Gizmos.color = colorResult;
            DrawArrow(Vector3.zero, sum, 0.4f);
            DrawLabel(sum / 2, $"A + B\n= ({sum.x:F1}, {sum.y:F1}, {sum.z:F1})");

            // 显示平行四边形法则
            Gizmos.color = new Color(colorResult.r, colorResult.g, colorResult.b, 0.3f);
            Gizmos.DrawLine(vecA, sum);
            Gizmos.DrawLine(vecB, sum);
        }

        // 向量减法: B - A (从A指向B的向量)
        if (showSubtraction)
        {
            Vector3 diff = vecB - vecA;
            Gizmos.color = colorResult;
            DrawArrow(vecA, vecB, 0.4f);

            Vector3 midPoint = (vecA + vecB) / 2;
            DrawLabel(midPoint, $"B - A\n长度: {diff.magnitude:F2}\n= ({diff.x:F1}, {diff.y:F1}, {diff.z:F1})");
        }

        // 归一化向量 (单位向量)
        if (showNormalized && vecA.magnitude > 0.001f)
        {
            Vector3 normalized = vecA.normalized;
            Gizmos.color = colorNormalized;
            DrawArrow(Vector3.zero, normalized, 0.3f);
            DrawLabel(normalized / 2, $"A归一化\n长度: {normalized.magnitude:F3}");
        }

        // 缩放向量
        if (showScaled)
        {
            Vector3 scaled = vecA * scaleMultiplier;
            Gizmos.color = colorResult;
            DrawArrow(Vector3.zero, scaled, 0.4f);
            DrawLabel(scaled / 2, $"A × {scaleMultiplier:F1}\n长度: {scaled.magnitude:F2}");
        }

        // 绘制坐标系
        DrawCoordinateSystem();
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
    /// 绘制文本标签（在Scene视图中）
    /// </summary>
    void DrawLabel(Vector3 position, string text)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, text);
#endif
    }

    /// <summary>
    /// 绘制坐标系
    /// </summary>
    void DrawCoordinateSystem()
    {
        float axisLength = 1f;

        // X轴 - 红色
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawLine(Vector3.zero, Vector3.right * axisLength);

        // Y轴 - 绿色
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawLine(Vector3.zero, Vector3.up * axisLength);

        // Z轴 - 蓝色
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        Gizmos.DrawLine(Vector3.zero, Vector3.forward * axisLength);
    }

    void Update()
    {
        // 运行时的一些向量运算示例（在Console中查看）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 vecA = pointA.position;
            Vector3 vecB = pointB.position;

            Debug.Log("=== 向量运算示例 ===");
            Debug.Log($"向量A: {vecA}");
            Debug.Log($"向量B: {vecB}");
            Debug.Log($"A的长度: {vecA.magnitude}");
            Debug.Log($"B的长度: {vecB.magnitude}");
            Debug.Log($"A + B = {vecA + vecB}");
            Debug.Log($"B - A = {vecB - vecA}");
            Debug.Log($"A的归一化: {vecA.normalized}");
            Debug.Log($"A和B的距离: {Vector3.Distance(vecA, vecB)}");
        }
    }
}
