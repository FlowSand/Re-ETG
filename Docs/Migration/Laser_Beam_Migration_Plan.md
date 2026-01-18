# 激光弹幕系统迁移计划

## 概述

本计划旨在将ETG项目中的激光弹幕（Beam/Laser）系统迁移到新的Unity工程。该系统是一个复杂的、约2500行核心代码的弹幕子系统，具有完整的物理碰撞、渲染、AI控制和特效支持。

**核心价值：**
- 成熟的激光束渲染系统（直线、曲线、闪电链）
- 完整的碰撞检测与伤害计算
- 灵活的AI行为控制系统
- 丰富的视觉特效支持

---

## 系统架构概览

### 三层架构

```
Layer 1: 核心抽象层
├── BeamController (抽象基类)
│   ├── 属性：Owner, Gun, Origin, Direction
│   ├── 碰撞忽略列表管理
│   └── 抽象方法：LateUpdatePosition, CeaseAttack, DestroyBeam

Layer 2: 具体实现层
├── BasicBeamController (标准激光束)
│   ├── 状态机：Charging → Telegraphing → Firing → Dissipating
│   ├── 骨骼系统：LinkedList<BeamBone>
│   ├── 碰撞检测：基于Raycast
│   └── 渲染：tk2dTiledSprite + 自定义几何
├── RaidenBeamController (闪电链)
│   └── 贝塞尔曲线多目标链接
└── ReverseBeamController (反射激光)

Layer 3: 集成层
├── AIBeamShooter (AI发射控制器)
├── ShootBeamBehavior (攻击行为模式)
├── Gun.BeginFiringBeam() (玩家武器集成)
└── FireSubBeamSynergyProcessor (协同效果)
```

### 关键设计模式

1. **骨骼渲染系统**：激光束由链表结构的"骨骼"组成，每个骨骼存储位置、旋转、动画帧信息
2. **状态机驱动**：充能→预警→发射→消散→断开，每个状态有不同的视觉/物理行为
3. **事件委托机制**：`SpeculativeRigidbody.OnBeamCollision` 事件让对象响应激光击中
4. **延迟更新定位**：`LateUpdatePosition()` 每帧调用，使激光跟随发射源移动

---

## 核心依赖系统

### 必需依赖（不可替换）

| 系统 | 文件位置 | 功能 | 替代方案复杂度 |
|------|---------|------|---------------|
| **SpeculativeRigidbody** | `Core/Framework/SpeculativeRigidbody.cs` | 自定义物理体，提供像素级碰撞、忽略列表、OnHitByBeam委托 | 高（Unity刚体无法直接替代） |
| **PhysicsEngine** | `Combat/Projectiles/PhysicsEngine.cs` | RaycastWithIgnores()方法，核心碰撞检测 | 中（可用Physics2D.Raycast + 手动过滤） |
| **tk2dTiledSprite** | `VFX/Animation/tk2dTiledSprite.cs` | 平铺精灵渲染器，支持自定义几何和UV操作 | 中（可用LineRenderer但视觉效果不同） |
| **Projectile** | `Core/Framework/Projectile.cs` | 弹幕基类，提供baseData、damageTypes、Owner等 | 低（可简化抽象） |
| **GameActor** | - | 游戏Actor接口，用于Owner、生命值、状态效果 | 低（可简化为接口） |

### 可选依赖（Phase 4+）

- **GameActorEffect**：状态效果系统（冰冻、燃烧、中毒等）
- **VFXPool**：特效池管理
- **GlobalDispersalParticleManager**：粒子系统管理器
- **GoopModifier**：地面效果（毒池、油渍等）

---

## 分阶段迁移计划

### Phase 1: 依赖抽象层（第1-2周）

**目标：** 创建接口抽象层，降低与ETG系统的耦合

**任务：**
1. 创建接口抽象
```csharp
// 物理系统抽象
public interface IBeamRaycastProvider {
    bool Raycast(Vector2 origin, Vector2 direction, float distance,
                 out RaycastHit2D result, LayerMask mask,
                 IBeamCollisionBody[] ignoreList);
}

// 碰撞体抽象
public interface IBeamCollisionBody {
    event Action<BeamController> OnBeamHit;
    Vector2 Position { get; }
    Bounds Bounds { get; }
}

// 渲染器抽象
public interface IBeamRenderer {
    void Initialize(string animationName);
    void UpdateGeometry(List<BeamBone> bones, float uvOffset);
    void SetVisibility(bool visible);
}
```

2. 实现ETG系统的适配器
```csharp
public class SpeculativeRigidbodyAdapter : IBeamCollisionBody {
    private SpeculativeRigidbody body;
    // 实现接口...
}
```

3. 迁移最小依赖
   - BraveBehaviour（简化版）
   - GameActor接口定义
   - 基础数据结构（Vector2扩展、BraveTime等）

**验证里程碑：** 空的BeamController子类能够实例化并调用生命周期方法

**关键文件：**
- 新建：`Abstractions/IBeamRaycastProvider.cs`
- 新建：`Abstractions/IBeamCollisionBody.cs`
- 新建：`Abstractions/IBeamRenderer.cs`

---

### Phase 2: BeamController基础架构（第3-4周）

**目标：** 迁移BeamController抽象基类，建立核心API

**任务：**
1. 迁移 `BeamController.cs` (150行)
   - **保留属性**：Owner, Gun, Origin, Direction, HitsPlayers, HitsEnemies, DamageModifier
   - **保留方法**：
     - `GetIgnoreRigidbodies()` - 构建忽略列表
     - `HandleChanceTick()` - 机会tick处理（玩家特有，可暂时简化）
     - 抽象方法：LateUpdatePosition, CeaseAttack, DestroyBeam
   - **暂时移除**：
     - knockback相关（Phase 4添加）
     - chargeDelay相关（Phase 3添加）
     - ChanceBased属性（Phase 5玩家特有功能）

2. 实现简单状态枚举
```csharp
public enum BeamState {
    Firing,        // Phase 2: 仅实现发射状态
    // Charging,   // Phase 3
    // Telegraphing, // Phase 3
    // Dissipating, // Phase 3
    // Disconnected // Phase 3
}
```

3. 创建测试用BeamController子类
```csharp
public class TestBeamController : BeamController {
    public override void LateUpdatePosition(Vector3 origin) { }
    public override void CeaseAttack() { }
    public override void DestroyBeam() { }
    public override void AdjustPlayerBeamTint(Color color, int priority, float time) { }
    public override bool ShouldUseAmmo => State == BeamState.Firing;
}
```

**验证里程碑：** TestBeamController能够从测试枪械生成，设置Origin/Direction，并正常销毁

**关键文件：**
- 迁移：`Core/Framework/BeamController.cs` → `Beams/Core/BeamController.cs`
- 新建：`Beams/Test/TestBeamController.cs`

---

### Phase 3: BasicBeamController - 直线激光（第5-7周）

**目标：** 实现功能完整的直线激光束（不含曲线、穿透、反射）

