---
name: sts2-card-creator
description: Create, port, review, or modify Slay the Spire 2 card models in a TestTheSpireTemplateRitsu-derived mod. Use when Codex needs to add or change card cost/type/rarity/targeting/keywords/upgrades/DynamicVars/components/card pools/starting decks/localization/portraits, implement mechanics with RitsuLib or MinionLib, or add focused TestTheSpire combat tests. Also use for STS2 卡牌制作、卡牌效果移植、卡牌模型修改相关请求。
---

# STS2 Card Creator

Use current local source as the contract. References are navigation aids, not authority. If sources disagree, trust this order:

1. resolved `STS_SOURCE_PATH` source root
2. current mod repo source
3. RitsuLib package docs and `local/MinionLib/`
4. this skill's references

Keep the run concrete: read the nearest source examples, make the narrow change, validate, and summarize exact behavior. Do not create planning documents or spawn workers unless the user asks for coordination.

## Path Inputs

Resolve `STS_SOURCE_PATH` before using any type-specific reference file:

1. Use the `$STS_SOURCE_PATH` environment variable when it points at a source root containing `MegaCrit/Sts2/Core/Models/CardModel.cs`.
2. Otherwise use `local/sts2-decompiled/` when that file exists there. This is the TestTheSpireTemplateRitsu default after running the decompile target.
3. Otherwise use the project-documented STS2 source checkout or decompiled output.
4. If no source root exists, run the repo's decompile target or state that reference examples cannot be verified from local source.

Reference files use paths like `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Bash.cs`. Expand that placeholder to the resolved source root before opening files or citing line numbers.

## Source Workflow

1. Classify the request as answer-only, design, implementation, or review. If the user asks to add or change a card, implement it.
2. Locate current examples with `rg` before reading references:
   - `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs`
   - `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Commands/`
   - `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/`
   - project card folders under `CharModCode/` or the renamed mod namespace
   - `CharModCode/Components/` for component-driven mechanics
   - `CharMod.Tests/Tests/` or the renamed test project
3. Read references only when local examples do not settle the pattern:
   - Shared STS2 card rules: [references/foundation.md](references/foundation.md)
   - Component cards: [references/minionlib-component.md](references/minionlib-component.md)
   - Type-specific cards: [references/attack.md](references/attack.md), [references/skill.md](references/skill.md), [references/power.md](references/power.md), [references/status.md](references/status.md), [references/curse.md](references/curse.md), [references/quest.md](references/quest.md)
4. When citing code, produce fresh file:line references from the current checkout.

## Implementation Rules

- Match the nearest current sample by card type, target type, resource model, and mechanic.
- Plain cards derive from the project's base card class, such as `CharModCard`; component cards use the project base class that already derives from the generated Ritsu/MinionLib adapter.
- Component-aware overrides use `OnPlay(PlayerChoiceContext, CardPlay, ComponentContext)` and `OnUpgrade(ComponentContext)`.
- Register new cards through the established RitsuLib content-pack pattern, normally `RitsuLibFramework.CreateContentPack(...).Card<TPool, TCard>()`, unless current source uses a different local registration helper.
- Update starting decks only when the requested card should be a starter.
- Keep localization in the project localization JSON unless the repo defines a source-generation pipeline.
- Keep portrait paths aligned with the project base card's `PortraitPath` and `CustomPortraitPath` rules.
- Use deterministic game commands and existing synchronization patterns. For random selection, card selection, generated cards, or pile mutation, copy a current STS2 or repo pattern.
- Ask only when card rules are ambiguous enough that a reasonable implementation would likely be wrong.

## TestTheSpire Validation

Prefer focused tests when behavior changes. Typical command:

```bash
dotnet msbuild CharMod.Tests/CharMod.Tests.csproj \
  -restore \
  -t:RunSts2Tests \
  -p:Sts2Path=/path/to/SlayTheSpire2 \
  -p:Sts2TestArgs=--sts2-test-filter=<test-or-card-name>
```

If the repo renamed `CharMod.Tests`, discover the test project with `rg --files -g '*.Tests.csproj'`.

Before running tests, check whether the STS2 `mods/` directory contains another active TestTheSpire test mod. Multiple test entry mods can initialize in one run; if that would hide this repo's tests, isolate the runtime or state the limitation.

## Review Checklist

- Cost, type, rarity, target, pool, and class name match the request.
- `CanonicalVars`, `CanonicalKeywords`, components, tags, and hover tips are necessary and consistent.
- `OnPlay`, hooks, component state, and upgrade logic match card text.
- Single-target cards handle missing `cardPlay.Target` consistently with nearby samples.
- Localization includes title, description, component text, keyword text, and any selection prompt required by code.
- Starter cards, generated tokens, portraits, and alternate art are wired only when required.
- Component mechanics follow current `local/MinionLib/` patterns.
- Tests check mechanisms and state transitions first; exact numeric assertions are used only for card text contracts or regression guards.
- Build or focused TestTheSpire validation has run, or the limitation is stated clearly.

## Response Style

- For explanation questions, lead with current source paths and member names, then explain briefly.
- For implementation tasks, summarize changed files, card behavior, and validation.
- For reviews, list blocking findings first with file:line references.
