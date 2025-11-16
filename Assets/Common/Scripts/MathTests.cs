using UnityEngine;

/// <summary>
/// 单元测试工具 - 验证数学函数的正确性
/// 在Unity Console中查看测试结果
/// </summary>
public class MathTests : MonoBehaviour
{
    [Header("测试选项")]
    public bool runTestsOnStart = true;
    public bool runVectorTests = true;
    public bool runMathHelperTests = true;
    public bool runGeometryTests = true;

    void Start()
    {
        if (runTestsOnStart)
        {
            RunAllTests();
        }
    }

    [ContextMenu("运行所有测试")]
    public void RunAllTests()
    {
        Debug.Log("========== 开始数学测试 ==========");
        int passedTests = 0;
        int totalTests = 0;

        if (runVectorTests)
        {
            Debug.Log("\n--- 向量基础测试 ---");
            passedTests += RunVectorBasicTests(out int vectorTests);
            totalTests += vectorTests;
        }

        if (runMathHelperTests)
        {
            Debug.Log("\n--- MathHelper测试 ---");
            passedTests += RunMathHelperTests(out int mathTests);
            totalTests += mathTests;
        }

        if (runGeometryTests)
        {
            Debug.Log("\n--- 几何计算测试 ---");
            passedTests += RunGeometryTests(out int geoTests);
            totalTests += geoTests;
        }

        Debug.Log($"\n========== 测试完成: {passedTests}/{totalTests} 通过 ==========");

        if (passedTests == totalTests)
        {
            Debug.Log("<color=green>✓ 所有测试通过！</color>");
        }
        else
        {
            Debug.LogWarning($"<color=yellow>⚠ {totalTests - passedTests} 个测试失败</color>");
        }
    }

    int RunVectorBasicTests(out int totalTests)
    {
        int passed = 0;
        totalTests = 0;

        // 测试1: 向量长度
        totalTests++;
        Vector3 v1 = new Vector3(3, 4, 0);
        if (Mathf.Approximately(v1.magnitude, 5f))
        {
            Debug.Log("✓ 向量长度计算正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 向量长度错误: 期望5, 得到{v1.magnitude}");
        }

        // 测试2: 向量归一化
        totalTests++;
        Vector3 normalized = v1.normalized;
        if (Mathf.Approximately(normalized.magnitude, 1f))
        {
            Debug.Log("✓ 向量归一化正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 归一化向量长度错误: 期望1, 得到{normalized.magnitude}");
        }

        // 测试3: 点积
        totalTests++;
        Vector3 a = Vector3.right;
        Vector3 b = Vector3.up;
        float dot = Vector3.Dot(a, b);
        if (Mathf.Approximately(dot, 0f))
        {
            Debug.Log("✓ 垂直向量点积为0");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 点积错误: 期望0, 得到{dot}");
        }

        // 测试4: 叉积
        totalTests++;
        Vector3 cross = Vector3.Cross(Vector3.right, Vector3.up);
        if (Vector3.Distance(cross, Vector3.forward) < 0.001f)
        {
            Debug.Log("✓ 叉积方向正确 (右手法则)");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 叉积错误: 期望{Vector3.forward}, 得到{cross}");
        }

        // 测试5: 向量加法
        totalTests++;
        Vector3 sum = new Vector3(1, 2, 3) + new Vector3(4, 5, 6);
        Vector3 expected = new Vector3(5, 7, 9);
        if (Vector3.Distance(sum, expected) < 0.001f)
        {
            Debug.Log("✓ 向量加法正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 向量加法错误: 期望{expected}, 得到{sum}");
        }

        return passed;
    }

