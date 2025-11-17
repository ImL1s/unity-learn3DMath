using UnityEngine;

/// <summary>
/// 叉积(Cross Product)演示
/// 叉积可以用来：
/// 1. 获得垂直于两个向量的新向量
/// 2. 判断左右方向（顺时针/逆时针）
/// 3. 计算三角形面积
/// 4. 计算法线向量
/// </summary>
public class CrossProductDemo : MonoBehaviour
{
    [Header("向量设置")]
    public Transform pointA;
    public Transform pointB;

    [Header("显示选项")]
    public bool showVectorA = true;
    public bool showVectorB = true;
    public bool showCrossProduct = true;
    public bool showTriangleArea = false;
    public bool showDirectionTest = false;

    [Header("方向测试")]
    public Transform testPoint;         // 用于测试左右方向

    [Header("颜色设置")]
    public Color colorA = Color.red;
    public Color colorB = Color.blue;
    public Color colorCross = Color.green;
    public Color colorTriangle = new Color(1f, 1f, 0f, 0.3f);

    void Start()
    {
        // 自动创建演示对象（如果为空）
        if (pointA == null)
        {
            GameObject objA = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objA.name = "PointA";
            pointA = objA.transform;
            pointA.position = transform.position + new Vector3(2, 0, 0);
            pointA.localScale = Vector3.one * 0.3f;

            Renderer renderer = objA.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = colorA;
                renderer.material = mat;
            }
        }

