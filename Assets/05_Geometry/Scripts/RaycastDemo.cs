using UnityEngine;

/// <summary>
/// 射线检测示例
/// 演示各种射线检测方法和实际应用
/// </summary>
public class RaycastDemo : MonoBehaviour
{
    [Header("射线设置")]
    public Transform rayOrigin;
    public Vector3 rayDirection = Vector3.forward;
    public float rayLength = 10f;

    [Header("检测模式")]
    public RaycastMode mode = RaycastMode.Simple;

    [Header("射线投射设置")]
    public float sphereRadius = 0.5f;
    public float capsuleRadius = 0.5f;
    public float capsuleHeight = 2f;
    public Vector3 boxSize = new Vector3(1, 1, 1);

    [Header("检测设置")]
    public LayerMask hitLayers = ~0;
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal;
    public int maxHits = 10;

    [Header("鼠标交互")]
    public bool useMouseRay = false;
    public Camera raycastCamera;

    [Header("视线检测目标")]
    public Transform lineOfSightTarget;

    [Header("显示选项")]
    public bool showRay = true;
    public bool showHitInfo = true;
    public bool showAllHits = false;
    public Color rayColor = Color.yellow;
    public Color hitColor = Color.red;

    private RaycastHit lastHit;
    private RaycastHit[] allHits;
    private bool hasHit = false;

    public enum RaycastMode
    {
        Simple,         // 简单射线
        SphereCast,     // 球形投射
        BoxCast,        // 盒形投射
        CapsuleCast,    // 胶囊投射
        RaycastAll,     // 检测所有
        MouseRay,       // 鼠标射线
        LineOfSight     // 视线检测
    }

