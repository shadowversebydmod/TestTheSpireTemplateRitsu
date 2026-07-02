# Shared Foundations

This file is an index and checklist. Re-open current source with `rg -n` before citing line numbers or editing behavior.

`${STS_SOURCE_PATH}` is the resolved STS2 source root from `SKILL.md`. In TestTheSpireTemplate, it normally points to `local/sts2-decompiled/` after running the decompile target.

## Core STS2 Entry Points

Start from decompiled local source when available:

- `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs`: title/description/selection prompt, costs, keywords, dynamic vars, hover tips, play/upgrade hooks, targeting, asset paths, and pool lookup.
- `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Commands/`: damage, powers, card mutations, card movement, selection, and combat actions.
- `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/`: base game card examples.
- `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardPools/`: base game pool membership.
- `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Entities/Cards/`: `CardType`, `CardRarity`, `TargetType`, `CardKeyword`.

If the decompiled source is missing, run the repo target documented in `AGENTS.md` before implementing behavior.

## Choosing a Sample

Choose examples by behavior, not by name:

- Same `CardType`
- Same `TargetType`
- Same cost family: fixed energy, energy X, fixed stars, or star X
- Same action family: damage, block, draw/discard, apply power, generate card, select card, transform, exhaust, quest hook, or component hook
- Same multiplayer risk: local choice, random choice, generated card, pile mutation, or combat-state mutation

Only use type references after finding at least one current source sample:

- [attack.md](attack.md)
- [skill.md](skill.md)
- [power.md](power.md)
- [status.md](status.md)
- [curse.md](curse.md)
- [quest.md](quest.md)

## Project Overlay

For TestTheSpireTemplate-derived repos, inspect these after the STS2 entry points:

- project card base, such as `CharModCode/Cards/CharModCard.cs`
- project card pool, such as `CharModCode/Character/CharModCardPool.cs`
- project character file, such as `CharModCode/Character/CharMod.cs`
- project components, such as `CharModCode/Components/`
- localization JSON under the mod payload, such as `CharMod/localization/eng/`
- tests under `CharMod.Tests/Tests/` or the renamed test project

Ritsu template cards are normally registered through the content pack in `CharModCode/MainFile.cs`. Do not add `[Pool(typeof(...))]` unless current source proves the project uses attribute-based pool registration.

## Localization and Assets

Most template-derived projects edit JSON directly:

- `CharMod/localization/eng/cards.json`
- `CharMod/localization/eng/card_keywords.json`
- `CharMod/localization/eng/static_hover_tips.json`
- other `CharMod/localization/<locale>/*.json` files as needed

If the repo defines a localization source-generation pipeline, follow that repo's pipeline instead.

`CardModel` logical localization keys are usually:

- `cards.<Id.Entry>.title`
- `cards.<Id.Entry>.description`
- `cards.<Id.Entry>.selectionScreenPrompt`

For template cards, serialized JSON keys may be unprefixed under RitsuLib, such as `SAMPLE_ENHANCE_STRIKE.title`. Confirm with existing JSON before adding keys.

Portrait resources should follow the project base card's `PortraitPath` and `CustomPortraitPath` rules. In the template this usually maps to `CharMod/images/card_portraits/`.

## Dynamic Vars, Keywords, and Hover Tips

Use these only when card text or logic needs them:

- `CanonicalVars`: values consumed by code or displayed in localization.
- `CanonicalKeywords`: keyword display and tooltip integration.
- `ExtraHoverTips`: additional tooltip entries when current samples use it.
- component `HoverTips`: keyword/tooltips owned by a `CardComponent`.

Do not duplicate values in prose if a `DynamicVar` already drives the description. Keep card text, dynamic variables, and upgrade previews tied to the same source of truth.

## Costs and Targets

- Fixed energy: constructor cost argument.
- Energy X: match current `HasEnergyCostX` and `ResolveEnergyXValue()` samples.
- Fixed stars and star X: copy current star-cost samples from decompiled source.
- Single target cards usually guard `cardPlay.Target` in `OnPlay`; match the closest sample's null handling.
- `AllEnemies` and `RandomEnemy` cards should use group/random targeting helpers instead of `cardPlay.Target`.

## Pools and Ownership

Base game cards belong to explicit `CardPoolModel.GenerateAllCards()` lists. Template custom cards normally use `[Pool(typeof(...))]`.

Starter decks live in the project character model, such as `CharModCode/Character/CharMod.cs`.

Generated tokens and option cards should usually be hidden from normal generation or placed in a token/status/curse pool, following nearby samples.

## Hooks and State

Add lifecycle hooks only when the effect needs them. Common examples:

- `AfterCreated()` for one-time quest or generated-card state.
- `HasTurnEndInHandEffect` / `OnTurnEndInHand()` for hand penalties.
- `AfterCardDrawn()` for draw-triggered status or curse effects.
- `AfterCardPlayed(...)`, `AfterCardExhausted(...)`, and component hooks for cross-card behavior.
- `[SavedProperty]` for persistent state; copy setter patterns from current source.

For component cards, read [minionlib-component.md](minionlib-component.md).

## General Checklist

- Class name produces the intended model id.
- Pool attribute or pool list is correct.
- Cost, type, rarity, target, and upgrade count match the design.
- Dynamic vars cover every variable in localization.
- Selection prompts exist only when code reads `SelectionScreenPrompt`.
- Generated-card, random, and selection behavior follows a deterministic current sample.
- Localization JSON and portrait assets match the card state surface.
