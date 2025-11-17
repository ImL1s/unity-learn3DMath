using UnityEngine;

/// <summary>
/// 物体朝向目标示例
/// 演示各种朝向目标的方法，常用于炮塔、敌人AI等
/// </summary>
public class LookAtTarget : MonoBehaviour
{
    [Header("目标")]
    public Transform target;

    [Header("朝向模式")]
    public LookAtMode mode = LookAtMode.SmoothLookAt;

    [Header("平滑设置")]
    [Range(0.1f, 20f)]
    public float rotationSpeed = 5f;

    [Header("约束设置")]
    public bool constrainRotation = false;
    public RotationConstraint constraint = RotationConstraint.YAxisOnly;

    [Header("偏移")]
    public Vector3 targetOffset = Vector3.zero;
    public bool useTargetHeight = false;
    [Range(-5f, 5f)]
    public float heightOffset = 0f;

    [Header("高级选项")]
    public bool predictTarget = false;
    public float predictionTime = 0.5f;
    public bool showDebugInfo = true;

    private Vector3 lastTargetPosition;
    private Vector3 targetVelocity;

    public enum LookAtMode
    {
        Instant,            // 立即朝向
        SmoothLookAt,       // 平滑朝向
        RotateTowards,      // 固定角速度旋转
        CustomLerp          // 自定义插值
    }

    public enum RotationConstraint
    {
        None,               // 无约束
        YAxisOnly,          // 只绕Y轴旋转（适合地面单位）
        XAxisOnly,          // 只绕X轴旋转（适合上下瞄准）
        ZAxisOnly           // 只绕Z轴旋转
    }

    void Start()
    {
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }

