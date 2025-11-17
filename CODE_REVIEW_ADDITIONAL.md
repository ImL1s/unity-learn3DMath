# 补充代码审查 - 新发现的问题

## 🔴 新发现的问题

### 1. RaycastDemo.cs - LineOfSight可视化缺失 ⚠️

**问题描述**：
虽然添加了LineOfSight的case分支，但DrawRayVisualization()的switch中没有处理LineOfSight，导致该模式下没有可视化。

**位置**：`Assets/05_Geometry/Scripts/RaycastDemo.cs:189-220`

**代码**：
```csharp
switch (mode)
{
    case RaycastMode.Simple:
    case RaycastMode.RaycastAll:
    case RaycastMode.MouseRay:
        DebugDrawer.DrawArrow(ray.origin, rayEnd, 0.3f);
        break;
    // ... 其他模式
    // ❌ 缺少 LineOfSight 的 case
}
```

**影响**：LineOfSight模式下射线不可见

**修复**：在switch中添加LineOfSight的case，或将其与Simple模式合并

---

### 2. ProjectileMotion.cs - 过时的警告信息 ⚠️

**问题描述**：
OnGUI中仍然显示"警告: 未设置发射物预制体！"，但现在已支持自动创建，这个警告已过时。

**位置**：`Assets/06_Applications/Scripts/ProjectileMotion.cs:489-494`

**代码**：
```csharp
if (projectilePrefab == null)
{
    GUI.color = Color.red;
    GUILayout.Label("\n警告: 未设置发射物预制体！");  // ❌ 过时的警告
    GUI.color = Color.white;
}
```

**影响**：
- 用户看到红色警告会误以为有错误
- 实际上系统会自动创建球体，没有问题

**建议修复**：
改为友好的提示信息：
```csharp
if (projectilePrefab == null)
{
    GUI.color = Color.yellow;
    GUILayout.Label("\n提示: 将自动创建黄色球体抛射物");
    GUI.color = Color.white;
}
```

---

### 3. LineOfSight实现不够完善 ⚠️

**问题描述**：
当前LineOfSight实现与Simple模式完全相同，没有体现"视线检测"的特殊性。

**建议增强**：
- 添加目标Transform字段
- 检测到目标的视线是否被遮挡
- 显示"视线畅通/视线被遮挡"状态

---

### 4. RaycastDemo缺少目标对象 ⚠️

**问题描述**：
LineOfSight模式需要一个目标对象来进行视线检测，但当前没有这个字段。

**建议添加**：
```csharp
[Header("视线检测目标")]
public Transform lineOfSightTarget;
```

---

## 📊 问题优先级

**P0 必须修复（影响功能）**：
1. ✅ RaycastDemo.cs - 添加LineOfSight可视化
2. ✅ ProjectileMotion.cs - 更新过时警告信息

**P1 建议修复（增强功能）**：
3. ✅ RaycastDemo.cs - 增强LineOfSight实现
4. ✅ RaycastDemo.cs - 添加目标对象字段

---

## ✅ 修复计划

1. 修复LineOfSight可视化缺失
2. 更新ProjectileMotion的警告信息
3. 为LineOfSight添加目标Transform字段
4. 增强LineOfSight逻辑（检测视线是否被遮挡）
5. 更新CHANGELOG.md记录这些修复
