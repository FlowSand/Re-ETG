# Task-02: 命名空间到目录映射表

**创建时间：** 2026-01-17
**迁移状态：** 已完成
**总文件数：** 2,323个文件

---

## 命名空间层次结构

### ETG.Core.Actors (167文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.Actors.Player` | `Core/Actors/Player/` | 1 | 玩家控制器和状态 |
| `ETG.Core.Actors.Enemy` | `Core/Actors/Enemy/` | 17 | AIActor子类、敌人逻辑 |
| `ETG.Core.Actors.Companions` | `Core/Actors/Companions/` | 0 | 伙伴系统（未使用） |
| `ETG.Core.Actors.Behaviors` | `Core/Actors/Behaviors/` | 149 | AI行为树、攻击模式 |

### ETG.Core.Items (175文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.Items.Guns` | `Core/Items/Guns/` | 6 | Gun子类、枪械修饰器 |
| `ETG.Core.Items.Passive` | `Core/Items/Passive/` | 102 | PassiveItem子类 |
| `ETG.Core.Items.Active` | `Core/Items/Active/` | 55 | ActiveItem/PlayerItem子类 |
| `ETG.Core.Items.Pickups` | `Core/Items/Pickups/` | 12 | 拾取物：弹药、货币、心 |

### ETG.Core.Combat (172文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.Combat.Projectiles` | `Core/Combat/Projectiles/` | 139 | Projectile层次、子弹行为 |
| `ETG.Core.Combat.Damage` | `Core/Combat/Damage/` | 2 | 伤害系统、爆炸、击退 |
| `ETG.Core.Combat.Effects` | `Core/Combat/Effects/` | 31 | 状态效果、Buff/Debuff |

### ETG.Core.Dungeon (125文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.Dungeon.Generation` | `Core/Dungeon/Generation/` | 3 | 地牢生成、流程定义 |
| `ETG.Core.Dungeon.Rooms` | `Core/Dungeon/Rooms/` | 23 | 房间控制器、修饰器 |
| `ETG.Core.Dungeon.Interactables` | `Core/Dungeon/Interactables/` | 99 | 宝箱、神龛、NPC、商店 |

### ETG.Core.UI (100文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.UI.Ammonomicon` | `Core/UI/Ammonomicon/` | 6 | 图鉴系统 |
| `ETG.Core.UI.HUD` | `Core/UI/HUD/` | 94 | HUD元素、Boss卡片、动作栏 |
| `ETG.Core.UI.Menus` | `Core/UI/Menus/` | 0 | 菜单系统（未使用） |

### ETG.Core.VFX (67文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.VFX.Animation` | `Core/VFX/Animation/` | 67 | 精灵动画、拖尾渲染 |
| `ETG.Core.VFX.Particles` | `Core/VFX/Particles/` | 0 | 粒子系统（未使用） |
| `ETG.Core.VFX.Rendering` | `Core/VFX/Rendering/` | 0 | 渲染效果（未使用） |

### ETG.Core.Audio (150文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.Audio.Integration` | `Core/Audio/Integration/` | 150 | Wwise音频引擎集成 (Ak*) |

### ETG.Core.Systems (990文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.Systems.Management` | `Core/Systems/Management/` | 40 | *Manager单例类 |
| `ETG.Core.Systems.Data` | `Core/Systems/Data/` | 84 | 数据库、配置、存档 |
| `ETG.Core.Systems.Utilities` | `Core/Systems/Utilities/` | 866 | 辅助类、扩展、工具 |

### ETG.Core.Core (377文件)

| 命名空间 | 目录路径 | 文件数 | 描述 |
|---------|---------|--------|------|
| `ETG.Core.Core.Framework` | `Core/Core/Framework/` | 285 | BraveBehaviour、GameActor基类 |
| `ETG.Core.Core.Interfaces` | `Core/Core/Interfaces/` | 19 | 核心接口定义 |
| `ETG.Core.Core.Enums` | `Core/Core/Enums/` | 73 | 核心枚举类型 |

---

## 分类规则优先级

### 优先级1: 文件名模式（高置信度）
- `*Manager.cs` → `ETG.Core.Systems.Management`
- `*Database.cs` → `ETG.Core.Systems.Data`
- `Ammonom*.cs` → `ETG.Core.UI.Ammonomicon`
- `Ak*.cs`, `*Audio*.cs` → `ETG.Core.Audio.Integration`
- `Actionbars*.cs` → `ETG.Core.UI.HUD`
- `*Shrine*.cs`, `*Shop*.cs`, `*NPC*.cs` → `ETG.Core.Dungeon.Interactables`

### 优先级2: 继承层次（高置信度）
- `: PlayerController` → `ETG.Core.Actors.Player`
- `: AIActor` → `ETG.Core.Actors.Enemy`
- `: GameActor` → `ETG.Core.Actors.Enemy`
- `: Gun` → `ETG.Core.Items.Guns`
- `: PassiveItem` → `ETG.Core.Items.Passive`
- `: ActiveItem` → `ETG.Core.Items.Active`
- `: PlayerItem` → `ETG.Core.Items.Active`
- `: PickupObject` → `ETG.Core.Items.Pickups`
- `: Projectile` → `ETG.Core.Combat.Projectiles`
- `: *Behavior` → `ETG.Core.Actors.Behaviors`
- `: DungeonPlaceableBehaviour` → `ETG.Core.Dungeon.Interactables`
- `: SpecificIntroDoer` → `ETG.Core.Dungeon.Interactables`
- `: RobotRoomFeature` → `ETG.Core.Dungeon.Rooms`
- `: ChallengeModifier` → `ETG.Core.Systems.Data`
- `: dfControl`, `: dfMarkupTag` → `ETG.Core.UI.HUD`
- `: BraveBehaviour` → `ETG.Core.Core.Framework`
- `: TimeInvariantMonoBehaviour` → `ETG.Core.Core.Framework`

