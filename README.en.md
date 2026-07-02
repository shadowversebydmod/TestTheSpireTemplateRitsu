# TestTheSpireTemplateRitsu

Chinese: [README.md](README.md)

TestTheSpireTemplateRitsu is a Slay the Spire 2 character mod template. It keeps the Godot mod project, C# code, RitsuLib, MinionLib, a TestTheSpire test project, local API references, and skills in one repository. Developers can start character and card design from that project shape, and the AI can enter the work through the local source, tests, and skill instructions already present in the repo.

After a developer gives the AI card text, the STS2 path, and expected test coverage, the AI can inspect `local/sts2-decompiled/`, `local/RitsuLib/`, `local/MinionLib/`, and nearby examples before changing code, localization, and tests. Tests run through MSBuild targets that launch STS2 in the headless TestTheSpire flow, so review output can point to the exact card and combat path that failed.

## Problem

STS2 mod work often gets stuck on manual testing and hard-to-reach reference material. A prompt such as "add a damage card" still leaves the AI needing the project base card class, RitsuLib content registration, dynamic variable style, MinionLib component APIs, localization keys, portrait paths, the test entry point, and the local STS2 install path. When one of those inputs is missing, the AI tends to guess an API or spend a long time manually decompiling assemblies to recover interface details. The result may compile while still giving no evidence that the in-game behavior is correct, so the developer has to test by hand, report failures, and ask for another round of changes.

This template makes those inputs visible in the repository:

- `local/` stores real dependency sources and decompiled STS2 sources for API lookup.
- `CharMod.Tests/` uses the `TestTheSpire` NuGet package so new cards can get focused combat tests.
- `.agents/skills/` exposes single-card and batch-card skills to Codex.
- `AGENTS.md` records the checks the AI should run before development starts.

With this structure, the AI can search third-party code with familiar tools such as `rg` and `grep`, then write and run TestTheSpire tests. When a test fails, the output names a concrete fact and combat state, so the AI can return to the card implementation, component code, or localization file instead of asking the developer to reproduce the same path in-game after every generated patch.

## Start Your First Mod

Install the template and create a project:

```bash
cd TestTheSpireTemplateRitsu

dotnet new install .
dotnet new testthespire-sts2-character-ritsu \
  -n MyFirstMod \
  -o ../MyFirstMod \
  --ModAuthor "Your Name"
```

Initialize submodules in the new project. `local/RitsuLib`, `local/MinionLib`, and `bundled_skills` come from `.gitmodules`. The `dotnet new` output already contains `.agents/skills`; after submodule initialization, the upstream skill source is visible as well:

```bash
cd ../MyFirstMod
git init
git submodule update --init --recursive
```

If STS2 is outside a standard Steam path, put your local paths in the ignored `LocalSettings.props` file:

```xml
<Project>
  <PropertyGroup>
    <Sts2Path>/path/to/SlayTheSpire2</Sts2Path>
    <GodotPath>/path/to/MegaDot_v4.5.1-stable_mono</GodotPath>
  </PropertyGroup>
</Project>
```

Run one build, one decompile pass, and one test run. These commands assume the project is named `MyFirstMod`:

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

`DecompileSts2` writes decompiled `sts2.dll` sources to `local/sts2-decompiled/`. That directory is for reading APIs and stays ignored by build and git.

## Inputs For AI

Before asking the AI to develop, provide these inputs:

- STS2 install path, such as `/home/me/games/SlayTheSpire2` or the Steam install directory on Windows.
- Current mod project and test project names, such as `MyFirstMod.csproj` and `MyFirstMod.Tests/MyFirstMod.Tests.csproj`.
- Card data: card name, class name, cost, type, rarity, target, pool, full effect text, and upgrade changes.
- Generated cards, tokens, keywords, powers, relics, images, and localization locations.
- Test expectations: base effect, upgrade effect, boundary states, random choices, or pile changes.
- Batch source file, such as a CSV, Markdown table, or issue body, plus rows that should be skipped.

A useful first prompt:

```text
Please follow AGENTS.md and check whether this repository is ready for AI development. The STS2 path is `/path/to/SlayTheSpire2`. Confirm submodules, local/sts2-decompiled, local/RitsuLib, local/MinionLib, bundled skills, and the TestTheSpire smoke test before implementing anything.
```

## Single Card Prompt

For one card, use `sts2-card-creator`:

```text
Please use $sts2-card-creator to add one card to this mod.

Card name: Training Smite
Class name: TrainingSmite
Type / rarity / cost / target: Attack / Common / 1 / AnyEnemy
Pool: current character card pool
Effect: Deal 6 damage.
Upgrade: damage becomes 9.
Image: reuse CharMod/images/card_portraits/card.png for now.
Localization: add entries to CharMod/localization/eng/cards.json.
Test: add a focused TestTheSpire test for base damage and upgraded damage. Use TrainingSmite as the test filter.
STS2 path: /path/to/SlayTheSpire2

Please inspect nearby Attack examples, CharModCard, local/sts2-decompiled, local/RitsuLib, and local/MinionLib before implementing, then run the focused test.
```

For RitsuLib registration or MinionLib components, spell out the trigger:

```text
Please use $sts2-card-creator to add an attack card with Enhance.

Card name: Training Enhance Strike
Class name: TrainingEnhanceStrike
Type / rarity / cost / target: Attack / Common / 1 / AnyEnemy
Effect: Deal 6 damage. Enhance 3: deal 6 additional damage.
Upgrade: both hits become 9.
Reference: CharModCode/Cards/SampleEnhanceStrike.cs and CharModCode/Components/Enhances/.
Test: with 1 energy it deals one hit; with 3 energy it triggers Enhance and deals two hits.
```

## Batch Card Inputs

