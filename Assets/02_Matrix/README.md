# 第2章：矩阵 (Matrix)

矩阵是表示和计算几何变换的数学工具。

## 📝 学习内容

### 1. 矩阵基础
- 矩阵的表示和运算
- 单位矩阵
- 矩阵乘法
- 矩阵的意义

### 2. 变换矩阵
- 平移矩阵
- 旋转矩阵
- 缩放矩阵
- 组合变换

## 🎯 核心概念

矩阵可以表示：
- **位置变换**（平移）
- **旋转变换**
- **缩放变换**
- **投影变换**

## 💡 Unity中的矩阵

```csharp
// Unity的Matrix4x4
Matrix4x4 matrix = transform.localToWorldMatrix;

// 应用矩阵变换
Vector3 worldPos = matrix.MultiplyPoint3x4(localPos);
```

## 🔗 相关资源
- [Unity Matrix4x4 文档](https://docs.unity3d.com/ScriptReference/Matrix4x4.html)

---

**注意**：本章节的详细示例代码将在后续添加。当前可以先学习向量章节的内容。
