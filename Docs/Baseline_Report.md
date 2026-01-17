# Baseline Report - Enter the Gungeon Unity代码库清洗基线报告

**报告日期:** 2026-01-17
**Unity目标版本:** 2022.3 LTS
**反编译工具:** JetBrains decompiler
**源程序集:** Assembly-CSharp.dll (主程序集)

---

## 1. 概览摘要

本项目是对"Enter the Gungeon"游戏的反编译Unity C#代码库进行工程化清洗。代码已从`Assembly-CSharp.dll`成功反编译，但需要全面清洗才能在Unity 2022.3中实现零编译错误并建立专业代码组织结构。

**当前状态:**
- 4,051个C#源文件 (35 MB)
- 57个目录（扁平结构，未按命名空间对齐）
- 位置: `D:\Github\Re-ETG\UnityProject\Assets\Scripts\ETG\`
- 私有实现存在中高度混淆
- 公共API清晰可读

**核心挑战:** 将反编译代码转化为可维护的Unity工程，实现零编译错误。

---

## 2. 输入源清单

### 2.1 反编译源代码
- **位置:** `D:\Github\Re-ETG\UnityProject\Assets\Scripts\ETG\`
- **文件总数:** 4,051个 `.cs` 文件
- **总大小:** 35 MB
- **反编译工具:** JetBrains decompiler (文件头标识)
- **源程序集:** Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- **Assembly MVID:** E27C5245-924B-4031-BFBB-14AA632E24E2

### 2.2 Managed DLL目录
- **位置:** `D:\Github\Re-ETG\Managed\`
- **DLL总数:** 72个文件
- **总大小:** ~20 MB
- **分类:**
  - 游戏程序集: 2个DLL
  - 第三方框架: 6个DLL
  - .NET Framework: 8个DLL
  - Unity内置模块: 52个DLL
  - 废弃运行时: 3个DLL (UnityScript/Boo)

详见 `Managed_Dll_List.txt` 完整清单。

### 2.3 Unity版本
- **目标版本:** Unity 2022.3 LTS
- **原始版本(推测):** Unity 2017.x-2019.x (基于模块签名)
- **版本跨度:** 显著 (3-5年的API变化)

---

## 3. 代码库统计

### 3.1 文件分布

| 类别 | 数量 | 备注 |
|------|------|------|
| 总C#文件数 | 4,051 | 全部反编译源码 |
| 根级文件 | 2,323 | 游戏特定脚本(无命名空间或全局命名空间) |
| MonoBehaviour类 | 531 | Unity组件 |
| ScriptableObject类 | 47 | Unity数据资源 |
| 泛型类 | 210+ | 模板/泛型类型 |
| Editor相关文件 | 21 | 带ExecuteInEditMode属性 |

### 3.2 目录结构

**顶层目录 (主要20个):**
```
_003CPrivateImplementationDetails_003E{...}/  (3个目录 - 编译器生成)
AK/                                           (音频中间件 - Wwise)
Brave/                                        (弹幕脚本框架)
BraveDynamicTree/                            (物理/空间)
DaikonForge/                                 (UI/补间框架)
Dungeonator/                                 (地牢生成系统)
DungeonGenUtility/                           (地牢生成工具)
FullInspector/                               (序列化/Inspector框架)
FullSerializer/                              (对象序列化)
HutongGames/                                 (PlayMaker可视化脚本)
InControl/                                   (输入管理系统)
Pathfinding/                                 (A*寻路)
Kvant/                                       (视觉特效)
tk2dRuntime/                                 (2D Toolkit)
Reactive/                                    (响应式扩展)
[及其他框架目录...]
```

**当前问题:** 目录结构与命名空间组织不一致。

### 3.3 命名空间分布

**主要命名空间统计:**

| 命名空间 | 文件数 | 说明 |
|---------|--------|------|
| HutongGames.PlayMaker.Actions | 874 | 可视化脚本动作库 |
| InControl | 254 | 输入系统核心 |
| InControl.NativeProfile | 136 | 游戏手柄配置文件 |
| FullInspector | 188 | 自定义Inspector/序列化 |
| Dungeonator | 67 | 地牢生成系统 |
| Brave.BulletScript | 14 | 弹幕模式引擎 |
| DaikonForge.Tween | 48 | 动画/补间系统 |
| Pathfinding | ~30 | A*导航 |
| AK (Wwise Audio) | ~80 | 音频中间件绑定 |
| (全局命名空间) | 2,323+ | 游戏逻辑(无命名空间) |

**命名空间分析:**
- 组织良好的第三方库有正确的命名空间
- 核心游戏代码(2,323个文件)没有命名空间
- 私有实现细节使用转义unicode命名空间

---

## 4. 代码质量评估

### 4.1 混淆等级: 中等偏高

**混淆指标:**

1. **转义Unicode名称 (30,389+处)**
   - 模式: `\u003C...\u003E` (表示unicode转义的 `<...>`)
   - 示例:
     ```csharp
     return (IEnumerator) new ActiveGunVolleyModificationItem.\u003CHandleDuration\u003Ec__Iterator0()
     return (IEnumerator) new AdditionalBraveLight.\u003CStart\u003Ec__Iterator0()
     ```
   - 这些是编译器生成的迭代器/闭包类
   - **影响:** 内部实现难以阅读，但不影响公共API

2. **编译器生成的私有类型**
   - 3个混淆名称目录:
     - `_003CPrivateImplementationDetails_003E{017812A5-3652-47BD-840C-29D05E4D7339}/`
     - `_003CPrivateImplementationDetails_003E{DE5600AD-6212-4D84-9A32-9D951E3289D1}/`
     - `_003CPrivateImplementationDetails_003E{ea0bc259-5423-40a1-bdac-df73f0367646}/`
   - 包含混淆器标记: `__BB_OBFUSCATOR_VERSION_1_16_1_2.cs`
   - **检测到的混淆器:** BabelObfuscator v1.16.1.2

3. **公共API质量: 良好**
   - 公共类名: 清晰描述性 (例: `AIActor`, `Achievement`, `Dungeonator`)
   - 公共成员名: 可读 (例: `MovementSpeed`, `CollisionDamage`)
   - MonoBehaviour字段: 命名良好 (对Unity序列化至关重要)

4. **私有实现: 较差**
   - 大量使用编译器生成的闭包
   - Lambda/LINQ表达式转换为嵌套类
   - 迭代器方法(`IEnumerator`)生成复杂状态机

**整体评估:** 公共表面整洁；内部实现混淆但结构健全。

### 4.2 第三方库识别

**已确认的第三方库:**

| 库名 | 文件数 | 用途 | 来源 |
|-----|--------|------|------|
| PlayMaker | 874 | 可视化脚本FSM | Unity Asset Store |
| InControl | 390 | 输入管理 | Unity Asset Store / GitHub |
| FullInspector | 188 | 自定义Inspector框架 | Asset Store |
| Dungeonator | 67 | 程序化地牢生成 | 自研(游戏特定?) |
| DaikonForge | 48 | UI/补间框架 | Asset Store (旧版) |
| Brave.BulletScript | 14 | 弹幕模式DSL | 自研 |
| Pathfinding (A* Project) | ~30 | 导航/寻路 | Asset Store (Aron Granberg) |
| Wwise (AK.*) | ~80 | 音频中间件 | Audiokinetic SDK |

**风险因素:**
- 许多Asset Store包可能有许可限制
- 部分框架已废弃或不再维护
- 需要决策每个库的策略: 原始DLL vs 源代码

---

## 5. UnityEditor依赖分析

### 5.1 直接UnityEditor引用

**搜索结果:**
```
using UnityEditor: 0个文件
#if UNITY_EDITOR: 0个文件
```

**结论:** 反编译代码中未检测到直接的UnityEditor API使用。

### 5.2 ExecuteInEditMode特性

**发现:** 21个文件带有 `[ExecuteInEditMode]` 特性

**示例:**
- `tk2dEditorSpriteDataUnloader.cs` (精灵数据管理)
- `AkInitializer.cs` (Wwise音频初始化)
- `AkGameObj.cs`, `AkBank.cs`, `AkEnvironment.cs` (音频组件)
- `DaikonForge/Tween/SplineObject.cs`, `SplineNode.cs` (编辑器可视化)
- `InControl/TouchManager.cs` (输入测试)
- `Kvant/Tunnel.cs` (程序化特效)

**风险:** 低
- `[ExecuteInEditMode]` 运行时兼容 (编辑器和构建版本都运行)
- 不会阻塞运行时编译
- 可能需要验证行为不会导致仅编辑器的错误

### 5.3 Editor命名文件

**模式:** 名称中包含"Editor"的文件: 12个

**关键文件:**
- `FullInspector/tkControlEditor.cs` - 非UnityEditor依赖(自定义编辑器抽象)
- `FullInspector/Internal/fiValueProxyEditor.cs` - 运行时编辑器逻辑
- `DaikonForge/Editor/Inspector*.cs` - 可能需要条件编译

**风险:** 中等
- 需要验证每个文件的实际UnityEditor使用情况
- 可能需要 `#if UNITY_EDITOR` 包装或移动到Editor/文件夹

