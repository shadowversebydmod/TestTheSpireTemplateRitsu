# TestTheSpireTemplateRitsu

English: [README.en.md](README.en.md)

TestTheSpireTemplateRitsu 是一个 Slay the Spire 2 角色 mod 模板。它把 Godot mod 项目、C# 代码、RitsuLib、MinionLib、TestTheSpire 测试项目、本地 API 参考目录和 skill 都整合到了同一个仓库中。开发者可以直接开始角色设计和卡牌设计，AI 也能沿着仓库里的源码、测试和 skill 说明进入具体开发工作。


## 解决的问题

STS2 mod 开发经常卡在人工测试和相关资源获取上。开发者给 AI 一句“加一张造成伤害的卡”，AI 需要知道当前项目的基类、RitsuLib 内容注册方式、动态数值写法、MinionLib 组件 API、本地化 key、图片路径、测试入口和本机 STS2 安装位置等等信息。少了其中一个输入，AI 往往会去猜 API，通过手动反编译的方式低效地获取所需的接口信息，耗费冗长的时间，最终交给开发者一个只能通过编译的版本，无法对具体的游戏行为做出保证的代码。之后还需要人工进行测试，反馈给 AI 问题再进行反复修改。

这个模板把这些输入固定成仓库结构：

- `local/` 保存真实依赖源码和 STS2 反编译源码，AI 可以查当前版本的 API。
- `CharMod.Tests/` 使用 `TestTheSpire` NuGet 包，AI 可以给新增卡牌补 focused combat test。
- `.agents/skills/` 暴露单卡开发和批量卡协调 skill，AI 在开始前知道该读哪些文件。
- `AGENTS.md` 记录开发前检查项，开发者可以直接要求 AI 先确认仓库状态。

通过这个结构，AI 可以使用它熟悉的 `rg`、`grep` 等方式搜寻第三方代码库，也可以自行编写测试用例并运行 TestTheSpire。测试失败时，输出会落到具体 fact 和战斗状态上，AI 可以回到卡牌实现、组件或本地化文件里继续修正，开发者不用在每次生成代码后手动进游戏复现同一条路径。

## 开始第一个 mod

先安装模板并创建项目：

```bash
cd TestTheSpireTemplateRitsu

dotnet new install .
dotnet new testthespire-sts2-character-ritsu \
  -n MyFirstMod \
  -o ../MyFirstMod \
  --ModAuthor "Your Name"
```

进入新项目后初始化 submodule。`local/RitsuLib`、`local/MinionLib` 和 `bundled_skills` 都通过 `.gitmodules` 取得。`dotnet new` 输出里已经带有 `.agents/skills`，初始化 submodule 后也能看到 skill 的上游来源：

```bash
cd ../MyFirstMod
git init
git submodule update --init --recursive
```

如果 STS2 没有安装在标准 Steam 路径，把本机路径写到 ignored 的 `LocalSettings.props`：

```xml
<Project>
  <PropertyGroup>
    <Sts2Path>/path/to/SlayTheSpire2</Sts2Path>
    <GodotPath>/path/to/MegaDot_v4.5.1-stable_mono</GodotPath>
  </PropertyGroup>
</Project>
```

然后跑一次构建、反编译和测试。下面命令假设项目名是 `MyFirstMod`：

```bash
dotnet build MyFirstMod.csproj -v:minimal

dotnet msbuild MyFirstMod.csproj \
  -t:DecompileSts2 \
  -p:Sts2Path=/path/to/SlayTheSpire2

dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2
```

`DecompileSts2` 会把 `sts2.dll` 反编译到 `local/sts2-decompiled/`。这个目录用于阅读 API，已经从构建和 git 提交中排除。

## 交给 AI 的输入

在让 AI 开发前，先提供这些信息：

- STS2 安装路径，例如 `/home/me/games/SlayTheSpire2` 或 Windows 上的 Steam 安装目录。
- 当前要改的项目名和测试项目名，例如 `MyFirstMod.csproj`、`MyFirstMod.Tests/MyFirstMod.Tests.csproj`。
- 需要新增或修改的卡牌信息：卡名、类名、费用、类型、稀有度、目标、卡池、完整效果文本、升级变化。
- 生成牌、token、关键字、Power、Relic、图片资源和本地化文本的位置。
- 测试期待：基础效果、升级效果、边界状态、随机选择或牌堆变化。
- 批量任务的来源文件，例如 CSV、Markdown 表格或 issue 文本，以及要跳过的行。

