# 更新日志

所有重要的项目更改都会记录在此文件中。

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
