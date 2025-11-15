# 第5章：几何计算 (Geometry)

几何计算在碰撞检测、射线检测、AI寻路等方面有广泛应用。

## 📝 学习内容

### 1. 射线检测
- Ray的构建
- Raycast的使用
- RaycastHit信息
- 应用：点击选择、射击检测

### 2. 平面计算
- 平面的表示（点+法线）
- 点到平面距离
- 射线与平面相交
- 点在平面上的投影

### 3. 距离计算
- 点到点距离
- 点到线段距离
- 点到平面距离
- 最近点计算

## 🎯 常用API

```csharp
// 射线检测
Ray ray = new Ray(origin, direction);
if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
{
    Debug.Log($"击中: {hit.collider.name}");
    Debug.Log($"位置: {hit.point}");
    Debug.Log($"法线: {hit.normal}");
}

// 平面
Plane plane = new Plane(normal, point);
float distance = plane.GetDistanceToPoint(testPoint);

// 射线与平面相交
if (plane.Raycast(ray, out float enter))
{
    Vector3 hitPoint = ray.GetPoint(enter);
}

// 最近点
Vector3 closest = ClosestPointOnLine(point, lineStart, lineEnd);
```

## 💡 重要概念

- **射线**：起点 + 方向（无限长）
- **线段**：起点 + 终点（有限长）
- **平面**：点 + 法线向量
- **距离**：带符号 vs 绝对值

## 🔗 相关资源
- [Unity Physics.Raycast 文档](https://docs.unity3d.com/ScriptReference/Physics.Raycast.html)
- [Unity Plane 文档](https://docs.unity3d.com/ScriptReference/Plane.html)

---

**注意**：本章节的详细示例代码将在后续添加。