可以先发一条检查提示：

```text
请先按照 AGENTS.md 检查这个仓库是否适合 AI 开发。STS2 路径是 `/path/to/SlayTheSpire2`。确认 submodule、local/sts2-decompiled、local/RitsuLib、local/MinionLib、bundled skills 和 TestTheSpire smoke test 正常后，再开始实现。
```

## 新增单张卡牌的提示词

单张卡牌可以直接使用 `sts2-card-creator`：

```text
请使用 $sts2-card-creator 在这个 mod 中新增一张卡。

卡名：Training Smite
类名：TrainingSmite
类型/稀有度/费用/目标：Attack / Common / 1 / AnyEnemy
卡池：当前角色卡池
效果：造成 6 点伤害。
升级：伤害变为 9。
图片：先复用 CharMod/images/card_portraits/card.png。
本地化：补到 CharMod/localization/eng/cards.json。
测试：新增 focused TestTheSpire 测试，验证基础伤害和升级伤害。测试 filter 使用 TrainingSmite。
STS2 路径：/path/to/SlayTheSpire2

请先读取当前项目里的相近 Attack 样例、CharModCard、local/sts2-decompiled、local/RitsuLib 和 local/MinionLib，再实现并运行 focused test。
```

涉及 RitsuLib 注册或 MinionLib 组件时，把触发条件写具体：

```text
请使用 $sts2-card-creator 新增一张带爆能强化的攻击牌。

卡名：Training Enhance Strike
类名：TrainingEnhanceStrike
类型/稀有度/费用/目标：Attack / Common / 1 / AnyEnemy
效果：造成 6 点伤害。爆能强化 3：再造成 6 点伤害。
升级：两段伤害都变为 9。
参考：CharModCode/Cards/SampleEnhanceStrike.cs 和 CharModCode/Components/Enhances/。
测试：能量为 1 时只造成一次伤害；能量为 3 时触发爆能强化并造成两次伤害。
```

## 批量卡牌的输入

一批卡牌适合使用 `card-task-orchestrator`。README 里只需要给 AI 输入材料；分支、占位符、开发测试和 review 细节已经写在 skill 中。

```text
请使用 $card-task-orchestrator 按 `cards.csv` 实现这批卡。

范围：只实现 `角色卡牌` 章节，跳过 `遗物` 和 `仅资源占位` 行。
集成分支：main
tracker：impl_task_list.md
worktree 根目录：../myfirstmod-worktrees
STS2 路径：/path/to/SlayTheSpire2
测试命令模板：
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj -restore -t:RunSts2Tests -p:Sts2Path=/path/to/SlayTheSpire2 -p:Sts2TestArgs=--sts2-test-filter=<filter>

请先检查 CSV 中缺失的类名、资源、关键字、生成牌和共性机制，把需要我确认的问题列出来。易冲突的本地化和资源清单先建立占位条目，再开始单卡开发。
```

CSV 或表格建议包含这些列：

```text
CardName,ClassName,Type,Rarity,Cost,Target,Pool,Description,Upgrade,Tokens,Asset,Notes
```

下面是从实际卡牌表改写出的几行示例。原表里的项目专用 `CardID` 等列已经去掉，保留 AI 实现卡牌时真正需要读取的信息：

```csv
CardName,DisplayName,ClassName,Type,Rarity,Cost,Tags,Description,Notes
灵魂捕食,Soul Predation,SoulPredation,Skill,Basic,1,亡灵,【破坏】抽取 2(3/5) 张牌。,Race: Undead；破坏=选择弃牌堆的一张牌并消耗，选择成功才触发后续
夜魔,Night Fiend,NightFiend,Attack,Basic,1,月下,失去 2 点生命值，造成 8(10/15) 伤害。,Race: Moonlit
剧毒公主·美杜莎,"Medusa, Venomfang Royalty",MedusaVenomfangRoyalty,Attack,Rare,2,月下,对随机敌人造成 2 易伤，和 4(6/10) 伤害。重放 3。,Race: Moonlit
```

