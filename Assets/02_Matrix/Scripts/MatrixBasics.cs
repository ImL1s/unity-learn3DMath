using UnityEngine;

/// <summary>
/// 矩阵基础示例
/// 演示矩阵的基本概念、运算和可视化
/// </summary>
public class MatrixBasics : MonoBehaviour
{
    [Header("演示对象")]
    public Transform cubeA;
    public Transform cubeB;

    [Header("矩阵操作")]
    public MatrixOperation operation = MatrixOperation.Multiply;

    [Header("自定义矩阵参数")]
    [Range(0f, 360f)]
    public float rotationAngle = 45f;
    public Vector3 scaleFactor = Vector3.one;
    public Vector3 translationOffset = Vector3.zero;

    [Header("显示选项")]
    public bool showMatrixInfo = true;
    public bool showBasisVectors = true;
    public bool showTransformedObject = true;
    public bool showDebugInfo = true;

    private Matrix4x4 matrixA;
    private Matrix4x4 matrixB;
    private Matrix4x4 resultMatrix;

    public enum MatrixOperation
    {
        Identity,           // 单位矩阵
        Translation,        // 平移矩阵
        Rotation,          // 旋转矩阵
        Scale,             // 缩放矩阵
        Multiply,          // 矩阵乘法
        Inverse,           // 逆矩阵
        Transpose,         // 转置矩阵
        TRS                // 组合变换（平移-旋转-缩放）
    }

