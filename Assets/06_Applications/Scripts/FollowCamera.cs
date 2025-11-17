using UnityEngine;

/// <summary>
/// 跟随相机示例
/// 演示第三人称相机跟随，包括平滑跟随、视角旋转、碰撞检测等
/// </summary>
public class FollowCamera : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("相机位置设置")]
    public Vector3 offset = new Vector3(0, 2, -5);
    public float distance = 5f;
    public float height = 2f;

    [Header("跟随设置")]
    [Range(0.01f, 1f)]
    public float positionSmoothing = 0.1f;
    [Range(0.01f, 1f)]
    public float rotationSmoothing = 0.1f;

    [Header("视角控制")]
    public bool enableMouseControl = true;
    public float mouseSensitivity = 3f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    [Header("碰撞检测")]
    public bool avoidCollision = true;
    public LayerMask collisionLayers = ~0;
    public float collisionPadding = 0.3f;
    public float collisionCheckRadius = 0.5f;

    [Header("高级选项")]
    public bool lookAtTarget = true;
    public Vector3 lookAtOffset = new Vector3(0, 1.5f, 0);
    public bool useFixedUpdate = false;

    [Header("调试")]
    public bool showDebugInfo = true;

    private float currentHorizontalAngle = 0f;
    private float currentVerticalAngle = 20f;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 desiredPosition;
    private Vector3 actualPosition;

    void Start()
    {
        if (target != null)
        {
            // 初始化角度
            Vector3 angles = transform.eulerAngles;
            currentHorizontalAngle = angles.y;
            currentVerticalAngle = angles.x;

            // 确保初始位置正确
            UpdateCameraPosition();
        }
    }

    void LateUpdate()
    {
        if (!useFixedUpdate)
        {
            UpdateCamera();
        }
    }

    void FixedUpdate()
    {
        if (useFixedUpdate)
        {
            UpdateCamera();
        }
    }

    void UpdateCamera()
    {
        if (target == null) return;

        // 处理鼠标输入
        HandleMouseInput();

        // 更新相机位置
        UpdateCameraPosition();

        // 更新相机朝向
        UpdateCameraRotation();
    }

    void HandleMouseInput()
    {
        if (!enableMouseControl) return;

        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 更新水平角度
        currentHorizontalAngle += mouseX;

        // 更新垂直角度并限制
        currentVerticalAngle -= mouseY;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
    }

    void UpdateCameraPosition()
    {
        // 计算期望的相机位置
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 negDistance = new Vector3(0, 0, -distance);
        Vector3 position = rotation * negDistance + target.position + new Vector3(0, height, 0);

        desiredPosition = position;

        // 碰撞检测
        if (avoidCollision)
        {
            position = HandleCollision(target.position + new Vector3(0, height, 0), position);
        }

        actualPosition = position;

        // 平滑移动
        if (positionSmoothing > 0.01f)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                position,
                ref currentVelocity,
                positionSmoothing
            );
        }
        else
        {
            transform.position = position;
        }
    }

    Vector3 HandleCollision(Vector3 targetPos, Vector3 desiredPos)
    {
        Vector3 direction = desiredPos - targetPos;
        float targetDistance = direction.magnitude;

        RaycastHit hit;
        if (Physics.SphereCast(
            targetPos,
            collisionCheckRadius,
            direction.normalized,
            out hit,
            targetDistance,
            collisionLayers))
        {
            // 有碰撞，将相机拉近
            float safeDistance = hit.distance - collisionPadding;
            safeDistance = Mathf.Max(safeDistance, 0.5f); // 最小距离

            return targetPos + direction.normalized * safeDistance;
        }

        return desiredPos;
    }

    void UpdateCameraRotation()
    {
        if (lookAtTarget && target != null)
        {
            Vector3 lookAtPos = target.position + lookAtOffset;
            Quaternion targetRotation = Quaternion.LookRotation(lookAtPos - transform.position);

            if (rotationSmoothing > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    1f - rotationSmoothing
                );
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
        else
        {
            // 使用当前角度
            Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);

            if (rotationSmoothing > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    rotation,
                    1f - rotationSmoothing
                );
            }
            else
            {
                transform.rotation = rotation;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (target == null || !showDebugInfo) return;

        Vector3 targetPos = target.position + new Vector3(0, height, 0);

        // 绘制目标位置
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position, 0.5f);
        Gizmos.DrawWireSphere(targetPos, 0.3f);

        // 绘制期望位置
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(desiredPosition, 0.4f);
        Gizmos.DrawLine(targetPos, desiredPosition);

        // 绘制实际位置（如果与期望位置不同）
        if (Vector3.Distance(actualPosition, desiredPosition) > 0.1f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(actualPosition, 0.4f);
            Gizmos.DrawLine(desiredPosition, actualPosition);
        }

        // 绘制视线
        if (lookAtTarget)
        {
            Vector3 lookAtPos = target.position + lookAtOffset;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, lookAtPos);
            Gizmos.DrawWireSphere(lookAtPos, 0.2f);
        }

        // 绘制碰撞检测范围
        if (avoidCollision)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Vector3 direction = desiredPosition - targetPos;
            float checkDistance = direction.magnitude;

            // 绘制球形投射路径
            int steps = 10;
            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                Vector3 pos = targetPos + direction.normalized * (checkDistance * t);
                Gizmos.DrawWireSphere(pos, collisionCheckRadius);
            }
        }

        DrawCameraInfo();
    }

    void DrawCameraInfo()
    {
        Vector3 pos = transform.position + transform.up * 1f;

#if UNITY_EDITOR
        string info = $"相机跟随\n" +
                      $"距离: {distance:F1}\n" +
                      $"高度: {height:F1}\n" +
                      $"水平角: {currentHorizontalAngle:F1}°\n" +
                      $"垂直角: {currentVerticalAngle:F1}°";

        if (avoidCollision && Vector3.Distance(actualPosition, desiredPosition) > 0.1f)
        {
            info += "\n<color=red>检测到碰撞</color>";
        }

        UnityEditor.Handles.Label(pos, info);
#endif
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 250));
        GUILayout.Box("跟随相机示例");

        GUILayout.Label($"水平角度: {currentHorizontalAngle:F1}°");
        GUILayout.Label($"垂直角度: {currentVerticalAngle:F1}°");
        GUILayout.Label($"距离: {distance:F1}");
        GUILayout.Label($"高度: {height:F1}");

        if (target != null)
        {
            float actualDist = Vector3.Distance(transform.position, target.position);
            GUILayout.Label($"实际距离: {actualDist:F1}");

            if (avoidCollision)
            {
                float desiredDist = Vector3.Distance(desiredPosition, target.position);
                if (Mathf.Abs(actualDist - desiredDist) > 0.5f)
                {
                    GUI.color = Color.red;
                    GUILayout.Label("碰撞！相机已调整");
                    GUI.color = Color.white;
                }
            }
        }

        GUILayout.Label($"\n鼠标控制: {(enableMouseControl ? "启用" : "禁用")}");
        GUILayout.Label($"平滑度: {positionSmoothing:F2}");

        GUILayout.EndArea();
    }

    // 公共API供外部调用
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            UpdateCameraPosition();
        }
    }

    public void SetDistance(float newDistance)
    {
        distance = Mathf.Max(newDistance, 1f);
    }

    public void SetAngles(float horizontal, float vertical)
    {
        currentHorizontalAngle = horizontal;
        currentVerticalAngle = Mathf.Clamp(vertical, minVerticalAngle, maxVerticalAngle);
    }

    public void ResetCamera()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            currentHorizontalAngle = angles.y;
            currentVerticalAngle = angles.x;
            currentVelocity = Vector3.zero;
        }
    }
}
