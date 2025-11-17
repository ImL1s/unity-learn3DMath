using UnityEngine;

/// <summary>
/// 抛物线运动示例
/// 演示抛射体物理、轨迹计算、落点预测等
/// </summary>
public class ProjectileMotion : MonoBehaviour
{
    [Header("抛射设置")]
    public Transform launchPoint;
    public Transform targetPoint;
    public GameObject projectilePrefab;

    [Header("物理参数")]
    public float launchSpeed = 20f;
    public float gravity = 9.8f;
    public float launchAngle = 45f;

    [Header("发射模式")]
    public LaunchMode mode = LaunchMode.FixedAngle;

    [Header("轨迹设置")]
    public bool showTrajectory = true;
    public int trajectoryResolution = 50;
    public float maxTrajectoryTime = 5f;

    [Header("交互")]
    public bool autoLaunch = false;
    public float autoLaunchInterval = 2f;
    public KeyCode launchKey = KeyCode.Space;

    [Header("显示选项")]
    public bool showLandingPoint = true;
    public bool showVelocityVectors = true;
    public bool showApex = true;
    public bool showDebugInfo = true;

    private float lastLaunchTime = 0f;
    private Vector3 calculatedLandingPoint;
    private float calculatedFlightTime;
    private float calculatedApexHeight;
    private Vector3 launchVelocity;

    public enum LaunchMode
    {
        FixedAngle,         // 固定角度发射
        ToTarget,           // 瞄准目标（低弧线）
        ToTargetHighArc,    // 瞄准目标（高弧线）
        MaxRange            // 最大射程（45度）
    }

    void Start()
    {
        if (launchPoint == null)
            launchPoint = transform;

        CalculateTrajectory();
    }

    void Update()
    {
        // 重新计算轨迹
        CalculateTrajectory();

        // 自动发射
        if (autoLaunch && Time.time - lastLaunchTime > autoLaunchInterval)
        {
            LaunchProjectile();
        }

        // 手动发射
        if (Input.GetKeyDown(launchKey))
        {
            LaunchProjectile();
        }

        // 调整角度
        if (mode == LaunchMode.FixedAngle)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                launchAngle = Mathf.Min(launchAngle + 30f * Time.deltaTime, 89f);
            if (Input.GetKey(KeyCode.DownArrow))
                launchAngle = Mathf.Max(launchAngle - 30f * Time.deltaTime, 1f);
        }

