using UnityEngine;

/// <summary>
/// 四元数基础示例
/// 演示四元数的基本概念和与欧拉角的对比
/// </summary>
public class QuaternionBasics : MonoBehaviour
{
    [Header("目标物体")]
    public Transform targetObject;

    [Header("旋转方式")]
    public bool useQuaternion = true;
    public bool useEulerAngles = false;

    [Header("欧拉角设置")]
    [Range(0, 360)]
    public float eulerX = 0f;
    [Range(0, 360)]
    public float eulerY = 0f;
    [Range(0, 360)]
    public float eulerZ = 0f;

    [Header("四元数设置")]
    [Range(-360, 360)]
    public float rotationAngle = 45f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("显示选项")]
    public bool showRotationAxis = true;
    public bool showLocalAxes = true;
    public bool showQuaternionInfo = true;

    [Header("万向锁演示")]
    public bool demonstrateGimbalLock = false;
    [Range(-90, 90)]
    public float gimbalPitch = 0f;

    void Start()
    {
        // 自动创建目标对象（如果为空）
        if (targetObject == null)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "TargetObject";
            targetObject = obj.transform;
            targetObject.position = transform.position;
            targetObject.localScale = Vector3.one;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.cyan;
                renderer.material = mat;
            }
        }

        Debug.Log("QuaternionBasics: 已自动创建TargetObject");
    }

    void OnDrawGizmos()
    {
        if (targetObject == null) return;

        Vector3 pos = targetObject.position;

        // 显示旋转轴
        if (showRotationAxis && useQuaternion)
        {
            Vector3 normalizedAxis = rotationAxis.normalized;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pos - normalizedAxis * 2f, pos + normalizedAxis * 2f);

            // 绘制旋转方向
            DrawRotationIndicator(pos, normalizedAxis, rotationAngle);

            DrawLabel(pos + normalizedAxis * 2.5f,
                $"旋转轴: {normalizedAxis.ToString("F2")}\n" +
                $"角度: {rotationAngle:F1}°");
        }

        // 显示本地坐标轴
        if (showLocalAxes)
        {
            float axisLength = 1.5f;

            // X轴 - 红色
            Vector3 right = targetObject.right;
            Gizmos.color = Color.red;
            DebugDrawer.DrawArrow(pos, pos + right * axisLength, 0.2f);
            DrawLabel(pos + right * (axisLength + 0.3f), "Right (X)");

            // Y轴 - 绿色
            Vector3 up = targetObject.up;
            Gizmos.color = Color.green;
            DebugDrawer.DrawArrow(pos, pos + up * axisLength, 0.2f);
            DrawLabel(pos + up * (axisLength + 0.3f), "Up (Y)");

            // Z轴 - 蓝色
            Vector3 forward = targetObject.forward;
            Gizmos.color = Color.blue;
            DebugDrawer.DrawArrow(pos, pos + forward * axisLength, 0.2f);
            DrawLabel(pos + forward * (axisLength + 0.3f), "Forward (Z)");
        }

        // 显示四元数信息
        if (showQuaternionInfo)
        {
            Quaternion rot = targetObject.rotation;
            Vector3 euler = rot.eulerAngles;

            DrawLabel(pos + Vector3.up * 3f,
                $"四元数: ({rot.x:F3}, {rot.y:F3}, {rot.z:F3}, {rot.w:F3})\n" +
                $"欧拉角: ({euler.x:F1}°, {euler.y:F1}°, {euler.z:F1}°)");
        }

        // 万向锁演示
        if (demonstrateGimbalLock)
        {
            DrawGimbalLockVisualization(pos);
        }
    }

    void Update()
    {
        if (targetObject == null) return;

        // 使用四元数旋转
        if (useQuaternion)
        {
            // 方法1: 使用AngleAxis
            Quaternion rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis.normalized);
            targetObject.rotation = rotation;
        }
        // 使用欧拉角旋转
        else if (useEulerAngles)
        {
            if (demonstrateGimbalLock)
            {
                // 演示万向锁：先绕X轴旋转到90度
                targetObject.rotation = Quaternion.Euler(gimbalPitch, eulerY, eulerZ);
            }
            else
            {
                targetObject.rotation = Quaternion.Euler(eulerX, eulerY, eulerZ);
            }
        }

        // 按空格键输出详细信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogQuaternionInfo();
        }
    }

    /// <summary>
    /// 绘制旋转方向指示器
    /// </summary>
    void DrawRotationIndicator(Vector3 center, Vector3 axis, float angle)
    {
        // 获取垂直于旋转轴的向量
        Vector3 perpendicular = Vector3.Cross(axis, Vector3.up);
        if (perpendicular.magnitude < 0.001f)
            perpendicular = Vector3.Cross(axis, Vector3.right);
        perpendicular = perpendicular.normalized;

        // 绘制圆弧表示旋转
        int segments = 20;
        float radius = 1f;
        Vector3 previousPoint = center + perpendicular * radius;

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            Quaternion rot = Quaternion.AngleAxis(angle * t, axis);
            Vector3 point = center + rot * perpendicular * radius;
            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        // 绘制箭头表示方向
        Quaternion finalRot = Quaternion.AngleAxis(angle, axis);
        Vector3 arrowEnd = center + finalRot * perpendicular * radius;
        Vector3 arrowDirection = (arrowEnd - previousPoint).normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(arrowEnd - arrowDirection * 0.2f, arrowEnd);
    }

    /// <summary>
    /// 绘制万向锁可视化
    /// </summary>
    void DrawGimbalLockVisualization(Vector3 center)
    {
        float radius = 2f;

        // 绘制三个万向环
        // X轴环（红色）
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        DebugDrawer.DrawCircle(center, Vector3.right, radius, 32);

        // Y轴环（绿色）- 会随X轴旋转
        Quaternion xRot = Quaternion.Euler(gimbalPitch, 0, 0);
        Vector3 yAxis = xRot * Vector3.up;
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        DebugDrawer.DrawCircle(center, yAxis, radius * 0.9f, 32);

        // Z轴环（蓝色）- 会随X和Y旋转
        Vector3 zAxis = xRot * Vector3.forward;
        Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
        DebugDrawer.DrawCircle(center, zAxis, radius * 0.8f, 32);

        // 当pitch接近90度时，显示警告
        if (Mathf.Abs(gimbalPitch) > 85f)
        {
            DrawLabel(center + Vector3.down * 3f,
                "<color=red>警告：接近万向锁！\n" +
                "Y轴和Z轴几乎重合</color>");
        }
    }

    /// <summary>
    /// 输出四元数详细信息
    /// </summary>
    void LogQuaternionInfo()
    {
        Quaternion rot = targetObject.rotation;
        Vector3 euler = rot.eulerAngles;

        Debug.Log("=== 四元数信息 ===");
        Debug.Log($"四元数 (x,y,z,w): ({rot.x:F4}, {rot.y:F4}, {rot.z:F4}, {rot.w:F4})");
        Debug.Log($"欧拉角 (x,y,z): ({euler.x:F2}°, {euler.y:F2}°, {euler.z:F2}°)");

        // 验证四元数是单位四元数
        float magnitude = Mathf.Sqrt(rot.x * rot.x + rot.y * rot.y + rot.z * rot.z + rot.w * rot.w);
        Debug.Log($"四元数长度: {magnitude:F6} (应该≈1.0)");

        if (useQuaternion)
        {
            Debug.Log($"旋转轴: {rotationAxis.normalized}");
            Debug.Log($"旋转角度: {rotationAngle}°");

            // 从四元数反推旋转轴和角度
            float angle;
            Vector3 axis;
            rot.ToAngleAxis(out angle, out axis);
            Debug.Log($"从四元数提取的旋转轴: {axis}");
            Debug.Log($"从四元数提取的角度: {angle}°");
        }

        // 显示旋转矩阵的行列式（应该为1）
        Matrix4x4 matrix = Matrix4x4.Rotate(rot);
        Debug.Log($"旋转矩阵:\n{matrix}");
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

    void OnGUI()
    {
        // 在Game视图中显示说明
        GUILayout.BeginArea(new Rect(10, 10, 400, 300));
        GUILayout.Box("四元数基础示例");

        GUILayout.Label($"当前模式: {(useQuaternion ? "四元数" : useEulerAngles ? "欧拉角" : "无")}");

        if (demonstrateGimbalLock)
        {
            GUILayout.Label("万向锁演示模式");
            GUILayout.Label($"Pitch: {gimbalPitch:F1}°");

            if (Mathf.Abs(gimbalPitch) > 85f)
            {
                GUI.color = Color.red;
                GUILayout.Label("警告：万向锁！");
                GUI.color = Color.white;
            }
        }

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }
}
