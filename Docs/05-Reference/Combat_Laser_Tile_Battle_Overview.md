# Re-ETG 战斗系统、激光效果与地块拼接梳理

## 范围与资料来源
- 本文聚焦 ETG.Runtime 内的 Brave.BulletScript、Core.Core、Core.Systems、Core.Combat，以及与激光/地块拼接相关的关键实现。
- 参考了工程内的模块清单与模块边界文档，以及对应核心代码文件。

## 总体结构概览（模块视角）
- Core.Core：基础框架与物理壳（GameActor、SpeculativeRigidbody 等）。
- Core.Systems：管理/数据库/工具层（GameManager、Boss/Enemy/Item 数据库等）。
- Core.Combat：投射物、伤害、状态效果与战斗表现。
- Brave.BulletScript：弹幕 DSL，负责脚本化弹幕的时序与发射。

## 弹幕系统（Brave.BulletScript + Core 集成）
### 1) DSL 核心对象
- Bullet：弹幕“脚本实例 + 运行状态”容器。内部以 1/60 秒为理想帧步长推进 Tick，运行任务队列并更新速度/位置。
- Direction/Speed/Offset：发射参数组合，用于实现 Aim/Relative/Sequence 等方向与速度逻辑，并支持位移与旋转偏移。
- IBulletManager：Bullet 的宿主接口，提供玩家位置/速度、Transform 解析、子弹生命周期管理。

### 2) 执行与生命周期
- BulletScriptSource：RootBullet 容器，Update 时驱动 RootBullet.FrameUpdate（并确保单帧只更新一次）。
- AIBulletBank：IBulletManager 的主要实现，负责根据 bullet entry 创建 Projectile，并挂接 BulletScriptBehavior。
- BulletScriptBehavior：运行中将 Bullet 的预测位置写回到 SpeculativeRigidbody/Transform，保证弹幕脚本与物理同步。
- BulletScriptSelector：通过类型名反射实例化 Script/ScriptLite 派生类，供 AI/行为树引用。

### 3) 触发路径（AI/行为层）
AIShooter.ShootBulletScript -> BulletScriptSource.Initialize -> BulletScriptSelector.CreateInstance -> RootBullet.Initialize。

### 4) 典型弹幕脚本示例
TridentShot1：连续 Fire 三发 Aim 弹，体现 DSL 的方向/速度组合用法。

## 激光效果（Beam + 激光瞄具）
### 1) 激光瞄具（Laser Sight）
- Gun.HandleAimRotation 中按需创建激光瞄具 VFX（tk2dTiledSprite）。
- Raycast 计算碰撞距离，动态调整激光长度；支持双宽（DoubleWide）模式。

### 2) Beam 发射与生命周期
- Gun.BeginFiringBeam：Spawn beam projectile，绑定 BeamController，写入 m_activeBeams 列表。
- BeamController：抽象基类，维护 Owner/Origin/Direction 等通用状态与 chance tick 逻辑。
- BasicBeamController：具体 Beam 实现，Start 中初始化 tk2dTiledSprite、muzzle/impact，并进入 Charging/Telegraphing/Firing 等状态机。
- Projectile.Start：若 Projectile 带 BeamController，则禁用默认更新，由 Beam 控制渲染/碰撞逻辑。
- AIBeamShooter：AI 激光发射控制器，负责开火/停止与 LateUpdate 中的激光位置更新。

### 3) Beam 形态与碰撞
- BeamBone 负责轨迹骨骼段与可选的 homing；BeamCollisionType/BeamTileType 定义碰撞与贴图生长方式。

## 地块拼接（Tilemap Stitching）
### 1) 触发与重建
- Dungeon.DestroyWallAtPosition / ConstructWallAtPosition：改变 cell 类型后，调用 assembler 清理并重建周围 tile。
- Dungeon.RebuildTilemap：触发 tk2dTileMap.Build 重新生成网格。

### 2) 拼接主流程
- TK2DDungeonAssembler.BuildTileIndicesForCell：核心拼接逻辑，顺序处理 floor/deco/edge/border/collision/shadow/pit 等层。

### 3) 拼接规则数据
- TileIndices：tileset 与各类 TileIndexGrid 入口（decal/pattern/edge decoration 等）。
- TileIndexGrid：根据邻接边界选择 tile index，支持八方向/四方向与“benubbed”规则。

### 4) 装饰/Stamp 机制
- DungeonTileStampData：tile/sprite/object stamp 数据定义。
- TK2DInteriorDecorator：按 placement rule 选择 stamp，并调用 ApplyStampGeneric 放置。