#### 3.1 骨骼系统与渲染（第5周）

**任务：**
1. 实现BeamBone数据结构
```csharp
public class BeamBone {
    public float PosX;              // 沿激光长度的位置
    public Vector2 Position;        // 世界坐标（曲线激光用）
    public Vector2 Velocity;        // 速度（曲线激光用）
    public float RotationAngle;     // 精灵旋转角度
    public int SubtileNum;          // 动画帧索引
    // Phase 4: Homing相关属性
}
```

2. 迁移tk2dTiledSprite渲染逻辑
   - `GetTiledSpriteGeomDesc()` 回调 - 计算几何描述
   - `SetTiledSpriteGeom()` 回调 - 应用顶点/UV数据
   - 骨骼链表管理：`LinkedList<BeamBone> m_bones`

3. 实现BeamBoneType.Straight（直线模式）
   - 骨骼只使用PosX位置（1D）
   - 简单线性排列

4. 实现BeamTileType
   - **Flowing**：UV滚动效果（流动感）
   - **Tiley**：静态平铺（暂不实现GrowAtEnd/GrowAtBeginning）

**验证里程碑：** 直线激光能够渲染，UV滚动正常，跟随Origin和Direction更新

**关键文件：**
- 迁移：`Systems/Utilities/BasicBeamController.cs` (仅渲染部分，约600行)
- 参考：BasicBeamController.cs:264-300 (Start方法，精灵初始化)
- 参考：BasicBeamController.cs:1500-1800 (几何生成逻辑)

#### 3.2 碰撞检测（第6周）

**任务：**
1. 实现核心碰撞循环 `HandleBeamFrame()`
```csharp
// 伪代码
foreach frame:
    rayDirection = Direction
    rayOrigin = Origin

    // 执行射线检测
    if (RaycastWithIgnores(rayOrigin, rayDirection, maxDistance, out hit)) {
        // 检测到碰撞
        beamEndPosition = hit.point
        HandleCollision(hit.rigidbody, hit.point, hit.normal)
    } else {
        // 未碰撞，延伸到最大距离
        beamEndPosition = rayOrigin + rayDirection * maxDistance
    }

    UpdateBones(beamEndPosition)
```

2. 实现BeamCollisionType.Raycast（圆形碰撞）
   - 参数：`collisionRadius`（默认1.5单位）
   - 宽度补偿：对Y轴偏移做多条射线（可选）

3. 集成PhysicsEngine.RaycastWithIgnores()
   - 使用GetIgnoreRigidbodies()构建忽略列表
   - 正确处理Owner的刚体排除

4. 实现CollisionLayer.BeamBlocker支持
   - 激光可以被特定层（墙壁、掩体）阻挡

**验证里程碑：** 激光射向墙壁时正确停止，射向空中时延伸到最大距离

**关键文件：**
- 参考：BasicBeamController.cs:1900-2100 (HandleBeamFrame方法)
- 参考：BasicBeamController.cs:2069-2093 (射线检测逻辑)

#### 3.3 伤害系统（第6周）

**任务：**
1. 实现持续伤害计算
```csharp
damagePerFrame = baseData.damage * DeltaTime * RateOfFire * DamageModifier
```

2. 碰撞到HealthHaver时应用伤害
   - 调用`ApplyDamage(amount, direction, damageTypes)`

3. 实现穿透伤害衰减
```csharp
// GameManager.Instance.PierceDamageScaling - 默认0.5每层穿透
damageScaling = Mathf.Pow(pierceDamageScaling, hitCount)
```

4. 触发SpeculativeRigidbody.OnBeamCollision事件
   - 让被击中物体响应（如反射镜、特殊敌人）

**验证里程碑：** 激光持续照射敌人造成伤害，伤害量正确

**关键文件：**
- 参考：BasicBeamController.cs:2200-2400 (伤害应用逻辑)

#### 3.4 动画系统（第7周）

**任务：**
1. 实现三段式动画支持
   - **beamAnimation**：中段（主体）精灵，可动画
   - **beamStartAnimation**：起始端（枪口附近）精灵
   - **beamEndAnimation**：末端（碰撞点附近）精灵

2. 实现覆盖层动画
   - **muzzleAnimation**：枪口闪光特效
   - **impactAnimation**：撞击点特效

3. 精灵动画播放器集成
   - 使用tk2dSpriteAnimator播放动画序列
   - 支持循环播放

4. 动画帧同步
   - BeamBone.SubtileNum 控制每段骨骼的动画帧

**验证里程碑：** 激光有起始/中段/末端动画，枪口和撞击点有特效

**关键文件：**
- 参考：BasicBeamController.cs:182-220 (动画属性getter)
- 参考：BasicBeamController.cs:284-330 (精灵初始化)
- 参考：BasicBeamController.cs:1100-1200 (撞击点特效更新)

---

### Phase 4: 高级特性（第8-10周）

#### 4.1 完整状态机（第8周）

**任务：**
1. 实现Charging状态
   - `usesChargeDelay` + `chargeDelay` 参数
   - 显示充能动画（chargeAnimation）
   - 不消耗弹药，不造成伤害

2. 实现Telegraphing状态
   - `usesTelegraph` + `telegraphTime` 参数
   - 显示预警动画（telegraphAnimations）
   - 告知玩家即将发射的激光路径

3. 实现Dissipating状态
   - `endType = BeamEndType.Dissipate`
   - `dissipateTime` 参数
   - 渐隐效果（dissipateAnimations）

4. 实现Disconnected状态
   - `endType = BeamEndType.Persist`
   - 使用decayNear/decayFar控制衰减速度
   - 角度阈值（breakAimAngle）触发断开

**验证里程碑：** Boss激光攻击有充能预警→发射→渐隐的完整流程

**关键文件：**
- 参考：BasicBeamController.cs:800-1000 (状态更新逻辑)
- 参考：BasicBeamController.cs:164 (BeamState枚举)

#### 4.2 曲线激光（BeamBoneType.Projectile）（第8-9周）

**任务：**
1. 实现BeamBone物理更新
   - 骨骼有Position（2D坐标）和Velocity
   - 每帧更新骨骼运动：`bone.Position += bone.Velocity * DeltaTime`

2. 实现贝塞尔插值（interpolateStretchedBones）
   - 在骨骼之间插入平滑曲线
   - 提升视觉流畅度

3. 骨骼生成/移除逻辑
   - 根据BoneSpeed在激光头部动态生成新骨骼
   - 尾部骨骼超出距离后移除

4. 支持ProjectileAndBeamMotionModule
   - 允许外部模块控制骨骼运动（如正弦波、螺旋）

**验证里程碑：** 曲线激光能够绘制弧线轨迹，如抛物线或波浪

**关键文件：**
- 参考：BasicBeamController.cs:1300-1500 (骨骼更新逻辑)
- 参考：BasicBeamController.cs:1600-1700 (贝塞尔插值)

#### 4.3 穿透与反射（第9周）