当效果文本里有缩写或项目内约定，先补一段规则说明。例如：

```text
规则补充：
- A(B) 表示基础值 A，升级后 B。
- A(B/C) 表示基础值 A，升级后 B，特殊状态下 C。
- “生成一张复制”表示保留升级状态，进入弃牌堆。
- 随机敌人按当前战斗中的敌人列表筛选活着的敌人。
```

## 目录作用

- `.template.config/`：dotnet template 元数据，定义 `dotnet new testthespire-sts2-character-ritsu`。
- `.agents/`：给 Codex 读取的项目级说明和 skill 入口。模板仓库里这些入口指向 `bundled_skills`，`dotnet new` 输出里会直接带上可用的 skill 目录。
- `AGENTS.md`：AI 开发前的仓库状态检查清单。
- `bundled_skills/`：`TestTheSpireSkills` submodule，提供单卡开发和批量协调 skill。
- `CharMod/`：Godot 资源、本地化 JSON、图片和 mod 图标。
- `CharModCode/`：mod 的 C# 代码。样例卡是 `CharModCode/Cards/SampleEnhanceStrike.cs`。
- `CharMod.Tests/`：TestTheSpire 测试项目，`Entry.cs` 是测试 mod initializer。
- `STS2.RitsuLib`：RitsuLib NuGet 依赖，提供内容注册、角色/卡牌模板和运行时 patch。
- `local/RitsuLib/`：RitsuLib 源码 submodule，用于查内容注册、角色/卡牌模板和运行时 patch API。
- `local/MinionLib/`：MinionLib 源码 submodule，用于查组件 API 和样例。
- `local/sts2-decompiled/`：`DecompileSts2` 生成的 STS2 反编译源码，保持 ignored。
- `CharMod.csproj`：主 mod 项目，包含依赖、复制到 STS2 mods 目录和反编译 target。
- `Sts2PathDiscovery.props`：按 Windows、Linux、macOS 推导 STS2 根目录和 `sts2.dll` 数据目录。
- `Directory.Build.props`：本机 `LocalSettings.props` 入口和 Godot 默认路径。
- `CharMod.json`：STS2 mod manifest。
- `project.godot`、`export_presets.cfg`：Godot 项目和导出配置。

## 测试命令

列出测试：

```bash
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:ListSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2
```

运行全部测试：

```bash
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2
```

Linux 下如果 RitsuLib 输出 `undefined symbol: _Unwind_RaiseException`，给测试命令加上：

```bash
LD_PRELOAD=/usr/lib/x86_64-linux-gnu/libgcc_s.so.1 \
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2
```

运行单个 focused test：

```bash
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2 \
  -p:Sts2TestArgs=--sts2-test-filter=Sample_enhance
```

测试前确认 STS2 的 `mods/` 目录里没有其它 TestTheSpire 测试 mod。STS2 会加载所有启用的测试 mod，后初始化的测试入口可能会接管本次运行。

## 更多参考

- TestTheSpire: <https://github.com/shadowversebydmod/TestTheSpire>
- TestTheSpire NuGet: <https://www.nuget.org/packages/TestTheSpire>
- TestTheSpireSkills: <https://github.com/shadowversebydmod/TestTheSpireSkills>
- ModTemplate-StS2: <https://github.com/Alchyr/ModTemplate-StS2>
- RitsuLib NuGet: <https://www.nuget.org/packages/STS2.RitsuLib>
- RitsuLib source: <https://github.com/BAKAOLC/STS2-RitsuLib>
- MinionLib: <https://github.com/FuYnAloft/MinionLib>
- ModTemplate-StS2 wiki: <https://github.com/Alchyr/ModTemplate-StS2/wiki>
- .NET custom templates: <https://learn.microsoft.com/dotnet/core/tools/custom-templates>

## 致谢

这个模板参考并延续了 Alchyr 的 [ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2) 项目结构。RitsuLib、MinionLib 和 STS2 mod 社区已有的模板实践提供了最初的项目形状；TestTheSpireTemplateRitsu 在这个基础上补充了 TestTheSpire 测试项目、本地源码参考目录和面向 AI 开发的 skill 入口。

## License

This template is licensed under the GNU Affero General Public License v3.0. See [LICENSE](LICENSE).
