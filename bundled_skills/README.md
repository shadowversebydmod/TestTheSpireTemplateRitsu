# TestTheSpireSkills

这个仓库保存 TestTheSpireTemplateRitsu 附带的 Codex skills。模板项目通过 `bundled_skills` 子模块引用这里的内容，再用 `.agents/skills/` 下的符号链接把 skill 暴露给 Codex。

## 包含的 Skill

`card-task-orchestrator`

用于协调一批卡牌的实现工作。它负责澄清实现范围、维护 `impl_task_list.md`、先处理本地化和资源清单这类易冲突文件的占位、安排共性机制设计 review、启动单卡开发自测 agent、启动只读 review agent、控制合入顺序，并记录 TestTheSpire 验证结果。

`sts2-card-creator`

用于实现、修改或 review 单张 STS2 卡牌。它会优先读取当前项目里的真实源码和依赖文档，例如 `local/sts2-decompiled/`、当前 mod 源码、RitsuLib NuGet XML 文档、`local/MinionLib/`。单卡代码、本地化、组件、Focused TestTheSpire 测试都属于它的工作范围。

## 在模板项目中使用

模板项目初始化后先拉取子模块：

```bash
git submodule update --init --recursive
```

期望目录结构：

```text
template-project/
  bundled_skills/
    skills/
      card-task-orchestrator/
      sts2-card-creator/
  .agents/
    skills/
      card-task-orchestrator -> ../../bundled_skills/skills/card-task-orchestrator
      sts2-card-creator -> ../../bundled_skills/skills/sts2-card-creator
```

开始卡牌开发前，告诉 AI 先检查：

- `git submodule status -- bundled_skills`
- `.agents/skills/card-task-orchestrator`
- `.agents/skills/sts2-card-creator`
- `local/sts2-decompiled/`
- RitsuLib NuGet XML 文档
- `local/MinionLib/`

这些检查能确认 AI 读取到的是模板项目期望的 skill 文本、反编译源码和依赖源码。

## 许可

本仓库使用 GNU Affero General Public License v3.0，完整文本见 [LICENSE](LICENSE)。

## 更新方式

修改 skill 时，先在这个仓库提交：

```bash
git status --short
git add skills README.md LICENSE
git commit -m "Update bundled skills"
```

然后在模板项目里更新子模块指针：

```bash
git -C bundled_skills pull --ff-only origin main
git add bundled_skills
```

模板里的 `.agents/skills/...` 是符号链接。维护 skill 时直接改这个仓库，提交后再更新模板项目的 `bundled_skills` 指针。

## 维护原则

每个 skill 只承担一个清晰职责。流程、角色边界和必须执行的检查写在 `SKILL.md`；需要按场景加载的详细卡牌模式放在 `references/`。

项目级说明放在这个根目录 README。单个 skill 目录里保持精简，避免加入独立的 README、安装指南或变更日志。
