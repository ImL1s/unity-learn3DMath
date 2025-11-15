# 第6章：综合应用 (Applications)

将前面学到的知识应用到实际游戏开发场景中。

## 📝 实际应用

### 1. 物体朝向目标 (Look At)
- 平滑朝向
- 约束旋转轴
- 应用：炮塔、敌人AI

```csharp
// 立即朝向
transform.LookAt(target);

// 平滑朝向
Vector3 direction = target.position - transform.position;
Quaternion targetRotation = Quaternion.LookRotation(direction);
transform.rotation = Quaternion.Slerp(
    transform.rotation,
    targetRotation,
    Time.deltaTime * rotationSpeed
);
```

### 2. 跟随相机
- 第三人称相机
- 平滑跟随
- 碰撞检测
- 视角控制

```csharp
// 基础跟随
Vector3 targetPos = player.position + offset;
transform.position = Vector3.Lerp(
    transform.position,
    targetPos,
    Time.deltaTime * followSpeed
);

// 始终看向玩家
transform.LookAt(player.position + Vector3.up * lookAtHeight);
```

### 3. 抛物线运动
- 物理抛射
- 轨迹预测
- 炮弹、投掷物

```csharp
// 抛物线运动
Vector3 velocity = initialVelocity;
velocity += Physics.gravity * Time.deltaTime;
transform.position += velocity * Time.deltaTime;

// 计算抛射初速度（击中目标）
Vector3 CalculateLaunchVelocity(Vector3 target, float angle)
{
    // ... 使用物理公式计算
}
```

## 💡 综合技术

这些应用通常会用到：
- 向量运算（方向、距离）
- 四元数旋转（平滑朝向）
- 坐标转换（世界/本地坐标）
- 射线检测（碰撞、地面检测）
- 投影计算（光照、阴影）

## 🎯 学习目标

完成本章后，你应该能够：
- [ ] 实现第三人称相机系统
- [ ] 创建自动瞄准的炮塔
- [ ] 实现抛物线投掷
- [ ] 组合使用多种数学技术

## 🔗 相关资源
- [Unity 第三人称相机教程](https://learn.unity.com/)
- [Unity 物理系统文档](https://docs.unity3d.com/ScriptReference/Physics.html)

---

**注意**：本章节的详细示例代码将在后续添加。建议先完成前面所有章节的学习。