For a card batch, use `card-task-orchestrator`. The README only needs to give the AI the source material; branch handling, placeholder reservation, development tests, and review loops are documented in the skill.

```text
Please use $card-task-orchestrator to implement the cards in `cards.csv`.

Scope: implement only the `character cards` section. Skip `relic` and `resource placeholder only` rows.
Integration branch: main
Tracker: impl_task_list.md
Worktree root: ../myfirstmod-worktrees
STS2 path: /path/to/SlayTheSpire2
Test command template:
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj -restore -t:RunSts2Tests -p:Sts2Path=/path/to/SlayTheSpire2 -p:Sts2TestArgs=--sts2-test-filter=<filter>

Please check the CSV for missing class names, assets, keywords, generated cards, and shared mechanics first, then list questions I need to answer. Reserve placeholders in conflict-prone localization and asset manifests before single-card development starts.
```

Suggested CSV columns:

```text
CardName,ClassName,Type,Rarity,Cost,Target,Pool,Description,Upgrade,Tokens,Asset,Notes
```

These rows are adapted from a real card table with project-specific columns such as `CardID` removed. They keep the information the AI needs when implementing cards:

```csv
CardName,DisplayName,ClassName,Type,Rarity,Cost,Tags,Description,Notes
灵魂捕食,Soul Predation,SoulPredation,Skill,Basic,1,Undead,Destroy: draw 2(3/5) cards.,Race: Undead; Destroy means choose a card from the discard pile and exhaust it; resolve the follow-up only after a successful choice
夜魔,Night Fiend,NightFiend,Attack,Basic,1,Moonlit,Lose 2 HP. Deal 8(10/15) damage.,Race: Moonlit
剧毒公主·美杜莎,"Medusa, Venomfang Royalty",MedusaVenomfangRoyalty,Attack,Rare,2,Moonlit,Apply 2 Vulnerable to a random enemy and deal 4(6/10) damage. Replay 3.,Race: Moonlit
```

If the effect text uses project-specific notation, add a rules block:

```text
Rules:
- A(B) means base value A and upgraded value B.
- A(B/C) means base value A, upgraded value B, and special-state value C.
- "Create a copy" keeps upgrade state and puts the copy into the discard pile.
- Random enemy means choose from living enemies in the current combat enemy list.
```

## Directory Guide

- `.template.config/`: dotnet template metadata for `dotnet new testthespire-sts2-character-ritsu`.
- `.agents/`: project instructions and skill entries for Codex. In the template repository these entries point to `bundled_skills`; the `dotnet new` output contains usable skill directories directly.
- `AGENTS.md`: repository readiness checklist for AI development.
- `bundled_skills/`: `TestTheSpireSkills` submodule with single-card and batch-card skills.
- `CharMod/`: Godot assets, localization JSON, images, and mod icon.
- `CharModCode/`: C# mod code. The sample card is `CharModCode/Cards/SampleEnhanceStrike.cs`.
- `CharMod.Tests/`: TestTheSpire test project. `Entry.cs` is the test mod initializer.
- `STS2.RitsuLib`: RitsuLib NuGet dependency for content registration, character/card templates, and runtime patches.
- `local/RitsuLib/`: RitsuLib source submodule for content registration, character/card templates, and runtime patch APIs.
- `local/MinionLib/`: MinionLib source submodule for component APIs and examples.
- `local/sts2-decompiled/`: decompiled STS2 sources generated by `DecompileSts2`; kept ignored.
- `CharMod.csproj`: main mod project with dependencies, copy-to-mods logic, and the decompile target.
- `Sts2PathDiscovery.props`: derives the STS2 root and `sts2.dll` data directory on Windows, Linux, and macOS.
- `Directory.Build.props`: local settings import and default Godot path.
- `CharMod.json`: STS2 mod manifest.
- `project.godot`, `export_presets.cfg`: Godot project and export settings.

## Test Commands

List tests:

```bash
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:ListSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2
```

Run all tests:

```bash
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2
```

On Linux, if RitsuLib logs `undefined symbol: _Unwind_RaiseException`, prefix the test command with:

```bash
LD_PRELOAD=/usr/lib/x86_64-linux-gnu/libgcc_s.so.1 \
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2
```

Run one focused test:

```bash
dotnet msbuild MyFirstMod.Tests/MyFirstMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2 \
  -p:Sts2TestArgs=--sts2-test-filter=Sample_enhance
```

Before testing, make sure the STS2 `mods/` directory does not contain another active TestTheSpire test mod. STS2 loads every enabled test mod, and the last initialized test entry can take over the run.

## References

- TestTheSpire: <https://github.com/shadowversebydmod/TestTheSpire>
- TestTheSpire NuGet: <https://www.nuget.org/packages/TestTheSpire>
- TestTheSpireSkills: <https://github.com/shadowversebydmod/TestTheSpireSkills>
- ModTemplate-StS2: <https://github.com/Alchyr/ModTemplate-StS2>
- RitsuLib NuGet: <https://www.nuget.org/packages/STS2.RitsuLib>
- RitsuLib source: <https://github.com/BAKAOLC/STS2-RitsuLib>
- MinionLib: <https://github.com/FuYnAloft/MinionLib>
- ModTemplate-StS2 wiki: <https://github.com/Alchyr/ModTemplate-StS2/wiki>
- .NET custom templates: <https://learn.microsoft.com/dotnet/core/tools/custom-templates>

## Acknowledgements

This template references and extends the project structure from Alchyr's [ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2). RitsuLib, MinionLib, and the existing STS2 mod template practices provide the starting shape; TestTheSpireTemplateRitsu adds the TestTheSpire test project, local source reference directories, and AI-facing skill entries on top of that foundation.

## License

This template is licensed under the GNU Affero General Public License v3.0. See [LICENSE](LICENSE).
