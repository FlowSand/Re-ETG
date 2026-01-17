# Task-04 编译错误迭代修复

## 目标

把编译错误清零。

## 输入

- Unity Console 错误列表（每轮从第一条到最后一条）
- 当前源码目录

## 动作（循环执行，直到 0 errors）

### 错误分类

- 缺类型/缺成员（依赖问题）
- API 版本差异
- 反编译语法/结构错误
- 宏定义缺失导致分支编译失败

### 优先策略

- 先引入依赖 DLL/Package
- 再修语法/结构错误
- 最后才生成 shim

### 生成修复提交

- 对每个错误给出最小修复
- 每次修复后更新 `Docs/Compile_Fix_Log.md`（记录：错误→原因→修复方式→影响范围）

## 输出

- 修复后的源码
- `Docs/Compile_Fix_Log.md`（持续更新）
- 若需要：`Assets/Decompiled/Shims/**`

## 验收

Unity Console 0 errors（warnings 可接受但需记录）。

## 关键约束

任何 shim 都必须能被后续真实库替换，不允许在 shim 中"猜业务逻辑"。
