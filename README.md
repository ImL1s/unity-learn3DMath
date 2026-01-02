# Unity 3D 數學學習項目 | Unity 3D Math Learning

<div align="center">

**透過視覺化範例與自動化實作，掌握遊戲開發中的 3D 數學核心**

[![Unity](https://img.shields.io/badge/Unity-2017.1+-black.svg)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23-blue.svg)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Version](https://img.shields.io/badge/Version-v1.3.0-orange.svg)](CHANGELOG.md)
[![Tests](https://img.shields.io/badge/Tests-15/15%20Passed-green.svg)](CODE_REVIEW.md)

</div>

---

## 🎯 项目目标

通过实际的Unity场景和代码示例，掌握以下3D数学核心知识：
- 向量运算及其应用
- 矩阵变换原理
- 四元数旋转
- 坐标系统转换
- 几何计算方法
- 实际游戏开发应用

## ✅ 项目完成状态

**当前版本**: v1.3.0 | **完成度**: 100% | **总脚本数**: 21个 | **✨ 100%自动初始化**

- ✅ **第1章：向量** (4个脚本) - 向量运算、点积、叉积、投影
- ✅ **第2章：矩阵** (2个脚本) - 矩阵基础、自定义变换
- ✅ **第3章：四元数** (2个脚本) - 四元数基础、旋转插值
- ✅ **第4章：坐标变换** (2个脚本) - 坐标转换、父子层级
- ✅ **第5章：几何计算** (2个脚本) - 射线检测、距离计算
- ✅ **第6章：综合应用** (3个脚本) - 朝向、跟随相机、抛物线
- ✅ **公共工具** (3 個腳本) - 偵錯繪製、數學輔助、單元測試
- ✅ **代碼質量** - 5.0/5.0 (15/15 單元測試通過)

## 🛡️ 穩定性與自動化 | Guaranteed Stability
本專案與一般教學範例不同，強調 **「即插即用」**：
- ✨ **100% 自動初始化**：將腳本掛載到 GameObject 後，所有輔助物件與 Gizmos 都會自動生成，無需手動配置場景。
- ✅ **單元測試覆蓋**：所有數學核心邏輯均通過內置的 `MathTests` 驗證，確保計算精準無誤。

## 🎨 視覺化學習 | Visual Learning Guide
為了獲得最佳學習效果，建議：
1. **進入專用場景**：專案中的 `Assets/Scenes/` 包含多個可互動場景。
2. **切換 Gizmos**：確保 Unity 編輯器的 Gizmos 開關已打開，以便看到向量、射線與矩陣的實時繪製。
3. **場景視角控制**：大部分範例在 Scene 視角下觀察效果更佳。


## 📚 学习路径

### 第1章：向量 (Vector) ✅
**位置**: `Assets/01_Vector/`

向量是3D数学的基础，理解向量运算是理解所有后续内容的关键。

#### 学习内容：
1. **向量基础** - `VectorBasics.unity`
   - 向量的表示和可视化
   - 向量加法、减法
   - 向量缩放和归一化
   - 向量长度（模）计算

2. **点积 (Dot Product)** - `DotProduct.unity`
   - 点积的计算和几何意义
   - 判断两向量的夹角
   - 判断方向（前/后/左/右）
   - 实际应用：视野检测

3. **叉积 (Cross Product)** - `CrossProduct.unity`
   - 叉积的计算方法
   - 获得垂直向量
   - 计算三角形面积
   - 判断左右方向

4. **向量投影** - `VectorProjection.unity`
   - 向量投影计算
   - 最近点计算
   - 实际应用：光照计算

### 第2章：矩阵 (Matrix) ✅
**位置**: `Assets/02_Matrix/`

矩阵是变换的数学基础，理解矩阵有助于深入理解Unity的变换系统。

#### 学习内容：
1. **矩阵基础** - `MatrixBasics.cs`
   - 单位矩阵、平移矩阵、旋转矩阵、缩放矩阵
   - 矩阵乘法和运算顺序
   - 逆矩阵、转置矩阵
   - TRS组合变换
   - 矩阵可视化（基向量）
   - 实际应用：MultiplyPoint3x4 vs MultiplyVector

2. **自定义变换矩阵** - `TransformMatrix.cs`
   - 切变变换（Shear）
   - 镜像变换（Mirror）
   - 投影变换（Projection）
   - 非均匀缩放
   - 组合自定义变换
   - 行列式意义解析
   - 实际应用：特殊视觉效果

### 第3章：四元数 (Quaternion) ✅
**位置**: `Assets/03_Quaternion/`

四元数是Unity中处理旋转的主要方式，避免了欧拉角的万向锁问题。

#### 学习内容：
1. **四元数基础** - `QuaternionBasics.unity`
   - 四元数的表示
   - 欧拉角 vs 四元数
   - 四元数的优势

2. **旋转插值** - `QuaternionRotation.unity`
   - Lerp vs Slerp
   - 平滑旋转
   - 朝向目标

### 第4章：坐标变换 (Transform) ✅
**位置**: `Assets/04_Transform/`

理解不同坐标系统及其转换是3D编程的核心技能。

#### 学习内容：
1. **坐标系统转换** - `CoordinateTransform.cs`
   - 世界坐标 ↔ 本地坐标转换
   - TransformPoint vs TransformDirection vs TransformVector
   - 屏幕坐标 ↔ 世界坐标转换
   - 鼠标交互示例
   - 实际应用：坐标系统切换

2. **父子层级关系** - `ParentChildHierarchy.cs`
   - 祖父-父-子三级层级演示
   - 世界坐标与本地坐标对比
   - 层级深度计算
   - SetParent() 动态附加/分离
   - lossyScale 世界缩放演示
   - WASD/QE 手动控制
   - 实际应用：骨骼动画、武器挂载

### 第5章：几何计算 (Geometry) ✅
**位置**: `Assets/05_Geometry/`

几何计算在碰撞检测、AI寻路等方面有广泛应用。

#### 学习内容：
1. **射线检测全家桶** - `RaycastDemo.cs`
   - 6种检测模式：Raycast、SphereCast、BoxCast、CapsuleCast、RaycastAll、LineOfSight
   - 鼠标射线交互
   - 可视化射线路径（球形、盒形、胶囊形）
   - Hit信息详细展示（点、法线、距离、碰撞体）
   - 多物体检测演示
   - 实际应用：射击检测、点击选择、视线判断

2. **距离计算算法** - `DistanceCalculation.cs`
   - 点到点距离
   - 点到线段距离（最近点算法）
   - 点到平面距离（有向距离）
   - 点到包围盒距离
   - 线段到线段距离（3D空间）
   - 完整可视化和数学公式展示
   - 实际应用：AI寻路、碰撞检测、范围判断

### 第6章：综合应用 (Applications) ✅
**位置**: `Assets/06_Applications/`

将前面学到的知识应用到实际游戏开发场景中。

#### 实际应用：
1. **物体朝向目标** - `LookAtTarget.cs`
   - 4种朝向模式（instant, smooth, rotate towards, custom lerp）
   - 旋转约束（Y轴、X轴、Z轴）
   - 目标预测功能
   - 高度偏移和目标偏移
   - 实际应用：炮塔、敌人AI、NPC视线

2. **第三人称跟随相机** - `FollowCamera.cs`
   - 平滑跟随系统
   - 鼠标视角控制
   - 碰撞检测和避让（SphereCast）
   - 可配置的距离和高度
   - 实际应用：第三人称游戏、跟随镜头

3. **抛物线运动物理** - `ProjectileMotion.cs`
   - 4种发射模式：固定角度、瞄准目标（低弧）、瞄准目标（高弧）、最大射程
   - 抛物线轨迹计算和可视化
   - 落点预测、飞行时间计算
   - 最高点计算、速度向量演示
   - 自定义重力系统
   - 实际应用：炮弹、投掷物、弓箭、篮球

## 🗂️ 项目结构

```
unity-learn3DMath/
├── Assets/
│   ├── 01_Vector/           # 向量相关示例
│   ├── 02_Matrix/           # 矩阵相关示例
│   ├── 03_Quaternion/       # 四元数相关示例
│   ├── 04_Transform/        # 坐标变换相关示例
│   ├── 05_Geometry/         # 几何计算相关示例
│   ├── 06_Applications/     # 综合应用示例
│   ├── Common/              # 公共工具和材质
│   └── Scenes/              # 主菜单场景
├── Docs/                    # 详细文档
│   ├── 01_Vector.md
│   ├── 02_Matrix.md
│   ├── 03_Quaternion.md
│   ├── 04_Transform.md
│   ├── 05_Geometry.md
│   └── LearningPath.md
└── README.md
```

## 🚀 使用方法

### 环境要求
- Unity 2017.1.1f1 或更高版本
- 基础的C#编程知识

### 学习建议
1. **按顺序学习**：建议按照章节顺序学习，因为后面的内容会用到前面的知识
2. **动手实践**：每个示例都可以在Scene视图中实时查看效果
3. **修改参数**：尝试修改脚本中的参数，观察变化
4. **阅读代码**：每个脚本都有详细注释，理解实现原理
5. **查看文档**：`Docs/` 目录下有每个主题的详细理论说明

### 快速开始

#### 方法1：一键运行示例（最简单！✨ v1.3.0新功能）
体验向量的可视化效果，无需任何手动配置：
1. 用Unity打开项目
2. 创建新场景，添加空GameObject
3. 添加任意示例脚本（如 `VectorBasics`）
4. 点击Play - **所有对象自动创建！** 🎉

**所有21个脚本都支持自动初始化**，创建场景后立即可见效果！

#### 方法2：运行单元测试
验证项目功能正常：
1. 用Unity打开项目
2. 创建新场景，添加空GameObject
3. 添加 `MathTests` 组件
4. 点击Play，查看Console输出（应该看到15/15测试通过）

详细步骤请查看 → [QUICKSTART.md](QUICKSTART.md)

#### 方法3：查看文档学习
- **快速上手**: [QUICKSTART.md](QUICKSTART.md) - 5分钟快速开始
- **代码审查**: [CODE_REVIEW.md](CODE_REVIEW.md) - 代码质量报告
- **学习路径**: [Docs/LearningPath.md](Docs/LearningPath.md) - 8周完整学习计划

## 📖 学习资源

### 推荐阅读
- 《3D数学基础：图形与游戏开发》
- Unity官方文档：Vector3、Quaternion、Transform
- 《Unity Shader入门精要》（涉及矩阵变换）

### 在线资源
- [Unity Manual - Vector3](https://docs.unity3d.com/ScriptReference/Vector3.html)
- [Unity Manual - Quaternion](https://docs.unity3d.com/ScriptReference/Quaternion.html)
- [Understanding Vector Math](https://docs.unity3d.com/Manual/UnderstandingVectorArithmetic.html)

## 🎓 预期学习成果

完成本项目的学习后，你将能够：
- ✅ 理解和使用向量进行方向、距离、角度计算
- ✅ 运用点积和叉积解决实际问题
- ✅ 理解矩阵变换的原理
- ✅ 正确使用四元数处理旋转
- ✅ 熟练进行坐标系统转换
- ✅ 运用几何计算解决碰撞、检测等问题
- ✅ 将数学知识应用到实际游戏开发中

## 📝 笔记和练习

建议在学习过程中：
1. 记录关键公式和概念
2. 尝试自己实现类似的示例
3. 思考如何将这些知识应用到你的项目中
4. 对比Unity内置函数和手动实现的区别

## 🤝 贡献

欢迎提交Issues和Pull Requests来改进这个学习项目！

## 📄 许可

本项目仅用于学习目的。

---

**开始你的3D数学学习之旅吧！** 🚀