        // 调整速度
        if (Input.GetKey(KeyCode.RightArrow))
            launchSpeed += 5f * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
            launchSpeed = Mathf.Max(launchSpeed - 5f * Time.deltaTime, 1f);
    }

    void CalculateTrajectory()
    {
        if (launchPoint == null) return;

        Vector3 startPos = launchPoint.position;

        // 根据模式计算发射速度
        switch (mode)
        {
            case LaunchMode.FixedAngle:
                launchVelocity = CalculateVelocityFromAngle(launchAngle);
                break;

            case LaunchMode.ToTarget:
                if (targetPoint != null)
                    launchVelocity = CalculateVelocityToTarget(targetPoint.position, false);
                break;

            case LaunchMode.ToTargetHighArc:
                if (targetPoint != null)
                    launchVelocity = CalculateVelocityToTarget(targetPoint.position, true);
                break;

            case LaunchMode.MaxRange:
                launchVelocity = CalculateVelocityFromAngle(45f);
                break;
        }

        // 计算落点和飞行时间
        calculatedFlightTime = CalculateFlightTime(launchVelocity);
        calculatedLandingPoint = CalculateLandingPoint(launchVelocity, calculatedFlightTime);
        calculatedApexHeight = CalculateApexHeight(launchVelocity);
    }

    Vector3 CalculateVelocityFromAngle(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        Vector3 direction = launchPoint.forward;

        // 在垂直平面上旋转
        Vector3 velocity = direction * Mathf.Cos(angleRad) * launchSpeed;
        velocity.y = Mathf.Sin(angleRad) * launchSpeed;

        return velocity;
    }

    Vector3 CalculateVelocityToTarget(Vector3 target, bool highArc)
    {
        Vector3 startPos = launchPoint.position;
        Vector3 toTarget = target - startPos;

        // 水平和垂直距离
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float horizontalDist = toTargetXZ.magnitude;
        float verticalDist = toTarget.y;

        // 计算所需角度
        // 公式: tan(2θ) = (v² ± √(v⁴ - g(gx² + 2yv²))) / (gx)
        float v2 = launchSpeed * launchSpeed;
        float v4 = v2 * v2;
        float gx = gravity * horizontalDist;
        float gx2 = gravity * horizontalDist * horizontalDist;

        float underRoot = v4 - gravity * (gx2 + 2 * verticalDist * v2);

        if (underRoot < 0)
        {
            // 目标超出射程，使用45度
            return CalculateVelocityFromAngle(45f);
        }

        float sqrtVal = Mathf.Sqrt(underRoot);
        float tan2Theta = highArc ? (v2 + sqrtVal) / gx : (v2 - sqrtVal) / gx;

        float theta = Mathf.Atan(tan2Theta) * 0.5f;

        // 构建发射方向
        Vector3 direction = toTargetXZ.normalized;
        Vector3 velocity = direction * Mathf.Cos(theta) * launchSpeed;
        velocity.y = Mathf.Sin(theta) * launchSpeed;

        return velocity;
    }

    float CalculateFlightTime(Vector3 velocity)
    {
        // t = 2v_y / g (假设落地高度相同)
        // 更精确的公式考虑高度差
        float a = -0.5f * gravity;
        float b = velocity.y;
        float c = launchPoint.position.y;

        // 使用二次方程求解: at² + bt + c = 0
        float discriminant = b * b - 4 * a * (-c);

        if (discriminant < 0)
            return 0f;

        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);

        return Mathf.Max(t1, t2);
    }

    Vector3 CalculateLandingPoint(Vector3 velocity, float time)
    {
        Vector3 startPos = launchPoint.position;

        // 位置方程: p = p0 + v*t - 0.5*g*t²
        Vector3 position = startPos;
        position.x += velocity.x * time;
        position.z += velocity.z * time;
        position.y += velocity.y * time - 0.5f * gravity * time * time;

        return position;
    }

    float CalculateApexHeight(Vector3 velocity)
    {
        // 最高点: h = h0 + v_y² / (2g)
        float startHeight = launchPoint.position.y;
        float maxHeight = startHeight + (velocity.y * velocity.y) / (2f * gravity);
        return maxHeight;
    }

    Vector3 GetPositionAtTime(float t)
    {
        Vector3 startPos = launchPoint.position;

        Vector3 position = startPos;
        position += new Vector3(launchVelocity.x, 0, launchVelocity.z) * t;
        position.y += launchVelocity.y * t - 0.5f * gravity * t * t;

        return position;
    }

    Vector3 GetVelocityAtTime(float t)
    {
        Vector3 velocity = launchVelocity;
        velocity.y -= gravity * t;
        return velocity;
    }

    void LaunchProjectile()
    {
        if (launchPoint == null)
        {
            Debug.LogWarning("未设置发射点！");
            return;
        }

        GameObject projectile;

        // 如果没有预制体，自动创建一个简单的球体
        if (projectilePrefab == null)
        {
            projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "Projectile";
            projectile.transform.position = launchPoint.position;
            projectile.transform.localScale = Vector3.one * 0.3f;

            // 添加Rigidbody
            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.velocity = launchVelocity;

            // 设置自定义重力
            if (Mathf.Abs(gravity - 9.8f) > 0.1f)
            {
                rb.useGravity = false;
                projectile.AddComponent<CustomGravity>().gravityStrength = gravity;
            }

            // 添加颜色
            Renderer renderer = projectile.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.yellow;
                renderer.material = mat;
            }
        }
        else
        {
            // 使用预制体
            projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.velocity = launchVelocity;

                // 设置自定义重力
                if (Mathf.Abs(gravity - 9.8f) > 0.1f)
                {
                    rb.useGravity = false;
                    projectile.AddComponent<CustomGravity>().gravityStrength = gravity;
                }
            }
        }

        // 自动销毁
        Destroy(projectile, calculatedFlightTime + 1f);

        lastLaunchTime = Time.time;

        Debug.Log($"发射！速度: {launchVelocity}, 预计飞行时间: {calculatedFlightTime:F2}s");
    }

    void OnDrawGizmos()
    {
        if (launchPoint == null) return;

        Vector3 startPos = launchPoint.position;

        // 绘制发射点
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPos, 0.3f);

        // 绘制目标点
        if (targetPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPoint.position, 0.4f);
            Gizmos.DrawLine(startPos, targetPoint.position);
        }

        // 绘制轨迹
        if (showTrajectory)
        {
            DrawTrajectory();
        }

        // 绘制落点
        if (showLandingPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(calculatedLandingPoint, 0.4f);
            Gizmos.DrawLine(new Vector3(calculatedLandingPoint.x, 0, calculatedLandingPoint.z),
                           calculatedLandingPoint);

            DrawLabel(calculatedLandingPoint + Vector3.up * 0.5f,
                $"预计落点\n飞行时间: {calculatedFlightTime:F2}s");
        }

        // 绘制速度向量
        if (showVelocityVectors)
        {
            DrawVelocityVectors();
        }

        // 绘制最高点
        if (showApex)
        {
            float apexTime = launchVelocity.y / gravity;
            Vector3 apexPos = GetPositionAtTime(apexTime);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(apexPos, 0.3f);
            DrawLabel(apexPos + Vector3.up * 0.5f,
                $"最高点\n高度: {calculatedApexHeight:F2}m\n时间: {apexTime:F2}s");
        }

        // 绘制发射方向
        Gizmos.color = Color.blue;
        DebugDrawer.DrawArrow(startPos, startPos + launchVelocity.normalized * 2f, 0.3f);

        // 显示信息
        if (showDebugInfo)
        {
            DrawDebugInfo();
        }
    }

    void DrawTrajectory()
    {
        if (calculatedFlightTime <= 0) return;

        Vector3 previousPos = launchPoint.position;

        float timeStep = Mathf.Min(calculatedFlightTime, maxTrajectoryTime) / trajectoryResolution;

        for (int i = 1; i <= trajectoryResolution; i++)
        {
            float t = i * timeStep;
            Vector3 currentPos = GetPositionAtTime(t);

            // 渐变颜色
            float colorT = (float)i / trajectoryResolution;
            Gizmos.color = Color.Lerp(Color.green, Color.red, colorT);

            Gizmos.DrawLine(previousPos, currentPos);

            // 每隔几个点绘制一个小球
            if (i % (trajectoryResolution / 10) == 0)
            {
                Gizmos.DrawWireSphere(currentPos, 0.1f);
            }

            previousPos = currentPos;

            // 如果到达地面，停止绘制
            if (currentPos.y < 0)
                break;
        }
    }

    void DrawVelocityVectors()
    {
        int vectorCount = 5;
        float timeStep = calculatedFlightTime / vectorCount;

        for (int i = 0; i <= vectorCount; i++)
        {
            float t = i * timeStep;
            Vector3 pos = GetPositionAtTime(t);
            Vector3 vel = GetVelocityAtTime(t);

            Gizmos.color = Color.magenta;
            DebugDrawer.DrawArrow(pos, pos + vel.normalized * 0.8f, 0.15f);

            // 显示速度大小
            DrawLabel(pos + vel.normalized * 1f,
                $"{vel.magnitude:F1} m/s");
        }
    }

    void DrawDebugInfo()
    {
        Vector3 infoPos = launchPoint.position + Vector3.right * 3f;

        string info = $"抛物线运动\n\n";
        info += $"模式: {mode}\n";
        info += $"发射速度: {launchSpeed:F1} m/s\n";

        if (mode == LaunchMode.FixedAngle)
        {
            info += $"发射角度: {launchAngle:F1}°\n";
        }
        else
        {
            float angle = Mathf.Atan2(launchVelocity.y,
                new Vector2(launchVelocity.x, launchVelocity.z).magnitude) * Mathf.Rad2Deg;
            info += $"计算角度: {angle:F1}°\n";
        }

        info += $"重力: {gravity:F1} m/s²\n";
        info += $"\n飞行时间: {calculatedFlightTime:F2}s\n";
        info += $"最高点: {calculatedApexHeight:F2}m\n";

        if (targetPoint != null && mode != LaunchMode.FixedAngle)
        {
            float error = Vector3.Distance(calculatedLandingPoint, targetPoint.position);
            info += $"误差: {error:F2}m";
        }

        DrawLabel(infoPos, info);
    }

    void DrawLabel(Vector3 position, string text)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, text);
