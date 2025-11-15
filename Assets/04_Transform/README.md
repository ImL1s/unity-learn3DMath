# 第4章：坐标变换 (Transform)

理解不同坐标系统及其转换是3D编程的核心技能。

## 📝 学习内容

### 1. 坐标系统
- **世界坐标系**（World Space）：全局坐标系统
- **本地坐标系**（Local Space）：相对于父对象的坐标
- **屏幕坐标系**（Screen Space）：像素坐标
- **视口坐标系**（Viewport Space）：归一化的屏幕坐标(0-1)

### 2. 坐标转换
- 世界坐标 ↔ 本地坐标
- 世界坐标 ↔ 屏幕坐标
- 不同空间的转换方法

### 3. 父子关系
- Transform层级结构
- 父子坐标关系
- 相对变换

## 🎯 常用API

```csharp
// 本地 → 世界
Vector3 worldPos = transform.TransformPoint(localPos);
Vector3 worldDir = transform.TransformDirection(localDir);

// 世界 → 本地
Vector3 localPos = transform.InverseTransformPoint(worldPos);
Vector3 localDir = transform.InverseTransformDirection(worldDir);

// 屏幕 → 世界（需要相机）
Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

// 世界 → 屏幕
Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
```

## 💡 重要区别

### TransformPoint vs TransformDirection
- **TransformPoint**：转换位置（受平移、旋转、缩放影响）
- **TransformDirection**：转换方向（仅受旋转和缩放影响，不受平移影响）
- **TransformVector**：转换向量（受旋转和缩放影响）

## 🔗 相关资源
- [Unity Transform 文档](https://docs.unity3d.com/ScriptReference/Transform.html)
- [Unity Camera 文档](https://docs.unity3d.com/ScriptReference/Camera.html)

---

**注意**：本章节的详细示例代码将在后续添加。