**任务：**
1. 实现穿透计数（penetration参数）
   - 跟踪每个敌人被击中次数
   - penetration > 0时激光穿过敌人继续前进
   - 生成pierce impact sprites（穿透点标记）

2. 实现反射计数（reflections参数）
   - 碰到墙壁时生成`m_reflectedBeam`
   - 反射方向 = 入射方向 - 2 * (入射·法线) * 法线
   - 设置`ReflectedFromRigidbody`防止重复反射

3. 特殊反射物体支持（TorchController）
   - 检测特定反射器物体
   - 可能改变激光属性

**验证里程碑：** 激光穿透1个敌人伤害2个，反射后弹向另一方向

**关键文件：**
- 参考：BasicBeamController.cs:2100-2300 (穿透/反射逻辑)

#### 4.4 归位系统（Homing）（第9-10周）

**任务：**
1. 为BeamBone添加归位属性
```csharp
public float HomingRadius;           // 搜索半径
public float HomingAngularVelocity;  // 转向角速度
public AIActor HomingTarget;         // 当前目标
```

2. 实现BeamBone.ApplyHoming()
   - 在HomingRadius范围内搜索敌人
   - 计算转向角度
   - 应用HomingAngularVelocity * DeltaTime

3. 实现HomingDampenMotion
   - 平滑转向，避免剧烈抖动

**验证里程碑：** 激光能够追踪移动的敌人，形成弯曲轨迹

**关键文件：**
- 参考：BasicBeamController.cs:232-246 (Homing属性)
- 参考：BeamBone归位逻辑（在骨骼更新中）

---

### Phase 5: 专用控制器与特效（第11-12周）

#### 5.1 RaidenBeamController（闪电链）（第11周）

**目标：** 迁移闪电链激光（同时击中多个目标）

**任务：**
1. 迁移RaidenBeamController.cs（约500行）
   - **多目标系统**：`List<AIActor> m_targets` + `maxTargets`
   - **贝塞尔曲线连接**：在源点和多个目标之间绘制平滑曲线
   - **目标选择模式**：TargetType.Screen vs TargetType.Room

2. 实现分段渲染
   - 每个目标独立的骨骼链（c_segmentCount = 20）
   - 骨骼长度：4像素/骨骼 = 0.25单位

3. 实现多种撞击动画
   - EnemyImpactAnimation
   - BossImpactAnimation
   - OtherImpactAnimation

**验证里程碑：** 闪电链能够同时连接3个敌人，形成多段弯曲的闪电效果

**关键文件：**
- 迁移：`Systems/Utilities/RaidenBeamController.cs`

#### 5.2 高级视觉特效（第11-12周）

**任务：**
1. **分散粒子系统**（Dispersal Particles）
   - `UsesDispersalParticles` + `DispersalDensity`
   - 撞击点产生粒子飞散
   - 使用GlobalDispersalParticleManager池化

2. **屏幕扭曲效果**（Screen Distortion）
   - `doesScreenDistortion` 标志
   - 使用材质着色器实现画面扭曲
   - distortionRadius/Power参数控制强度

3. **地面效果**（Goop Modifier）
   - GoopModifier组件
   - 激光路径上生成毒池、火焰、油渍等
   - SpawnCollisionGoop()调用

**验证里程碑：** 激光撞击产生粒子飞溅，强力激光造成屏幕扭曲

**关键文件：**
- 参考：BasicBeamController.cs:73-84 (粒子配置)
- 参考：BasicBeamController.cs:86-101 (屏幕扭曲配置)
- 参考：VFX/GlobalDispersalParticleManager.cs

#### 5.3 状态效果系统（第12周）

**任务：**
1. 实现TimeToStatus机制
   - 激光持续照射一定时间后触发状态效果
   - 为每个Actor维护BeamStatusAmount累加器

2. 实现概率切片（Probability Slicing）
```csharp
// BraveMathCollege.SliceProbability()
float adjustedChance = statusEffectChance * statusEffectAccumulateMultiplier
bool shouldApply = SliceProbability(adjustedChance, DeltaTime)
```

3. 支持全部状态效果类型
   - Freeze（冰冻）
   - Fire（燃烧）
   - Poison（中毒）
   - Charm（魅惑）
   - Stun（眩晕）
   - Bleed（流血）

**验证里程碑：** 激光持续照射敌人1秒后触发冰冻效果

**关键文件：**
- 参考：BeamController.cs:21-22 (状态效果参数)
- 参考：BasicBeamController.cs:33 (TimeToStatus)

---

### Phase 6: AI与武器集成（第13-14周）

#### 6.1 AI发射控制器（第13周）

**任务：**
1. 迁移AIBeamShooter.cs
   - **椭圆发射位置**：firingEllipseCenter/A/B参数
     ```csharp
     // 计算椭圆上的点
     angle = m_laserAngle
     x = center.x + A * cos(angle)
     y = center.y + B * sin(angle)
     ```
   - **高度偏移**：heightOffset, northAngleTolerance, northRampHeight
   - **充能特效**：chargeVfx池

2. 实现激光发射API
   - `StartFiringLaser(float angle)` - 开始发射
   - `StopFiringLaser()` - 停止发射
   - `LaserAngle` 属性 - 动态瞄准

3. 集成动画系统
   - shootAnim播放
   - CurrentAiAnimator.FacingDirection锁定

4. 生命周期管理
   - 受伤时停止激光
   - OnDestroy清理激光

**验证里程碑：** Boss能够发射激光，激光跟随Boss移动和转向

**关键文件：**
- 迁移：`Core/Framework/AIBeamShooter.cs`
- 参考：AIBeamShooter.cs:122-135 (StartFiringLaser)
- 参考：AIBeamShooter.cs:74-93 (LateUpdate位置更新)

#### 6.2 行为模式系统（第13周）

**任务：**
1. 迁移ShootBeamBehavior.cs（AI攻击行为）
   - **发射时间控制**：firingTime
   - **移动锁定**：stopWhileFiring
   - **初始瞄准偏移**：InitialAimOffset

2. 实现追踪模式（Tracking Types）
   - **No Tracking**：固定方向
   - **Player Follow**：跟随玩家，带追赶速度
   - **Unit-based**：单位制追踪加速度
   - **Degree-based**：角度制追踪
   - **Overshoot**：超前瞄准（预判玩家移动）

3. 正弦波摆动（Oscillate）
   - 可选的正弦波摆动效果
   - 摆动幅度和频率可配置

**验证里程碑：** Boss激光能够追踪玩家，形成扫射攻击模式

**关键文件：**
- 迁移：`Actors/Behaviors/ShootBeamBehavior.cs`

#### 6.3 Gun武器集成（第14周）

