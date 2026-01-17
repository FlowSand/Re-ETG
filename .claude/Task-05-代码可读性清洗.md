# Task-05 代码可读性清洗

## 目标

在不改行为的前提下显著提升可读性。

## 输入

- 已 0 errors 的代码

## 动作

- 统一格式化与 using（按 Rider/EditorConfig 规则）。
- 局部变量/参数重命名：
  - `num/flag/tmp/a/b` → 语义化命名（基于上下文推断）
- 清理噪声：
  - 多余括号、冗余 cast、重复空行、无效注释
- 对关键模块加"意图注释"：
  - 生命周期（Awake/Start/Update）
  - 事件流（UI 点击、网络回包、存档加载）
  - 管线/状态机（如战斗流程/关卡流程）

## 输出

- 清洗后的源码（保持 0 errors）
- `Docs/Readability_Guidelines.md`（本项目采用的风格规则简述）

## 验收

- Unity 仍为 0 errors
- 抽查 10 个核心类：局部命名可理解、结构不混乱。