    void Update()
    {
        UpdateMatrices();

        // 应用变换到对象
        if (showTransformedObject && cubeB != null)
        {
            ApplyMatrixToObject();
        }

        // 按空格键输出信息
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LogMatrixInfo();
        }
    }

    void UpdateMatrices()
    {
        // 获取cubeA的变换矩阵
        if (cubeA != null)
        {
            matrixA = cubeA.localToWorldMatrix;
        }
        else
        {
            matrixA = Matrix4x4.identity;
        }

        // 根据操作类型计算结果矩阵
        switch (operation)
        {
            case MatrixOperation.Identity:
                resultMatrix = Matrix4x4.identity;
                break;

            case MatrixOperation.Translation:
                resultMatrix = Matrix4x4.Translate(translationOffset);
                break;

            case MatrixOperation.Rotation:
                resultMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, rotationAngle, 0));
                break;

            case MatrixOperation.Scale:
                resultMatrix = Matrix4x4.Scale(scaleFactor);
                break;

            case MatrixOperation.Multiply:
                matrixB = Matrix4x4.TRS(translationOffset,
                                       Quaternion.Euler(0, rotationAngle, 0),
                                       scaleFactor);
                resultMatrix = matrixA * matrixB;
                break;

            case MatrixOperation.Inverse:
                resultMatrix = matrixA.inverse;
                break;

            case MatrixOperation.Transpose:
                resultMatrix = matrixA.transpose;
                break;

            case MatrixOperation.TRS:
                resultMatrix = Matrix4x4.TRS(translationOffset,
                                            Quaternion.Euler(0, rotationAngle, 0),
                                            scaleFactor);
                break;
        }
    }

    void ApplyMatrixToObject()
    {
        // 从矩阵提取位置、旋转、缩放
        Vector3 position = resultMatrix.GetColumn(3);
        Quaternion rotation = resultMatrix.rotation;
        Vector3 scale = resultMatrix.lossyScale;

        cubeB.position = position;
        cubeB.rotation = rotation;
        cubeB.localScale = scale;
    }

    void OnDrawGizmos()
    {
        // 绘制cubeA的变换
        if (cubeA != null && showBasisVectors)
        {
            DrawMatrixBasis(cubeA.position, matrixA, "矩阵 A", Color.green);
        }

        // 绘制结果矩阵
        if (showBasisVectors)
        {
            Vector3 resultPos = resultMatrix.GetColumn(3);
            DrawMatrixBasis(resultPos, resultMatrix, "结果矩阵", Color.cyan);
        }

        // 显示矩阵信息
        if (showMatrixInfo)
        {
            DrawMatrixInfo();
        }

        // 绘制操作说明
        if (showDebugInfo)
        {
            DrawOperationInfo();
        }
    }

    void DrawMatrixBasis(Vector3 origin, Matrix4x4 matrix, string label, Color color)
    {
        // 提取基向量（矩阵的列）
        Vector3 right = matrix.GetColumn(0);
        Vector3 up = matrix.GetColumn(1);
        Vector3 forward = matrix.GetColumn(2);

        float length = 1.5f;

        // X轴（右）- 红色
        Gizmos.color = new Color(color.r, 0, 0);
        DebugDrawer.DrawArrow(origin, origin + right.normalized * length, 0.2f);
        DrawLabel(origin + right.normalized * (length + 0.3f), "X");

        // Y轴（上）- 绿色
        Gizmos.color = new Color(0, color.g, 0);
        DebugDrawer.DrawArrow(origin, origin + up.normalized * length, 0.2f);
        DrawLabel(origin + up.normalized * (length + 0.3f), "Y");

        // Z轴（前）- 蓝色
        Gizmos.color = new Color(0, 0, color.b);
        DebugDrawer.DrawArrow(origin, origin + forward.normalized * length, 0.2f);
        DrawLabel(origin + forward.normalized * (length + 0.3f), "Z");

        // 原点
        Gizmos.color = color;
        Gizmos.DrawWireSphere(origin, 0.2f);

        DrawLabel(origin + Vector3.up * 2f, label);
    }

    void DrawMatrixInfo()
    {
        Vector3 infoPos = transform.position + Vector3.right * 5f;

        string info = "矩阵值:\n\n";
        info += FormatMatrix(resultMatrix);

        // 提取TRS
        Vector3 pos = resultMatrix.GetColumn(3);
        Quaternion rot = resultMatrix.rotation;
        Vector3 scale = resultMatrix.lossyScale;

        info += $"\n\n提取的变换:\n";
        info += $"位置: {pos.ToString("F2")}\n";
        info += $"旋转: {rot.eulerAngles.ToString("F1")}°\n";
        info += $"缩放: {scale.ToString("F2")}";

        DrawLabel(infoPos, info);
    }

    void DrawOperationInfo()
    {
        Vector3 infoPos = transform.position + Vector3.left * 5f;

        string info = $"矩阵操作: {operation}\n\n";

        switch (operation)
        {
            case MatrixOperation.Identity:
                info += "单位矩阵\n对角线为1，其余为0\n不改变任何变换";
                break;

            case MatrixOperation.Translation:
                info += $"平移矩阵\n偏移: {translationOffset.ToString("F2")}";
                break;

            case MatrixOperation.Rotation:
                info += $"旋转矩阵\n角度: {rotationAngle:F1}° (Y轴)";
                break;

            case MatrixOperation.Scale:
                info += $"缩放矩阵\n缩放: {scaleFactor.ToString("F2")}";
                break;

            case MatrixOperation.Multiply:
                info += "矩阵乘法\n结果 = A × B\n顺序很重要！";
                break;

            case MatrixOperation.Inverse:
                info += "逆矩阵\nA × A⁻¹ = I\n撤销变换";
                break;

            case MatrixOperation.Transpose:
                info += "转置矩阵\n行列互换\n正交矩阵: Tᵀ = T⁻¹";
                break;

            case MatrixOperation.TRS:
                info += "组合变换\nT × R × S\n先缩放，后旋转，最后平移";
                break;
        }

        DrawLabel(infoPos, info);
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

    void LogMatrixInfo()
    {
        Debug.Log("=== 矩阵基础信息 ===");
        Debug.Log($"操作: {operation}");
        Debug.Log($"\n结果矩阵:");
        Debug.Log(FormatMatrix(resultMatrix));

        // 矩阵属性
        Debug.Log($"\n矩阵属性:");
        Debug.Log($"行列式: {resultMatrix.determinant}");
        Debug.Log($"是否为单位矩阵: {resultMatrix.isIdentity}");
        Debug.Log($"合法性检查: {resultMatrix.ValidTRS()}");

        // 提取TRS
        Vector3 pos = resultMatrix.GetColumn(3);
        Quaternion rot = resultMatrix.rotation;
        Vector3 scale = resultMatrix.lossyScale;

        Debug.Log($"\n提取的变换:");
        Debug.Log($"位置: {pos}");
        Debug.Log($"旋转: {rot.eulerAngles}");
        Debug.Log($"缩放: {scale}");

        // 基向量
        Debug.Log($"\n基向量:");
        Debug.Log($"右向量 (X轴): {resultMatrix.GetColumn(0)}");
        Debug.Log($"上向量 (Y轴): {resultMatrix.GetColumn(1)}");
        Debug.Log($"前向量 (Z轴): {resultMatrix.GetColumn(2)}");

        // 如果是乘法操作，演示顺序重要性
        if (operation == MatrixOperation.Multiply)
        {
            Matrix4x4 ab = matrixA * matrixB;
            Matrix4x4 ba = matrixB * matrixA;

            Debug.Log($"\n矩阵乘法顺序:");
            Debug.Log($"A × B ≠ B × A");
            Debug.Log($"A × B 位置: {ab.GetColumn(3)}");
            Debug.Log($"B × A 位置: {ba.GetColumn(3)}");
        }

        // 如果是逆矩阵，验证 A × A⁻¹ = I
        if (operation == MatrixOperation.Inverse && cubeA != null)
        {
            Matrix4x4 identity = matrixA * resultMatrix;
            Debug.Log($"\n逆矩阵验证:");
            Debug.Log($"A × A⁻¹ = I");
            Debug.Log($"是否为单位矩阵: {identity.isIdentity}");
        }

        // 实用技巧
        Debug.Log($"\n实用技巧:");
        Debug.Log($"- localToWorldMatrix: 本地空间 → 世界空间");
        Debug.Log($"- worldToLocalMatrix: 世界空间 → 本地空间");
        Debug.Log($"- Matrix4x4.TRS(): 创建变换矩阵");
        Debug.Log($"- MultiplyPoint3x4(): 变换点（考虑平移）");
        Debug.Log($"- MultiplyVector(): 变换向量（不考虑平移）");
    }

    void DrawLabel(Vector3 position, string text)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, text);