**任务：**
1. 在Gun类中实现激光发射
```csharp
// Gun.BeginFiringBeam(ProjectileModule module)
步骤：
1. 在枪口位置生成beam projectile GameObject
2. 获取BeamController组件
3. 设置属性：
   - Owner = player
   - Gun = this
   - HitsPlayers = false
   - HitsEnemies = true
   - Origin = barrelPosition
   - Direction = aimDirection
4. 应用玩家伤害/速度倍率
5. 添加到m_activeBeams列表
6. 应用持续后坐力（如果knocksShooterBack）
```

2. 实现激光停止
```csharp
// Gun.CeaseBeamFiring()
步骤：
1. 遍历m_activeBeams列表
2. 调用BeamController.CeaseAttack()
3. 清空m_activeBeams列表
```

3. 每帧更新
   - 在Gun.Update()中更新激光Origin/Direction
   - 同步玩家瞄准方向

**验证里程碑：** 玩家装备激光枪，按住开火键持续发射激光束

**关键文件：**
- 参考：Gun.cs:3613 (BeginFiringBeam方法)

#### 6.4 协同效果系统（第14周）

**任务：**
1. 迁移FireSubBeamSynergyProcessor
   - 在主激光上生成子激光
   - **模式**：
     - FROM_BEAM：沿激光长度生成
     - FROM_PROJECTILE_CENTER：从弹幕中心生成
   - **SubbeamData**：subbeam, percent, angle

2. 迁移ModifyBeamSynergyProcessor
   - 修改激光属性（基于玩家道具组合）
   - 添加状态效果
   - 修改statusEffectChance/Accumulate

**验证里程碑：** 玩家拥有特定道具组合时，激光产生分支或附加冰冻效果

**关键文件：**
- 迁移：`Combat/Projectiles/FireSubBeamSynergyProcessor.cs`
- 迁移：`Combat/Projectiles/ModifyBeamSynergyProcessor.cs`
- 迁移：`Systems/Data/SubbeamData.cs`

---

## 公共API契约（稳定接口）

以下接口在迁移过程中**必须保持兼容**，确保外部系统能够正确使用激光系统：

### BeamController（抽象基类）

```csharp
public abstract class BeamController : BraveBehaviour
{
    // === 核心属性（不可变） ===
    public GameActor Owner { get; set; }           // 发射者
    public Gun Gun { get; set; }                   // 发射武器
    public Vector2 Origin { get; set; }            // 起点
    public Vector2 Direction { get; set; }         // 方向
    public bool HitsPlayers { get; set; }          // 击中玩家
    public bool HitsEnemies { get; set; }          // 击中敌人
    public float DamageModifier { get; set; }      // 伤害倍率

    // === 状态效果（不可变） ===
    public float statusEffectChance;               // 状态触发概率
    public float statusEffectAccumulateMultiplier; // 状态累积倍率

    // === 碰撞忽略列表（不可变） ===
    public List<SpeculativeRigidbody> IgnoreRigidbodes;
    public List<Tuple<SpeculativeRigidbody, float>> TimedIgnoreRigidbodies;

    // === 抽象方法（子类必须实现） ===
    public abstract void LateUpdatePosition(Vector3 origin);  // 每帧更新位置
    public abstract void CeaseAttack();                       // 停止攻击
    public abstract void DestroyBeam();                       // 销毁激光
    public abstract void AdjustPlayerBeamTint(Color color, int priority, float lerpTime);
    public abstract bool ShouldUseAmmo { get; }               // 是否消耗弹药

    // === 辅助方法（不可变） ===
    protected SpeculativeRigidbody[] GetIgnoreRigidbodies();  // 获取忽略列表
    protected bool HandleChanceTick();                         // 处理机会tick
}
```

### BasicBeamController（主要实现）

```csharp
public class BasicBeamController : BeamController
{
    // === 配置属性（不可变） ===
    public BeamBoneType boneType;              // 骨骼类型：Straight/Projectile
    public int penetration;                     // 穿透次数
    public int reflections;                     // 反射次数
    public float homingRadius;                  // 归位半径
    public float homingAngularVelocity;         // 归位角速度
    public BeamTileType TileType;              // 平铺类型：Flowing/Tiley
    public BeamCollisionType collisionType;    // 碰撞类型：Raycast/Rectangle

    // === 动画配置（不可变） ===
    public string beamAnimation;                // 激光主体动画
    public string beamStartAnimation;           // 起始端动画
    public string beamEndAnimation;             // 末端动画
    public string muzzleAnimation;              // 枪口闪光动画
    public string impactAnimation;              // 撞击特效动画

    // === 状态属性（不可变） ===
    public BeamState State { get; set; }        // 当前状态
    public float ProjectileScale { get; set; }  // 缩放比例

    // === 枚举定义（不可变） ===
    public enum BeamState {
        Charging,      // 充能中
        Telegraphing,  // 预警中
        Firing,        // 发射中
        Dissipating,   // 消散中
        Disconnected   // 已断开
    }

    public enum BeamBoneType {
        Straight,      // 直线
        Projectile     // 曲线（弹道）
    }

    public enum BeamTileType {
        Flowing,       // 流动（UV滚动）
        Tiley          // 静态平铺
    }

    public enum BeamCollisionType {
        Raycast,       // 射线检测（圆形）
        Rectangle      // 矩形检测
    }
}
```

### AIBeamShooter（AI控制器）

```csharp
public class AIBeamShooter : BraveBehaviour
{
    // === 配置属性（不可变） ===
    public Transform beamTransform;        // 发射点
    public Projectile beamProjectile;      // 激光弹幕预制体
    public ProjectileModule beamModule;    // 备用模块

    // === 发射控制（不可变） ===
    public float LaserAngle { get; set; }  // 激光角度（自动更新facing）
    public bool IsFiringLaser { get; }     // 是否正在发射
    public BeamController LaserBeam { get; } // 当前激光实例

    // === API方法（不可变） ===
    public void StartFiringLaser(float angle);  // 开始发射
    public void StopFiringLaser();               // 停止发射
    public Vector2 GetTrueLaserOrigin();         // 获取真实发射点
}
```

### Gun集成（不可变API）

```csharp
public class Gun : PickupObject
{
    // === 激光管理（不可变） ===
    public void BeginFiringBeam(ProjectileModule module);  // 开始发射激光
    public void CeaseBeamFiring();                          // 停止激光发射
    protected List<ModuleShootData> m_activeBeams;          // 活跃激光列表
}
```

---

## Unity原生替代方案（Phase 7+，可选）

### 方案A：Unity LineRenderer替代tk2dTiledSprite

**优势：**
- Unity原生组件，无需第三方依赖
- GPU Instancing，性能更好
- 内置宽度曲线、颜色梯度
- Trail Renderer支持

**劣势：**
- UV映射不同，动画效果需要重新实现
- 无像素级精确控制
- 需要重写所有几何生成逻辑

**迁移路径：**
1. 创建 `IBeamRenderer` 接口
2. 实现 `LineRendererBeamRenderer` 适配器
3. 保留 `Tk2dTiledSpriteBeamRenderer` 作为备选
4. 性能对比测试

**评估：** 建议Phase 7+尝试，不影响核心功能