## 战斗系统框架（粗略）
### 1) 模块职责分层
- Core.Core：Actor 基类 + 物理（SpeculativeRigidbody/PixelCollider）。
- Core.Systems：管理器与数据库（GameManager、Boss/Enemy/Item/协同数据）。
- Core.Combat：Projectile/伤害/状态效果与战斗结算。
- Core.Items：武器系统（Gun/Volley/Beam 发射入口）。
- Brave.BulletScript：弹幕 DSL，依赖 AIBulletBank 等 Core 桥接。

### 2) 典型战斗流程（玩家/AI）
1. 触发：Gun/AI 行为 -> Projectile 或 BulletScript 生成。
2. 运行：Projectile/Beam 更新 + PhysicsEngine 碰撞。
3. 结算：Damage/Effects -> HealthHaver 与状态效果。
4. 数据驱动：伤害/协同/配置来自 Core.Systems 数据库。

## 关键对象速查（责任与入口）
- Bullet：弹幕脚本运行体，负责时序与子弹发射。
- AIBulletBank：弹幕与 Projectile 的桥接器，负责创建 Projectile 并挂脚本行为。
- BulletScriptSource/BulletScriptSelector：弹幕脚本实例化与 RootBullet 驱动入口。
- Projectile：核心投射物类型，承载伤害/状态/渲染与碰撞。
- BeamController/BasicBeamController：激光武器控制与渲染碰撞实现。
- TK2DDungeonAssembler：tilemap 拼接与层级构建入口。
- TileIndexGrid/DungeonTileStampData：拼接规则与装饰数据来源。

## 参考代码位置
- Brave.BulletScript 概况：`Assets/Scripts/ETG/Brave/MODULE_BOUNDARY.md:1`
- 弹幕核心（Bullet/Fire/FrameUpdate）：`Assets/Scripts/ETG/Brave/BulletScript/Bullet.cs:104`
- 弹幕发射参数（Direction/Speed/Offset）：`Assets/Scripts/ETG/Brave/BulletScript/Direction.cs:6`
- 弹幕宿主接口（IBulletManager）：`Assets/Scripts/ETG/Brave/BulletScript/IBulletManager.cs:6`
- AIBulletBank 创建与绑定：`Assets/Scripts/ETG/Core/Core/Framework/AIBulletBank.cs:145`
- BulletScriptBehavior 同步逻辑：`Assets/Scripts/ETG/Core/Core/Framework/BulletScriptBehavior.cs:7`
- BulletScriptSource RootBullet 驱动：`Assets/Scripts/ETG/Core/Core/Framework/BulletScriptSource.cs:7`
- BulletScriptSelector 反射创建：`Assets/Scripts/ETG/Core/Systems/Utilities/BulletScriptSelector.cs:15`
- AI 触发弹幕：`Assets/Scripts/ETG/Core/Core/Framework/AIShooter.cs:211`
- 弹幕脚本示例：`Assets/Scripts/ETG/Core/Systems/Utilities/TridentShot1.cs:7`
- 激光瞄具：`Assets/Scripts/ETG/Core/Items/Pickups/Gun.cs:1501`
- Beam 发射入口：`Assets/Scripts/ETG/Core/Items/Pickups/Gun.cs:3613`
- Beam 抽象基类：`Assets/Scripts/ETG/Core/Core/Framework/BeamController.cs:10`
- Beam 具体实现与状态机：`Assets/Scripts/ETG/Core/Systems/Utilities/BasicBeamController.cs:264`
- Projectile 对 Beam 的特判：`Assets/Scripts/ETG/Core/Core/Framework/Projectile.cs:424`
- AI 激光发射：`Assets/Scripts/ETG/Core/Core/Framework/AIBeamShooter.cs:122`
- 地块重建入口：`Assets/Scripts/ETG/Dungeonator/Dungeon.cs:330`
- tilemap 拼接主流程：`Assets/Scripts/ETG/Core/Systems/Utilities/TK2DDungeonAssembler.cs:237`
- TileIndexGrid 规则：`Assets/Scripts/ETG/Core/Systems/Data/TileIndexGrid.cs:177`
- Stamp 数据：`Assets/Scripts/ETG/Core/Systems/Data/DungeonTileStampData.cs:9`
- Stamp 放置：`Assets/Scripts/ETG/Core/Systems/Utilities/TK2DInteriorDecorator.cs:51`
