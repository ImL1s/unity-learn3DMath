using UnityEngine;

/// <summary>
/// 坐标系统转换示例
/// 演示世界坐标、本地坐标、屏幕坐标之间的转换
/// </summary>
public class CoordinateTransform : MonoBehaviour
{
    [Header("参考对象")]
    public Transform parentObject;
    public Transform childObject;

    [Header("测试点")]
    public Vector3 worldPoint = new Vector3(3, 1, 2);
    public Vector3 localPoint = new Vector3(1, 0, 1);

    [Header("显示选项")]
    public bool showWorldCoordinates = true;
    public bool showLocalCoordinates = true;
    public bool showTransformations = true;
    public bool showParentAxes = true;
    public bool showChildAxes = true;

    [Header("交互")]
    public bool useMousePosition = false;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 使用鼠标位置进行坐标转换演示
        if (useMousePosition && Input.GetMouseButton(0))
        {
            UpdateMouseWorldPosition();
        }

        // 按空格键输出坐标信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogCoordinateInfo();
        }
    }

    void UpdateMouseWorldPosition()
    {
        if (mainCamera == null) return;

        // 屏幕坐标转世界坐标
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // 距离相机的距离

        worldPoint = mainCamera.ScreenToWorldPoint(mousePos);
    }

    void OnDrawGizmos()
    {
        // 绘制世界坐标系
        DebugDrawer.DrawCoordinateSystem(Vector3.zero, 2f, 0.5f);
        DrawLabel(Vector3.up * 2.5f, "世界坐标系");

        if (parentObject != null)
        {
            // 绘制父对象坐标系
            if (showParentAxes)
            {
                DrawObjectAxes(parentObject, 1.5f, "父对象");
            }

            // 显示世界坐标点
            if (showWorldCoordinates)
            {
                DrawWorldPoint();
            }

            // 显示本地坐标点
            if (showLocalCoordinates)
            {
                DrawLocalPoint();
            }

            // 显示坐标转换
            if (showTransformations)
            {
                DrawTransformations();
            }
        }

        if (childObject != null && showChildAxes)
        {
            DrawObjectAxes(childObject, 1.2f, "子对象");
        }
    }

    void DrawWorldPoint()
    {
        // 世界坐标点
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldPoint, 0.2f);
        DrawLabel(worldPoint + Vector3.up * 0.5f,
            $"世界坐标:\n({worldPoint.x:F2}, {worldPoint.y:F2}, {worldPoint.z:F2})");

        // 如果有父对象，显示该世界点在父对象本地坐标系中的位置
        if (parentObject != null)
        {
            Vector3 localPos = parentObject.InverseTransformPoint(worldPoint);
            DrawLabel(worldPoint + Vector3.down * 0.5f,
                $"在父对象本地坐标:\n({localPos.x:F2}, {localPos.y:F2}, {localPos.z:F2})");

            // 绘制从父对象原点到该点的线
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawLine(parentObject.position, worldPoint);
        }
    }

    void DrawLocalPoint()
    {
        if (parentObject == null) return;

        // 将本地坐标转换为世界坐标
        Vector3 worldPos = parentObject.TransformPoint(localPoint);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(worldPos, 0.2f);
        DrawLabel(worldPos + Vector3.up * 0.5f,
            $"本地坐标:\n({localPoint.x:F2}, {localPoint.y:F2}, {localPoint.z:F2})");

        DrawLabel(worldPos + Vector3.down * 0.5f,
            $"对应世界坐标:\n({worldPos.x:F2}, {worldPos.y:F2}, {worldPos.z:F2})");

        // 在父对象本地空间中绘制坐标轴到该点的线
        Vector3 parentPos = parentObject.position;

        // 本地X方向
        Vector3 localX = parentObject.TransformPoint(new Vector3(localPoint.x, 0, 0));
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawLine(parentPos, localX);

        // 本地Y方向
        Vector3 localY = parentObject.TransformPoint(new Vector3(localPoint.x, localPoint.y, 0));
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawLine(localX, localY);

        // 本地Z方向
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawLine(localY, worldPos);
    }

    void DrawTransformations()
    {
        if (parentObject == null) return;

        Vector3 offset = Vector3.right * 5f;

        // 演示TransformPoint vs TransformDirection
        Vector3 testVector = new Vector3(1, 0, 0);

        // TransformPoint: 受平移、旋转、缩放影响
        Vector3 transformedPoint = parentObject.TransformPoint(testVector);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transformedPoint, 0.15f);
        DebugDrawer.DrawArrow(parentObject.position, transformedPoint, 0.2f);
        DrawLabel(transformedPoint + Vector3.up * 0.3f, "TransformPoint\n(受平移影响)");

        // TransformDirection: 只受旋转和缩放影响
        Vector3 transformedDir = parentObject.TransformDirection(testVector);
        Vector3 dirEnd = parentObject.position + transformedDir;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(dirEnd, 0.15f);
        DebugDrawer.DrawArrow(parentObject.position, dirEnd, 0.2f);
        DrawLabel(dirEnd + Vector3.down * 0.3f, "TransformDirection\n(不受平移影响)");

        // TransformVector: 受旋转和缩放影响（类似TransformDirection）
        Vector3 transformedVec = parentObject.TransformVector(testVector);
        Vector3 vecEnd = parentObject.position + transformedVec;

        DrawLabel(parentObject.position + Vector3.up * 2f,
            $"测试向量: {testVector}\n" +
            $"TransformPoint: {transformedPoint.ToString("F2")}\n" +
            $"TransformDirection: {transformedDir.ToString("F2")}\n" +
            $"TransformVector: {transformedVec.ToString("F2")}");
    }

    void DrawObjectAxes(Transform obj, float length, string label)
    {
        Vector3 pos = obj.position;

        // X轴 - 红色
        Gizmos.color = Color.red;
        DebugDrawer.DrawArrow(pos, pos + obj.right * length, 0.2f);
        DrawLabel(pos + obj.right * (length + 0.3f), $"X");

        // Y轴 - 绿色
        Gizmos.color = Color.green;
        DebugDrawer.DrawArrow(pos, pos + obj.up * length, 0.2f);
        DrawLabel(pos + obj.up * (length + 0.3f), $"Y");

        // Z轴 - 蓝色
        Gizmos.color = Color.blue;
        DebugDrawer.DrawArrow(pos, pos + obj.forward * length, 0.2f);
        DrawLabel(pos + obj.forward * (length + 0.3f), $"Z");

        DrawLabel(pos + Vector3.up * (length + 0.8f), label);
    }

    void LogCoordinateInfo()
    {
        Debug.Log("=== 坐标系统转换信息 ===");

        if (parentObject != null)
        {
            Debug.Log($"\n父对象:");
            Debug.Log($"世界位置: {parentObject.position}");
            Debug.Log($"世界旋转: {parentObject.rotation.eulerAngles}");
            Debug.Log($"世界缩放: {parentObject.lossyScale}");
            Debug.Log($"本地位置: {parentObject.localPosition}");
            Debug.Log($"本地旋转: {parentObject.localRotation.eulerAngles}");
            Debug.Log($"本地缩放: {parentObject.localScale}");

            // 测试坐标转换
            Debug.Log($"\n坐标转换测试:");
            Debug.Log($"世界点 {worldPoint} 转本地: {parentObject.InverseTransformPoint(worldPoint)}");
            Debug.Log($"本地点 {localPoint} 转世界: {parentObject.TransformPoint(localPoint)}");

            // TransformPoint vs TransformDirection
            Vector3 testVec = Vector3.right;
            Debug.Log($"\n变换对比 (输入: {testVec}):");
            Debug.Log($"TransformPoint: {parentObject.TransformPoint(testVec)}");
            Debug.Log($"TransformDirection: {parentObject.TransformDirection(testVec)}");
            Debug.Log($"TransformVector: {parentObject.TransformVector(testVec)}");
        }

        if (childObject != null && childObject.parent != null)
        {
            Debug.Log($"\n子对象:");
            Debug.Log($"世界位置: {childObject.position}");
            Debug.Log($"本地位置(相对父): {childObject.localPosition}");

            // 相对于根的变换
            Transform root = childObject.root;
            if (root != childObject)
            {
                Vector3 relativePos = root.InverseTransformPoint(childObject.position);
                Debug.Log($"相对于根的本地位置: {relativePos}");
            }
        }

        // 屏幕坐标转换
        if (mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPoint);
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(worldPoint);

            Debug.Log($"\n屏幕坐标转换:");
            Debug.Log($"世界坐标: {worldPoint}");
            Debug.Log($"屏幕坐标: {screenPos}");
            Debug.Log($"视口坐标: {viewportPos} (0-1范围)");

            // 反向转换
            Vector3 backToWorld = mainCamera.ScreenToWorldPoint(screenPos);
            Debug.Log($"屏幕坐标转回世界: {backToWorld}");
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
        GUILayout.BeginArea(new Rect(10, 10, 350, 250));
        GUILayout.Box("坐标系统转换示例");

        if (parentObject != null)
        {
            GUILayout.Label($"父对象位置: {parentObject.position.ToString("F2")}");
            GUILayout.Label($"父对象旋转: {parentObject.eulerAngles.ToString("F1")}°");
        }

        GUILayout.Label($"\n世界点: {worldPoint.ToString("F2")}");
        if (parentObject != null)
        {
            Vector3 local = parentObject.InverseTransformPoint(worldPoint);
            GUILayout.Label($"本地坐标: {local.ToString("F2")}");
        }

        GUILayout.Label($"\n本地点: {localPoint.ToString("F2")}");
        if (parentObject != null)
        {
            Vector3 world = parentObject.TransformPoint(localPoint);
            GUILayout.Label($"世界坐标: {world.ToString("F2")}");
        }

        if (useMousePosition)
        {
            GUILayout.Label("\n点击鼠标移动世界点");
        }

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }
}
