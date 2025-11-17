using UnityEngine;

/// <summary>
/// 四元数旋转和插值示例
/// 演示Lerp、Slerp、LookRotation等常用旋转操作
/// </summary>
public class QuaternionRotation : MonoBehaviour
{
    [Header("旋转对象")]
    public Transform rotatingObject;

    [Header("旋转模式")]
    public RotationMode mode = RotationMode.Lerp;

    [Header("Lerp vs Slerp 设置")]
    public Transform startRotation;
    public Transform endRotation;
    [Range(0, 1)]
    public float interpolationT = 0.5f;
    public bool animate = false;
    public float animationSpeed = 0.5f;

    [Header("LookAt 设置")]
    public Transform lookAtTarget;
    public bool smoothLookAt = true;
    public float lookAtSpeed = 5f;

    [Header("旋转到目标")]
    public Transform targetRotation;
    public float rotateToSpeed = 2f;

    [Header("显示选项")]
    public bool showStartEnd = true;
    public bool showInterpolated = true;
    public bool showDifference = true;

    private float animationTime = 0f;

    public enum RotationMode
    {
        Lerp,           // 线性插值
        Slerp,          // 球面线性插值
        LookAt,         // 朝向目标
        RotateTowards   // 旋转到目标
    }

    void Update()
    {
        if (rotatingObject == null) return;

        switch (mode)
        {
            case RotationMode.Lerp:
            case RotationMode.Slerp:
                UpdateInterpolation();
                break;

            case RotationMode.LookAt:
                UpdateLookAt();
                break;

            case RotationMode.RotateTowards:
                UpdateRotateTowards();
                break;
        }

        // 按空格键输出信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogRotationInfo();
        }
    }

    void UpdateInterpolation()
    {
        if (startRotation == null || endRotation == null) return;

        float t = interpolationT;

        if (animate)
        {
            animationTime += Time.deltaTime * animationSpeed;
            t = Mathf.PingPong(animationTime, 1f);
        }

        Quaternion start = startRotation.rotation;
        Quaternion end = endRotation.rotation;

        if (mode == RotationMode.Lerp)
        {
            // 线性插值 - 不保证匀速
            rotatingObject.rotation = Quaternion.Lerp(start, end, t);
        }
        else if (mode == RotationMode.Slerp)
        {
            // 球面线性插值 - 保证匀速
            rotatingObject.rotation = Quaternion.Slerp(start, end, t);
        }
    }

    void UpdateLookAt()
    {
        if (lookAtTarget == null) return;

        Vector3 direction = lookAtTarget.position - rotatingObject.position;

        if (direction.magnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (smoothLookAt)
        {
            // 平滑旋转
            rotatingObject.rotation = Quaternion.Slerp(
                rotatingObject.rotation,
                targetRotation,
                Time.deltaTime * lookAtSpeed
            );
        }
        else
        {
            // 立即朝向
            rotatingObject.rotation = targetRotation;
        }
    }

    void UpdateRotateTowards()
    {
        if (targetRotation == null) return;

        // RotateTowards: 以固定角速度旋转
        float maxDegreesDelta = rotateToSpeed * 180f * Time.deltaTime;

        rotatingObject.rotation = Quaternion.RotateTowards(
            rotatingObject.rotation,
            targetRotation.rotation,
            maxDegreesDelta
        );
    }

    void OnDrawGizmos()
    {
        if (rotatingObject == null) return;

        Vector3 pos = rotatingObject.position;

        // 绘制旋转对象的坐标轴
        DrawObjectAxes(rotatingObject, 1.5f);

        // 不同模式的可视化
        switch (mode)
        {
            case RotationMode.Lerp:
            case RotationMode.Slerp:
                DrawInterpolationVisualization();
                break;

            case RotationMode.LookAt:
                DrawLookAtVisualization();
                break;

            case RotationMode.RotateTowards:
                DrawRotateTowardsVisualization();
                break;
        }
    }

    void DrawInterpolationVisualization()
    {
        if (startRotation == null || endRotation == null) return;

        // 绘制起始和结束旋转
        if (showStartEnd)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            DrawObjectAxes(startRotation, 1.2f);
            DrawLabel(startRotation.position + Vector3.up * 2f, "起始旋转");

            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            DrawObjectAxes(endRotation, 1.2f);
            DrawLabel(endRotation.position + Vector3.up * 2f, "结束旋转");
        }

        // 显示差异
        if (showDifference && rotatingObject != null)
        {
            float angle = Quaternion.Angle(
                startRotation.rotation,
                endRotation.rotation
            );

            float currentAngle = Quaternion.Angle(
                startRotation.rotation,
                rotatingObject.rotation
            );

            DrawLabel(rotatingObject.position + Vector3.up * 3f,
                $"模式: {mode}\n" +
                $"总角度差: {angle:F1}°\n" +
                $"当前角度: {currentAngle:F1}°\n" +
                $"插值参数t: {interpolationT:F3}");
        }

        // 绘制Lerp和Slerp的区别
        if (mode == RotationMode.Lerp || mode == RotationMode.Slerp)
        {
            DrawInterpolationPath();
        }
    }

    void DrawInterpolationPath()
    {
        if (startRotation == null || endRotation == null) return;

        Quaternion start = startRotation.rotation;
        Quaternion end = endRotation.rotation;

        // 绘制插值路径
        int steps = 20;
        Vector3 basePos = rotatingObject.position + Vector3.down * 2f;

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;

            Quaternion lerp = Quaternion.Lerp(start, end, t);
            Quaternion slerp = Quaternion.Slerp(start, end, t);

            // Lerp路径（黄色）
            Vector3 lerpDir = lerp * Vector3.forward * 0.5f;
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(basePos + lerpDir + Vector3.left * 1.5f, 0.05f);

            // Slerp路径（青色）
            Vector3 slerpDir = slerp * Vector3.forward * 0.5f;
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawSphere(basePos + slerpDir + Vector3.right * 1.5f, 0.05f);
        }

        DrawLabel(basePos + Vector3.left * 1.5f + Vector3.down * 0.5f, "Lerp路径");
        DrawLabel(basePos + Vector3.right * 1.5f + Vector3.down * 0.5f, "Slerp路径");
    }

    void DrawLookAtVisualization()
    {
        if (lookAtTarget == null) return;

        Vector3 start = rotatingObject.position;
        Vector3 end = lookAtTarget.position;

        // 绘制朝向线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);

        // 绘制目标
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(end, 0.3f);

        // 绘制前方向
        Gizmos.color = Color.blue;
        Vector3 forward = rotatingObject.forward * 2f;
        DebugDrawer.DrawArrow(start, start + forward, 0.3f);

        // 计算角度差
        Vector3 direction = (end - start).normalized;
        float angle = Vector3.Angle(rotatingObject.forward, direction);

        DrawLabel(start + Vector3.up * 2f,
            $"朝向目标\n" +
            $"角度差: {angle:F1}°\n" +
            $"平滑: {smoothLookAt}\n" +
            $"速度: {lookAtSpeed:F1}");
    }

    void DrawRotateTowardsVisualization()
    {
        if (targetRotation == null) return;

        // 绘制目标旋转
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        DrawObjectAxes(targetRotation, 1.2f);
        DrawLabel(targetRotation.position + Vector3.up * 2f, "目标旋转");

        // 计算角度差
        float angle = Quaternion.Angle(
            rotatingObject.rotation,
            targetRotation.rotation
        );

        DrawLabel(rotatingObject.position + Vector3.up * 3f,
            $"旋转到目标\n" +
            $"剩余角度: {angle:F1}°\n" +
            $"角速度: {rotateToSpeed * 180f:F1}°/s");
    }

    void DrawObjectAxes(Transform obj, float length)
    {
        Vector3 pos = obj.position;

        // X轴 - 红色
        Gizmos.color = Color.red;
        DebugDrawer.DrawArrow(pos, pos + obj.right * length, 0.2f);

        // Y轴 - 绿色
        Gizmos.color = Color.green;
        DebugDrawer.DrawArrow(pos, pos + obj.up * length, 0.2f);

        // Z轴 - 蓝色
        Gizmos.color = Color.blue;
        DebugDrawer.DrawArrow(pos, pos + obj.forward * length, 0.2f);
    }

    void LogRotationInfo()
    {
        Debug.Log("=== 四元数旋转信息 ===");
        Debug.Log($"模式: {mode}");
        Debug.Log($"当前旋转: {rotatingObject.rotation}");
        Debug.Log($"欧拉角: {rotatingObject.eulerAngles}");

        switch (mode)
        {
            case RotationMode.Lerp:
            case RotationMode.Slerp:
                if (startRotation != null && endRotation != null)
                {
                    float angle = Quaternion.Angle(startRotation.rotation, endRotation.rotation);
                    Debug.Log($"起始和结束之间的角度: {angle}°");

                    // 对比Lerp和Slerp
                    Quaternion lerp = Quaternion.Lerp(startRotation.rotation, endRotation.rotation, interpolationT);
                    Quaternion slerp = Quaternion.Slerp(startRotation.rotation, endRotation.rotation, interpolationT);
                    float diff = Quaternion.Angle(lerp, slerp);
                    Debug.Log($"Lerp和Slerp的差异: {diff}° (t={interpolationT:F3})");
                }
                break;

            case RotationMode.LookAt:
                if (lookAtTarget != null)
                {
                    Vector3 direction = lookAtTarget.position - rotatingObject.position;
                    float angle = Vector3.Angle(rotatingObject.forward, direction.normalized);
                    Debug.Log($"到目标的角度: {angle}°");
                }
                break;

            case RotationMode.RotateTowards:
                if (targetRotation != null)
                {
                    float angle = Quaternion.Angle(rotatingObject.rotation, targetRotation.rotation);
                    Debug.Log($"到目标的角度: {angle}°");
                }
                break;
        }
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
        GUILayout.Box("四元数旋转示例");

        GUILayout.Label($"模式: {mode}");

        switch (mode)
        {
            case RotationMode.Lerp:
                GUILayout.Label("Lerp: 线性插值（不匀速）");
                break;
            case RotationMode.Slerp:
                GUILayout.Label("Slerp: 球面线性插值（匀速）");
                break;
            case RotationMode.LookAt:
                GUILayout.Label($"LookAt: 朝向目标");
                break;
            case RotationMode.RotateTowards:
                GUILayout.Label("RotateTowards: 固定角速度");
                break;
        }

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }
}
