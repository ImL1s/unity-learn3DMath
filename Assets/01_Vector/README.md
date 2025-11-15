# 第1章：向量 (Vector)

向量是3D数学的基础，掌握向量运算对理解后续所有内容至关重要。

## 什么是向量？

向量是既有**大小（长度）**又有**方向**的量。在Unity中，向量主要用Vector3表示，包含x、y、z三个分量。

```csharp
Vector3 vec = new Vector3(1, 2, 3);  // x=1, y=2, z=3
```

## 📝 学习内容

### 1. 向量基础 (VectorBasics.cs)

#### 核心概念：
- **向量表示**：位置、方向、速度都可以用向量表示
- **向量长度**：`vector.magnitude` 或 `Vector3.Distance(a, b)`
- **向量加法**：两个向量首尾相接
- **向量减法**：`B - A` 得到从A指向B的向量
- **向量归一化**：`vector.normalized` 得到方向相同但长度为1的单位向量
- **向量缩放**：`vector * scalar` 改变长度但保持方向

#### 重要公式：
```csharp
// 长度（模）
float length = Mathf.Sqrt(x*x + y*y + z*z);
// 或使用
float length = vector.magnitude;

// 归一化
Vector3 normalized = vector / vector.magnitude;
// 或使用
Vector3 normalized = vector.normalized;

// 距离
float distance = (pointB - pointA).magnitude;
// 或使用
float distance = Vector3.Distance(pointA, pointB);
```

#### 使用场景：
- 计算两点间距离
- 获取两点间的方向
- 角色移动方向控制
- 速度和加速度计算

---

### 2. 点积 (DotProductDemo.cs)

#### 核心概念：
点积（Dot Product）是两个向量的一种运算，结果是一个**标量（数值）**。

**几何意义**：
- 点积 = |A| × |B| × cos(θ)
- 其中θ是两向量的夹角

**重要性质**：
- 当两向量垂直时，点积 = 0
- 当两向量同向时，点积 > 0（最大值：两向量长度相乘）
- 当两向量反向时，点积 < 0（最小值：负的两向量长度相乘）

#### 重要公式：
```csharp
// 点积计算
float dot = Vector3.Dot(vectorA, vectorB);

// 如果A和B都是单位向量
float dot = Vector3.Dot(vectorA.normalized, vectorB.normalized);
// 则 dot = cos(θ)

// 计算夹角
float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
```

#### 使用场景：

**1. 判断方向**
```csharp
Vector3 forward = transform.forward;
Vector3 toTarget = (target.position - transform.position).normalized;
float dot = Vector3.Dot(forward, toTarget);

if (dot > 0.5f)
    Debug.Log("目标在前方");
else if (dot < -0.5f)
    Debug.Log("目标在后方");
else
    Debug.Log("目标在侧面");
```

**2. 视野检测（FOV）**
```csharp
float dot = Vector3.Dot(forward, toTarget);
float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

if (angle < fieldOfViewAngle / 2f)
    Debug.Log("目标在视野内");
```

**3. 计算投影长度**
```csharp
// 向量A在向量B方向上的投影长度
float projectionLength = Vector3.Dot(vectorA, vectorB.normalized);
```

---

### 3. 叉积 (CrossProductDemo.cs)

#### 核心概念：
叉积（Cross Product）的结果是一个**向量**，该向量垂直于原来的两个向量。

**几何意义**：
- 叉积的方向：遵循右手法则
- 叉积的长度：|A × B| = |A| × |B| × sin(θ)
- 叉积的长度 = 两向量构成的平行四边形面积

**重要性质**：
- A × B = -(B × A)  （不满足交换律）
- 如果A和B平行，叉积 = 零向量
- 如果A和B垂直，叉积长度 = |A| × |B|

#### 重要公式：
```csharp
// 叉积计算
Vector3 cross = Vector3.Cross(vectorA, vectorB);

// 平行四边形面积
float area = cross.magnitude;

// 三角形面积
float triangleArea = cross.magnitude / 2f;
```

#### 使用场景：

**1. 计算法线**
```csharp
// 已知三角形三个顶点
Vector3 edge1 = vertex2 - vertex1;
Vector3 edge2 = vertex3 - vertex1;
Vector3 normal = Vector3.Cross(edge1, edge2).normalized;
```

**2. 判断左右方向**
```csharp
Vector3 forward = transform.forward;
Vector3 toTarget = target.position - transform.position;
Vector3 cross = Vector3.Cross(forward, toTarget);

if (cross.y > 0)
    Debug.Log("目标在左侧");
else if (cross.y < 0)
    Debug.Log("目标在右侧");
```

**3. 计算面积**
```csharp
// 三角形面积
float area = Vector3.Cross(edge1, edge2).magnitude / 2f;
```

