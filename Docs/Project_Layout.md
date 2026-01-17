# Enter the Gungeon - Unity项目布局文档

**创建时间：** 2026-01-17 (Task-01)
**Unity版本：** 2022.3.62f1
**状态：** Assembly Definition已建立

---

## 程序集结构

### ETG.Runtime.asmdef
- **位置：** `Assets/Scripts/ETG/`
- **文件数：** ~4,048 C#文件
- **用途：** 所有游戏代码、第三方库和游戏逻辑
- **依赖：** 无
- **allowUnsafeCode：** true（反编译代码需要）

**主要组件：**
- 核心游戏逻辑（2,323个文件，无命名空间）
- HutongGames.PlayMaker（875个文件）
- InControl（391个文件）
- FullInspector（188个文件）
- Dungeonator（67个文件）
- DaikonForge（61个文件）
- FullSerializer（52个文件）
- tk2dRuntime（14个文件）
- Brave.BulletScript（14个文件）
- Wwise Audio (AK.*)（11个文件）
- 其他工具库（BraveDynamicTree、Pathfinding等）（20+文件）

### ETG.PrivateImpl.*.asmdef（3个程序集）
- **位置：** `Assets/Scripts/ETG/_003CPrivateImplementationDetails_003E{...}/`
- **文件数：** 4个文件总计
- **用途：** 编译器生成的混淆静态初始化器
- **依赖：** ETG.Runtime
- **状态：** 如果不需要可以删除

### ETG.Shims.asmdef
- **位置：** `Assets/Decompiled/Shims/`
- **文件数：** 0（将在Task-03创建）
- **用途：** 平台SDK存根（GOG、Sony）
- **依赖：** 无

---

## 目录结构

```
D:\Github\Re-ETG\
├── Assets\
│   ├── _RawDump\                      [备份 - 不参与编译]
│   │   └── C#\
│   │       └── Assembly-CSharp\       [原始反编译输出]
│   │
│   ├── Decompiled\                    [未来的组织化结构]
│   │   ├── Shims\                     [平台SDK存根]
│   │   │   └── ETG.Shims.asmdef
│   │   └── Plugins\                   [外部DLL（如需要）]
│   │
│   ├── Scripts\
│   │   └── ETG\                       [工作代码库]
│   │       ├── ETG.Runtime.asmdef     [主程序集]
│   │       │
│   │       ├── HutongGames\           [PlayMaker - 875文件]
│   │       ├── InControl\             [输入 - 391文件]
│   │       ├── FullInspector\         [Inspector - 188文件]
│   │       ├── Dungeonator\           [地牢生成 - 67文件]
│   │       ├── DaikonForge\           [UI/补间 - 61文件]
│   │       ├── FullSerializer\        [序列化 - 52文件]
│   │       ├── Brave\                 [弹幕脚本 - 14文件]
│   │       ├── tk2dRuntime\           [2D工具包 - 14文件]
│   │       ├── AK\                    [Wwise - 11文件]
│   │       ├── Pathfinding\           [A* - 4文件]
│   │       │
│   │       ├── _003CPrivateImplementationDetails_003E{...}\ [3个目录]
│   │       │   └── ETG.PrivateImpl.*.asmdef
│   │       │
│   │       └── [2,323个根文件]        [核心游戏逻辑]
│   │           (AIActor, PlayerController, GameManager等)
│   │
│   └── Scenes\
│
├── Docs\
│   ├── Baseline_Report.md             [Task-00]
│   ├── Managed_Dll_List.txt           [Task-00]
│   ├── Project_Layout.md              [Task-01 - 本文件]
│   └── [未来任务的输出]
│
├── ManagedDll\                        [原始DLL备份]
│   └── [72个DLL文件]
│
└── ProjectSettings\
```

---

## 编译边界

**程序集数量：** 4个（1个运行时 + 3个混淆 + shims占位）

**依赖关系图：**
```
ETG.Shims（独立）

ETG.Runtime（独立）
    ↓
ETG.PrivateImpl.*（依赖Runtime）
```

**编译顺序：**
1. ETG.Shims（并行）
2. ETG.Runtime（并行）
3. ETG.PrivateImpl.*（在Runtime之后）

**单一运行时程序集的理由：**
- 简化依赖管理
- 避免游戏代码与第三方actions之间的循环依赖
- Unity增量编译可高效处理大型程序集
- 清晰分离：Runtime vs Shims vs 混淆代码
- 如需要可在未来进一步模块化

---

## 已知问题与决策

### 问题1：示例目录
**状态：** 包含在ETG.Runtime中
**影响：** 12个演示文件（占代码库的0.3%）
**解决方案：** Task-01可接受；清理延迟到Task-02

### 问题2：混淆目录
**状态：** 已创建独立程序集
**风险：** 可能导致编译错误
**缓解：** 如果不被游戏代码引用可以删除

### 问题3：平台SDK
**状态：** ETG.Shims占位已创建
**解决方案：** Task-03将创建存根代码：
- GalaxyCSharp.dll（GOG）
- SonyNP.dll、SonyPS4SavedGames.dll（PlayStation）

### 问题4：第三方库组织
**状态：** 全部包含在单一ETG.Runtime中
**权衡：** 粒度较粗的编译 vs 更简单的管理
**未来：** 如需要可拆分特定库的隔离

---

## 下一步（Task-02）

- 为2,323个根文件进行命名空间到目录的对齐
- 将组织化代码移到 `Assets/Decompiled/Core/`
- 建立一致的命名模式
- 清理示例目录

---

## 验收检查清单

- [x] ETG.Runtime.asmdef已创建
- [x] ETG.PrivateImpl.*.asmdef已创建（3个文件）
- [x] ETG.Shims.asmdef已创建（占位）
- [x] C#/备份已移至Assets/_RawDump/
- [x] Decompiled/目录结构已创建
- [x] Project_Layout.md已记录
- [ ] Unity编译成功（将在Task-04验证）

---

## 设计决策记录

### Q: 为什么不创建多个模块化程序集（如ETG.ThirdParty、ETG.Core等）？
**A:** 分析发现核心游戏代码与PlayMaker Actions之间存在双向依赖。拆分会导致循环依赖。单一Runtime程序集避免了这个问题，且Unity的增量编译可以高效处理。

### Q: 为什么不在Task-01移动文件到命名空间对应的目录？
**A:** Task-01专注于建立编译边界。移动文件有破坏Unity序列化的风险。Task-02专门处理命名空间清理，会保留GUID以避免引用丢失。

### Q: PrivateImpl目录是否必需？
**A:** 不确定。这些是编译器生成的静态初始化器，可能不被引用。如果导致编译错误，可以安全删除。

### Q: 示例目录（12个文件）如何处理？
**A:** Task-01保留原位（影响小，仅0.3%）。Task-02清理时可移至_Examples/或删除。

---

**Task-01状态：** 完成
**下一任务：** Task-02 - 命名空间到目录映射
**阻塞问题：** 无