    void Update()
    {
        if (target == null) return;

        // 更新目标速度（用于预测）
        if (predictTarget)
        {
            targetVelocity = (target.position - lastTargetPosition) / Time.deltaTime;
            lastTargetPosition = target.position;
        }

        // 计算目标位置
        Vector3 targetPosition = GetTargetPosition();

        // 根据模式执行朝向
        switch (mode)
        {
            case LookAtMode.Instant:
                InstantLookAt(targetPosition);
                break;

            case LookAtMode.SmoothLookAt:
                SmoothLookAt(targetPosition);
                break;

            case LookAtMode.RotateTowards:
                RotateTowardsTarget(targetPosition);
                break;

            case LookAtMode.CustomLerp:
                CustomLerpLookAt(targetPosition);
                break;
        }

        // 按空格键输出信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogLookAtInfo();
        }
    }

    Vector3 GetTargetPosition()
    {
        Vector3 targetPos = target.position + targetOffset;

        // 预测目标位置
        if (predictTarget && targetVelocity.magnitude > 0.1f)
        {
            targetPos += targetVelocity * predictionTime;
        }

        // 高度调整
        if (useTargetHeight)
        {
            targetPos.y = transform.position.y + heightOffset;
        }

        return targetPos;
    }

    void InstantLookAt(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude < 0.001f) return;

        Quaternion targetRotation = GetConstrainedRotation(direction);
        transform.rotation = targetRotation;
    }

    void SmoothLookAt(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude < 0.001f) return;

        Quaternion targetRotation = GetConstrainedRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude < 0.001f) return;

        Quaternion targetRotation = GetConstrainedRotation(direction);

        float maxDegreesDelta = rotationSpeed * 50f * Time.deltaTime;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            maxDegreesDelta
        );
    }

    void CustomLerpLookAt(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude < 0.001f) return;

        Quaternion targetRotation = GetConstrainedRotation(direction);

        // 使用自定义的平滑函数
        float t = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            t
        );
    }

    Quaternion GetConstrainedRotation(Vector3 direction)
    {
        if (!constrainRotation || constraint == RotationConstraint.None)
        {
            return Quaternion.LookRotation(direction);
        }

        Vector3 constrainedDirection = direction;

        switch (constraint)
        {
            case RotationConstraint.YAxisOnly:
                // 只绕Y轴旋转，保持水平
                constrainedDirection.y = 0;
                if (constrainedDirection.magnitude < 0.001f)
                    return transform.rotation;
                break;

            case RotationConstraint.XAxisOnly:
                // 只绕X轴旋转（俯仰）
                float angleX = Mathf.Atan2(direction.y,
                    new Vector2(direction.x, direction.z).magnitude) * Mathf.Rad2Deg;
                return Quaternion.Euler(angleX, transform.eulerAngles.y, 0);

            case RotationConstraint.ZAxisOnly:
                // 只绕Z轴旋转（2D旋转）
                float angleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                return Quaternion.Euler(0, 0, angleZ - 90f);
        }

        return Quaternion.LookRotation(constrainedDirection);
    }

    void OnDrawGizmos()
    {
        if (target == null || !showDebugInfo) return;

        Vector3 pos = transform.position;
        Vector3 targetPos = GetTargetPosition();

        // 绘制到目标的线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos, targetPos);

        // 绘制目标位置
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPos, 0.3f);

        // 绘制前方向
        Gizmos.color = Color.blue;
        DebugDrawer.DrawArrow(pos, pos + transform.forward * 2f, 0.3f);

        // 如果启用预测，绘制预测位置
        if (predictTarget && targetVelocity.magnitude > 0.1f)
        {
            Vector3 predictedPos = target.position + targetVelocity * predictionTime;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(predictedPos, 0.2f);
            Gizmos.DrawLine(target.position, predictedPos);

            DrawLabel(predictedPos + Vector3.up * 0.5f, "预测位置");
        }

        // 绘制旋转约束可视化
        if (constrainRotation)
        {
            DrawConstraintVisualization(pos);
        }

        // 显示角度信息
        Vector3 direction = (targetPos - pos).normalized;
        float angle = Vector3.Angle(transform.forward, direction);

        DrawLabel(pos + Vector3.up * 2f,
            $"模式: {mode}\n" +
            $"到目标角度: {angle:F1}°\n" +
            $"约束: {(constrainRotation ? constraint.ToString() : "无")}");
    }

    void DrawConstraintVisualization(Vector3 center)
    {
        float radius = 1.5f;

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);

        switch (constraint)
        {
            case RotationConstraint.YAxisOnly:
                // 绘制水平圆圈
                DebugDrawer.DrawCircle(center, Vector3.up, radius, 32);
                DrawLabel(center + Vector3.down * 2f, "仅Y轴旋转（水平）");
                break;

            case RotationConstraint.XAxisOnly:
                // 绘制垂直圆圈
                DebugDrawer.DrawCircle(center, Vector3.right, radius, 32);
                DrawLabel(center + Vector3.down * 2f, "仅X轴旋转（俯仰）");
                break;

            case RotationConstraint.ZAxisOnly:
                // 绘制Z轴圆圈
                DebugDrawer.DrawCircle(center, Vector3.forward, radius, 32);
                DrawLabel(center + Vector3.down * 2f, "仅Z轴旋转（2D）");
                break;
        }
    }

    void LogLookAtInfo()
    {
        Debug.Log("=== 朝向目标信息 ===");
        Debug.Log($"模式: {mode}");
        Debug.Log($"旋转速度: {rotationSpeed}");
        Debug.Log($"当前旋转: {transform.rotation.eulerAngles}");

        if (target != null)
        {
            Vector3 targetPos = GetTargetPosition();
            Vector3 direction = targetPos - transform.position;

            Debug.Log($"目标位置: {targetPos}");
            Debug.Log($"方向向量: {direction.normalized}");
            Debug.Log($"距离: {direction.magnitude:F2}");

            float angle = Vector3.Angle(transform.forward, direction);
            Debug.Log($"角度差: {angle:F2}°");

            if (predictTarget)
            {
                Debug.Log($"目标速度: {targetVelocity}");
                Debug.Log($"预测时间: {predictionTime}s");
            }

            if (constrainRotation)
            {
                Debug.Log($"旋转约束: {constraint}");
            }
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
        GUILayout.Box("物体朝向目标示例");

        GUILayout.Label($"模式: {mode}");

        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);
            GUILayout.Label($"到目标角度: {angle:F1}°");
            GUILayout.Label($"距离: {direction.magnitude:F2}");
        }

        if (constrainRotation)
        {
            GUILayout.Label($"约束: {constraint}");
        }

        if (predictTarget)
        {
            GUILayout.Label($"预测: {predictionTime:F1}s");
        }

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }
}