    void Start()
    {
        if (raycastCamera == null)
            raycastCamera = Camera.main;

        // 自动创建rayOrigin（如果为空）
        if (rayOrigin == null)
        {
            GameObject originObj = new GameObject("RayOrigin");
            rayOrigin = originObj.transform;
            rayOrigin.position = transform.position;
            rayOrigin.SetParent(transform);

            Debug.Log("已自动创建RayOrigin对象");
        }

        // 自动创建视线检测目标（如果为空）
        if (lineOfSightTarget == null)
        {
            GameObject targetObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            targetObj.name = "LineOfSightTarget";
            lineOfSightTarget = targetObj.transform;
            lineOfSightTarget.position = transform.position + new Vector3(0, 0, 5);
            lineOfSightTarget.localScale = Vector3.one * 0.5f;

            // 添加颜色
            Renderer renderer = lineOfSightTarget.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.yellow;
                renderer.material = mat;
            }

            Debug.Log("已自动创建LineOfSightTarget对象");
        }
    }

    void Update()
    {
        PerformRaycast();

        // 按空格键输出信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogRaycastInfo();
        }
    }

    void PerformRaycast()
    {
        if (rayOrigin == null && !useMouseRay) return;

        Ray ray = GetRay();
        hasHit = false;

        switch (mode)
        {
            case RaycastMode.Simple:
                hasHit = Physics.Raycast(ray, out lastHit, rayLength, hitLayers, triggerInteraction);
                break;

            case RaycastMode.SphereCast:
                hasHit = Physics.SphereCast(ray, sphereRadius, out lastHit, rayLength, hitLayers, triggerInteraction);
                break;

            case RaycastMode.BoxCast:
                hasHit = Physics.BoxCast(ray.origin, boxSize / 2f, ray.direction, out lastHit,
                    Quaternion.identity, rayLength, hitLayers, triggerInteraction);
                break;

            case RaycastMode.CapsuleCast:
                Vector3 point1 = ray.origin;
                Vector3 point2 = ray.origin + Vector3.up * capsuleHeight;
                hasHit = Physics.CapsuleCast(point1, point2, capsuleRadius, ray.direction,
                    out lastHit, rayLength, hitLayers, triggerInteraction);
                break;

            case RaycastMode.RaycastAll:
                allHits = Physics.RaycastAll(ray, rayLength, hitLayers, triggerInteraction);
                hasHit = allHits.Length > 0;
                if (hasHit)
                    lastHit = allHits[0];
                break;

            case RaycastMode.MouseRay:
                if (Input.GetMouseButton(0))
                {
                    Ray mouseRay = GetMouseRay();
                    hasHit = Physics.Raycast(mouseRay, out lastHit, Mathf.Infinity, hitLayers, triggerInteraction);
                }
                break;

            case RaycastMode.LineOfSight:
                // 视线检测：检查从rayOrigin到目标的路径是否被遮挡
                if (lineOfSightTarget != null)
                {
                    Vector3 directionToTarget = lineOfSightTarget.position - ray.origin;
                    float distanceToTarget = directionToTarget.magnitude;

                    // 检测到目标的射线是否被遮挡
                    hasHit = Physics.Raycast(ray.origin, directionToTarget.normalized, out lastHit,
                        distanceToTarget, hitLayers, triggerInteraction);

                    // hasHit=true表示视线被遮挡，击中了障碍物
                    // hasHit=false表示视线畅通，可以看到目标
                }
                else
                {
                    // 如果没有目标，使用默认方向
                    hasHit = Physics.Raycast(ray, out lastHit, rayLength, hitLayers, triggerInteraction);
                }
                break;
        }
    }

    Ray GetRay()
    {
        if (useMouseRay || mode == RaycastMode.MouseRay)
        {
            return GetMouseRay();
        }

        if (rayOrigin == null)
            return new Ray(Vector3.zero, Vector3.forward);

        return new Ray(rayOrigin.position, rayOrigin.TransformDirection(rayDirection.normalized));
    }

    Ray GetMouseRay()
    {
        if (raycastCamera == null) return new Ray();
        return raycastCamera.ScreenPointToRay(Input.mousePosition);
    }

    void OnDrawGizmos()
    {
        if (!showRay && !hasHit) return;

        Ray ray = GetRay();

        // 绘制射线
        if (showRay)
        {
            DrawRayVisualization(ray);
        }

        // 绘制击中信息
        if (hasHit && showHitInfo)
        {
            DrawHitInfo(lastHit);
        }

        // 显示所有击中点
        if (mode == RaycastMode.RaycastAll && showAllHits && allHits != null)
        {
            for (int i = 0; i < allHits.Length; i++)
            {
                DrawHitInfo(allHits[i], i);
            }
        }
    }

    void DrawRayVisualization(Ray ray)
    {
        Vector3 rayEnd = ray.origin + ray.direction * rayLength;

        // 绘制射线
        Gizmos.color = hasHit ? hitColor : rayColor;

        switch (mode)
        {
            case RaycastMode.Simple:
            case RaycastMode.RaycastAll:
            case RaycastMode.MouseRay:
            case RaycastMode.LineOfSight:
                // 简单射线和视线检测使用箭头
                if (mode == RaycastMode.LineOfSight && lineOfSightTarget != null)
                {
                    // 视线检测：绘制到目标的射线
                    Vector3 targetPos = lineOfSightTarget.position;
                    Gizmos.color = hasHit ? Color.red : Color.green;
                    DebugDrawer.DrawArrow(ray.origin, targetPos, 0.3f);

                    // 绘制目标
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(targetPos, 0.3f);

                    // 显示状态
                    DrawLabel(targetPos + Vector3.up * 0.8f,
                        hasHit ? "视线被遮挡" : "视线畅通");
                }
                else
                {
                    DebugDrawer.DrawArrow(ray.origin, rayEnd, 0.3f);
                }
                break;

            case RaycastMode.SphereCast:
                // 绘制球形投射路径
                Gizmos.color = new Color(rayColor.r, rayColor.g, rayColor.b, 0.3f);
                int segments = 10;
                for (int i = 0; i <= segments; i++)
                {
                    float t = (float)i / segments;
                    Vector3 pos = ray.origin + ray.direction * (rayLength * t);
                    Gizmos.DrawWireSphere(pos, sphereRadius);
                }
                Gizmos.color = rayColor;
                DebugDrawer.DrawArrow(ray.origin, rayEnd, 0.2f);
                break;

            case RaycastMode.BoxCast:
                // 绘制盒形投射
                DrawBoxCastVisualization(ray);
                break;

            case RaycastMode.CapsuleCast:
                // 绘制胶囊投射
                DrawCapsuleCastVisualization(ray);
                break;
        }

        // 绘制原点
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ray.origin, 0.1f);
    }

    void DrawBoxCastVisualization(Ray ray)
    {
        Gizmos.color = new Color(rayColor.r, rayColor.g, rayColor.b, 0.3f);

        Vector3 halfExtents = boxSize / 2f;
        int segments = 5;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 center = ray.origin + ray.direction * (rayLength * t);
            Gizmos.DrawWireCube(center, boxSize);
        }

        Gizmos.color = rayColor;
        DebugDrawer.DrawArrow(ray.origin, ray.origin + ray.direction * rayLength, 0.2f);
    }

    void DrawCapsuleCastVisualization(Ray ray)
    {
        Gizmos.color = new Color(rayColor.r, rayColor.g, rayColor.b, 0.3f);

        int segments = 5;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 offset = ray.direction * (rayLength * t);
            Vector3 point1 = ray.origin + offset;
            Vector3 point2 = point1 + Vector3.up * capsuleHeight;

            Gizmos.DrawWireSphere(point1, capsuleRadius);
            Gizmos.DrawWireSphere(point2, capsuleRadius);
            Gizmos.DrawLine(point1, point2);
        }

        Gizmos.color = rayColor;
        DebugDrawer.DrawArrow(ray.origin, ray.origin + ray.direction * rayLength, 0.2f);
    }

    void DrawHitInfo(RaycastHit hit, int index = -1)
    {
        // 绘制击中点
        Gizmos.color = hitColor;
        Gizmos.DrawWireSphere(hit.point, 0.2f);

        // 绘制法线
        Gizmos.color = Color.cyan;
        DebugDrawer.DrawArrow(hit.point, hit.point + hit.normal, 0.2f);

        // 绘制到击中点的距离
        Ray ray = GetRay();
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawLine(ray.origin, hit.point);

        // 显示信息
        string label = index >= 0 ? $"击中 #{index + 1}\n" : "击中点\n";
        label += $"对象: {hit.collider.name}\n";
        label += $"距离: {hit.distance:F2}\n";
        label += $"点: {hit.point.ToString("F2")}\n";
        label += $"法线: {hit.normal.ToString("F2")}";

        DrawLabel(hit.point + Vector3.up * 0.5f, label);

        // 绘制击中对象的边界框
        if (hit.collider != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawWireCube(hit.collider.bounds.center, hit.collider.bounds.size);
        }
    }

    void LogRaycastInfo()
    {
        Debug.Log("=== 射线检测信息 ===");
        Debug.Log($"模式: {mode}");

        Ray ray = GetRay();
        Debug.Log($"射线原点: {ray.origin}");
        Debug.Log($"射线方向: {ray.direction}");
        Debug.Log($"射线长度: {rayLength}");

        if (hasHit)
        {
            Debug.Log($"\n击中对象: {lastHit.collider.name}");
            Debug.Log($"击中点: {lastHit.point}");
            Debug.Log($"击中法线: {lastHit.normal}");
            Debug.Log($"击中距离: {lastHit.distance}");
            Debug.Log($"三角形索引: {lastHit.triangleIndex}");
            Debug.Log($"UV坐标: {lastHit.textureCoord}");

            if (lastHit.rigidbody != null)
            {
                Debug.Log($"刚体: {lastHit.rigidbody.name}");
            }
        }
        else
        {
            Debug.Log("\n未击中任何对象");
        }

        if (mode == RaycastMode.RaycastAll && allHits != null)
        {
            Debug.Log($"\n总共击中 {allHits.Length} 个对象:");
            for (int i = 0; i < allHits.Length; i++)
            {
                Debug.Log($"  {i + 1}. {allHits[i].collider.name} (距离: {allHits[i].distance:F2})");
            }
        }

        // 性能提示
        Debug.Log($"\n性能提示:");
        Debug.Log($"- 使用LayerMask过滤可以提高性能");
        Debug.Log($"- SphereCast比Raycast开销更大");
        Debug.Log($"- RaycastAll会返回所有击中，注意性能");
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
        GUILayout.Box("射线检测示例");

        GUILayout.Label($"模式: {mode}");

        if (hasHit)
        {
            GUI.color = Color.green;
            GUILayout.Label($"✓ 击中: {lastHit.collider.name}");
            GUILayout.Label($"距离: {lastHit.distance:F2}");
            GUI.color = Color.white;

            if (mode == RaycastMode.RaycastAll && allHits != null)
            {
                GUILayout.Label($"总击中数: {allHits.Length}");
            }
        }
        else
        {
            GUI.color = Color.gray;
            GUILayout.Label("✗ 未击中");
            GUI.color = Color.white;
        }

        GUILayout.Label($"\n射线长度: {rayLength:F1}");

        switch (mode)
        {
            case RaycastMode.SphereCast:
                GUILayout.Label($"球体半径: {sphereRadius:F2}");
                break;
            case RaycastMode.BoxCast:
                GUILayout.Label($"盒体大小: {boxSize}");
                break;
            case RaycastMode.CapsuleCast:
                GUILayout.Label($"胶囊半径: {capsuleRadius:F2}");
                GUILayout.Label($"胶囊高度: {capsuleHeight:F2}");
                break;
        }

        if (useMouseRay || mode == RaycastMode.MouseRay)
        {
            GUILayout.Label("\n点击鼠标进行射线检测");
        }

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }
}
