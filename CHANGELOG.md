# 更新日志

所有重要的项目更改都会记录在此文件中。

## [v1.2.0] - 2025-11-17

### ✨ 新增内容

#### 第2章：矩阵 (Matrix) - 全新
- **MatrixBasics.cs** - 矩阵基础操作
  - 单位矩阵、平移矩阵、旋转矩阵、缩放矩阵
  - 矩阵乘法和运算顺序演示
  - 逆矩阵、转置矩阵
  - TRS组合变换
  - 矩阵可视化（基向量）
  - 矩阵属性展示（行列式、合法性检查）

- **TransformMatrix.cs** - 自定义变换矩阵
  - 切变变换（Shear）
  - 镜像变换（Mirror）
  - 投影变换（Projection）
  - 非均匀缩放
  - 组合自定义变换
  - 网格变形可视化
  - 行列式意义解析

#### 第4章：坐标变换 (Transform) - 补充
- **ParentChildHierarchy.cs** - 父子层级关系深入
  - 祖父-父-子三级层级演示
  - 世界坐标与本地坐标对比
  - 层级深度计算
  - SetParent() 动态附加/分离
  - lossyScale 世界缩放演示
  - WASD/QE 手动控制
  - 相对变换计算

#### 第5章：几何计算 (Geometry) - 全新
- **RaycastDemo.cs** - 射线检测全家桶
  - 6种检测模式：Raycast、SphereCast、BoxCast、CapsuleCast、RaycastAll、LineOfSight
  - 鼠标射线交互
  - 可视化射线路径（球形、盒形、胶囊形）
  - Hit信息详细展示（点、法线、距离、碰撞体）
  - 多物体检测演示

- **DistanceCalculation.cs** - 距离计算算法
  - 点到点距离
  - 点到线段距离（最近点算法）
  - 点到平面距离（有向距离）
  - 点到包围盒距离
  - 线段到线段距离（3D空间）
  - 完整可视化和数学公式展示

#### 第6章：综合应用 (Applications) - 补充
- **ProjectileMotion.cs** - 抛物线运动物理
  - 4种发射模式：固定角度、瞄准目标（低弧）、瞄准目标（高弧）、最大射程
  - 抛物线轨迹计算和可视化
  - 落点预测
  - 飞行时间计算
  - 最高点计算
  - 速度向量演示
  - 自定义重力系统
  - 实际应用：炮弹、投掷物、弓箭

### 📊 统计
- **新增脚本**: 7个
- **新增代码行数**: ~2800行
- **完成章节**: 全部6章完整覆盖
- **总脚本数**: 18个示例脚本 + 3个工具脚本

### 📚 项目完成度
- ✅ 第1章：向量 (4个脚本)
- ✅ 第2章：矩阵 (2个脚本)
- ✅ 第3章：四元数 (2个脚本)
- ✅ 第4章：坐标变换 (2个脚本)
- ✅ 第5章：几何计算 (2个脚本)
- ✅ 第6章：综合应用 (3个脚本)
- ✅ 公共工具 (3个脚本)

---

## [v1.1.0] - 2025-11-16

### ✨ 新增内容

#### 第3章：四元数 (Quaternion)
- **QuaternionBasics.cs** - 四元数基础
  - 四元数与欧拉角对比
  - AngleAxis 旋转方式
  - 万向锁(Gimbal Lock)演示
  - 四元数可视化

- **QuaternionRotation.cs** - 四元数旋转和插值
  - Lerp vs Slerp 对比
  - LookRotation 朝向目标
  - RotateTowards 固定角速度旋转
  - 旋转路径可视化

#### 第4章：坐标变换 (Transform)
- **CoordinateTransform.cs** - 坐标系统转换
  - 世界坐标 ↔ 本地坐标转换
  - TransformPoint vs TransformDirection 对比
  - 屏幕坐标 ↔ 世界坐标转换
  - 父子层级关系演示
  - 鼠标交互示例

#### 第6章：综合应用 (Applications)
- **LookAtTarget.cs** - 物体朝向目标
  - 多种朝向模式（instant, smooth, rotate towards）
  - 旋转约束（Y轴、X轴、Z轴）
  - 目标预测功能
  - 高度偏移和目标偏移
  - 实际应用：炮塔、敌人AI

- **FollowCamera.cs** - 第三人称跟随相机
  - 平滑跟随系统
  - 鼠标视角控制
  - 碰撞检测和避让
  - 可配置的距离和高度
  - 实际应用：第三人称游戏

### 📊 统计
- **新增脚本**: 5个
- **新增代码行数**: ~1200行
- **覆盖章节**: 第3、4、6章

---

## [v1.0.0] - 2025-11-16

### ✨ 初始发布

#### 第1章：向量 (Vector)
- VectorBasics.cs - 向量基础运算
- DotProductDemo.cs - 点积应用
- CrossProductDemo.cs - 叉积应用
- VectorProjectionDemo.cs - 向量投影

#### 公共工具
- DebugDrawer.cs - 调试绘制工具
- MathHelper.cs - 数学辅助函数
- MathTests.cs - 单元测试

#### 文档
- README.md - 项目说明
- QUICKSTART.md - 快速开始指南
- CODE_REVIEW.md - 代码审查报告
- DETAILED_REVIEW.md - 深度审查报告
- Docs/LearningPath.md - 8周学习计划

### 🐛 修复
- 修复DotProductDemo中Acos参数域错误
- 修复MathHelper中除零风险
- 优化距离重复计算

### ✅ 测试
- 15个单元测试全部通过
- 代码质量评分: 5.0/5

---

## 版本说明

### 版本号格式
- 主版本号.次版本号.修订号
- 主版本号：重大更新
- 次版本号：新功能添加
- 修订号：bug修复

### 符号说明
- ✨ 新增功能
- 🐛 Bug修复
- 📈 性能优化
- 📚 文档更新
- ♻️ 代码重构
- ✅ 测试相关
