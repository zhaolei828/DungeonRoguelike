# Git 使用说明

## 🎯 问题说明

在Windows PowerShell中，直接使用 `git commit -m "中文消息"` 会导致提交消息出现乱码。

### 问题原因
- PowerShell默认编码与Git不一致
- 命令行参数传递时中文字符编码转换出错

---

## ✅ 解决方案

### 方法1：使用UTF-8脚本（推荐）

项目根目录已提供 `git_commit_utf8.ps1` 脚本：

```powershell
# 用法
.\git_commit_utf8.ps1 "你的中文提交消息"
```

**原理**：将提交消息先写入UTF-8文件，然后使用 `git commit -F` 读取文件提交。

---

### 方法2：手动创建消息文件

```powershell
# 1. 创建提交消息文件
@"
你的提交消息
可以多行
支持emoji ✅
"@ | Out-File -FilePath commit_msg.txt -Encoding UTF8

# 2. 使用文件提交
git commit -F commit_msg.txt

# 3. 删除临时文件
Remove-Item commit_msg.txt
```

---

### 方法3：使用Git GUI工具

推荐使用以下工具，它们对UTF-8支持良好：
- **VS Code** 内置Git插件
- **GitHub Desktop**
- **SourceTree**
- **GitKraken**

---

## 🔧 Git配置

项目已配置以下Git设置（自动生效）：

```bash
git config --local core.quotepath false
git config --local i18n.commitencoding utf-8
git config --local i18n.logoutputencoding utf-8
git config --local gui.encoding utf-8
```

---

## 📝 提交消息规范

### 格式建议

```
类型：简短描述（50字以内）

详细说明：
- 变更点1
- 变更点2
- 变更点3

统计/备注：
- 可选的额外信息
```

### 类型标识

- **初始提交**：项目初始化
- **功能**：新增功能
- **修复**：Bug修复
- **优化**：性能优化
- **重构**：代码重构
- **文档**：文档更新
- **测试**：测试相关
- **配置**：配置文件修改
- **清理**：代码清理

### 示例

```
功能：实现玩家移动系统

完成内容：
- 实现A*寻路算法
- 添加输入管理器
- 集成相机跟随

统计：
- 新增5个C#脚本
- 约800行代码
- 0个编译错误
```

---

## 📊 查看提交日志

### 基本查看

```powershell
# 简洁视图
git log --oneline

# 详细视图
git log --format=fuller

# 最近N条
git log --oneline -5

# 图形化视图
git log --graph --oneline --all
```

### 查看特定提交

```powershell
# 查看某个提交的详情
git show f8caf11

# 查看某个文件的修改历史
git log -- path/to/file.cs
```

---

## 🚀 常用命令

### 暂存修改

```powershell
# 暂存所有修改
git add .

# 暂存特定文件
git add Assets/_Project/Scripts/Core/GameManager.cs

# 暂存特定目录
git add Assets/_Project/Scripts/
```

### 提交修改

```powershell
# 使用UTF-8脚本提交（推荐）
.\git_commit_utf8.ps1 "提交消息"

# 或使用临时文件
"提交消息" | Out-File -FilePath temp.txt -Encoding UTF8
git commit -F temp.txt
Remove-Item temp.txt
```

### 查看状态

```powershell
# 查看工作区状态
git status

# 查看简洁状态
git status --short

# 查看修改内容
git diff

# 查看暂存区修改
git diff --cached
```

### 撤销操作

```powershell
# 撤销工作区修改（危险）
git checkout -- path/to/file.cs

# 取消暂存
git reset HEAD path/to/file.cs

# 修改最后一次提交
git commit --amend -F new_message.txt
```

---

## ⚠️ 注意事项

### 不要提交的文件

已在 `.gitignore` 中配置：
- `Library/` - Unity生成的缓存
- `Temp/` - 临时文件
- `Obj/` - 编译输出
- `Build/` - 构建输出
- `Logs/` - 日志文件
- `*.csproj` - IDE生成文件
- `*.sln` - 解决方案文件

### 提交前检查清单

- [ ] 代码编译无错误
- [ ] 删除调试代码
- [ ] 更新相关文档
- [ ] 测试核心功能
- [ ] 检查提交内容（`git status`）
- [ ] 编写清晰的提交消息

---

## 🔍 故障排除

### 问题1：提交消息仍然乱码

**解决**：确保使用 `git_commit_utf8.ps1` 脚本或手动创建UTF-8文件。

### 问题2：提交了不该提交的文件

**解决**：
```powershell
# 从Git中移除但保留本地文件
git rm --cached path/to/file

# 提交移除操作
.\git_commit_utf8.ps1 "移除不需要的文件"
```

### 问题3：想修改上一次提交

**解决**：
```powershell
# 修改上一次提交消息
"新的提交消息" | Out-File -FilePath temp.txt -Encoding UTF8
git commit --amend -F temp.txt
Remove-Item temp.txt

# 添加遗漏的文件到上一次提交
git add forgotten_file.cs
git commit --amend --no-edit
```

### 问题4：误提交了敏感信息

**解决**：
```powershell
# 如果还没push，使用reset
git reset --soft HEAD~1  # 撤销提交但保留修改
git reset --hard HEAD~1  # 撤销提交并丢弃修改（危险）

# 如果已经push，需要联系管理员
```

---

## 📚 参考资源

- [Git官方文档](https://git-scm.com/doc)
- [Git Book（中文）](https://git-scm.com/book/zh/v2)
- [GitHub学习实验室](https://lab.github.com/)

---

**创建时间**：2025-10-17  
**最后更新**：2025-10-17  
**维护者**：项目开发团队