        if (pointB == null)
        {
            GameObject objB = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objB.name = "PointB";
            pointB = objB.transform;
            pointB.position = transform.position + new Vector3(1, 0, 2);
            pointB.localScale = Vector3.one * 0.3f;

            Renderer renderer = objB.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = colorB;
                renderer.material = mat;
            }
        }

        if (testPoint == null)
        {
            GameObject testObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObj.name = "TestPoint";
            testPoint = testObj.transform;
            testPoint.position = transform.position + new Vector3(0.5f, 0, 1.5f);
            testPoint.localScale = Vector3.one * 0.3f;

            Renderer renderer = testObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.yellow;
                renderer.material = mat;
            }
        }

        Debug.Log("CrossProductDemo: 已自动创建PointA, PointB和TestPoint");
    }

    void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;

        Vector3 origin = transform.position;
        Vector3 vecA = pointA.position - origin;
        Vector3 vecB = pointB.position - origin;

        // 计算叉积
        Vector3 crossProduct = Vector3.Cross(vecA, vecB);

        // 显示向量A
        if (showVectorA)
        {
            Gizmos.color = colorA;
            DrawArrow(origin, origin + vecA, 0.3f);
            DrawLabel(origin + vecA / 2, $"向量A\n{vecA.ToString("F2")}");
        }

        // 显示向量B
        if (showVectorB)
        {
            Gizmos.color = colorB;
            DrawArrow(origin, origin + vecB, 0.3f);
            DrawLabel(origin + vecB / 2, $"向量B\n{vecB.ToString("F2")}");
        }

        // 显示叉积结果（垂直向量）
        if (showCrossProduct && crossProduct.magnitude > 0.001f)
        {
            Gizmos.color = colorCross;
            Vector3 normalizedCross = crossProduct.normalized * 2f;
            DrawArrow(origin, origin + normalizedCross, 0.4f);

            DrawLabel(origin + normalizedCross / 2,
                $"A × B (叉积)\n" +
                $"长度: {crossProduct.magnitude:F2}\n" +
                $"方向: {crossProduct.normalized.ToString("F2")}");

            // 绘制垂直于叉积向量的平面（半透明）
            DrawPerpendicularPlane(origin, crossProduct.normalized, 2f);
        }

        // 显示三角形面积
        if (showTriangleArea)
        {
            // 叉积的长度 = 平行四边形的面积
            // 三角形面积 = 平行四边形面积 / 2
            float parallelogramArea = crossProduct.magnitude;
            float triangleArea = parallelogramArea / 2f;

            // 绘制三角形
            Gizmos.color = colorTriangle;
            Gizmos.DrawLine(origin, origin + vecA);
            Gizmos.DrawLine(origin, origin + vecB);
            Gizmos.DrawLine(origin + vecA, origin + vecB);

            // 填充三角形（用多条线模拟）
            for (float t = 0; t <= 1f; t += 0.05f)
            {
                Vector3 start = origin + vecA * t;
                Vector3 end = origin + vecB * t;
                Gizmos.DrawLine(start, end);
            }

            Vector3 center = origin + (vecA + vecB) / 3f;
            DrawLabel(center,
                $"三角形面积: {triangleArea:F2}\n" +
                $"平行四边形面积: {parallelogramArea:F2}");
        }

        // 方向测试（判断点在向量的左侧还是右侧）
        if (showDirectionTest && testPoint != null)
        {
            Vector3 toTest = testPoint.position - origin;

            // 使用叉积判断方向
            // 如果叉积的Y分量为正，说明testPoint在vecA的左侧
            // 如果为负，说明在右侧
            Vector3 cross2D = Vector3.Cross(vecA, toTest);
            string direction = cross2D.y > 0 ? "左侧" : cross2D.y < 0 ? "右侧" : "共线";

            Gizmos.color = cross2D.y > 0 ? Color.cyan : Color.magenta;
            DrawArrow(origin, testPoint.position, 0.3f);

            DrawLabel(testPoint.position + Vector3.up,
                $"测试点在向量A的{direction}\n" +
                $"叉积Y值: {cross2D.y:F2}");

            // 绘制参考线
            Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
            Gizmos.DrawLine(origin, testPoint.position);
        }

        // 绘制坐标系参考
        DrawCoordinateSystem(origin);

        // 显示右手法则提示
        if (showCrossProduct)
        {
            DrawLabel(origin + Vector3.up * 3f,
                "右手法则：\n" +
                "食指指向A，中指指向B\n" +
                "大拇指指向A×B");
        }
    }

    /// <summary>
    /// 绘制垂直于给定向量的平面
    /// </summary>
    void DrawPerpendicularPlane(Vector3 center, Vector3 normal, float size)
    {
        Vector3 right = Vector3.Cross(normal, Vector3.up);
        if (right.magnitude < 0.001f)
            right = Vector3.Cross(normal, Vector3.right);
        right = right.normalized;

        Vector3 forward = Vector3.Cross(right, normal).normalized;

        Gizmos.color = new Color(colorCross.r, colorCross.g, colorCross.b, 0.2f);

        Vector3 p1 = center + (right + forward) * size * 0.5f;
        Vector3 p2 = center + (-right + forward) * size * 0.5f;
        Vector3 p3 = center + (-right - forward) * size * 0.5f;
        Vector3 p4 = center + (right - forward) * size * 0.5f;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        // 绘制对角线
        Gizmos.DrawLine(p1, p3);
        Gizmos.DrawLine(p2, p4);
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
    void DrawCoordinateSystem(Vector3 origin)
    {
        float axisLength = 1f;

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawLine(origin, origin + Vector3.right * axisLength);

        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawLine(origin, origin + Vector3.up * axisLength);

        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
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
            Vector3 origin = transform.position;
            Vector3 vecA = pointA.position - origin;
            Vector3 vecB = pointB.position - origin;
            Vector3 cross = Vector3.Cross(vecA, vecB);

            Debug.Log("=== 叉积示例 ===");
            Debug.Log($"向量A: {vecA}");
            Debug.Log($"向量B: {vecB}");
            Debug.Log($"A × B = {cross}");
            Debug.Log($"叉积长度(平行四边形面积): {cross.magnitude}");
            Debug.Log($"三角形面积: {cross.magnitude / 2f}");

            // 验证垂直性
            float dotAC = Vector3.Dot(vecA, cross);
            float dotBC = Vector3.Dot(vecB, cross);
            Debug.Log($"A · (A×B) = {dotAC:F6} (应该接近0，表示垂直)");
            Debug.Log($"B · (A×B) = {dotBC:F6} (应该接近0，表示垂直)");

            if (testPoint != null && showDirectionTest)
            {
                Vector3 toTest = testPoint.position - origin;
                Vector3 cross2D = Vector3.Cross(vecA, toTest);
                Debug.Log($"测试点方向: {(cross2D.y > 0 ? "左侧" : "右侧")} (叉积Y={cross2D.y:F2})");
            }
        }
    }
}
