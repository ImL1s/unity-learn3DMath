using UnityEngine;

/// <summary>
/// 点积(Dot Product)演示
/// 点积可以用来：
/// 1. 计算两个向量的夹角
/// 2. 判断方向（前/后/左/右）
/// 3. 计算投影长度
/// 4. 视野检测（FOV）
/// </summary>
public class DotProductDemo : MonoBehaviour
{
    [Header("参考对象")]
    public Transform observer;      // 观察者
    public Transform target;        // 目标

    [Header("显示选项")]
    public bool showVectors = true;
    public bool showAngle = true;
    public bool showProjection = false;
    public bool showFOV = false;

    [Header("视野检测设置")]
    [Range(0, 180)]
    public float fieldOfViewAngle = 60f;

    [Header("颜色设置")]
    public Color forwardColor = Color.blue;
    public Color toTargetColor = Color.red;
    public Color fovColor = new Color(1f, 1f, 0f, 0.3f);

    private bool isInFOV = false;

    void Start()
    {
        // 自动创建演示对象（如果为空）
        if (observer == null)
        {
            GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obs.name = "Observer";
            observer = obs.transform;
            observer.position = Vector3.zero;
            observer.rotation = Quaternion.Euler(0, 45, 0);
            observer.localScale = new Vector3(0.5f, 1f, 0.5f);

            Renderer renderer = obs.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.blue;
                renderer.material = mat;
            }
        }

        if (target == null)
        {
            GameObject targ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            targ.name = "Target";
            target = targ.transform;
            target.position = new Vector3(3, 0, 2);
            target.localScale = Vector3.one * 0.4f;

            Renderer renderer = targ.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.red;
                renderer.material = mat;
            }
        }

        Debug.Log("DotProductDemo: 已自动创建Observer和Target");
    }

    void OnDrawGizmos()
    {
        if (observer == null || target == null) return;

        Vector3 observerPos = observer.position;
        Vector3 targetPos = target.position;

        // 观察者的前方向
        Vector3 forward = observer.forward;
        // 从观察者指向目标的向量
        Vector3 toTarget = (targetPos - observerPos).normalized;

        // 计算点积
        float dotProduct = Vector3.Dot(forward, toTarget);

        // 计算夹角（弧度转角度）
        // Clamp防止浮点误差导致超出[-1,1]范围
        dotProduct = Mathf.Clamp(dotProduct, -1f, 1f);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        // 显示向量
        if (showVectors)
        {
            // 观察者前方向
            Gizmos.color = forwardColor;
            DrawArrow(observerPos, observerPos + forward * 2f, 0.3f);
            DrawLabel(observerPos + forward, "前方向");

            // 指向目标的向量
            Gizmos.color = toTargetColor;
            DrawArrow(observerPos, targetPos, 0.3f);
            DrawLabel((observerPos + targetPos) / 2, "到目标");
        }

        // 显示夹角和点积值
        if (showAngle)
        {
            Vector3 labelPos = observerPos + Vector3.up * 2f;
            string direction = GetDirectionText(dotProduct);
            DrawLabel(labelPos,
                $"点积: {dotProduct:F3}\n" +
                $"夹角: {angle:F1}°\n" +
                $"方向: {direction}");

            // 绘制夹角弧线
            DrawAngleArc(observerPos, forward, toTarget, angle);
        }

        // 显示投影
        if (showProjection)
        {
            // 计算toTarget在forward上的投影
            float projectionLength = Vector3.Dot(toTarget, forward);
            Vector3 projection = forward * projectionLength;

            // 计算一次距离，避免重复计算
            float distance = Vector3.Distance(observerPos, targetPos);

            Gizmos.color = Color.green;
            DrawArrow(observerPos, observerPos + projection * distance, 0.25f);

            // 投影点
            Vector3 projectionPoint = observerPos + projection * distance;
            Gizmos.DrawWireSphere(projectionPoint, 0.1f);

            // 从投影点到目标的垂直线
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawLine(projectionPoint, targetPos);

            DrawLabel(projectionPoint, $"投影长度: {projectionLength:F2}");
        }

        // 视野检测
        if (showFOV)
        {
            isInFOV = angle <= fieldOfViewAngle / 2f;

            // 绘制视野范围
            Gizmos.color = fovColor;
            DrawFOVCone(observerPos, forward, fieldOfViewAngle, 3f);

            // 目标指示
            Gizmos.color = isInFOV ? Color.green : Color.red;
            Gizmos.DrawWireSphere(targetPos, 0.3f);

            DrawLabel(targetPos + Vector3.up,
                isInFOV ? "在视野内" : "不在视野内");
        }
    }

    /// <summary>
    /// 根据点积值判断方向
    /// </summary>
    string GetDirectionText(float dotProduct)
    {
        if (dotProduct > 0.5f)
            return "前方";
        else if (dotProduct < -0.5f)
            return "后方";
        else
            return "侧面";
    }

    /// <summary>
    /// 绘制夹角弧线
    /// </summary>
    void DrawAngleArc(Vector3 center, Vector3 from, Vector3 to, float angle)
    {
        if (angle < 0.1f || angle > 179.9f) return;

        Gizmos.color = Color.yellow;

        Vector3 cross = Vector3.Cross(from, to);
        int segments = 20;
        Vector3 previousPoint = center + from * 0.5f;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            Quaternion rotation = Quaternion.AngleAxis(angle * t, cross);
            Vector3 point = center + rotation * (from * 0.5f);
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }

    /// <summary>
    /// 绘制视野锥体
    /// </summary>
    void DrawFOVCone(Vector3 origin, Vector3 forward, float fovAngle, float distance)
    {
        float halfFOV = fovAngle / 2f;

        // 获取垂直于forward的两个轴
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        if (right.magnitude < 0.001f)
            right = Vector3.Cross(Vector3.right, forward);
        right = right.normalized;
        Vector3 up = Vector3.Cross(forward, right).normalized;

        // 绘制视野边界
        int segments = 32;
        Vector3 previousPoint = origin + Quaternion.AngleAxis(-halfFOV, up) * forward * distance;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfFOV + (fovAngle * i / segments);
            Vector3 dir = Quaternion.AngleAxis(angle, up) * forward;
            Vector3 point = origin + dir * distance;

            Gizmos.DrawLine(previousPoint, point);
            Gizmos.DrawLine(origin, point);
            previousPoint = point;
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

        Vector3 arrowTip = end - direction * arrowHeadSize;
        Gizmos.DrawLine(end, arrowTip + right * arrowHeadSize * 0.5f);
        Gizmos.DrawLine(end, arrowTip - right * arrowHeadSize * 0.5f);
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
            Vector3 forward = observer.forward;
            Vector3 toTarget = (target.position - observer.position).normalized;
            float dot = Vector3.Dot(forward, toTarget);
            dot = Mathf.Clamp(dot, -1f, 1f);  // 防止浮点误差
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            Debug.Log("=== 点积示例 ===");
            Debug.Log($"前方向: {forward}");
            Debug.Log($"到目标方向: {toTarget}");
            Debug.Log($"点积值: {dot}");
            Debug.Log($"夹角: {angle}度");
            Debug.Log($"方向判断: {GetDirectionText(dot)}");

            if (showFOV)
            {
                Debug.Log($"是否在视野内: {isInFOV}");
            }
        }
    }
}
