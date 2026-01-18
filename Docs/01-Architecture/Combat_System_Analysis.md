# Combat 战斗系统深度分析

**创建时间：** 2026-01-18
**模块路径：** Assets/Scripts/ETG/Core/Combat
**文件数：** 172（139 Projectile + 31 Effect + 2 Damage）
**核心类大小：** Projectile.cs (2,348 lines), Gun.cs (4,363 lines)
**优先级：** ⭐⭐⭐ CRITICAL

---

## 执行摘要

Combat 系统是 Enter the Gungeon 的 **"弹幕地狱"战斗机制核心**，实现了一个高度优化的弹道战斗引擎。

**核心特性：**
- **像素级精确碰撞** - 自定义物理引擎（SpeculativeRigidbody）
- **丰富的修改器系统** - 穿透、反弹、爆炸、生成子弹等
- **综合状态效果** - 火焰、冰冻、毒素、眩晕、魅惑、流血
- **深度协同系统** - 基于处理器的物品组合（12+ 协同处理器）
- **性能优化** - 对象池、空间分区、简化弹道快速路径

该系统处理 **200+ 屏幕上同时子弹**，维持 60 FPS，定义了游戏的手感和平衡。被 **72 个依赖者**使用，是游戏玩法的核心。

---

## 1. 系统概览

### 1.1 模块组成