**UnityEditor引用比例: 0%**

---

## 6. 风险评估

### 6.1 高优先级风险

| 风险项 | 影响 | 缓解策略 |
|--------|------|---------|
| **废弃Unity脚本依赖** | 阻塞 | 移除/替换Boo & UnityScript DLL - Unity 2022不支持 |
| **缺失第三方DLL** | 重大 | 策略: 引入DLL OR 引用包 OR 保留反编译源 |
| **命名空间→目录不匹配** | 重大 | Task-02: 将4,051个文件重组织到命名空间对齐的目录 |
| **平台特定SDK** | 中等 | Sony/GOG DLL可能编译失败 - 创建shims或条件编译 |

### 6.2 中等风险

| 风险项 | 影响 | 缓解策略 |
|--------|------|---------|
| **Unity API版本变化** | 中等 | 预期2017→2022 API废弃导致的编译错误 |
| **混淆名称** | 可读性 | Task-05: 在安全的地方清理可读的私有成员 |
| **潜在Editor依赖** | 低-中等 | 验证21个ExecuteInEditMode文件 + 12个Editor命名文件 |

### 6.3 低风险

| 风险项 | 影响 | 缓解策略 |
|--------|------|---------|
| **MonoBehaviour字段重命名** | 序列化 | 不要重命名 - Unity序列化需要精确名称 |
| **泛型类型编译** | 低 | 210+泛型类可能有边缘情况 |

