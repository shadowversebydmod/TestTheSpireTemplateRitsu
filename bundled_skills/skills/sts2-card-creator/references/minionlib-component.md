# MinionLib Component Cards

This file summarizes stable MinionLib usage for TestTheSpireTemplate-derived repos. Re-open current source with `rg -n` before citing line numbers.

## Source Files

Inspect these first:

- `local/MinionLib/Component/ComponentsCardModel.cs`
- `local/MinionLib/Component/CardComponent.cs`
- `local/MinionLib/Component/Partials/ComponentsCardModel_Hooks.cs`
- `local/MinionLib/Component/Core/ComponentStateAttribute.cs`
- `local/MinionLib/Component/Core/LocArgAttribute.cs`
- `local/MinionLib/Example/Cards/`
- `local/MinionLib/Example/Components/`

If the repo uses a different MinionLib layout or version, trust the local submodule and NuGet package version over this reference.

## Minimal Pattern

- Cards inherit `ComponentsCardModel`, `CustomComponentsCardModel`, or a project base card that already derives from `CustomComponentsCardModel`.
- Initial components live in `CanonicalComponents`.
- Runtime components are added with `AddComponent()` or related helpers.
- Component classes normally use `public sealed partial class XxxComponent : CardComponent`.
- Component state that must persist uses `[ComponentState]`; values that feed localization use `[LocArg]` or generated DynamicVar patterns from the current MinionLib source.

## Component-Aware Hooks

Component cards should not override the sealed base signatures. Use the `ComponentContext` overloads:

- `OnPlay(PlayerChoiceContext, CardPlay, ComponentContext)`
- `OnUpgrade(ComponentContext)`
- generated component timing hooks in `Component/Partials/`

When a build error says a member is sealed, search `ComponentsCardModel_Hooks.cs` for the matching overload.

## Description and Localization

Component prefix/postfix text defaults to card localization keys based on `ComponentId`, unless the component overrides `PrefixLocString` or `PostfixLocString`.

Cards using components normally include `{CompPre}` and/or `{CompPost}` in their description. Confirm current project JSON before adding keys. In the template, component text is stored in files such as `CharMod/localization/eng/cards.json`.

Changing a component class name, namespace, or `ComponentId` can affect save deserialization and localization keys. Treat those as compatibility changes after a mod has shipped.

## Common Patterns

- Single numeric stack component: inspect MinionLib amount-style examples and current project components.
- Multiple timing hooks: inspect `TimingCardComponent` and generated timing files.
- Keyword tooltip owned by a component: override `HoverTips`.
- Combat energy/cost modification: inspect current components that override energy-cost modification hooks and refresh their state when energy or piles change.

## Pitfalls

- Same-type components may merge depending on `TryMergeWith`; do not assume `AddComponent(new X())` always appends a second copy.
- Component state records what is attached to the card and the component's values. Actual combat behavior should still use STS2 commands such as damage, powers, card movement, or pile commands.
- Keep component classes `sealed partial` unless current MinionLib source shows a reason not to.
- Keep tests focused on visible behavior: description prefix, cost modification, hook timing, or combat result.