### 方案B：Unity Physics2D替代PhysicsEngine

**优势：**
- Unity原生，兼容性好
- 标准API，易于维护

**劣势：**
- 需要手动实现忽略列表过滤
- 无像素级精确碰撞（ETG使用像素碰撞）
- CollisionLayer映射到LayerMask需要额外工作

**迁移路径：**
```csharp
// 替换
PhysicsEngine.RaycastWithIgnores(origin, dir, dist, out hit, mask, ignoreList)

// 为
RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, dist, mask);
foreach (var hit in hits) {
    if (!ignoreList.Contains(hit.rigidbody)) {
        // 处理碰撞
        break;
    }
}
```

**评估：** 可行，但需要完整测试碰撞准确性

### 方案C：Unity VFX Graph替代粒子系统

**优势：**
- GPU驱动，性能极佳
- 可视化编辑
- 现代渲染管线支持（URP/HDRP）

**劣势：**
- 需要VFX Graph包
- 学习曲线
- 运行时控制不如脚本直接

**迁移路径：**
1. 保留现有ParticleSystem实现（Phase 5）
2. Phase 8+创建VFX Graph版本
3. 性能对比测试
4. 根据目标平台选择

**评估：** 适合现代项目，但不紧急

---

## 资源需求清单

### 精灵资源（Sprites & Animations）

| 资源类型 | 数量 | 规格 | 用途 |
|---------|------|------|------|
| **激光主体精灵** | 1 | 4-16px宽，可平铺 | beamAnimation |
| **激光起始精灵** | 1（可选） | 16-32px，可动画 | beamStartAnimation |
| **激光末端精灵** | 1（可选） | 16-32px，可动画 | beamEndAnimation |
| **枪口闪光精灵** | 1（可选） | 32-64px，动画序列 | muzzleAnimation |
| **撞击特效精灵** | 1-3（可选） | 32-64px，动画序列 | impactAnimation |
| **充能特效精灵** | 1（可选） | 32-64px，动画序列 | chargeAnimation |

**格式要求：**
- ETG使用tk2dSpriteCollection
- 可替代：Unity Sprite Atlas（2D Sprite包）

**提取方式：**
- 从ETG资源文件中提取
- 或使用占位符精灵（Phase 2-3）

### 材质与着色器（Materials & Shaders）

| 资源类型 | 用途 | 需求 |
|---------|------|------|
| **激光基础材质** | 激光主体渲染 | Additive混合模式，支持颜色叠加 |
| **扭曲材质** | 屏幕扭曲效果 | 需要屏幕空间扭曲着色器 |
| **粒子材质** | 撞击粒子 | Additive/Alpha Blend |

**着色器需求：**
- 激光发光效果（Bloom友好）
- 可选：自定义着色器实现UV滚动

### 预制体结构（Prefabs）

#### BeamProjectile预制体

```
BeamProjectile (GameObject)
├── Components:
│   ├── BasicBeamController
│   ├── Projectile
│   ├── tk2dTiledSprite (或 LineRenderer)
│   ├── tk2dSpriteAnimator
│   └── SpeculativeRigidbody (如果需要被反弹)
│
└── 子对象 (Children):
    ├── "beam muzzle flare" (GameObject, 可选)
    │   ├── tk2dSprite
    │   └── tk2dSpriteAnimator
    │
    ├── "beam impact vfx" (GameObject, 可选)
    │   ├── tk2dSprite
    │   └── tk2dSpriteAnimator
    │
    └── "beam impact vfx 2" (GameObject, 可选，用于第二个撞击点)
        ├── tk2dSprite
        └── tk2dSpriteAnimator
```

### 音频资源（Audio）

如果使用Wwise音频引擎（ETG使用）：
- `startAudioEvent` - 激光启动音效
- `endAudioEvent` - 激光结束音效
- `objectImpactEventName` - 击中墙壁音效
- `enemyImpactEventName` - 击中敌人音效

如果使用Unity AudioSource（简化版）：
- 替换为AudioClip引用
- 在BeamController中管理AudioSource

---

## 测试策略

### 单元测试（每阶段）

#### Phase 2: BeamController基础
```csharp
[Test]
public void BeamController_Lifecycle() {
    // 测试：生成 → 设置属性 → 销毁
    var beam = SpawnTestBeam();
    beam.Origin = Vector2.zero;
    beam.Direction = Vector2.right;
    Assert.IsNotNull(beam.Owner);
    beam.DestroyBeam();
    Assert.IsTrue(beam == null);
}

[Test]
public void BeamController_IgnoreList() {
    // 测试：忽略列表正确构建
    var beam = SpawnTestBeam();
    beam.IgnoreRigidbodes.Add(playerRigidbody);
    var ignoreList = beam.GetIgnoreRigidbodies();
    Assert.Contains(playerRigidbody, ignoreList);
}
```

#### Phase 3: 渲染与碰撞
```csharp
[Test]
public void BasicBeam_RendersStraightLine() {
    // 测试：直线激光渲染
    var beam = SpawnBasicBeam();
    beam.Origin = Vector2.zero;
    beam.Direction = Vector2.right;
    beam.Update(); // 手动更新一帧

    // 验证骨骼生成
    Assert.IsTrue(beam.BoneCount > 0);
    // 验证几何数据
    Assert.IsTrue(beam.SpriteGeometry.VertexCount > 0);
}

[Test]
public void BasicBeam_CollisionDetection() {
    // 测试：激光碰撞检测
    var beam = SpawnBasicBeam();
    var wall = SpawnTestWall(new Vector2(5, 0));

    beam.Origin = Vector2.zero;
    beam.Direction = Vector2.right;
    beam.HandleBeamFrame();

    // 验证激光在墙前停止
    Assert.Less(beam.CurrentLength, 5.5f);
}

[Test]
public void BasicBeam_DamageApplication() {
    // 测试：持续伤害
    var beam = SpawnBasicBeam();
    var enemy = SpawnTestEnemy(new Vector2(3, 0));
    float initialHealth = enemy.Health;

    beam.Origin = Vector2.zero;
    beam.Direction = Vector2.right;

    // 模拟1秒
    for (int i = 0; i < 60; i++) {
        beam.HandleBeamFrame();
    }

    // 验证敌人受伤
    Assert.Less(enemy.Health, initialHealth);
}
```

#### Phase 4: 高级特性
```csharp
[Test]
public void BasicBeam_Penetration() {
    // 测试：穿透
    var beam = SpawnBasicBeam();
    beam.penetration = 1;

    var enemy1 = SpawnTestEnemy(new Vector2(2, 0));
    var enemy2 = SpawnTestEnemy(new Vector2(4, 0));

    beam.Origin = Vector2.zero;
    beam.Direction = Vector2.right;
    beam.HandleBeamFrame();

    // 验证两个敌人都被击中
    Assert.IsTrue(enemy1.WasHitByBeam);
    Assert.IsTrue(enemy2.WasHitByBeam);
}

[Test]
public void BasicBeam_Reflection() {
    // 测试：反射
    var beam = SpawnBasicBeam();
    beam.reflections = 1;

    var wall = SpawnTestWall(new Vector2(5, 0), Vector2.left); // 法线向左

    beam.Origin = Vector2.zero;
    beam.Direction = Vector2.right;
    beam.HandleBeamFrame();

    // 验证生成了反射激光
    Assert.IsNotNull(beam.ReflectedBeam);
    // 验证反射方向正确（应该向左）
    Assert.Less(beam.ReflectedBeam.Direction.x, 0);
}
```

