# 上下文
文件名：MainMenuUI_fix_task.md
创建于：2025-10-17 10:00:00
创建者：AI
关联协议：RIPER-5 + Multidimensional + Agent Protocol

# 任务描述
修复 `MainMenuUI.cs` 中对已弃用 API `Object.FindObjectOfType<T>()` 的调用，替换为 `Object.FindFirstObjectByType<T>()`，并确保代码风格与项目一致；同时修正因临时硬编码导致的按钮交互状态问题。

# 项目概述
在 `Assets/_Project/Scripts/UI/MainMenuUI.cs` 文件中，存在对已弃用 API 的调用。需要替换为新的 API，并避免重复查找场景加载器实例。同时确保类的命名空间与文件路径匹配并修复 continue 按钮的 hasSave 计算逻辑。

---

# 分析 (由 RESEARCH 模式填充)
- 在 `MainMenuUI.cs` 中发现两处对 `FindObjectOfType<SceneLoader>()` 的使用位置（加载场景与退出逻辑）。
- 文件缺少与路径匹配的命名空间 `_Project.Scripts.UI`。
- `UpdateContinueButton` 中有硬编码 `hasSave = false`，导致按钮永远不可交互。
- 替换为推荐 API `Object.FindFirstObjectByType<SceneLoader>()` 可以消除过时警告与提高性能（避免重复查找）。

# 提议的解决方案 (由 INNOVATE 模式填充)
- 使用 `Object.FindFirstObjectByType<SceneLoader>()` 并将返回值缓存到局部变量 `loader`。如果返回空则回退到使用 `SceneManager.LoadScene` 或退出应用的方式。
- 添加文件命名空间 `_Project.Scripts.UI` 使静态分析器消除命名空间不匹配的警告。
- 将 `hasSave` 更改为 `PlayerPrefs.HasKey("SaveGame")`（可替换为项目实际存储系统）。

# 实施计划 (由 PLAN 模式生成)
实施检查清单：
1. 在 `Assets/_Project/Scripts/UI/MainMenuUI.cs` 中将 `FindObjectOfType<SceneLoader>()` 替换为 `Object.FindFirstObjectByType<SceneLoader>()` 并缓存结果。
2. 将类包装在命名空间 `namespace _Project.Scripts.UI { ... }` 中以匹配文件路径。
3. 将 `hasSave` 的硬编码替换为 `PlayerPrefs.HasKey("SaveGame")`。
4. 运行静态错误检查；若存在构建设置中缺少场景的警告，向用户报告并建议将场景添加到 Build Settings 或忽略。

# 当前执行步骤 (由 EXECUTE 模式在开始执行某步骤时更新)
> 正在执行: "步骤 1-3：替换过时 API、添加命名空间并修复 hasSave 逻辑"

# 任务进度 (由 EXECUTE 模式在每步完成后追加)
*   2025-10-17 10:00:00
    *   步骤：1-3（替换过时 API、添加命名空间、修复 hasSave）
    *   修改：
        - 文件：`Assets/_Project/Scripts/UI/MainMenuUI.cs`
        - 更改摘要：
            - 将 `FindObjectOfType<SceneLoader>()` 替换为 `Object.FindFirstObjectByType<SceneLoader>()`，并缓存到局部变量 `loader`。
            - 将类添加到命名空间 `namespace _Project.Scripts.UI`。
            - 将 `hasSave` 修改为 `PlayerPrefs.HasKey("SaveGame")`。
        - 已报告的微小偏差修正：添加命名空间与修复 `hasSave`，被视为微小且必要的修正以满足静态分析器与运行时行为。
    *   原因：修复弃用 API 报警并确保按钮逻辑正确。
    *   阻碍：编译/静态检查显示仍有一条警告："Unity 构建设置中缺少场景"（与代码安全无关，属于项目构建配置）。
    *   用户确认状态：待确认

# 最终审查 (由 REVIEW 模式填充)
待执行所有检查清单并在用户确认后进入 REVIEW 阶段。