#endif
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 350, 300));
        GUILayout.Box("抛物线运动示例");

        GUILayout.Label($"模式: {mode}");
        GUILayout.Label($"发射速度: {launchSpeed:F1} m/s");

        if (mode == LaunchMode.FixedAngle)
        {
            GUILayout.Label($"发射角度: {launchAngle:F1}°");
            GUILayout.Label("\n上/下方向键: 调整角度");
        }
        else if (targetPoint != null)
        {
            float angle = Mathf.Atan2(launchVelocity.y,
                new Vector2(launchVelocity.x, launchVelocity.z).magnitude) * Mathf.Rad2Deg;
            GUILayout.Label($"计算角度: {angle:F1}°");

            float distance = Vector3.Distance(launchPoint.position, targetPoint.position);
            GUILayout.Label($"目标距离: {distance:F1}m");
        }

        GUILayout.Label($"\n飞行时间: {calculatedFlightTime:F2}s");
        GUILayout.Label($"最高点: {calculatedApexHeight:F2}m");

        GUILayout.Label("\n左/右方向键: 调整速度");
        GUILayout.Label($"{launchKey}键: 发射");

        if (projectilePrefab == null)
        {
            GUI.color = Color.red;
            GUILayout.Label("\n警告: 未设置发射物预制体！");
            GUI.color = Color.white;
        }

        GUILayout.EndArea();
    }

    // 辅助方法：获取最大射程
    public float GetMaxRange()
    {
        // 最大射程公式: R = v² / g (45度角)
        return (launchSpeed * launchSpeed) / gravity;
    }

    // 辅助方法：检查目标是否在射程内
    public bool IsTargetInRange(Vector3 target)
    {
        Vector3 toTarget = target - launchPoint.position;
        float horizontalDist = new Vector3(toTarget.x, 0, toTarget.z).magnitude;

        float maxRange = GetMaxRange();
        return horizontalDist <= maxRange;
    }
}

/// <summary>
/// 自定义重力组件
/// 用于模拟不同重力强度
/// </summary>
public class CustomGravity : MonoBehaviour
{
    public float gravityStrength = 9.8f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.AddForce(Vector3.down * gravityStrength, ForceMode.Acceleration);
        }
    }
}
