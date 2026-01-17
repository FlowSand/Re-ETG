# Task-06 模块化与边界标注

## 目标

为架构分析做准备：标注模块边界与依赖方向。

## 输入

- 清洗后的源码

## 动作

- 通过引用关系与命名空间聚类模块（示例：UI、Net、Data、Battle、Audio、Analytics）。
- 在模块根目录放置 `README.md`：
  - 模块职责
  - 对外入口类
  - 关键依赖
- 输出一份依赖图数据（用于文档）：模块A→模块B

## 输出

- `Assets/Decompiled/Scripts/<Module>/README.md`（每模块）
- `Docs/Module_Dependency_List.md`

## 验收

模块划分覆盖核心命名空间；依赖方向基本单向或可解释。
