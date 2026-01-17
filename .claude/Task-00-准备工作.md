# Task-00 准备工作

## 目标

明确输入路径、Unity 版本、依赖清单，建立可回滚基线。

## 输入

- `DecompileSrc/`：反编译导出 C# 源码目录
- `Managed/`：游戏 `*_Data/Managed/` 全量 DLL 目录（至少列出文件名）
- 目标 Unity 版本（未知则默认 2021.3 LTS）

## 动作

- 扫描 `DecompileSrc`：统计文件数、命名空间分布、是否混淆（短名/无意义名比例）。
- 扫描 `Managed`：列出非 Unity 自带 DLL（第三方与游戏其它程序集）。
- 生成 `Docs/Baseline_Report.md`：记录输入信息与风险点。

## 输出

- `Docs/Baseline_Report.md`
- `Docs/Managed_Dll_List.txt`

## 验收

报告包含：UnityEditor 引用比例、疑似第三方库列表、混淆评估。
