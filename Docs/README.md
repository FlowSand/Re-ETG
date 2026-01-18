# ETG 项目文档

本目录包含Enter the Gungeon反编译代码库的工程化清洗文档。

## 📁 目录结构

### 01-Architecture - 核心架构文档
**最重要的文档，包含完整的架构分析和模块组织**

- **[Architecture_Analysis.md](01-Architecture/Architecture_Analysis.md)** (78KB) - 完整架构分析报告
  - 10个部分：概览、架构分层、设计模式、技术决策、依赖分析等
  - 22个模块、85个依赖关系、14种设计模式
  - 4层架构设计、迁移策略、风险评估

- **[Module_Boundaries.md](01-Architecture/Module_Boundaries.md)** (10KB) - 模块边界中央索引
  - 指向8个MODULE_BOUNDARY.md文件
  - 迁移决策树

- **[Module_Manifest.md](01-Architecture/Module_Manifest.md)** (24KB) - 22个模块的完整目录
  - 按层级组织（Foundation/Core/Domain/Presentation）
  - 每个模块的职责、依赖、文件数

### 02-Dependencies - 依赖分析数据
**模块间依赖关系的详细数据**

- [dependency_analysis.json](02-Dependencies/dependency_analysis.json) - 机器可读依赖数据（85个关系）
- [module_definitions.yaml](02-Dependencies/module_definitions.yaml) - YAML格式模块定义
- [Module_Dependency_Matrix.md](02-Dependencies/Module_Dependency_Matrix.md) - 22x22依赖矩阵表
- [Module_Dependency_Graph.md](02-Dependencies/Module_Dependency_Graph.md) - Mermaid可视化依赖图

### 03-Task-Reports - 任务完成报告
**Task-00到Task-05的完成报告**

- [Task00_Baseline_Report.md](03-Task-Reports/Task00_Baseline_Report.md) - 项目基线评估
- [Task02_Completion_Report.md](03-Task-Reports/Task02_Completion_Report.md) - 命名空间→目录清洗
- [Task03_Completion_Report.md](03-Task-Reports/Task03_Completion_Report.md) - 依赖补齐策略
- [Task04_Completion_Report.md](03-Task-Reports/Task04_Completion_Report.md) - 编译错误修复（970→0）
- [Task05_Completion_Report.md](03-Task-Reports/Task05_Completion_Report.md) - 可读性清洗

### 04-Strategies - 决策和策略
**关键技术决策文档**

- [Task03_Dependency_Strategy.md](04-Strategies/Task03_Dependency_Strategy.md) - DLL依赖策略
- [DLL_Cleanup_Strategy.md](04-Strategies/DLL_Cleanup_Strategy.md) - DLL清理决策
- [Namespace_To_Folder_Map.md](04-Strategies/Namespace_To_Folder_Map.md) - 命名空间映射表

### 05-Reference - 参考资料
**项目参考文档和清单**

- [Project_Layout.md](05-Reference/Project_Layout.md) - 项目目录结构
- [Managed_Dll_List.txt](05-Reference/Managed_Dll_List.txt) - 72个DLL清单
- [Combat_Laser_Tile_Battle_Overview.md](05-Reference/Combat_Laser_Tile_Battle_Overview.md) - 战斗系统分析

### 99-Archive - 归档文件
**中间产物、过程日志、冗余文件**

- 大型数据文件（Namespace_Classification_Report.md 391KB）
- 过程日志（Compile_Fix_Log.md）
- 工作记忆（Task-07_Work_Memory.md）
- 其他临时文件

---

## 🔍 快速导航

**想了解架构？** → [01-Architecture/Architecture_Analysis.md](01-Architecture/Architecture_Analysis.md)

**想看模块清单？** → [01-Architecture/Module_Manifest.md](01-Architecture/Module_Manifest.md)

**想看依赖关系？** → [02-Dependencies/dependency_analysis.json](02-Dependencies/dependency_analysis.json)

**想看任务历史？** → [03-Task-Reports/](03-Task-Reports/)

**想了解技术决策？** → [04-Strategies/](04-Strategies/)

---

## 📊 项目统计

- **总文件数**: 4,009个C#文件
- **模块数**: 22个（13外部 + 9核心）
- **依赖关系**: 85个
- **循环依赖**: 3个（可控）
- **架构层级**: 4层（Foundation/Core/Domain/Presentation）

---

## 🛠️ 工具和脚本

- **Tools/analyze_module_dependencies.py** - 依赖分析脚本
  - 输入：module_definitions.yaml
  - 输出：dependency_analysis.json

---

## 📝 文档更新历史

- **2026-01-18**: Task-07完成，Architecture_Analysis.md创建
- **2026-01-18**: 整理Docs目录结构（6个分类目录）

---

## 💡 使用建议

### 对于新加入的开发者
1. 从 **Architecture_Analysis.md** 开始，了解整体架构
2. 查看 **Module_Manifest.md** 了解模块组织
3. 阅读 **Task Reports** 了解项目演进历史

### 对于架构师
1. 重点关注 **Architecture_Analysis.md** 的技术决策部分
2. 查看 **dependency_analysis.json** 进行自动化分析
3. 参考 **Strategies** 目录中的决策文档

### 对于AI助手
1. **01-Architecture/** 包含最重要的架构信息
2. 使用 **dependency_analysis.json** 进行程序化分析
3. 参考 **Module_Boundaries.md** 了解模块边界

---

**维护说明**: 当添加新文档或更新现有文档时，请相应更新此README.md文件。
