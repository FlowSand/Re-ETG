# Task-01 Unity承载工程骨架与目录

## 目标

在 Unity 工程中建立承载结构（不要求编译通过）。

## 输入

- Unity 工程路径 `UnityProject/`
- `DecompileSrc/`

## 动作

- 创建目录：
  - `Assets/Decompiled/Scripts/`
  - `Assets/Decompiled/Shims/`
  - `Assets/Decompiled/Plugins/`
  - `Assets/Decompiled/Editor/`
  - `Docs/`
- 将反编译源码复制到 `Assets/Decompiled/Scripts/_RawDump/`（保留原始备份）。
- 创建 `.asmdef`（Runtime/Shims/Editor）空壳文件。

## 输出

- 目录结构 + asmdef 文件（初版）
- `Docs/Project_Layout.md`

## 验收

Unity 打开后工程结构存在，且 `_RawDump` 不参与编译（通过 asmdef 或排除策略）。