---

## 7. 依赖策略建议

### 7.1 第三方框架

**选项A: 保留反编译源码 (本项目推荐)**
- 优点: 完全控制，无许可歧义，已反编译
- 缺点: 维护负担
- **适用于:** PlayMaker, InControl, FullInspector, DaikonForge, Pathfinding

**选项B: 引用原始DLL**
- 优点: 更简单，官方支持
- 缺点: 可能与反编译版本不完全匹配
- **适用于:** Wwise Audio (AK.*) - 使用官方Unity集成

**选项C: 替换为现代等价物**
- 优点: 更好的Unity 2022支持
- 缺点: 需要大量重构
- **适用于:** (初期不采用 - 保留为备用方案)

### 7.2 平台SDK

**GOG Galaxy SDK (`GalaxyCSharp.dll`):**
- 条件编译: `#if GOG_GALAXY`
- 非GOG构建使用Stub

**Sony SDK (`SonyNP.dll`, `SonyPS4SavedGames.dll`):**
- 条件编译: `#if UNITY_PS4 || UNITY_PS5`
- 非Sony平台使用Stub

### 7.3 废弃运行时

**UnityScript/Boo (3个DLL):**
- **行动:** 完全移除 - Unity 2022.3不支持
- **风险:** 如果游戏代码实际使用UnityScript，需要转译到C#
- **当前扫描:** 未发现 `.js` 或 `.boo` 文件 - 可能安全移除

---

## 8. 下一步行动 (Task-01)

**Task-01目标:** 创建Unity承载工程骨架

**预期操作:**
1. 为主要模块创建Assembly Definition文件 (`.asmdef`)
2. 建立编译边界
3. 设置正确的依赖图
4. 为命名空间重组做准备

**需要决策的关键点:**
- Assembly粒度 (单体 vs 模块化)
- 第三方库处理 (源码 vs DLL)
- 平台特定条件编译策略

---

## 9. 附录

### 9.1 反编译器元数据 (示例)

每个文件包含标准头:
```csharp
// Decompiled with JetBrains decompiler
// Type: <FullTypeName>
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll
```

### 9.2 公共API示例 (良好质量)

来自 `AIActor.cs`:
```csharp
public class AIActor : GameActor, IPlaceConfigurable
{
  [Header("Core Enemy Stats")]
  public bool IsNormalEnemy = true;
  public float MovementSpeed = 2f;
  public float CollisionDamage = 1f;
  // ... 结构良好，可读
}
```

### 9.3 混淆代码示例 (可读性差)

```csharp
return (IEnumerator) new ActiveGunVolleyModificationItem.\u003CHandleDuration\u003Ec__Iterator0()
{
  \u003C\u003Ef__this = this
};
```

### 9.4 命令参考

**文件统计:**
```bash
cd D:\Github\Re-ETG\UnityProject\Assets\Scripts\ETG
find . -name "*.cs" | wc -l                    # 总计: 4051
find . -type d | wc -l                         # 目录: 57
du -sh .                                       # 大小: 35M
```

**命名空间分析:**
```bash
find . -name "*.cs" -exec grep -o "^namespace [^{]*" {} \; | sort | uniq -c | sort -rn
```

**MonoBehaviour/ScriptableObject计数:**
```bash
find . -name "*.cs" -exec grep -l "MonoBehaviour" {} \; | wc -l     # 531
find . -name "*.cs" -exec grep -l "ScriptableObject" {} \; | wc -l  # 47
```

---

## 10. 验收检查清单

- [x] 文件总数已记录 (4,051)
- [x] 命名空间分布已分析 (HutongGames: 874, InControl: 390等)
- [x] UnityEditor引用扫描完成 (0个直接使用)
- [x] ExecuteInEditMode出现已识别 (21个文件)
- [x] 第三方库列表已编制 (8个主要框架)
- [x] 混淆等级已评估 (中高，BabelObfuscator 1.16.1.2)
- [x] MonoBehaviour/ScriptableObject清单 (531 + 47)
- [x] Managed DLL分类完成 (72个DLL，5类)
- [x] 风险评估已执行 (3个严重级别)
- [x] 依赖策略已推荐

---

**报告状态:** 完成
**下一任务:** Task-01 - Unity承载工程骨架与asmdef
**阻塞问题:** 无
**需要行动:** 在进入Task-01前审查并批准依赖策略
