# Task-02 命名空间-目录结构

## 目标

把 `_RawDump` 中的源码按命名空间移动并重排目录。

## 输入

- `Assets/Decompiled/Scripts/_RawDump/**`

## 动作

- 对每个 `.cs` 文件解析：
  - `namespace`
  - top-level 类型名
  - 是否引用 `UnityEditor`
- 执行目录映射规则：
  - Runtime → `Assets/Decompiled/Scripts/<NamespacePath>/`
  - Editor → `Assets/Decompiled/Editor/<NamespacePath>/`
- 生成 `Docs/Namespace_To_Folder_Map.md`

## 输出

- 清洗后的源码目录树
- `Docs/Namespace_To_Folder_Map.md`

## 验收

`_RawDump` 保留仅作备份；编译目录只包含清洗后的路径。
