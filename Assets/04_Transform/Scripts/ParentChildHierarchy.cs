using UnityEngine;

/// <summary>
/// 父子层级关系示例
/// 演示父子Transform的相对变换和层级影响
/// </summary>
public class ParentChildHierarchy : MonoBehaviour
{
    [Header("层级对象")]
    public Transform grandParent;
    public Transform parent;
    public Transform child;

    [Header("变换测试")]
    public Vector3 testLocalPosition = new Vector3(1, 0, 0);
    public Vector3 testWorldPosition = new Vector3(5, 0, 0);

    [Header("动画")]
    public bool animateGrandParent = false;
    public bool animateParent = false;
    public bool animateChild = false;
    public float rotationSpeed = 30f;

    [Header("显示选项")]
    public bool showLocalAxes = true;
    public bool showWorldPositions = true;
    public bool showHierarchyLines = true;
    public bool showTransformInfo = true;

    [Header("交互")]
    public bool enableManualControl = false;

    private float animationTime = 0f;

    void Start()
    {
        // 自动创建层级结构（如果引用为空）
        if (grandParent == null && parent == null && child == null)
        {
            CreateHierarchyAutomatically();
        }
    }

    void CreateHierarchyAutomatically()
    {
        // 创建祖父对象
        GameObject grandParentObj = new GameObject("Grandparent");
        grandParent = grandParentObj.transform;
        grandParent.position = transform.position + new Vector3(-3, 0, 0);
        grandParent.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        // 创建父对象
        GameObject parentObj = new GameObject("Parent");
        parent = parentObj.transform;
        parent.SetParent(grandParent);
        parent.localPosition = new Vector3(2, 0, 0);
        parent.localRotation = Quaternion.Euler(0, 30, 0);
        parent.localScale = new Vector3(1.1f, 1.1f, 1.1f);

        // 创建子对象
        GameObject childObj = new GameObject("Child");
        child = childObj.transform;
        child.SetParent(parent);
        child.localPosition = new Vector3(1.5f, 0, 0);
        child.localRotation = Quaternion.Euler(0, 0, 45);
        child.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        // 为每个对象添加可视化组件（创建立方体）
        CreateVisualCube(grandParent, Color.red, 0.5f);
        CreateVisualCube(parent, Color.green, 0.4f);
        CreateVisualCube(child, Color.blue, 0.3f);

        Debug.Log("已自动创建三级层级结构: Grandparent → Parent → Child");
    }

    void CreateVisualCube(Transform target, Color color, float size)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(target);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale = Vector3.one * size;
        cube.name = "Visual";