**Combat 目录包含 172 个 C# 文件：**
- **Projectiles/** (139 files, 80.8%) - 核心战斗实现
  - 专用类型（Arc, Helix, Homing, Boomerang）
  - 修改器（Pierce, Bounce, Explosive, Spawn, Homing）
  - 协同处理器（12 files）
  - Boss 专用弹道（BossFinal*）
- **Effects/** (31 files, 18.0%) - 状态效果和增益/减益
- **Damage/** (2 files, 1.2%) - 伤害工具

### 1.2 核心依赖

**外部依赖：**
- `SpeculativeRigidbody` (Core.Core.Framework) - 自定义物理
- `HealthHaver` (Core.Core.Framework) - 生命值管理
- `Gun` (Core.Items.Pickups) - 武器系统
- `AIActor` (Core.Actors.Enemy) - 敌人集成
- `BeamController` (Core.Core.Framework) - 光束武器
- `BulletScriptBehavior` (Brave.BulletScript) - 脚本弹道

---

## 2. 核心战斗机制

### 2.1 Projectile 系统

**基础：Projectile.cs** (2,348 lines) - 战斗系统的心脏

**核心组件：**
```csharp
public class Projectile : BraveBehaviour
{
    // 核心属性
    ProjectileData baseData;           // 伤害、速度、射程、力量
    GameActor m_owner;                 // 所有者（玩家/敌人）
    SpeculativeRigidbody specRigidbody; // 自定义物理

    // 碰撞标志
    bool collidesWithPlayer;           // 可击中玩家
    bool collidesWithEnemies;          // 可击中敌人
    bool collidesWithProjectiles;      // 可碰撞其他弹道

    // 状态效果
    bool AppliesFire/Poison/Freeze/Stun/Charm/Bleed;
    List<GameActorEffect> statusEffectsToApply;

    // 运动
    Vector2 m_currentDirection;        // 当前方向
    float m_currentSpeed;              // 当前速度
    bool shouldRotate;                 // 精灵跟随方向
}
```

**生命周期：**
1. **Start()** - 初始化精灵、物理、碰撞层
2. **Move()** - 用速度、加速曲线、阻尼更新位置
3. **HandleRange()** - 跟踪距离，超出射程时销毁
4. **OnCollision()** - 检测碰撞（瓦片/敌人/玩家）
5. **HandleDamage()** - 计算并应用伤害
6. **HandleDestruction()** - 清理、视效、生成死亡效果

**关键特性：**
- **池化系统** - Despawn 到池而不是 Destroy
- **碰撞例外** - 忽略射击者、队友
- **黑色子弹** - 增强的狂暴敌人弹道
- **缩放修改** - 运行时弹道大小调整（玩家属性缩放）
- **着色** - 玩家弹道颜色修改

### 2.2 ProjectileModule 系统

**文件：** ProjectileModule.cs (414 lines)

```csharp
public class ProjectileModule
{
    enum ShootStyle { SemiAutomatic, Automatic, Beam, Charged, Burst }
    enum ProjectileSequenceStyle { Random, Ordered, OrderedGroups }

    List<Projectile> projectiles;               // 要发射的弹道
    List<ChargeProjectile> chargeProjectiles;   // 充能等级

    float angleFromAim;          // 散射偏移
    float angleVariance;         // 随机散射
    int numberOfShotsInClip;     // 弹匣容量
    float cooldownTime;          // 射速
    float burstCooldownTime;     // 连发延迟
    int burstShotCount;          // 每次连发的子弹数
}
```

**处理：**
- 多弹道模式（霰弹枪散射、连发）
- 充能机制（充能时间 → 不同弹道）
- 最终射击机制（特殊的最后一发子弹行为）
- 角度量化（低差异随机以实现公平散射）

### 2.3 专用弹道类型

#### A. ArcProjectile（弧线弹道）
**文件：** ArcProjectile.cs

```csharp
float startingHeight;        // 起始高度
float startingZSpeed;        // 初始 Z 速度
float gravity;               // 重力
Vector3 m_current3DVelocity; // XY + Z 分量
```

- 带重力的 3D 轨迹
- 落地目标指示器
- 地面碰撞检测

#### B. HelixProjectile（螺旋弹道）
**文件：** HelixProjectile.cs

```csharp
float helixWavelength;       // 波长
float helixAmplitude;        // 振幅
```

- 正弦波运动
- 保持前进方向的同时振荡

#### C. 其他专用类型
- **HomingProjectile** - 目标追踪
- **BoomerangProjectile** - 返回发射者
- **BeeProjectile** - 蜂群行为
- **CerebralBoreProjectile** - 锁定钻孔

---

## 3. 物理和碰撞

### 3.1 SpeculativeRigidbody

**文件：** SpeculativeRigidbody.cs (887 lines)

像素完美碰撞的自定义物理引擎：

```csharp
public class SpeculativeRigidbody : BraveBehaviour, ICollidableObject
{
    Vector2 Velocity;                                // 速度
    List<PixelCollider> PixelColliders;              // 多个碰撞形状

    // 碰撞回调
    OnPreRigidbodyCollisionDelegate OnPreRigidbodyCollision;
    OnRigidbodyCollisionDelegate OnRigidbodyCollision;
    OnTileCollisionDelegate OnTileCollision;
    OnBeamCollisionDelegate OnBeamCollision;

    // 特殊属性
    bool ReflectProjectiles;     // 反射弹道
    bool ReflectBeams;           // 反射光束
    bool CanPush/CanBePushed;    // 推动物理
}
```

**碰撞层：**
- `Projectile` - 子弹层
- `EnemyHitBox` / `PlayerHitBox` - 伤害接收器
- `EnemyCollider` / `PlayerCollider` - 运动阻挡器
- `BulletBlocker` / `BulletBreakable` - 墙壁交互

**关键系统：**
- **像素级精确碰撞** 通过 `PixelCollider`（IntVector2 位置）
- **空间分区** 用于性能（b2AABB 包围盒）
- **碰撞例外**（临时/特定忽略列表）
- **运动预测** 使用投机积分

### 3.2 碰撞检测流程

```
Projectile.OnPreCollision() → 过滤器（幽灵玩家，队友）
↓
PhysicsEngine → 空间查询，碰撞测试
↓
Projectile.OnRigidbodyCollision() → 伤害，修改器，效果
↓
修改器检查：
- PierceProjModifier → 穿透敌人继续
- BounceProjModifier → 反弹离开表面
- ExplosiveModifier → 碰撞时爆炸
↓
HandleDamage() → 对 HealthHaver 应用伤害
```

---

## 4. 武器系统

### 4.1 Gun 类

**文件：** Gun.cs (4,363 lines!)

庞大的武器实现：

```csharp
public class Gun : PickupObject
{
    // 齐射系统
    ProjectileVolleyData rawVolley;
    ProjectileVolleyData modifiedVolley;  // 带协同效果

    // 弹药
    int ammo, maxAmmo;
    bool LocalInfiniteAmmo;

    // 换弹
    float reloadTime;
    bool blankDuringReload;  // 换弹时的清屏效果
    ActiveReloadData activeReloadData; // 计时小游戏

    // 射击模式
    bool AppliesHoming;
    bool CanCriticalFire;
    float CriticalChance, CriticalDamageMultiplier;

    // Boss 伤害缩放
    bool UsesBossDamageModifier;
    float CustomBossDamageModifier = 0.8f;

    // 特殊机制
    bool IsHeroSword;          // 近战武器
    bool RequiresFundsToShoot; // 每发子弹花费金钱
    bool UsesRechargeLikeActiveItem; // 基于冷却
}
```

**关键方法：**
- `HandleShootAnimation()` / `HandleShootEffects()`
- `ForceFireProjectile()`
- `ForceImmediateReload()`
- 与 PlayerController 集成

---

## 5. 伤害计算和应用

### 5.1 HealthHaver 组件

**文件：** HealthHaver.cs (300+ lines)

```csharp
public class HealthHaver : BraveBehaviour
{
    float maximumHealth, currentHealth, currentArmor;

    // 伤害修改
    Action<HealthHaver, ModifyDamageEventArgs> ModifyDamage;
    List<DamageTypeModifier> damageTypeModifiers;

    // 无敌
    bool usesInvulnerabilityPeriod;
    float invulnerabilityPeriod;
    bool incorporealityOnDamage; // 受伤后穿透子弹

    // Boss 机制
    BossBarType bossHealthBar;
    float m_bossDpsCap;  // 最大每秒伤害（防止秒杀）

    // 视觉反馈
    bool flashesOnDamage;
    List<Material> materialsToFlash;
}
```

**伤害流程：**
```
Projectile.HandleDamage()
  → 检查 IsVulnerable（无敌帧，动画帧）
  → 从 Projectile.ModifiedDamage 获取基础伤害
  → 应用敌人子弹伤害修正（如果 AI→敌人）
  → 应用穿透伤害缩放（递减返回）
  → 检查 DamageTypeModifiers（抗性/弱点）
  → 应用 Boss DPS 上限（防止爆发秒杀）
  → HealthHaver.ApplyDamage()
      → 先减护甲
      → 然后减生命
      → 触发 OnDamaged 事件
      → 闪烁/震动效果
      → 检查死亡
```

### 5.2 击退系统

**文件：** ActiveKnockbackData.cs

```csharp
public class ActiveKnockbackData
{
    Vector2 knockback, initialKnockback;
    float elapsedTime, curveTime;
    AnimationCurve curveFalloff;  // 击退衰减
    GameObject sourceObject;
    bool immutable;  // 不能被覆盖
}
```

通过 KnockbackDoer 组件应用于敌人 AIActor/PlayerController。

---

## 6. 弹道修改器（装饰器模式）

### 6.1 核心修改器

#### A. PierceProjModifier（穿透）
**文件：** PierceProjModifier.cs

```csharp
int penetration;  // 穿透敌人数量
bool penetratesBreakables;
BeastModeStatus BeastModeLevel;  // 超级穿透模式
```

#### B. BounceProjModifier（反弹）
**文件：** BounceProjModifier.cs

```csharp
int numberOfBounces;                        // 反弹次数
float percentVelocityToLoseOnBounce;       // 反弹速度损失
float damageMultiplierOnBounce;            // 反弹伤害倍数
bool bouncesTrackEnemies;                  // 智能反弹
float bounceTrackRadius;                   // 反弹追踪半径
```

#### C. ExplosiveModifier（爆炸）
**文件：** ExplosiveModifier.cs

```csharp
ExplosionData explosionData;
bool doDistortionWave;  // 屏幕扭曲效果
```

#### D. SpawnProjModifier（生成子弹）
**文件：** SpawnProjModifier.cs (200+ lines)

```csharp
// 飞行中生成弹道
bool spawnProjectilesInFlight;
float inFlightSpawnCooldown;

// 碰撞时生成
enum CollisionSpawnStyle { RADIAL, FLAK_BURST, REVERSE_FLAK_BURST }
int numberToSpawnOnCollison;
```

#### E. 其他修改器
- **HomingModifier** / **LockOnHomingModifier** - 追踪
- **ChainLightningModifier** - 敌人间电弧
- **GoopModifier** - 生成地面危害物
- **ScalingProjectileModifier** - 随时间增长

---

## 7. 状态效果系统

### 7.1 效果实现

#### A. GameActorFireEffect（火焰）
**文件：** GameActorFireEffect.cs

```csharp
public class GameActorFireEffect : GameActorHealthEffect
{
    List<GameObject> FlameVfx;      // 粒子火焰
    int flameNumPerSquareUnit;      // 根据角色大小缩放
    float flameMoveChance;          // 动态位置

    // 覆盖
    void EffectTick()
    {
        // 应用 DOT
        // 生成火星（高着色器质量）
        // 生成火焰粒子
        // 检查角色是否消失
    }
}
```

#### B. GameActorFreezeEffect（冰冻）
**文件：** GameActorFreezeEffect.cs

```csharp
public class GameActorFreezeEffect : GameActorEffect
{
    float FreezeAmount = 10f;          // 累积到 100
    float UnfreezeDamagePercent = 0.333f; // 伤害破冰
    List<GameObject> FreezeCrystals;   // 视觉冰块

    // 冰冻机制：
    // - 累积冰冻量
    // - 到达 100：生成冰晶，禁用 AI，冰冻角色
    // - Boss 上限 75f
    // - 随时间衰减
    // - 死亡时：碎裂效果，如适用则秒杀
}
```

#### C. 其他效果
- `GameActorHealthEffect` - 中毒 DOT
- `GameActorSpeedEffect` - 减速
- `GameActorCharmEffect` - 精神控制
- `GameActorBleedEffect` - 流血 DOT
- `GameActorCheeseEffect` - 奶酪变形
- `GameActorStunEffect` - 暂时禁用

**效果抗性：**
```csharp
public class ActorEffectResistance
{
    EffectResistanceType type;  // Fire, Freeze, Poison 等
    float amount;  // 0-1 (0=免疫, 1=完全效果)
}
```

Boss 有内置抗性：
- 冰冻：0.6-1.0（随应用次数增加）
- 火焰：0.25+ 最小值

---

## 8. 光束武器

### 8.1 BeamController

**文件：** BeamController.cs

```csharp
public abstract class BeamController : BraveBehaviour
{
    GameActor Owner;
    Vector2 Origin, Direction;
    float DamageModifier;

    bool HitsPlayers, HitsEnemies;
    bool knocksShooterBack;  // 后坐力
    float knockbackStrength;

    float usesChargeDelay;
    float chargeDelay;

    // 基于概率的修改器（每次 tick 应用）
    float ChanceBasedHomingRadius;
    float ChanceBasedHomingAngularVelocity;

    abstract void LateUpdatePosition(Vector3 origin);
    abstract void CeaseAttack();
    abstract void DestroyBeam();
}
```

**具体实现：**
- `BasicBeamController` - 直线激光
- `LinkedBeamController` - 在目标间链接
- 光束碰撞使用连续射线投射，而不是弹道物理

---

## 9. 协同系统

### 9.1 ModifyProjectileSynergyProcessor

**文件：** ModifyProjectileSynergyProcessor.cs (244 lines) - 协同处理器示例

当玩家拥有协同时，修改弹道：

```csharp
// 检查协同
if (owner.HasActiveBonusSynergy(SynergyToCheck))
{
    // 着色子弹
    if (TintsBullets) projectile.AdjustPlayerProjectileTint(BulletTint);

    // 添加反弹/穿透
    if (AddsBounces > 0) GetOrAdd<BounceProjModifier>().numberOfBounces += AddsBounces;

    // 添加追踪
    if (AddsHoming) AddComponent<HomingModifier>();

    // 添加状态效果
    if (AddsFire) projectile.AppliesFire = true;

    // 缩放伤害/速度/射程
    projectile.baseData.damage *= DamageMultiplier;
    projectile.baseData.speed *= SpeedMultiplier;

    // 添加爆炸
    if (AddsExplosion) AddComponent<ExplosiveModifier>();

    // 生成子弹道
    if (AddsSpawnedProjectileInFlight) AddComponent<SpawnProjModifier>();
}
```

### 9.2 其他协同处理器

- `VolleyReplacementSynergyProcessor` - 交换整个齐射
- `VolleyModificationSynergyProcessor` - 向齐射添加弹道
- `SubprojectileSynergyProcessor` - 嵌套弹道
- `OnKilledEnemySynergyProcessor` - 击杀时效果
- `FireAdditionalProjectileOnDeathSynergyProcessor` - 死亡时发射
- `AlphabetSoupSynergyProcessor` - 字母弹道
- `ModifyBeamSynergyProcessor` - 光束修改

---

## 10. AI 战斗集成

### 10.1 AIActor 集成

**AIActor** 通过以下方式与战斗集成：

1. **弹道生成** - 通过 BulletScriptSource 发起敌人攻击
2. **伤害接收** - HealthHaver 组件
3. **击退** - KnockbackDoer 处理物理
4. **状态效果** - GameActorEffect 列表
5. **瞄准** - PlayerTracker 用于弹道瞄准
6. **黑幻影模式** - 狂暴敌人（更多生命，黑色子弹）

### 10.2 通过 Brave.BulletScript 的 Boss 模式

- `BulletScriptBehavior` 附加到 Projectile
- 用脚本模式覆盖正常运动
- `Bullet.DestroyType` 用于模式清理
- 集成：`Projectile.braveBulletScript` 字段

---

## 11. 性能优化

### 11.1 关键性能系统

#### A. 弹道池化
```csharp
SpawnManager.Despawn(projectile.gameObject, m_spawnPool);
// 而不是：Destroy(projectile.gameObject)
```

#### B. 空间分区（BraveDynamicTree）
- 通过 AABB 树进行宽相碰撞
- 仅对附近对象进行窄相

#### C. 简单弹道优化
```csharp
bool IsSimpleProjectile =>
    PixelColliders.Count == 1 &&
    CollisionLayer == Projectile &&
    !collidesWithProjectiles;
```
基本子弹的快速路径。

#### D. 碰撞层掩码
- 预计算层过滤的位掩码
- O(1) 碰撞资格检查

#### E. 低差异随机
```csharp
BraveMathCollege.GetLowDiscrepancyRandom(iterator)
```
更公平的散射分布，无聚集。

---

## 12. 关键战斗流程示例：射击枪械

```
1. PlayerController.HandleGunFiring()
   ↓
2. Gun.GetAngleForShot() - 计算散射
   ↓
3. Gun.CreateProjectileFromModule()
   ↓
4. SpawnManager.SpawnProjectile()
   ↓
5. Projectile.Start()
   - 初始化 SpeculativeRigidbody
   - 设置碰撞层
   - 应用玩家子弹修改器（缩放、着色）
   - 向 PhysicsEngine 注册
   ↓
6. Projectile.Update()
   - PreMoveModifiers（追踪等）
   - Move() - 更新速度
   - PhysicsEngine.Movement() - 碰撞检测
     ↓
     OnPreCollision() - 过滤器
     ↓
     OnRigidbodyCollision() - 命中检测
       ↓
       HandleDamage() - 应用伤害
         ↓
         HealthHaver.ApplyDamage()
         ↓
         检查穿透/反弹修改器
       ↓
       HandleHitEffects() - VFX
       ↓
       SpawnProjModifier.SpawnCollisionProjectiles()
       ↓
       ExplosiveModifier.Explode()
       ↓
       HandleDestruction()
   - HandleRange() - 距离检查
   ↓
7. OnDestruction 事件链
   - 生成死亡时子弹脚本
   - 触发协同处理器
   - Despawn 到池
```

---

## 13. 关键设计模式

### 13.1 使用的设计模式

1. **基于组件的架构**
   - Projectile = 基础行为
   - Modifiers = 附加组件（穿透、反弹、爆炸）
   - Effects = 状态应用

2. **对象池**
   - 稳态战斗中零分配
   - SpawnManager 处理生命周期

3. **事件驱动回调**
   - OnHitEnemy, OnWillKillEnemy, OnDestruction
   - 系统间松耦合

4. **策略模式**
   - ProjectileModule.ShootStyle（自动/半自动/连发/充能）
   - CollisionSpawnStyle（径向/散弹）

5. **装饰器模式**
   - ProjectileModifiers 包装基础弹道行为

6. **观察者模式**
   - HealthHaver.OnDamaged 事件
   - Projectile.OnPostUpdate

---

## 14. 架构优势

### 14.1 优势

✅ **高度模块化** - 易于添加新弹道类型/修改器
✅ **性能优化** - 处理屏幕上 200+ 子弹
✅ **数据驱动** - ProjectileData, ExplosionData 可序列化
✅ **可扩展协同** - 物品组合的处理器模式
✅ **强大物理** - 自定义像素完美碰撞
✅ **丰富效果** - 综合状态效果系统

### 14.2 复杂性

**关键复杂性指标：**
- **2,348 行** Projectile 类 - 复杂状态机
- 自定义物理引擎（SpeculativeRigidbody 887 lines）
- 像素完美碰撞检测
- 37 个 BulletScript 依赖
- 100+ 协同定义

---

## 15. 迁移考虑

### 15.1 风险评估：极高

**风险因素：**
- **72 个依赖者** - 游戏玩法核心
- **2,348 行** Projectile - 庞大的核心类
- 自定义物理引擎（SpeculativeRigidbody）
- 像素完美碰撞精度
- 与 BulletScript 紧密耦合
- 协同系统复杂性

### 15.2 必须保留

1. **弹道轨迹计算**（精确物理）
2. **碰撞检测精度**（像素完美）
3. **伤害公式**（游戏平衡）
4. **Boss DPS 上限**
5. **协同处理逻辑**
6. **状态效果机制**

### 15.3 依赖优先级

**必须先移植：**
1. Brave.BulletScript（Boss 模式）
2. Dungeonator（空间碰撞）
3. Core.Core（基类）
4. FullInspector（数据序列化）

### 15.4 迁移策略

**选项 A：完整移植（推荐）**
- 保留所有战斗逻辑
- 替换 Unity 特定代码
- 移植自定义物理引擎
- 估计工作量：极高

**选项 B：简化重写**
- 使用标准物理引擎
- 简化修改器系统
- 风险：改变游戏手感
- 估计工作量：高

---

## 16. 文件统计摘要

### 16.1 顶层组织

```
Combat/
├── Projectiles/ (139 files, 80.8%)
│   ├── 专用类型（Arc, Helix, Homing, Boomerang）
│   ├── 修改器（Pierce, Bounce, Explosive, Spawn, Homing）
│   ├── 协同处理器（12 files）
│   └── Boss 专用弹道（BossFinal*）
├── Effects/ (31 files, 18.0%)
│   ├── 状态效果（Fire, Freeze, Poison, Stun）
│   ├── 增益/减益（Speed, Health）
│   └── 视觉效果（屏幕着色器）
└── Damage/ (2 files, 1.2%)
    ├── ActiveKnockbackData
    └── ActiveReloadData
```

### 16.2 关键外部依赖

- SpeculativeRigidbody (Core.Core.Framework)
- HealthHaver (Core.Core.Framework)
- Gun (Core.Items.Pickups)
- AIActor (Core.Actors.Enemy)
- BeamController (Core.Core.Framework)
- BulletScriptBehavior (Brave.BulletScript)

---

## 17. 性能分析

### 17.1 每帧成本

**典型战斗场景：**
- 屏幕上 100-200 个活动弹道
- 每个弹道：
  - 1x `Update()` 调用
  - 1x 物理更新
  - 1-2x 碰撞检测
  - 修改器处理

**估计 CPU 成本：**
- 简单弹道：~0.03ms/弹道
- 复杂弹道：~0.08ms/弹道
- 总计：~6-16ms/帧（200 弹道）
- 占 16.67ms（60 FPS）的 36-96%

### 17.2 优化技术

**已实现优化：**
1. 对象池（零分配稳态）
2. 空间分区（BraveDynamicTree）
3. 简单弹道快速路径
4. 碰撞层掩码预计算
5. 低差异随机（更公平散射）

**瓶颈：**
1. 像素完美碰撞（计算密集）
2. 修改器遍历（复杂弹道）
3. 协同处理器（每发射）

---

## 18. 总结

Combat 系统是 **代码库中最复杂的子系统**，实现了一个高度调优的弹幕地狱战斗引擎，包含：

- 像素完美自定义物理
- 丰富的弹道修改器系统
- 综合状态效果
- 深度协同交互
- 为密集弹道模式优化的性能

其 172 个文件和 2,348 行 Projectile 核心使其 **在任何迁移期间必须精确保留** - 游戏的平衡和手感依赖于精确的物理和伤害计算。

**架构总结：**
```
Projectile (2,348 lines) - 核心战斗引擎
  ├─> ProjectileModule - 射击模式
  ├─> ProjectileModifier[] - 装饰器链
  ├─> SpeculativeRigidbody - 自定义物理
  ├─> HealthHaver - 伤害应用
  └─> GameActorEffect[] - 状态效果

Gun (4,363 lines) - 武器系统
  ├─> ProjectileVolleyData - 齐射
  ├─> SynergyProcessor[] - 协同
  └─> ActiveReloadData - 换弹小游戏

SpeculativeRigidbody (887 lines) - 物理引擎
  ├─> PixelCollider[] - 碰撞形状
  ├─> PhysicsEngine - 空间分区
  └─> 碰撞回调链
```

这个系统是 **Enter the Gungeon 游戏玩法的基础** - 一个关于如何构建高性能、功能丰富的战斗系统的大师级课程。

---

**文档版本：** 1.0
**最后更新：** 2026-01-18
**分析代理 ID：** a3a5947
