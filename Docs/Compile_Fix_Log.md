# Task-04 编译错误修复日志

**任务开始时间：** 2026-01-18
**Unity版本：** 2022.3.62f1
**C# Language Version：** 9.0 (Unity默认)
**初始Git HEAD：** c25421c1

---

## 初始状态评估

### 编译错误统计
- **总错误数：** 970个编译错误
- **主要错误类型：**
  - CS1002: `;` expected
  - CS1056: Unexpected character
  - CS1519: Invalid token
  - CS8124: Tuple must contain at least two elements
  - CS8773: Feature 'file-scoped namespace' is not available in C# 9.0

### 问题文件分布
1. `Assets/Scripts/ETG/DaikonForge/Tween/TweenEasingFunctions.cs`
   - Unicode转义序列未解码
   - compiler-generated字段名包含特殊字符

2. `Assets/Scripts/ETG/Core/Systems/Management/SaveManager.cs`
   - Unicode转义序列未解码（第51行）
   - 加密密钥初始化代码包含特殊类型名

3. `Assets/Scripts/ETG/DaikonForge/Tween/TweenGroup.cs`
   - File-scoped namespace语法（C# 10.0）

4. `Assets/Scripts/ETG/DaikonForge/Tween/TweenGroupMode.cs`
   - File-scoped namespace语法（C# 10.0）

### 根本原因分析

**问题1：Unicode转义序列**
反编译工具（JetBrains decompiler）将某些特殊字符转义为Unicode序列：
- `\u003C` → `<`
- `\u003E` → `>`
- `\u0024` → `$`
- `\u002D` → `-`
- `\u007B` → `{`
- `\u007D` → `}`

这些转义序列在字符串字面量中有效，但在标识符、类型名中无效。

**示例错误代码：**
```csharp
// Line 51 in SaveManager.cs
private static string secret = \u003CPrivateImplementationDetails\u003E\u007BDE5600AD\u002D6212\u002D4D84\u002D9A32\u002D9D951E3289D1\u007D.Decrypt.DecryptLiteral(new byte[512]

// Lines 389, 392, etc in TweenEasingFunctions.cs
return TweenEasingFunctions.\u003C\u003Ef__mg\u0024cache14;
if (TweenEasingFunctions.\u003C\u003Ef__mg\u0024cache16 == null)
```

**问题2：C# Language Version**
Unity 2022.3.62f1默认使用C# 9.0，但部分文件使用了C# 10.0的file-scoped namespace语法：
```csharp
namespace DaikonForge.Tween;  // C# 10.0+ only
```

---

## 修复策略

### 策略1：批量Unicode转义序列修复
**目标文件：** 所有包含`\u003C`、`\u003E`、`\u0024`等转义序列的.cs文件

**方法：** 使用Python脚本批量替换
- 搜索模式：`\u003C`, `\u003E`, `\u0024`, `\u002D`, `\u007B`, `\u007D`
- 替换为：`<`, `>`, `$`, `-`, `{`, `}`

**风险评估：**
- ⚠️ 可能影响字符串字面量中的转义序列（低风险，可检查）
- ✅ 不修改API签名（符合CLAUDE.md约束）

### 策略2：File-Scoped Namespace转换
**目标文件：** TweenGroup.cs, TweenGroupMode.cs（以及其他类似文件）

**方法：** 转换为传统namespace语法
```csharp
// BEFORE (C# 10.0)
namespace DaikonForge.Tween;
public class TweenGroup { }

// AFTER (C# 9.0)
namespace DaikonForge.Tween
{
    public class TweenGroup { }
}
```

**风险评估：**
- ✅ 零风险，语义完全相同
- ✅ 不修改API签名

### 策略3：升级C# Language Version（备选方案）
**如果策略2不可行，考虑在asmdef中指定C# 10.0：**
```json
{
  "name": "ETG.Runtime",
  "references": ["ETG.Shims"],
  "allowUnsafeCode": true,
  "overrideReferences": false,
  "defineConstraints": [],
  "langVersion": "10"  // 添加此项
}
```

---

## 修复执行计划

### Round 1: Unicode转义序列修复（预计修复~900个错误）
- [ ] 创建Python脚本扫描所有.cs文件
- [ ] 批量替换Unicode转义序列
- [ ] 触发Unity重新编译
- [ ] 验证错误减少
- [ ] Git提交

### Round 2: File-Scoped Namespace修复（预计修复~10个错误）
- [ ] 查找所有file-scoped namespace文件
- [ ] 转换为传统namespace语法
- [ ] 触发Unity重新编译
- [ ] 验证错误减少
- [ ] Git提交

### Round 3: 残留错误修复（如有）
- [ ] 分析剩余错误
- [ ] 逐个修复
- [ ] 最终验证

---

## 修复记录

### Round 1: [待执行] Unicode转义序列批量修复

**开始时间：** 待定
**错误数（修复前）：** 970

**执行步骤：**
1. 待执行...

**结果：**
- 待完成

**Git提交：**
- 待提交

---

## 最终验证

- [ ] Unity Console显示 0 errors
- [ ] 所有程序集编译成功
- [ ] 可以进入Play Mode
- [ ] 无破坏性修改（API签名、序列化字段）

---

## 参考文档
- `CLAUDE.md` - 工程化清洗规则
- `Docs/Task03_to_Task04_Handoff.md` - Task-03交接文档