        // 设置颜色
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            renderer.material = mat;
        }
    }

    void Update()
    {
        // 动画
        if (animateGrandParent || animateParent || animateChild)
        {
            UpdateAnimation();
        }

        // 手动控制
        if (enableManualControl)
        {
            HandleInput();
        }

        // 按空格键输出信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogHierarchyInfo();
        }
    }

    void UpdateAnimation()
    {
        animationTime += Time.deltaTime;

        if (animateGrandParent && grandParent != null)
        {
            grandParent.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        if (animateParent && parent != null)
        {
            parent.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        }

        if (animateChild && child != null)
        {
            child.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleInput()
    {
        if (parent == null) return;

        float moveSpeed = 2f * Time.deltaTime;
        float rotSpeed = 50f * Time.deltaTime;

        // WASD 移动父对象
        if (Input.GetKey(KeyCode.W))
            parent.Translate(Vector3.forward * moveSpeed, Space.Self);
        if (Input.GetKey(KeyCode.S))
            parent.Translate(Vector3.back * moveSpeed, Space.Self);
        if (Input.GetKey(KeyCode.A))
            parent.Translate(Vector3.left * moveSpeed, Space.Self);
        if (Input.GetKey(KeyCode.D))
            parent.Translate(Vector3.right * moveSpeed, Space.Self);

        // QE 旋转父对象
        if (Input.GetKey(KeyCode.Q))
            parent.Rotate(Vector3.up, -rotSpeed);
        if (Input.GetKey(KeyCode.E))
            parent.Rotate(Vector3.up, rotSpeed);
    }

    void OnDrawGizmos()
    {
        // 绘制层级连线
        if (showHierarchyLines)
        {
            DrawHierarchyLines();
        }

        // 绘制每个对象的本地坐标轴
        if (showLocalAxes)
        {
            if (grandParent != null)
                DrawObjectAxes(grandParent, 2f, "祖父对象");
            if (parent != null)
                DrawObjectAxes(parent, 1.5f, "父对象");
            if (child != null)
                DrawObjectAxes(child, 1f, "子对象");
        }

        // 显示世界位置
        if (showWorldPositions)
        {
            DrawWorldPositions();
        }

        // 显示变换信息
        if (showTransformInfo)
        {
            DrawTransformInfo();
        }

        // 绘制测试点
        DrawTestPoints();
    }

    void DrawHierarchyLines()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);

        if (grandParent != null && parent != null)
        {
            Gizmos.DrawLine(grandParent.position, parent.position);
        }

        if (parent != null && child != null)
        {
            Gizmos.DrawLine(parent.position, child.position);
        }
    }

    void DrawObjectAxes(Transform obj, float length, string label)
    {
        Vector3 pos = obj.position;
        float arrowSize = length * 0.15f;

        // 本地X轴 - 红色
        Gizmos.color = Color.red;
        DebugDrawer.DrawArrow(pos, pos + obj.right * length, arrowSize);

        // 本地Y轴 - 绿色
        Gizmos.color = Color.green;
        DebugDrawer.DrawArrow(pos, pos + obj.up * length, arrowSize);

        // 本地Z轴 - 蓝色
        Gizmos.color = Color.blue;
        DebugDrawer.DrawArrow(pos, pos + obj.forward * length, arrowSize);

        // 绘制原点
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(pos, 0.2f);

        DrawLabel(pos + Vector3.up * (length + 0.5f), label);
    }

    void DrawWorldPositions()
    {
        if (parent != null)
        {
            DrawLabel(parent.position + Vector3.down * 0.8f,
                $"世界位置:\n{parent.position.ToString("F2")}\n" +
                $"本地位置:\n{parent.localPosition.ToString("F2")}");
        }

        if (child != null)
        {
            DrawLabel(child.position + Vector3.down * 0.8f,
                $"世界位置:\n{child.position.ToString("F2")}\n" +
                $"本地位置:\n{child.localPosition.ToString("F2")}");
        }
    }

    void DrawTransformInfo()
    {
        if (child == null) return;

        Vector3 infoPos = child.position + Vector3.right * 3f;

        // 计算相对于不同对象的位置
        string info = "相对变换:\n";

        if (parent != null)
        {
            Vector3 relativeToParent = parent.InverseTransformPoint(child.position);
            info += $"相对父: {relativeToParent.ToString("F2")}\n";
        }

        if (grandParent != null)
        {
            Vector3 relativeToGrandParent = grandParent.InverseTransformPoint(child.position);
            info += $"相对祖父: {relativeToGrandParent.ToString("F2")}\n";
        }

        // 世界和本地缩放
        info += $"\n本地缩放: {child.localScale.ToString("F2")}\n";
        info += $"世界缩放: {child.lossyScale.ToString("F2")}";

        DrawLabel(infoPos, info);
    }

    void DrawTestPoints()
    {
        if (parent == null) return;

        // 测试本地坐标转世界坐标
        Vector3 worldFromLocal = parent.TransformPoint(testLocalPosition);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(worldFromLocal, 0.15f);
        DebugDrawer.DrawArrow(parent.position, worldFromLocal, 0.2f);
        DrawLabel(worldFromLocal + Vector3.up * 0.3f,
            $"本地 {testLocalPosition.ToString("F1")}\n→ 世界 {worldFromLocal.ToString("F2")}");

        // 测试世界坐标转本地坐标
        Vector3 localFromWorld = parent.InverseTransformPoint(testWorldPosition);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(testWorldPosition, 0.15f);
        DrawLabel(testWorldPosition + Vector3.up * 0.3f,
            $"世界 {testWorldPosition.ToString("F1")}\n→ 本地 {localFromWorld.ToString("F2")}");
    }

    void LogHierarchyInfo()
    {
        Debug.Log("=== 父子层级信息 ===");

        if (grandParent != null)
        {
            Debug.Log($"\n祖父对象: {grandParent.name}");
            Debug.Log($"世界位置: {grandParent.position}");
            Debug.Log($"世界旋转: {grandParent.eulerAngles}");
            Debug.Log($"世界缩放: {grandParent.lossyScale}");
        }

        if (parent != null)
        {
            Debug.Log($"\n父对象: {parent.name}");
            Debug.Log($"世界位置: {parent.position}");
            Debug.Log($"本地位置: {parent.localPosition}");
            Debug.Log($"世界旋转: {parent.eulerAngles}");
            Debug.Log($"本地旋转: {parent.localEulerAngles}");
            Debug.Log($"父对象: {(parent.parent != null ? parent.parent.name : "无")}");
        }

        if (child != null)
        {
            Debug.Log($"\n子对象: {child.name}");
            Debug.Log($"世界位置: {child.position}");
            Debug.Log($"本地位置: {child.localPosition}");
            Debug.Log($"世界旋转: {child.eulerAngles}");
            Debug.Log($"本地旋转: {child.localEulerAngles}");
            Debug.Log($"本地缩放: {child.localScale}");
            Debug.Log($"世界缩放: {child.lossyScale}");
            Debug.Log($"父对象: {(child.parent != null ? child.parent.name : "无")}");
            Debug.Log($"根对象: {(child.root != null ? child.root.name : "无")}");
        }

        // 层级深度
        if (child != null)
        {
            int depth = 0;
            Transform current = child;
            while (current.parent != null)
            {
                depth++;
                current = current.parent;
            }
            Debug.Log($"\n子对象层级深度: {depth}");
        }

        // 坐标转换演示
        if (parent != null)
        {
            Debug.Log($"\n坐标转换演示:");
            Debug.Log($"测试本地点: {testLocalPosition}");
            Vector3 worldPos = parent.TransformPoint(testLocalPosition);
            Debug.Log($"转为世界坐标: {worldPos}");

            Debug.Log($"\n测试世界点: {testWorldPosition}");
            Vector3 localPos = parent.InverseTransformPoint(testWorldPosition);
            Debug.Log($"转为本地坐标: {localPos}");
        }

        // 实用提示
        Debug.Log($"\n实用技巧:");
        Debug.Log($"- 修改localPosition影响本地位置");
        Debug.Log($"- 修改position直接设置世界位置");
        Debug.Log($"- lossyScale是只读的世界缩放");
        Debug.Log($"- SetParent()可以改变父对象");
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
        GUILayout.Box("父子层级关系示例");

        if (grandParent != null)
        {
            GUILayout.Label($"祖父: {grandParent.name}");
            GUILayout.Label($"  世界位置: {grandParent.position.ToString("F1")}");
        }

        if (parent != null)
        {
            GUILayout.Label($"\n父: {parent.name}");
            GUILayout.Label($"  世界位置: {parent.position.ToString("F1")}");
            GUILayout.Label($"  本地位置: {parent.localPosition.ToString("F1")}");
        }

        if (child != null)
        {
            GUILayout.Label($"\n子: {child.name}");
            GUILayout.Label($"  世界位置: {child.position.ToString("F1")}");
            GUILayout.Label($"  本地位置: {child.localPosition.ToString("F1")}");
            GUILayout.Label($"  本地缩放: {child.localScale.ToString("F1")}");
            GUILayout.Label($"  世界缩放: {child.lossyScale.ToString("F1")}");
        }

        if (enableManualControl)
        {
            GUILayout.Label("\nWASD: 移动父对象");
            GUILayout.Label("QE: 旋转父对象");
        }

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }

    // 公共API
    public void AttachChild(Transform newChild)
    {
        if (parent != null && newChild != null)
        {
            newChild.SetParent(parent);
            Debug.Log($"{newChild.name} 已附加到 {parent.name}");
        }
    }

    public void DetachChild()
    {
        if (child != null)
        {
            child.SetParent(null);
            Debug.Log($"{child.name} 已从父对象分离");
        }
    }

    public void ResetChildLocalTransform()
    {
        if (child != null)
        {
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
            Debug.Log($"{child.name} 本地变换已重置");
        }
    }
}
