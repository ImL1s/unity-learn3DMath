# Unity 3D数学学习项目

这是一个系统学习Unity 3D数学的实践项目，通过可视化示例帮助理解游戏开发中的核心数学概念。

## 🎯 项目目标

通过实际的Unity场景和代码示例，掌握以下3D数学核心知识：
- 向量运算及其应用
- 矩阵变换原理
- 四元数旋转
- 坐标系统转换
- 几何计算方法
- 实际游戏开发应用

## 📚 学习路径

### 第1章：向量 (Vector)
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

### 第2章：矩阵 (Matrix)
**位置**: `Assets/02_Matrix/`

矩阵是变换的数学基础，理解矩阵有助于深入理解Unity的变换系统。

#### 学习内容：
1. **矩阵基础** - `MatrixBasics.unity`
   - 矩阵的表示
   - 矩阵乘法
   - 单位矩阵

2. **变换矩阵** - `TransformMatrix.unity`
   - 平移矩阵
   - 旋转矩阵
   - 缩放矩阵
   - 组合变换

### 第3章：四元数 (Quaternion)
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

### 第4章：坐标变换 (Transform)
**位置**: `Assets/04_Transform/`

理解不同坐标系统及其转换是3D编程的核心技能。

#### 学习内容：
1. **坐标系统** - `CoordinateSystem.unity`
   - 世界坐标系
   - 本地坐标系
   - 屏幕坐标系
   - 视口坐标系

2. **本地 vs 世界** - `LocalVsWorld.unity`
   - 本地坐标和世界坐标的区别
   - 坐标转换方法
   - TransformPoint vs TransformDirection

3. **父子关系** - `ParentChild.unity`
   - 父子层级对坐标的影响
   - 相对变换
   - 实际应用场景

### 第5章：几何计算 (Geometry)
**位置**: `Assets/05_Geometry/`

几何计算在碰撞检测、AI寻路等方面有广泛应用。

#### 学习内容：
1. **射线检测** - `RaycastDemo.unity`
   - Ray的构建
   - Raycast的使用
   - 应用：点击选择、射击

2. **平面相交** - `PlaneIntersection.unity`
   - 平面的表示
   - 射线与平面相交
   - 点到平面距离

3. **距离计算** - `DistanceCalculation.unity`
   - 点到点距离
   - 点到线距离
   - 点到平面距离
   - 最近点计算

### 第6章：综合应用 (Applications)
**位置**: `Assets/06_Applications/`

将前面学到的知识应用到实际游戏开发场景中。

#### 实际应用：
1. **物体朝向** - `LookAtTarget.unity`
   - 平滑朝向目标
   - 约束旋转轴
   - 炮塔、敌人AI应用

2. **跟随相机** - `FollowCamera.unity`
   - 第三人称相机
   - 平滑跟随
   - 碰撞检测

3. **抛物线运动** - `ProjectileMotion.unity`
   - 物理抛射
   - 预测轨迹
   - 炮弹、投掷物应用

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

#### 方法1：运行单元测试（推荐）
验证项目功能正常：
1. 用Unity打开项目
2. 创建新场景，添加空GameObject
3. 添加 `MathTests` 组件
4. 点击Play，查看Console输出（应该看到15/15测试通过）

详细步骤请查看 → [QUICKSTART.md](QUICKSTART.md)

#### 方法2：可视化示例
体验向量的可视化效果：
1. 创建空GameObject，添加 `VectorBasics` 组件
2. 创建两个空GameObject作为PointA和PointB
3. 连接引用，在Scene视图中观察向量可视化
4. 移动点A和点B，观察实时变化

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
