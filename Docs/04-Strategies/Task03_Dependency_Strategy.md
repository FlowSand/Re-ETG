# Task-03: 依赖补齐策略与实施

**创建时间：** 2026-01-17
**状态：** 进行中
**目标：** 补齐平台SDK依赖，使ETG.Runtime能够编译

---

## 1. 依赖分析

### 1.1 缺失的平台SDK

| DLL | 用途 | 使用情况 | 策略 |
|-----|------|---------|------|
| **GalaxyCSharp.dll** | GOG Galaxy SDK | 被GalaxyManager使用 | 创建完整Shims |
| **SonyNP.dll** | Sony PlayStation网络 | 未直接使用 | 创建空存根 |
| **SonyPS4SavedGames.dll** | PS4存档系统 | 未直接使用 | 创建空存根 |

### 1.2 Galaxy SDK使用分析

**使用文件：**
- `Core/Systems/Management/GalaxyManager.cs` - 核心管理器
- `Core/Systems/Utilities/PlatformInterfaceGalaxy.cs` - 平台接口实现
- `Core/Systems/Management/GameManager.cs` - 游戏管理器引用

**使用的API：**
```csharp
using Galaxy.Api;

// 静态类
GalaxyInstance.Init(string clientId, string clientSecret);
GalaxyInstance.Shutdown();
GalaxyInstance.ProcessData();
GalaxyInstance.User(); // 返回IUser接口

// 接口/类
GlobalAuthListener (基类或接口)
IUser.SignIn();
```

### 1.3 Sony SDK使用分析

**搜索结果：** 代码中未找到直接使用
**结论：** 可能仅在编译时被引用，或通过条件编译排除。创建最小存根即可。

---

## 2. 实施方案

### 2.1 Shims程序集结构

```
Assets/Decompiled/Shims/
├── ETG.Shims.asmdef          [已存在]
├── Galaxy/
│   └── GalaxyCSharp.cs       [Namespace: Galaxy.Api]
└── Sony/
    ├── SonyNP.cs             [Namespace: Sony.NP]
    └── SonyPS4SavedGames.cs  [Namespace: Sony.PS4]
```

### 2.2 Galaxy.Api Shims设计

**命名空间：** `Galaxy.Api`

**需要的类型：**

```csharp
namespace Galaxy.Api
{
    // 静态API入口
    public static class GalaxyInstance
    {
        public static void Init(string clientId, string clientSecret)
            => throw new NotImplementedException("GOG Galaxy not available");

        public static void Shutdown() { }

        public static void ProcessData() { }

        public static IUser User()
            => throw new NotImplementedException("GOG Galaxy not available");
    }

    // 用户接口
    public interface IUser
    {
        void SignIn();
    }

    // 认证监听器基类
    public abstract class GlobalAuthListener
    {
        // 可能包含虚方法，暂时留空
    }
}
```

**设计原则：**
- 所有方法抛出`NotImplementedException`，明确标注不可用
- 空方法（如Shutdown）不抛异常，允许安全调用
- 保持方法签名与原始API完全一致

### 2.3 Sony Shims设计

**最小存根：** 仅提供命名空间，无实际类型

```csharp
// Sony.NP命名空间
namespace Sony.NP
{
    // 如果编译时需要特定类型，此处添加
}

// Sony.PS4命名空间
namespace Sony.PS4
{
    // 如果编译时需要特定类型，此处添加
}
```

**后续补充：** 如果Unity编译时报告Sony相关错误，再根据错误信息添加缺失类型。

---

## 3. 实施步骤

### 步骤1: 创建Galaxy Shims ✓
1. 创建 `Assets/Decompiled/Shims/Galaxy/GalaxyCSharp.cs`
2. 实现Galaxy.Api命名空间的核心类型
3. 添加XML文档注释标注存根性质

### 步骤2: 创建Sony Shims
1. 创建 `Assets/Decompiled/Shims/Sony/SonyNP.cs`
2. 创建 `Assets/Decompiled/Shims/Sony/SonyPS4SavedGames.cs`
3. 提供最小命名空间定义

### 步骤3: 配置ETG.Runtime依赖
1. 修改 `Assets/Scripts/ETG/ETG.Runtime.asmdef`
2. 添加对ETG.Shims的引用：
   ```json
   {
     "name": "ETG.Runtime",
     "references": ["ETG.Shims"],
     ...
   }
   ```

### 步骤4: 测试编译
1. 保存所有文件
2. 返回Unity，等待自动重新编译
3. 检查Console错误
4. 根据错误补充缺失的类型/方法

### 步骤5: 迭代修复
- 如果出现Galaxy API相关错误，补充方法签名
- 如果出现Sony API相关错误，添加缺失类型
- 重复直到Shims相关错误消失

---

## 4. 验证标准

### 成功标准

- [ ] ETG.Shims.dll成功编译
- [ ] ETG.Runtime不再报告Galaxy/Sony相关类型缺失错误
- [ ] GalaxyManager.cs编译通过（即使运行时会抛异常）
- [ ] 无命名空间解析错误

### 预期结果

**编译阶段：** 所有平台SDK引用解析成功
**运行时阶段：** 如果尝试使用GOG/Sony功能，抛出清晰的NotImplementedException

### 已知限制

1. **GOG功能不可用：** GalaxyManager运行时会抛异常
2. **Sony功能不可用：** 如果有Sony相关代码，运行时会失败
3. **条件编译：** 原游戏可能使用`#if GOG_GALAXY`等宏，我们的Shims总是存在

---

## 5. 替代方案（未采用）

### 方案A: 引入原始DLL
- **优点：** 完全兼容，功能可用
- **缺点：** 需要GOG/Sony开发者账号，有许可限制
- **结论：** 不适用于开源/学习项目

### 方案B: 条件编译移除
- **优点：** 彻底移除依赖
- **缺点：** 需要大量手动修改代码，破坏原始结构
- **结论：** 与任务目标（保持原始API）冲突

### 方案C: Assembly Resolver运行时替换
- **优点：** 不修改代码
- **缺点：** 运行时复杂，调试困难
- **结论：** 过度工程化

---

## 6. 下一步（Task-04）

完成Task-03后，预期Unity Console仍会有大量编译错误，主要类型：

1. **命名空间解析错误** - 类型找不到（因为添加了命名空间）
2. **using语句缺失** - 需要添加using指令
3. **类型可见性问题** - internal/private访问限制
4. **跨命名空间引用** - 需要完全限定名或using

Task-04将迭代修复这些错误，目标：Unity Console 0 errors。

---

**任务状态：** 进行中
**当前阶段：** 步骤1 - 创建Galaxy Shims
**预计时间：** 1-2小时