### 优先级3: 关键字分析（中置信度）
- 包含 `Projectile` → `ETG.Core.Combat.Projectiles`
- 包含 `Effect/Buff/Debuff` → `ETG.Core.Combat.Effects`
- 包含 `VFX/Particle/Animation` → `ETG.Core.VFX.Animation`
- 包含 `Behavior` → `ETG.Core.Actors.Behaviors`
- 包含 `Room/Dungeon` + `Controller` → `ETG.Core.Dungeon.Rooms`
- `Prototype*Dungeon*` → `ETG.Core.Dungeon.Generation`
- 以 `*Data` 结尾 → 按上下文细分
  - 战斗Data → `ETG.Core.Combat.Damage`
  - 物品Data → `ETG.Core.Items.Guns`
  - 游戏Data → `ETG.Core.Systems.Data`

### 优先级4: 类型回退（低/中置信度）
- `: Attribute` → `ETG.Core.Core.Framework` (中)
- `: ScriptableObject` → `ETG.Core.Systems.Data` (中)
- `interface I*` → `ETG.Core.Core.Interfaces` (中)
- `enum *` → `ETG.Core.Core.Enums` (中)
- `: MonoBehaviour` → `ETG.Core.Systems.Utilities` (低)
- `: Script` → `ETG.Core.Systems.Utilities` (低)
- 默认 → `ETG.Core.Systems.Utilities` (低)

---

## 分类统计

| 置信度 | 文件数 | 百分比 | 说明 |
|--------|--------|--------|------|
| **高置信度** | 1,003 | 43.2% | 继承层次或文件名明确 |
| **中置信度** | 451 | 19.4% | 关键字或类型推断 |
| **低置信度** | 869 | 37.4% | 回退分类（需审核） |
| **总计** | 2,323 | 100.0% | |

### 低置信度文件分布
- `ETG.Core.Systems.Utilities`: 866文件（大部分为通用MonoBehaviour）
- `ETG.Core.Items.Passive`: 3文件

---

## 迁移验证

### 验证通过项 ✓
- [x] 根目录已清空（0个.cs文件）
- [x] Core/目录包含2,323个.cs文件
- [x] 所有预期子目录都存在
- [x] 命名空间声明正确（抽样检查）
- [x] 所有.meta文件完整（GUID保持不变）
- [x] 第三方库未受影响
- [x] 文件数量精确匹配

### 命名空间添加示例

```csharp
// BEFORE（根目录文件）
using UnityEngine;
#nullable disable
public class ActiveBasicStatItem : PlayerItem { }

// AFTER（添加命名空间）
using UnityEngine;
#nullable disable

namespace ETG.Core.Items.Active
{
    public class ActiveBasicStatItem : PlayerItem { }
}
```

---

## 使用指南

### 查找特定功能
- **玩家相关** → `Core/Actors/Player/`, `Core/Actors/Behaviors/`
- **敌人AI** → `Core/Actors/Enemy/`, `Core/Actors/Behaviors/`
- **物品系统** → `Core/Items/**`
- **武器** → `Core/Items/Guns/`, `Core/Combat/Projectiles/`
- **地牢生成** → `Core/Dungeon/Generation/`, `Core/Dungeon/Rooms/`
- **交互对象** → `Core/Dungeon/Interactables/`
- **UI** → `Core/UI/**`
- **管理器** → `Core/Systems/Management/`
- **数据结构** → `Core/Systems/Data/`
- **基类/框架** → `Core/Core/Framework/`

### 命名空间使用

```csharp
// 引用其他模块
using ETG.Core.Actors.Player;
using ETG.Core.Items.Guns;
using ETG.Core.Combat.Projectiles;

// 当前文件的命名空间
namespace ETG.Core.Systems.Management
{
    public class GameManager : MonoBehaviour { }
}
```

---

## 已知问题

### 高密度回退目录
- **Core/Systems/Utilities/** (866文件)
  **原因：** 包含大量通用MonoBehaviour、Boss战斗行为类、辅助工具
  **建议：** Task-05代码可读性清洗阶段进一步细分

### 空目录
以下命名空间已在架构中预留但当前无文件：
- `ETG.Core.Actors.Companions` (0文件)
- `ETG.Core.UI.Menus` (0文件)
- `ETG.Core.VFX.Particles` (0文件)
- `ETG.Core.VFX.Rendering` (0文件)

---

## 下一步（Task-03/Task-04）

1. **打开Unity验证编译**
2. **修复依赖错误**（命名空间解析、using语句）
3. **创建平台SDK存根**（Task-03）
4. **迭代修复编译错误**（Task-04）

---

**Task-02状态：** 完成 ✓
**Git提交：** 1e6622b6
**验证日期：** 2026-01-17
