# AGENTS.md

## AI 开发前 Checklist

开始改卡牌、角色或机制前，先确认这个仓库处在适合 AI 开发的状态。AI 需要能看到本地游戏、RitsuLib 和 MinionLib 的真实 API，再进入具体实现。

- 运行 `git status --short --ignored`，把源码变更和生成产物分开看。`local/sts2-decompiled/`、构建输出、测试输出、`LocalSettings.props` 应该保持 ignored。
- 运行 `git submodule status -- local/RitsuLib local/MinionLib bundled_skills`。如果目录缺失，先执行 `git submodule update --init --recursive`。
- 检查 `.agents/skills/card-task-orchestrator` 和 `.agents/skills/sts2-card-creator` 是否指向 `bundled_skills/skills/` 下的同名目录。缺失时先修复 skill submodule 和 symlink，再做具体开发。
- 对照 `CharMod.csproj` 里的 NuGet 版本，确认 `local/RitsuLib/` 对应 `STS2.RitsuLib`，`local/MinionLib/` 对应 `FuYnAloft.Sts2.MinionLib`。
- 检查 `local/sts2-decompiled/sts2.csproj` 是否存在。缺失时运行 `dotnet msbuild CharMod.csproj -t:DecompileSts2 -p:Sts2Path=/path/to/SlayTheSpire2`。
- 确认 STS2 路径能通过 `LocalSettings.props`、标准 Steam 路径或显式 `/p:Sts2Path=...` 找到。不要提交 `LocalSettings.props`。
- 运行 `dotnet build TestTheSpireTemplateRitsu.sln -v:minimal`，通过后再把工作区视为可开发状态。
- 确认 STS2 的 `mods/` 目录里没有其它 TestTheSpire 测试 mod。多个测试 mod 同时加载时，后初始化的测试入口会覆盖前一个，AI 会看到“命令成功但没有执行本仓库测试”的假象。
- 运行 `dotnet msbuild CharMod.Tests/CharMod.Tests.csproj -restore -t:RunSts2Tests -p:Sts2Path=/path/to/SlayTheSpire2`，确认测试框架、主 mod、RitsuLib 和 MinionLib 能一起加载。Linux 下如果 RitsuLib 报 `_Unwind_RaiseException`，在命令前加 `LD_PRELOAD=/usr/lib/x86_64-linux-gnu/libgcc_s.so.1`。
- 实现卡牌或机制时，先查 `local/sts2-decompiled/`、`local/RitsuLib/`、`local/MinionLib/`，再参考 `CharModCode/Components/Enhances/` 和 `CharModCode/Cards/SampleEnhanceStrike.cs`。

告诉 AI：这些检查正常后，再开始具体开发工作。这样 MR 里的改动来源清楚，测试失败时也能直接回到本地源码、submodule 和反编译目录定位问题。
