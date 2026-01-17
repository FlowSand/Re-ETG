# Unity C# 反编译代码库工程化清洗指南

你将对一个反编译得到的Unity C#代码库执行工程化清洗。最终目标：Unity工程内0编译错误、目录结构按命名空间清洗、可读性良好，并输出一份架构分析文档。

## 严格遵守规则

- 不改变public/protected/internal API签名
- MonoBehaviour/ScriptableObject字段名尽量不改
- 缺依赖优先引入DLL/Package，无法引入才生成最小Shims（throw NotImplementedException）
- 不新增业务逻辑

## 任务链

按以下任务链执行并产出对应文件：

- **Task-00** Baseline_Report + Managed_Dll_List
- **Task-01** Unity承载工程目录与asmdef
- **Task-02** 命名空间->目录映射清洗
- **Task-03** 依赖补齐策略
- **Task-04** 编译错误循环修复，直到Unity Console 0 errors，并维护 Compile_Fix_Log
- **Task-05** 可读性清洗（格式、using、局部命名、注释）
- **Task-06** 模块边界标注与依赖清单
- **Task-07** 输出 Docs/Architecture_Analysis.md
- **Task-08** 最终一致性检查与交付索引（Delivery_Index、Shims_Inventory）

## 任务输出要求

每完成一个任务，输出：

- 本任务做了什么（简短）
- 产生/修改了哪些文件（相对路径）
- 下一步需要的输入（如Unity错误日志）
