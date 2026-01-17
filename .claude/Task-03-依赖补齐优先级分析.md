# Task-03 依赖补齐优先级分析

## 目标

尽可能通过引入 DLL/Package 消灭"缺类型"错误，减少 shim 数量。

## 输入

- `Managed_Dll_List.txt`
- Unity Console 初次报错日志（如果尚未运行 Unity，则先运行一次拿日志）

## 动作

- 将 `Managed/` 中可能需要的 DLL 复制到 `Assets/Decompiled/Plugins/`（不包含 UnityEngine 自带）。
- 根据报错识别常见依赖：TMP/InputSystem/Addressables/DOTween/UniTask/Newtonsoft 等。
- 输出推荐：
  - 用 DLL 解决 vs 用 Unity Package 解决（给出优先顺序）

## 输出

- `Docs/Dependency_Strategy.md`（含"建议引入列表"与"仍缺失列表"）

## 验收

Unity 编译错误中 "type or namespace not found" 数量显著下降。
