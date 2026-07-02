# Skill 牌

`${STS_SOURCE_PATH}` 表示当前项目的 STS2 source root；TestTheSpireTemplateRitsu 默认使用 `local/sts2-decompiled/`。

## 最小骨架

Skill 牌通常从 `: base(cost, CardType.Skill, rarity, targetType)` 开始。基础样板可以直接看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Zap.cs:20`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Acrobatics.cs:22`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Alchemize.cs:20`。

只在需要时才覆写这些成员：

- `CanonicalKeywords`
- `CanonicalVars`
- `ExtraHoverTips`
- `OnPlay`
- `OnUpgrade`

## 抽牌、弃牌、选牌

- 抽牌后弃牌：先 `CardPileCmd.Draw(...)`，再 `CardSelectCmd.FromHandForDiscard(...)`，最后 `CardCmd.Discard(...)`，模板见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Acrobatics.cs:35`。
- 从手牌选牌：`CardSelectCmd.FromHand(...)` + `new CardSelectorPrefs(source.SelectionScreenPrompt, 1)`，模板见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Transfigure.cs:55`。
- 从网格选牌：`CardSelectCmd.FromSimpleGrid(...)`，模板见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Wish.cs:34`。

一旦代码读取 `SelectionScreenPrompt`，就必须存在 `cards.<id>.selectionScreenPrompt`，否则会直接抛异常，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:124`。

## Hover Tips 与关键词

`HoverTips` 会自动合并 `ExtraHoverTips`、关键词和其他系统提示，所以 `ExtraHoverTips` 只放技能卡专属信息，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:740`。

- Orb / channel 类提示：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Zap.cs:25`
- Forge 类提示：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/TheSmith.cs:36`
- `Exhaust` 等关键词：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Alchemize.cs:27`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Wish.cs:26`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Transfigure.cs:27`

## 生成限制与升级

- 默认情况下，卡牌允许战斗中生成，也允许被 modifier 生成，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:574` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:576`。
- 只有真的不该进随机生成池时才覆写，例如 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Alchemize.cs:25`。

常见升级模式：

- 降费用：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Zap.cs:44`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Alchemize.cs:42`
- 提高数值：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Acrobatics.cs:45`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/TheSmith.cs:45`
- 改关键词：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Wish.cs:44`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Transfigure.cs:66`

如果没有特别需求，就保持基类默认的单次升级模型 `MaxUpgradeLevel => 1`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:640`。

## 实用判断标准

- 选牌类 Skill 优先绑定 `SelectionScreenPrompt`，不要硬编码提示文本。
- `OnPlay` 只负责效果本身；额外说明走 `DynamicVars` / `ExtraHoverTips`。
- 需要特殊生成规则时，只覆写最小必要的标志位。
