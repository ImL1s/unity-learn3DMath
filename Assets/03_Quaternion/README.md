# 第3章：四元数 (Quaternion)

四元数是Unity中处理旋转的主要方式，避免了欧拉角的万向锁问题。

## 📝 学习内容

### 1. 四元数基础
- 四元数的表示（x, y, z, w）
- 欧拉角 vs 四元数
- 四元数的优势
- 万向锁问题

### 2. 旋转操作
- 创建旋转：`Quaternion.Euler()`, `Quaternion.AngleAxis()`
- 旋转插值：`Quaternion.Lerp()`, `Quaternion.Slerp()`
- 朝向目标：`Quaternion.LookRotation()`
- 旋转组合：四元数乘法

## 🎯 常用API

```csharp
// 创建旋转
Quaternion rotation = Quaternion.Euler(0, 90, 0);
Quaternion rotation2 = Quaternion.AngleAxis(90, Vector3.up);

// 旋转向量
Vector3 rotated = rotation * Vector3.forward;

// 平滑旋转
transform.rotation = Quaternion.Slerp(current, target, Time.deltaTime * speed);

// 朝向目标
Vector3 direction = target.position - transform.position;
Quaternion lookRotation = Quaternion.LookRotation(direction);
```

## 💡 重要概念

- **单位四元数**：表示旋转的四元数
- **四元数乘法**：组合旋转（注意顺序）
- **Slerp vs Lerp**：球面插值 vs 线性插值

## 🔗 相关资源
- [Unity Quaternion 文档](https://docs.unity3d.com/ScriptReference/Quaternion.html)

---

**注意**：本章节的详细示例代码将在后续添加。建议先完成向量章节的学习。
