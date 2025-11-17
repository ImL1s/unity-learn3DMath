using UnityEngine;

/// <summary>
/// 变换矩阵示例
/// 演示使用矩阵进行自定义变换，包括切变、镜像、投影等
/// </summary>
public class TransformMatrix : MonoBehaviour
{
    [Header("演示对象")]
    public Transform sourceObject;
    public Transform[] transformedObjects;

    [Header("变换类型")]
    public TransformType transformType = TransformType.Shear;

    [Header("切变参数")]
    [Range(-2f, 2f)]
    public float shearX = 0.5f;
    [Range(-2f, 2f)]
    public float shearY = 0.5f;

    [Header("镜像参数")]
    public MirrorPlane mirrorPlane = MirrorPlane.XZ;

    [Header("投影参数")]
    public ProjectionPlane projectionPlane = ProjectionPlane.XZ;

    [Header("自定义变换")]
    public bool animateTransform = false;
    public float animationSpeed = 1f;

    [Header("显示选项")]
    public bool showOriginal = true;
    public bool showTransformed = true;
    public bool showGrid = true;
    public bool showDebugInfo = true;

    private Matrix4x4 transformMatrix;
    private float animationTime = 0f;

    public enum TransformType
    {
        Shear,              // 切变
        Mirror,             // 镜像
        Projection,         // 投影
        NonUniformScale,    // 非均匀缩放
        CustomTransform     // 自定义变换
    }

    public enum MirrorPlane
    {
        XY,     // 沿XY平面镜像（翻转Z）
        XZ,     // 沿XZ平面镜像（翻转Y）
        YZ      // 沿YZ平面镜像（翻转X）
    }

    public enum ProjectionPlane
    {
        XY,     // 投影到XY平面（Z=0）
        XZ,     // 投影到XZ平面（Y=0）
        YZ      // 投影到YZ平面（X=0）
    }