#endif
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 350));
        GUILayout.Box("矩阵基础示例");

        GUILayout.Label($"操作: {operation}");

        switch (operation)
        {
            case MatrixOperation.Translation:
                GUILayout.Label($"平移: {translationOffset.ToString("F2")}");
                break;

            case MatrixOperation.Rotation:
                GUILayout.Label($"旋转: {rotationAngle:F1}°");
                break;

            case MatrixOperation.Scale:
                GUILayout.Label($"缩放: {scaleFactor.ToString("F2")}");
                break;

            case MatrixOperation.TRS:
                GUILayout.Label($"平移: {translationOffset.ToString("F2")}");
                GUILayout.Label($"旋转: {rotationAngle:F1}°");
                GUILayout.Label($"缩放: {scaleFactor.ToString("F2")}");
                break;
        }

        GUILayout.Label($"\n结果矩阵:");

        // 显示矩阵（简化版）
        for (int row = 0; row < 3; row++)
        {
            string rowText = "[ ";
            for (int col = 0; col < 4; col++)
            {
                rowText += $"{resultMatrix[row, col]:F1} ";
            }
            rowText += "]";
            GUILayout.Label(rowText);
        }

        // 提取的变换
        Vector3 pos = resultMatrix.GetColumn(3);
        GUILayout.Label($"\n位置: {pos.ToString("F1")}");

        Vector3 scale = resultMatrix.lossyScale;
        GUILayout.Label($"缩放: {scale.ToString("F2")}");

        GUILayout.Label("\n按空格键查看详细信息");

        GUILayout.EndArea();
    }

    // 公共API
    public Matrix4x4 GetResultMatrix()
    {
        return resultMatrix;
    }

    public void SetOperation(MatrixOperation newOperation)
    {
        operation = newOperation;
    }

    // 演示矩阵变换点
    public Vector3 TransformPoint(Vector3 point)
    {
        return resultMatrix.MultiplyPoint3x4(point);
    }

    // 演示矩阵变换向量
    public Vector3 TransformVector(Vector3 vector)
    {
        return resultMatrix.MultiplyVector(vector);
    }
}