### 集成测试

#### 测试场景：BeamTestScene

**场景内容：**
1. 测试枪械（Gun with BeamProjectile）
2. 目标假人（HealthHaver）
3. 墙壁障碍物（CollisionLayer.BeamBlocker）
4. 反射镜（TorchController或等效物）
5. UI面板显示激光状态

**测试用例：**

| 用例ID | 测试内容 | 预期结果 |
|--------|---------|---------|
| IT-01 | 玩家按住开火键 | 激光持续发射，跟随瞄准方向 |
| IT-02 | 激光照射假人 | 假人持续受伤，血量下降 |
| IT-03 | 激光射向墙壁 | 激光在墙前停止，不穿透 |
| IT-04 | 激光射向空中 | 激光延伸到最大距离 |
| IT-05 | 释放开火键 | 激光立即停止 |
| IT-06 | 穿透激光（penetration=1） | 激光穿过假人1击中假人2 |
| IT-07 | 反射激光（reflections=1） | 激光碰墙后弹向另一方向 |
| IT-08 | 归位激光（homing） | 激光追踪移动的假人 |
| IT-09 | 曲线激光（Projectile bone） | 激光绘制弧线轨迹 |
| IT-10 | 多个激光同时存在 | 多个激光互不干扰 |

#### Boss测试场景：BossBeamTestScene

**场景内容：**
1. 测试Boss（AIActor + AIBeamShooter）
2. 玩家角色（可移动）
3. 掩体障碍物

**测试用例：**

| 用例ID | 测试内容 | 预期结果 |
|--------|---------|---------|
| BT-01 | Boss发射激光 | 激光从Boss位置发出，跟随Boss移动 |
| BT-02 | 激光追踪玩家 | 激光角度实时调整，扫射玩家 |
| BT-03 | 充能预警 | 发射前有充能动画和预警效果 |
| BT-04 | 激光消散 | 停止发射后激光渐隐消失 |
| BT-05 | Boss受伤 | Boss受伤时激光停止 |
| BT-06 | Boss死亡 | Boss死亡时激光立即销毁 |

### 性能测试

#### 性能指标

| 指标 | 目标 | 测量方法 |
|------|------|---------|
| **单激光帧时间** | <0.5ms @ 60fps | Unity Profiler: BasicBeamController.HandleBeamFrame() |
| **10个激光帧时间** | <5ms @ 60fps | Profiler: 总计BeamController时间 |
| **骨骼数量限制** | <100/激光 | 监控LinkedList<BeamBone>大小 |
| **射线检测次数** | <10/激光/帧 | Profiler: PhysicsEngine.Raycast调用 |
| **渲染批次** | 每个激光1个Draw Call | Profiler: Rendering统计 |
| **内存占用** | <1MB/激光 | Profiler: Memory |

#### 压力测试

**测试场景：** 20个Boss同时发射激光

**目标：** 保持60fps，帧时间<16.67ms

**如果性能不足，优化方向：**
1. 减少骨骼数量（降低曲线精度）
2. 降低射线检测频率（每2帧检测1次）
3. 使用对象池（复用BeamController实例）
4. 简化粒子效果
5. LOD系统（远距离激光降低精度）

---

## 关键文件清单

### 核心文件（必须迁移）

| 优先级 | 文件路径 | 行数 | Phase | 说明 |
|--------|---------|------|-------|------|
| ⭐⭐⭐ | `Core/Framework/BeamController.cs` | 150 | 2 | 抽象基类，定义公共API |
| ⭐⭐⭐ | `Systems/Utilities/BasicBeamController.cs` | 2300 | 3-4 | 核心实现，状态机+渲染+碰撞 |
| ⭐⭐⭐ | `Core/Framework/SpeculativeRigidbody.cs` | ? | 1 | 物理体系统，碰撞检测基础 |
| ⭐⭐⭐ | `Combat/Projectiles/PhysicsEngine.cs` | ? | 1 | 射线检测引擎 |
| ⭐⭐ | `VFX/Animation/tk2dTiledSprite.cs` | ? | 1 | 平铺精灵渲染器 |
| ⭐⭐ | `Core/Framework/AIBeamShooter.cs` | 150+ | 6 | AI发射控制器 |
| ⭐⭐ | `Core/Framework/Projectile.cs` | ? | 1 | 弹幕基类 |
| ⭐ | `Systems/Utilities/RaidenBeamController.cs` | 500 | 5 | 闪电链实现 |
| ⭐ | `Systems/Utilities/ReverseBeamController.cs` | 300 | 5 | 反射激光实现 |
| ⭐ | `Actors/Behaviors/ShootBeamBehavior.cs` | 400 | 6 | AI攻击行为 |
| ⭐ | `Items/Pickups/Gun.cs` | ? | 6 | 武器系统（BeginFiringBeam部分） |

### 支持文件（按需迁移）

| 文件路径 | Phase | 说明 |
|---------|-------|------|
| `Combat/Projectiles/FireSubBeamSynergyProcessor.cs` | 6 | 子激光协同效果 |
| `Combat/Projectiles/ModifyBeamSynergyProcessor.cs` | 6 | 激光修改协同效果 |
| `Systems/Data/SubbeamData.cs` | 6 | 子激光数据结构 |
| `VFX/GlobalDispersalParticleManager.cs` | 5 | 粒子池管理器 |
| `Core/Framework/CustomTrailRenderer.cs` | 5 | 自定义轨迹渲染器 |
| `Combat/Effects/GlowEffect.cs` | 5 | 发光后处理效果 |
| `Systems/Utilities/BulletArcLightningController.cs` | 5 | 弧形闪电控制器 |

---

## 风险评估与缓解

### 高风险区域

#### 风险1：tk2dTiledSprite依赖
- **风险等级：** 🔴 高
- **影响：** 激光无法渲染
- **缓解措施：**
  - Phase 1创建IBeamRenderer抽象
  - Phase 7实现LineRenderer适配器
  - 保留简化版tk2dTiledSprite作为Fallback
  - 提供详细迁移文档

#### 风险2：PhysicsEngine自定义碰撞
- **风险等级：** 🟡 中
- **影响：** 碰撞检测不准确，激光穿墙或无法击中
- **缓解措施：**
  - Phase 1创建IRaycastProvider抽象
  - 详尽单元测试覆盖碰撞边界情况
  - 提供Unity Physics2D替代实现
  - 碰撞可视化调试工具