    void Update()
    {
        // 动画
        if (animateTransform)
        {
            animationTime += Time.deltaTime * animationSpeed;
            UpdateAnimatedParameters();
        }

        // 计算变换矩阵
        CalculateTransformMatrix();

        // 应用变换
        ApplyTransforms();

        // 按空格键输出信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogTransformInfo();
        }
    }

    void UpdateAnimatedParameters()
    {
        switch (transformType)
        {
            case TransformType.Shear:
                shearX = Mathf.Sin(animationTime) * 1.5f;
                shearY = Mathf.Cos(animationTime) * 1.5f;
                break;
        }
    }

    void CalculateTransformMatrix()
    {
        switch (transformType)
        {
            case TransformType.Shear:
                transformMatrix = CreateShearMatrix(shearX, shearY);
                break;

            case TransformType.Mirror:
                transformMatrix = CreateMirrorMatrix(mirrorPlane);
                break;

            case TransformType.Projection:
                transformMatrix = CreateProjectionMatrix(projectionPlane);
                break;

            case TransformType.NonUniformScale:
                // 沿不同轴不同缩放
                Vector3 scale = new Vector3(
                    1f + Mathf.Sin(animationTime) * 0.5f,
                    1f + Mathf.Cos(animationTime) * 0.5f,
                    1f
                );
                transformMatrix = Matrix4x4.Scale(scale);
                break;

            case TransformType.CustomTransform:
                transformMatrix = CreateCustomMatrix();
                break;
        }
    }

    Matrix4x4 CreateShearMatrix(float sx, float sy)
    {
        // 切变矩阵：在XY平面上切变
        Matrix4x4 matrix = Matrix4x4.identity;

        // X方向受Y影响
        matrix.m01 = sx;

        // Y方向受X影响
        matrix.m10 = sy;

        return matrix;
    }

    Matrix4x4 CreateMirrorMatrix(MirrorPlane plane)
    {
        Matrix4x4 matrix = Matrix4x4.identity;

        switch (plane)
        {
            case MirrorPlane.XY:
                matrix.m22 = -1f; // 翻转Z
                break;

            case MirrorPlane.XZ:
                matrix.m11 = -1f; // 翻转Y
                break;

            case MirrorPlane.YZ:
                matrix.m00 = -1f; // 翻转X
                break;
        }

        return matrix;
    }

    Matrix4x4 CreateProjectionMatrix(ProjectionPlane plane)
    {
        Matrix4x4 matrix = Matrix4x4.identity;

        switch (plane)
        {
            case ProjectionPlane.XY:
                matrix.m22 = 0f; // 消除Z分量
                break;

            case ProjectionPlane.XZ:
                matrix.m11 = 0f; // 消除Y分量
                break;

            case ProjectionPlane.YZ:
                matrix.m00 = 0f; // 消除X分量
                break;
        }

        return matrix;
    }

    Matrix4x4 CreateCustomMatrix()
    {
        // 创建一个组合旋转和切变的自定义矩阵
        float angle = animationTime * 30f;
        Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.Euler(0, angle, 0));
        Matrix4x4 shear = CreateShearMatrix(0.3f, 0.3f);

        return rotation * shear;
    }

    void ApplyTransforms()
    {
        if (sourceObject == null || transformedObjects == null) return;

        // 获取源对象的顶点位置（简化：只使用位置）
        Vector3 sourcePos = sourceObject.position;

        // 应用变换到每个目标对象
        for (int i = 0; i < transformedObjects.Length; i++)
        {
            if (transformedObjects[i] != null)
            {
                // 计算相对位置
                Vector3 relativePos = sourcePos - transform.position;

                // 应用变换矩阵
                Vector3 transformedPos = transformMatrix.MultiplyPoint3x4(relativePos);

                // 设置变换后的位置
                transformedObjects[i].position = transform.position + transformedPos;

                // 对于某些变换，还需要调整旋转和缩放
                if (transformType == TransformType.Mirror)
                {
                    transformedObjects[i].rotation = sourceObject.rotation;
                    Vector3 scale = sourceObject.localScale;

                    // 镜像时反转对应的缩放
                    switch (mirrorPlane)
                    {
                        case MirrorPlane.XZ:
                            scale.y = -scale.y;
                            break;
                        case MirrorPlane.XY:
                            scale.z = -scale.z;
                            break;
                        case MirrorPlane.YZ:
                            scale.x = -scale.x;
                            break;
                    }

                    transformedObjects[i].localScale = scale;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // 绘制网格
        if (showGrid)
        {
            DrawTransformGrid();
        }

        // 绘制原始对象
        if (showOriginal && sourceObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(sourceObject.position, sourceObject.localScale);
            DrawLabel(sourceObject.position + Vector3.up * 1.5f, "原始");
        }

        // 绘制变换后的对象
        if (showTransformed && transformedObjects != null)
        {
            for (int i = 0; i < transformedObjects.Length; i++)
            {
                if (transformedObjects[i] != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(transformedObjects[i].position,
                                       transformedObjects[i].localScale);
                    DrawLabel(transformedObjects[i].position + Vector3.up * 1.5f, "变换后");
                }
            }
        }

        // 绘制变换平面
        DrawTransformPlane();

        // 显示调试信息
        if (showDebugInfo)
        {
            DrawDebugInfo();
        }
    }

    void DrawTransformGrid()
    {
        // 绘制变换前的网格（绿色）
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        DrawGrid(transform.position, Matrix4x4.identity, 5, 1f);

        // 绘制变换后的网格（青色）
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        DrawGrid(transform.position, transformMatrix, 5, 1f);
    }

    void DrawGrid(Vector3 center, Matrix4x4 matrix, int size, float spacing)
    {
        int halfSize = size / 2;

        // 绘制X方向的线
        for (int z = -halfSize; z <= halfSize; z++)
        {
            Vector3 start = new Vector3(-halfSize * spacing, 0, z * spacing);
            Vector3 end = new Vector3(halfSize * spacing, 0, z * spacing);

            Vector3 transformedStart = matrix.MultiplyPoint3x4(start) + center;
            Vector3 transformedEnd = matrix.MultiplyPoint3x4(end) + center;

            Gizmos.DrawLine(transformedStart, transformedEnd);
        }

        // 绘制Z方向的线
        for (int x = -halfSize; x <= halfSize; x++)
        {
            Vector3 start = new Vector3(x * spacing, 0, -halfSize * spacing);
            Vector3 end = new Vector3(x * spacing, 0, halfSize * spacing);

            Vector3 transformedStart = matrix.MultiplyPoint3x4(start) + center;
            Vector3 transformedEnd = matrix.MultiplyPoint3x4(end) + center;

            Gizmos.DrawLine(transformedStart, transformedEnd);
        }
    }

    void DrawTransformPlane()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Vector3 center = transform.position;
        float size = 4f;

        switch (transformType)
        {
            case TransformType.Mirror:
                switch (mirrorPlane)
                {
                    case MirrorPlane.XZ:
                        DebugDrawer.DrawPlane(center, Vector3.up, size);
                        DrawLabel(center + Vector3.up * 0.5f, "镜像平面 (XZ)");
                        break;

                    case MirrorPlane.XY:
                        DebugDrawer.DrawPlane(center, Vector3.forward, size);
                        DrawLabel(center + Vector3.forward * 0.5f, "镜像平面 (XY)");
                        break;

                    case MirrorPlane.YZ:
                        DebugDrawer.DrawPlane(center, Vector3.right, size);
                        DrawLabel(center + Vector3.right * 0.5f, "镜像平面 (YZ)");
                        break;
                }
                break;

            case TransformType.Projection:
                switch (projectionPlane)
                {
                    case ProjectionPlane.XZ:
                        DebugDrawer.DrawPlane(center, Vector3.up, size);
                        DrawLabel(center, "投影平面 (XZ)");
                        break;

                    case ProjectionPlane.XY:
                        DebugDrawer.DrawPlane(center, Vector3.forward, size);
                        DrawLabel(center, "投影平面 (XY)");
                        break;

                    case ProjectionPlane.YZ:
                        DebugDrawer.DrawPlane(center, Vector3.right, size);
                        DrawLabel(center, "投影平面 (YZ)");
                        break;
                }
                break;
        }
    }

    void DrawDebugInfo()
    {
        Vector3 infoPos = transform.position + Vector3.right * 6f;

        string info = $"变换类型: {transformType}\n\n";

        switch (transformType)
        {
            case TransformType.Shear:
                info += $"切变参数:\n";
                info += $"X方向: {shearX:F2}\n";
                info += $"Y方向: {shearY:F2}\n";
                info += $"\n切变改变形状但保持面积";
                break;

            case TransformType.Mirror:
                info += $"镜像平面: {mirrorPlane}\n";
                info += $"\n镜像反转一个轴";
                break;

            case TransformType.Projection:
                info += $"投影平面: {projectionPlane}\n";
                info += $"\n投影消除一个维度";
                break;

            case TransformType.NonUniformScale:
                Vector3 scale = transformMatrix.lossyScale;
                info += $"非均匀缩放:\n";
                info += $"X: {scale.x:F2}\n";
                info += $"Y: {scale.y:F2}\n";
                info += $"Z: {scale.z:F2}";
                break;

            case TransformType.CustomTransform:
                info += "组合变换\n旋转 + 切变";
                break;
        }

        info += $"\n\n矩阵行列式: {transformMatrix.determinant:F3}";

        DrawLabel(infoPos, info);
    }

    void LogTransformInfo()
    {
        Debug.Log("=== 变换矩阵信息 ===");
        Debug.Log($"变换类型: {transformType}");

        Debug.Log($"\n变换矩阵:");
        Debug.Log(FormatMatrix(transformMatrix));

        Debug.Log($"\n矩阵属性:");
        Debug.Log($"行列式: {transformMatrix.determinant}");

        // 行列式的意义
        float det = transformMatrix.determinant;
        if (Mathf.Abs(det) < 0.001f)
        {
            Debug.Log("行列式 = 0: 矩阵是奇异的（不可逆），变换降维");
        }
        else if (det < 0)
        {
            Debug.Log("行列式 < 0: 变换改变了手性（翻转）");
        }
        else
        {
            Debug.Log($"行列式 = {det:F3}: 体积缩放因子");
        }

        // 测试点变换
        if (sourceObject != null)
        {
            Vector3 originalPos = sourceObject.position - transform.position;
            Vector3 transformedPos = transformMatrix.MultiplyPoint3x4(originalPos);

            Debug.Log($"\n点变换示例:");
            Debug.Log($"原始位置: {originalPos}");
            Debug.Log($"变换后位置: {transformedPos}");
        }

        // 特定变换的说明
        switch (transformType)
        {
            case TransformType.Shear:
                Debug.Log($"\n切变说明:");
                Debug.Log($"- 保持面积不变");
                Debug.Log($"- 改变角度但不改变平行性");
                Debug.Log($"- 常用于斜投影");
                break;

            case TransformType.Mirror:
                Debug.Log($"\n镜像说明:");
                Debug.Log($"- 保持距离和角度");
                Debug.Log($"- 改变手性（左手 ↔ 右手）");
                Debug.Log($"- 行列式 = -1");
                break;

            case TransformType.Projection:
                Debug.Log($"\n投影说明:");
                Debug.Log($"- 降低维度");
                Debug.Log($"- 不可逆（行列式 = 0）");
                Debug.Log($"- 丢失一个方向的信息");
                break;
        }
    }

    string FormatMatrix(Matrix4x4 m)
    {
        string result = "";
        for (int row = 0; row < 4; row++)
        {
            result += "[ ";
            for (int col = 0; col < 4; col++)
            {
                result += $"{m[row, col]:F2} ";
            }
            result += "]\n";
        }
        return result;
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
        GUILayout.Box("变换矩阵示例");

        GUILayout.Label($"变换类型: {transformType}");

        switch (transformType)
        {
            case TransformType.Shear:
                GUILayout.Label($"切变 X: {shearX:F2}");
                GUILayout.Label($"切变 Y: {shearY:F2}");
                break;

            case TransformType.Mirror:
                GUILayout.Label($"镜像平面: {mirrorPlane}");
                break;

            case TransformType.Projection:
                GUILayout.Label($"投影平面: {projectionPlane}");
                break;
        }

        GUILayout.Label($"\n行列式: {transformMatrix.determinant:F3}");

        if (Mathf.Abs(transformMatrix.determinant) < 0.001f)
        {
            GUI.color = Color.yellow;
            GUILayout.Label("不可逆变换（降维）");
            GUI.color = Color.white;
        }

        GUILayout.Label($"\n动画: {(animateTransform ? "开启" : "关闭")}");

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }

    // 公共API
    public Matrix4x4 GetTransformMatrix()
    {
        return transformMatrix;
    }
}
