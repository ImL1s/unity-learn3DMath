# 快速开始指南

本指南将帮助你在5分钟内开始使用这个Unity 3D数学学习项目。

## 📋 前置要求

- Unity 2017.1.1f1 或更高版本
- 基础的Unity编辑器使用经验

## 🚀 快速开始

### 方法1：运行单元测试（推荐新手）

这是验证项目工作正常的最快方法。

1. **在Unity中打开项目**
   ```
   File > Open Project > 选择 unity-learn3DMath 文件夹
   ```

2. **创建测试场景**
   - 在Project窗口中，右键点击 `Assets/Scenes`
   - 选择 `Create > Scene`
   - 命名为 `TestScene`
   - 双击打开场景

3. **添加测试脚本**
   - 在Hierarchy窗口中，右键选择 `Create Empty`
   - 命名为 "MathTests"
   - 在Inspector窗口中，点击 `Add Component`
   - 搜索并添加 `MathTests` 脚本

4. **运行测试**
   - 点击Unity顶部的播放按钮 ▶
   - 打开Console窗口 (Window > General > Console)
   - 查看测试结果

   你应该看到类似这样的输出：
   ```
   ========== 开始数学测试 ==========
   --- 向量基础测试 ---
   ✓ 向量长度计算正确
   ✓ 向量归一化正确
   ✓ 垂直向量点积为0
   ✓ 叉积方向正确 (右手法则)
   ✓ 向量加法正确

   --- MathHelper测试 ---
   ✓ SafeNormalize处理零向量正确
   ✓ 向量投影计算正确
   ✓ 角度计算正确
   ✓ Rejection计算正确
   ✓ 投影+垂直分量=原向量

   --- 几何计算测试 ---
   ✓ 点到线段最近点 (点在线段上)
   ✓ 点到线段最近点 (点在线段外，应该是端点)
   ✓ 三角形面积计算正确
   ✓ 点到平面距离正确
   ✓ 左侧方向判断正确

   ========== 测试完成: 15/15 通过 ==========
   ✓ 所有测试通过！
   ```

### 方法2：向量基础可视化示例

体验向量的可视化效果。

1. **创建新场景** (如果还没有)
   - File > New Scene

2. **创建示例对象**

   a. 创建控制器：
   - Hierarchy右键 > Create Empty
   - 命名为 "VectorDemo"
   - Add Component > 搜索 "Vector Basics"

   b. 创建点A：
   - Hierarchy右键 > Create Empty
   - 命名为 "PointA"
   - 在Inspector中设置Position: (2, 1, 0)

   c. 创建点B：
   - Hierarchy右键 > Create Empty
   - 命名为 "PointB"
   - 在Inspector中设置Position: (1, 3, 0)

3. **连接引用**
   - 选中 "VectorDemo"
   - 在Inspector的VectorBasics组件中：
     - 将 "PointA" 拖到 `Point A` 字段
     - 将 "PointB" 拖到 `Point B` 字段

4. **查看效果**
   - 确保Scene窗口可见
   - 在VectorDemo的Inspector中：
     - 勾选 `Show Vector A` - 看到红色向量
     - 勾选 `Show Vector B` - 看到蓝色向量
     - 勾选 `Show Addition` - 看到向量加法（绿色）
     - 勾选 `Show Subtraction` - 看到向量减法
     - 勾选 `Show Normalized` - 看到归一化向量（黄色）

5. **交互操作**
   - 在Scene视图中，选择PointA或PointB
   - 使用移动工具(W键)拖动它们
   - 观察向量的实时变化

6. **运行时测试**
   - 点击Play按钮
   - 按空格键，在Console中查看向量运算结果

### 方法3：点积示例（视野检测）

体验实用的游戏开发应用。

1. **创建新场景**

2. **设置观察者**
   - Create Empty，命名为 "Observer"
   - Position: (0, 0, 0)
   - Add Component > "Dot Product Demo"

3. **设置目标**
   - Create Empty，命名为 "Target"
   - Position: (3, 0, 3)

4. **连接引用**
   - 选中Observer
   - 拖动Observer到 `Observer` 字段
   - 拖动Target到 `Target` 字段

5. **查看视野检测**
   - 勾选 `Show FOV`
   - 调整 `Field Of View Angle` (如60度)
   - 在Scene视图中移动Target
   - 观察目标是否在视野内（绿色=在，红色=不在）

6. **实验不同效果**
   - 勾选 `Show Angle` - 查看角度和点积值
   - 勾选 `Show Projection` - 查看投影
   - 旋转Observer，观察视野变化

## 📊 学习路径建议

完成快速开始后，按以下顺序学习：

### 第1周：向量基础
1. ✅ VectorBasics.cs - 向量基本运算
2. ✅ DotProductDemo.cs - 点积应用
3. ✅ CrossProductDemo.cs - 叉积应用
4. ✅ VectorProjectionDemo.cs - 投影计算

阅读 `Assets/01_Vector/README.md` 了解详细理论。

### 第2-8周
参考 `Docs/LearningPath.md` 了解完整的8周学习计划。

## 🎯 常见任务

### 查看所有示例脚本
```
Assets/
├── 01_Vector/Scripts/      ← 从这里开始
├── Common/Scripts/         ← 工具类
└── ...
```

### 运行单元测试
- 播放模式下按 `T` 键
- 或在MathTests组件上右键选择 "运行所有测试"

### 查看文档
- 项目总览: `README.md`
- 学习路径: `Docs/LearningPath.md`
- 向量详解: `Assets/01_Vector/README.md`

### 调试技巧
```csharp
// 在Scene视图中绘制
Debug.DrawLine(start, end, Color.red, 2f);
Debug.DrawRay(origin, direction, Color.blue, 2f);

// 在Console中输出
Debug.Log($"向量: {vector}, 长度: {vector.magnitude}");
```

## ⚠️ 常见问题

### Q: Scene视图中看不到Gizmos？
A: 确保Scene视图右上角的 "Gizmos" 按钮是开启的。

### Q: 脚本编译错误？
A: 确保使用Unity 2017.1.1f1或更高版本。如果仍有问题，删除Library文件夹重新导入。

### Q: 找不到脚本组件？
A: 在Add Component搜索框中输入脚本名称，如 "Vector Basics"。

### Q: Console没有输出？
A: 确保Console窗口是打开的，并且点击了播放按钮。

## 📚 下一步

1. **完成向量章节**
   - 阅读 `Assets/01_Vector/README.md`
   - 尝试每个示例脚本
   - 做练习题

2. **查看学习计划**
   - 阅读 `Docs/LearningPath.md`
   - 按周计划学习

3. **实践项目**
   - 第1章学完后，尝试实现一个简单的敌人视野检测
   - 使用点积判断玩家是否在视野内

## 🆘 获取帮助

- 查看代码注释 - 每个函数都有详细说明
- 阅读README文档 - 每章都有详细文档
- 运行单元测试 - 验证功能正确性
- 查看Unity文档 - [Vector3](https://docs.unity3d.com/ScriptReference/Vector3.html)

---

**准备好了吗？开始你的3D数学学习之旅！** 🚀

有问题欢迎在Issues中提出。