#### 风险3：状态效果系统复杂度
- **风险等级：** 🟡 中
- **影响：** 激光无法造成状态效果（冰冻、燃烧等）
- **缓解措施：**
  - Phase 5延迟实现，不阻塞核心功能
  - 创建简化版状态效果接口
  - 提供空实现（NoOp）作为默认

#### 风险4：性能问题
- **风险等级：** 🟡 中
- **影响：** 多个激光同时存在时帧率下降
- **缓解措施：**
  - 每个Phase都进行性能测试
  - 提供骨骼数量/射线检测频率可配置
  - 实现对象池复用
  - LOD系统（远距离降低精度）

### 技术债务管理

#### 允许的技术债务（早期阶段）
- **Phase 2-3**：硬编码伤害公式（Phase 6重构为可配置）
- **Phase 2-3**：直接使用tk2dTiledSprite（Phase 7抽象为接口）
- **Phase 3-4**：最小状态效果支持（Phase 5扩展完整系统）
- **Phase 3-5**：简化音频系统（Phase 6集成Wwise或Unity Audio）

#### 不可接受的技术债务
- ❌ 破坏公共API兼容性
- ❌ 跳过核心碰撞测试
- ❌ 忽略内存泄漏（BeamController销毁不完整）
- ❌ 性能测试覆盖率<80%

---

## 开发时间表

### 14周迁移计划（全职开发）

```
Week 1-2:  Phase 1 - 依赖抽象层
           └─ 里程碑：空beam能生成和销毁

Week 3-4:  Phase 2 - BeamController基础
           └─ 里程碑：TestBeamController完整生命周期

Week 5:    Phase 3.1 - 渲染系统
           └─ 里程碑：直线激光可见，UV滚动

Week 6:    Phase 3.2 - 碰撞系统
           └─ 里程碑：激光击中墙壁和敌人

Week 6:    Phase 3.3 - 伤害系统
           └─ 里程碑：激光持续造成伤害

Week 7:    Phase 3.4 - 动画系统
           └─ 里程碑：完整动画播放（起/中/末+特效）

Week 8:    Phase 4.1 - 状态机 + 4.2 - 曲线激光
           └─ 里程碑：充能预警系统，弧线激光

Week 9:    Phase 4.3 - 穿透反射
           └─ 里程碑：穿透1敌，反射1次

Week 9-10: Phase 4.4 - 归位系统
           └─ 里程碑：追踪移动目标

Week 11:   Phase 5.1 - RaidenBeamController + 5.2 - VFX
           └─ 里程碑：闪电链3目标，粒子特效

Week 12:   Phase 5.3 - 状态效果
           └─ 里程碑：冰冻/燃烧效果触发

Week 13:   Phase 6.1 - AIBeamShooter + 6.2 - Behaviors
           └─ 里程碑：Boss激光攻击模式

Week 14:   Phase 6.3 - Gun集成 + 6.4 - 协同效果
           └─ 里程碑：玩家激光武器，道具协同

Week 14:   最终测试与交付
           └─ 完整功能验收
```

### 最小可行产品（MVP）时间表（加速版）

如果资源有限，优先实现核心功能：

```
Week 1-2:  Phase 1 - 依赖抽象（简化版）
Week 3:    Phase 2 - BeamController基础
Week 4-5:  Phase 3 - BasicBeamController（仅直线，无动画）
Week 6:    Phase 6.1 - AIBeamShooter（基础版）
Week 6:    Phase 6.3 - Gun集成（基础版）

MVP里程碑（6周）：
✅ 玩家能发射直线激光
✅ 激光能击中敌人造成伤害
✅ Boss能发射激光攻击玩家
✅ 基本碰撞检测正常
```

---

## 验证与交付标准

### 功能验收标准（所有阶段完成后）

#### 核心功能（必须100%通过）

- [ ] **渲染**
  - [ ] 直线激光正确渲染
  - [ ] 曲线激光绘制弧线轨迹
  - [ ] 起始/中段/末端动画播放
  - [ ] 枪口闪光和撞击特效显示
  - [ ] UV滚动流动效果正常

- [ ] **碰撞检测**
  - [ ] 激光击中墙壁停止
  - [ ] 激光击中敌人触发事件
  - [ ] 忽略列表正确工作（不击中Owner）
  - [ ] 穿透功能正常（penetration > 0）
  - [ ] 反射功能正常（reflections > 0）

- [ ] **伤害系统**
  - [ ] 持续伤害正确计算
  - [ ] 伤害倍率应用正确
  - [ ] 穿透伤害衰减正常
  - [ ] 不同damageType正确应用

- [ ] **状态机**
  - [ ] Charging状态：充能延迟生效
  - [ ] Telegraphing状态：预警动画显示
  - [ ] Firing状态：正常发射
  - [ ] Dissipating状态：渐隐消失
  - [ ] Disconnected状态：角度阈值触发

- [ ] **AI控制**
  - [ ] AIBeamShooter启动/停止激光
  - [ ] 激光跟随AI移动
  - [ ] 激光角度实时更新
  - [ ] 椭圆发射位置计算正确

- [ ] **玩家集成**
  - [ ] Gun.BeginFiringBeam()正常发射
  - [ ] 激光跟随玩家瞄准
  - [ ] 释放按键停止激光
  - [ ] 弹药消耗正确（ShouldUseAmmo）

#### 高级功能（建议90%+通过）

- [ ] **归位系统**
  - [ ] 激光追踪移动目标
  - [ ] 归位半径和角速度配置生效

- [ ] **闪电链（RaidenBeamController）**
  - [ ] 同时连接多个目标
  - [ ] 贝塞尔曲线平滑

- [ ] **状态效果**
  - [ ] 冰冻效果触发
  - [ ] 燃烧效果持续伤害
  - [ ] TimeToStatus阈值正确

- [ ] **视觉特效**
  - [ ] 分散粒子生成
  - [ ] 屏幕扭曲效果（如果实现）

- [ ] **协同效果**
  - [ ] 子激光生成
  - [ ] 属性修改器生效

### 性能验收标准

- [ ] 单个激光帧时间 <0.5ms （60fps目标）
- [ ] 10个激光同时存在时总帧时间 <5ms
- [ ] 支持至少10个并发激光束
- [ ] 无内存泄漏（长时间运行内存稳定）
- [ ] 激光销毁时正确清理所有资源

### 代码质量标准

- [ ] 公共API保持与原始系统兼容
- [ ] 单元测试覆盖率 ≥80%
- [ ] 集成测试全部通过
- [ ] 代码注释完整（关键算法、复杂逻辑）
- [ ] 无编译警告
- [ ] 符合项目代码规范

---

## 交付清单

### 代码交付物