**4. 构建坐标系**
```csharp
Vector3 forward = Vector3.forward;
Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
Vector3 up = Vector3.Cross(forward, right).normalized;
```

---

### 4. 向量投影 (VectorProjectionDemo.cs)

#### 核心概念：
投影是将一个向量"投射"到另一个向量方向上，得到在该方向上的分量。

**分解**：
任何向量A可以分解为：
- **平行分量（投影）**：沿着某个方向的分量
- **垂直分量（rejection）**：垂直于该方向的分量

#### 重要公式：
```csharp
// 向量A在向量B方向上的投影
Vector3 projectionB = Vector3.Dot(vectorA, vectorB.normalized) * vectorB.normalized;

// 或者
float projLength = Vector3.Dot(vectorA, vectorB.normalized);
Vector3 projection = vectorB.normalized * projLength;

// 垂直分量
Vector3 rejection = vectorA - projection;
```

#### 使用场景：

**1. 点到线的最近点**
```csharp
Vector3 lineStart, lineEnd, point;
Vector3 lineDir = (lineEnd - lineStart).normalized;
Vector3 startToPoint = point - lineStart;

float projLength = Vector3.Dot(startToPoint, lineDir);
Vector3 closestPoint = lineStart + lineDir * projLength;
```

**2. 光照计算（兰伯特）**
```csharp
Vector3 surfaceNormal = GetSurfaceNormal();
Vector3 lightDirection = GetLightDirection();

// 光照强度 = 法线在光线方向上的投影（取正值）
float intensity = Mathf.Max(0, Vector3.Dot(surfaceNormal, lightDirection));
```

**3. 斜坡上的速度分解**
```csharp
Vector3 velocity = GetVelocity();
Vector3 slopeNormal = GetSlopeNormal();
Vector3 slopeRight = Vector3.Cross(Vector3.up, slopeNormal).normalized;

// 分解为沿斜坡和垂直斜坡的分量
Vector3 velocityAlongSlope = Vector3.Dot(velocity, slopeRight) * slopeRight;
Vector3 velocityPerpSlope = velocity - velocityAlongSlope;
```

**4. 点到线的距离**
```csharp
Vector3 rejection = pointToLine - projection;
float distance = rejection.magnitude;
```

---

## 🎯 点积 vs 叉积对比

| 特性 | 点积 (Dot Product) | 叉积 (Cross Product) |
|------|-------------------|---------------------|
| **结果类型** | 标量（float） | 向量（Vector3） |
| **几何意义** | 投影长度 × 长度 | 垂直向量，长度=面积 |
| **计算公式** | A·B = Ax×Bx + Ay×By + Az×Bz | 见右手法则 |
| **交换律** | A·B = B·A | A×B = -(B×A) |
| **主要用途** | 角度、方向判断、投影 | 法线、左右判断、面积 |
| **垂直时** | = 0 | 长度最大 |
| **平行时** | = ±\|A\|×\|B\| | = 零向量 |

---

## 💡 实践建议

### 学习顺序：
1. 先理解向量基础，能够可视化向量的加减和归一化
2. 掌握点积，重点理解与角度的关系
3. 学习叉积，理解右手法则和垂直性
4. 最后学习投影，它综合运用了点积的概念

### 调试技巧：
- 使用 `Debug.DrawLine()` 和 `Gizmos.DrawLine()` 可视化向量
- 使用 `Debug.Log()` 输出数值，验证计算结果
- 在Scene视图中移动Transform，观察向量的实时变化

### 常见陷阱：
1. **忘记归一化**：计算方向时要用 `.normalized`
2. **混淆点积和叉积**：记住点积是标量，叉积是向量
3. **忘记检查零向量**：归一化或计算角度前要检查 `magnitude > 0`
4. **角度单位**：`Mathf.Acos` 返回弧度，需要 `* Mathf.Rad2Deg` 转换为角度

---

## 📚 扩展阅读

### Unity API文档：
- [Vector3](https://docs.unity3d.com/ScriptReference/Vector3.html)
- [Vector3.Dot](https://docs.unity3d.com/ScriptReference/Vector3.Dot.html)
- [Vector3.Cross](https://docs.unity3d.com/ScriptReference/Vector3.Cross.html)

### 数学原理：
- 线性代数基础
- 向量空间
- 内积和外积

---

## ✅ 检查清单

完成本章学习后，你应该能够：
- [ ] 理解向量的基本概念和表示方法
- [ ] 熟练使用向量加减、归一化、缩放
- [ ] 使用点积判断方向和计算夹角
- [ ] 使用点积实现视野检测（FOV）
- [ ] 使用叉积判断左右方向
- [ ] 使用叉积计算法线和面积
- [ ] 理解向量投影的概念和计算
- [ ] 计算点到线的最近点和距离
- [ ] 在实际项目中应用这些向量运算

---

**继续下一章：矩阵 (Matrix)** →