    int RunMathHelperTests(out int totalTests)
    {
        int passed = 0;
        totalTests = 0;

        // 测试1: SafeNormalize
        totalTests++;
        Vector3 zero = Vector3.zero;
        Vector3 safeNorm = MathHelper.SafeNormalize(zero, Vector3.up);
        if (safeNorm == Vector3.up)
        {
            Debug.Log("✓ SafeNormalize处理零向量正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ SafeNormalize错误: 期望{Vector3.up}, 得到{safeNorm}");
        }

        // 测试2: 向量投影
        totalTests++;
        Vector3 v = new Vector3(3, 4, 0);
        Vector3 onto = Vector3.right;
        Vector3 projection = MathHelper.ProjectOnVector(v, onto);
        Vector3 expectedProj = new Vector3(3, 0, 0);
        if (Vector3.Distance(projection, expectedProj) < 0.001f)
        {
            Debug.Log("✓ 向量投影计算正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 投影错误: 期望{expectedProj}, 得到{projection}");
        }

        // 测试3: 角度计算
        totalTests++;
        float angle = MathHelper.AngleBetween(Vector3.right, Vector3.up);
        if (Mathf.Approximately(angle, 90f))
        {
            Debug.Log("✓ 角度计算正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 角度错误: 期望90度, 得到{angle}度");
        }

        // 测试4: Rejection (垂直分量)
        totalTests++;
        Vector3 rejection = MathHelper.Rejection(v, onto);
        Vector3 expectedRej = new Vector3(0, 4, 0);
        if (Vector3.Distance(rejection, expectedRej) < 0.001f)
        {
            Debug.Log("✓ Rejection计算正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ Rejection错误: 期望{expectedRej}, 得到{rejection}");
        }

        // 测试5: 验证投影+垂直分量=原向量
        totalTests++;
        Vector3 reconstructed = projection + rejection;
        if (Vector3.Distance(reconstructed, v) < 0.001f)
        {
            Debug.Log("✓ 投影+垂直分量=原向量");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 重构失败: 期望{v}, 得到{reconstructed}");
        }

        return passed;
    }

    int RunGeometryTests(out int totalTests)
    {
        int passed = 0;
        totalTests = 0;

        // 测试1: 点到线段最近点 (点在线段上)
        totalTests++;
        Vector3 point = new Vector3(5, 5, 0);
        Vector3 lineStart = new Vector3(0, 0, 0);
        Vector3 lineEnd = new Vector3(10, 10, 0);
        Vector3 closest = MathHelper.ClosestPointOnLineSegment(point, lineStart, lineEnd);
        if (Vector3.Distance(closest, point) < 0.001f)
        {
            Debug.Log("✓ 点到线段最近点 (点在线段上)");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 最近点错误: 期望{point}, 得到{closest}");
        }

        // 测试2: 点到线段最近点 (点在线段外)
        totalTests++;
        Vector3 point2 = new Vector3(15, 15, 0);
        Vector3 closest2 = MathHelper.ClosestPointOnLineSegment(point2, lineStart, lineEnd);
        if (Vector3.Distance(closest2, lineEnd) < 0.001f)
        {
            Debug.Log("✓ 点到线段最近点 (点在线段外，应该是端点)");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 最近点错误: 期望{lineEnd}, 得到{closest2}");
        }

        // 测试3: 三角形面积
        totalTests++;
        Vector3 p1 = Vector3.zero;
        Vector3 p2 = new Vector3(4, 0, 0);
        Vector3 p3 = new Vector3(0, 3, 0);
        float area = MathHelper.TriangleArea(p1, p2, p3);
        float expectedArea = 6f; // (4 * 3) / 2
        if (Mathf.Approximately(area, expectedArea))
        {
            Debug.Log("✓ 三角形面积计算正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 面积错误: 期望{expectedArea}, 得到{area}");
        }

        // 测试4: 点到平面距离
        totalTests++;
        Vector3 planePoint = Vector3.zero;
        Vector3 planeNormal = Vector3.up;
        Vector3 testPoint = new Vector3(0, 5, 0);
        float distance = MathHelper.DistanceToPlane(testPoint, planePoint, planeNormal);
        if (Mathf.Approximately(distance, 5f))
        {
            Debug.Log("✓ 点到平面距离正确");
            passed++;
        }
        else
        {
            Debug.LogError($"✗ 距离错误: 期望5, 得到{distance}");
        }

        // 测试5: 方向判断 (左侧)
        totalTests++;
        Vector3 lineStart2 = Vector3.zero;
        Vector3 lineEnd2 = Vector3.forward;
        Vector3 leftPoint = Vector3.left;
        bool isLeft = MathHelper.IsLeft(lineStart2, lineEnd2, leftPoint);
        if (isLeft)
        {
            Debug.Log("✓ 左侧方向判断正确");
            passed++;
        }
        else
        {
            Debug.LogError("✗ 方向判断错误: 应该在左侧");
        }

        return passed;
    }

    void Update()
    {
        // 按T键运行测试
        if (Input.GetKeyDown(KeyCode.T))
        {
            RunAllTests();
        }
    }
}