```
NewProject/
├── Assets/
│   ├── Scripts/
│   │   ├── Beams/                          # 激光系统根目录
│   │   │   ├── Abstractions/               # 抽象层（Phase 1）
│   │   │   │   ├── IBeamRaycastProvider.cs
│   │   │   │   ├── IBeamCollisionBody.cs
│   │   │   │   └── IBeamRenderer.cs
│   │   │   ├── Core/                       # 核心（Phase 2）
│   │   │   │   ├── BeamController.cs
│   │   │   │   ├── BeamBone.cs
│   │   │   │   └── BeamEnums.cs
│   │   │   ├── Implementations/            # 实现（Phase 3-5）
│   │   │   │   ├── BasicBeamController.cs
│   │   │   │   ├── RaidenBeamController.cs
│   │   │   │   └── ReverseBeamController.cs
│   │   │   ├── AI/                         # AI系统（Phase 6）
│   │   │   │   ├── AIBeamShooter.cs
│   │   │   │   ├── ShootBeamBehavior.cs
│   │   │   │   └── BeholsterLaserBehavior.cs
│   │   │   ├── Synergies/                  # 协同系统（Phase 6）
│   │   │   │   ├── FireSubBeamSynergyProcessor.cs
│   │   │   │   ├── ModifyBeamSynergyProcessor.cs
│   │   │   │   └── SubbeamData.cs
│   │   │   ├── Rendering/                  # 渲染（Phase 7+可选）
│   │   │   │   ├── Tk2dTiledSpriteRenderer.cs
│   │   │   │   └── LineRendererAdapter.cs
│   │   │   └── Utils/                      # 工具类
│   │   │       ├── BeamPhysicsHelper.cs
│   │   │       └── BeamVisualDebugger.cs
│   │   └── Tests/                          # 测试
│   │       ├── Editor/
│   │       │   ├── BeamControllerTests.cs
│   │       │   ├── BasicBeamControllerTests.cs
│   │       │   └── CollisionTests.cs
│   │       └── Runtime/
│   │           └── IntegrationTests.cs
│   ├── Prefabs/
│   │   ├── Beams/
│   │   │   ├── BasicBeamProjectile.prefab
│   │   │   ├── RaidenBeamProjectile.prefab
│   │   │   └── TestBeamProjectile.prefab
│   │   └── VFX/
│   │       ├── BeamMuzzleFlare.prefab
│   │       ├── BeamImpact.prefab
│   │       └── DispersalParticles.prefab
│   └── Scenes/
│       └── Tests/
│           ├── BeamTestScene.unity
│           └── BossBeamTestScene.unity
```

### 文档交付物

```
Documentation/
├── 01_Migration_Plan.md                    # 本文档
├── 02_API_Reference.md                     # API参考手册
│   ├── BeamController API
│   ├── BasicBeamController API
│   ├── AIBeamShooter API
│   └── 枚举和数据结构
├── 03_Integration_Guide.md                 # 集成指南
│   ├── 如何创建新激光类型
│   ├── 如何集成到武器系统
│   ├── 如何配置AI激光攻击
│   └── 常见问题FAQ
├── 04_Performance_Tuning.md                # 性能调优
│   ├── 性能测试结果
│   ├── 优化建议
│   └── 性能监控工具
├── 05_Design_Rationale.md                  # 设计理念
│   ├── 为什么使用骨骼系统
│   ├── 为什么使用状态机
│   ├── 射线碰撞vs触发器碰撞
│   └── tk2dTiledSprite vs LineRenderer
└── 06_Test_Report.md                       # 测试报告
    ├── 单元测试结果
    ├── 集成测试结果
    ├── 性能测试结果
    └── 已知问题清单
```

---

## 下一步行动

### 立即行动项（开始迁移前）

1. **确认目标项目环境**
   - [ ] Unity版本（建议2021.3 LTS+）
   - [ ] 渲染管线（Built-in/URP/HDRP）
   - [ ] 物理系统配置（2D/3D）
   - [ ] 是否使用tk2d或其他精灵系统

2. **评估依赖可用性**
   - [ ] 是否能提取ETG的SpeculativeRigidbody系统
   - [ ] 是否能提取PhysicsEngine
   - [ ] 是否能提取tk2dTiledSprite
   - [ ] 是否需要完全用Unity原生替代

3. **资源准备**
   - [ ] 提取激光精灵资源
   - [ ] 准备测试用敌人/玩家预制体
   - [ ] 创建测试场景

4. **团队准备**
   - [ ] 分配开发人员（建议1-2名）
   - [ ] 安排代码审查
   - [ ] 设置版本控制分支策略

### 建议审查问题（启动会议）

在开始迁移前，建议与用户确认：

1. **范围确认**
   - 是否需要全部3种BeamController（Basic/Raiden/Reverse）？
   - 是否需要AI系统（还是只要玩家激光）？
   - 是否需要协同效果系统？

2. **技术选型**
   - 目标项目是否已有物理系统？能否复用？
   - 是否接受tk2d依赖，还是必须用Unity原生？
   - 音频系统使用Wwise还是Unity AudioSource？

3. **质量要求**
   - 性能目标是什么（60fps？30fps？）
   - 支持多少个并发激光？
   - 测试覆盖率要求？

4. **时间预算**
   - 是要完整迁移（14周）还是MVP（6周）？
   - 是否有硬截止日期？

---

## 附录

### 术语表

| 术语 | 英文 | 说明 |
|------|------|------|
| **激光束** | Beam | 持续性光束武器，非离散弹幕 |
| **骨骼** | Bone | 组成激光的分段单元，用于渲染和物理 |
| **穿透** | Penetration | 激光穿过敌人继续前进的次数 |
| **反射** | Reflection | 激光碰到墙壁弹射的次数 |
| **归位** | Homing | 激光自动追踪目标的能力 |
| **预警** | Telegraph | 发射前的警告动画 |
| **消散** | Dissipate | 停止发射后的渐隐效果 |
| **忽略列表** | Ignore List | 碰撞检测时跳过的刚体列表 |
| **状态效果** | Status Effect | 冰冻、燃烧、中毒等持续效果 |
| **协同效果** | Synergy | 基于道具组合的特殊效果 |

### 参考链接

- Unity 2D Physics: https://docs.unity3d.com/Manual/Physics2DReference.html
- Unity LineRenderer: https://docs.unity3d.com/Manual/class-LineRenderer.html
- Unity VFX Graph: https://unity.com/visual-effect-graph

---

**计划版本：** v1.0
**创建日期：** 2026-01-18
**最后更新：** 2026-01-18
**状态：** 已审批

---

## 总结

本迁移计划提供了一个清晰的路径，将ETG的激光弹幕系统迁移到新Unity项目。通过6个阶段的渐进式实现，我们可以：

1. **最小化风险** - 通过抽象层降低耦合，Phase 1就建立接口
2. **增量交付** - 每个Phase都有可测试的里程碑
3. **灵活选择** - 提供Unity原生替代方案（Phase 7+）
4. **保证质量** - 完整的测试策略和性能指标

**MVP可在6周内交付**，完整系统需要14周。系统具有高度可扩展性，支持直线/曲线/闪电链等多种激光类型，以及完整的AI控制和协同效果。
