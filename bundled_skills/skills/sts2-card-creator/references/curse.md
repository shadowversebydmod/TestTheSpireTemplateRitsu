# Curse 牌

`${STS_SOURCE_PATH}` 表示当前项目的 STS2 source root；TestTheSpireTemplateRitsu 默认使用 `local/sts2-decompiled/`。

## 最小骨架

Curse 牌的基础形状很稳定：`CardType.Curse`、`CardRarity.Curse`、`TargetType.None`、基础费用通常是 `-1`。直接参考 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/AscendersBane.cs:15`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Writhe.cs:15`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Decay.cs:21`。

`CardModel.CanPlay()` 会把 `Unplayable` 当作显式不可打出原因，所以诅咒的“不可打出”应该优先用关键词表达，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:1204` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:1213`。

## 关键词组合

- `AscendersBane`: `Eternal + Unplayable + Ethereal`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/AscendersBane.cs:24`
- `Writhe`: `Innate + Unplayable`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Writhe.cs:22`
- `Decay`: `Unplayable`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Decay.cs:36`
- `BadLuck`: `Eternal + Unplayable`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/BadLuck.cs:30`

## 固定不升级

现有诅咒样板都把 `MaxUpgradeLevel` 锁成 `0`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/AscendersBane.cs:22`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Writhe.cs:20`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Decay.cs:26`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/BadLuck.cs:28`。除非设计明确要求，否则新 Curse 也按这个规则处理。

## 生成限制

默认情况下，`CardModel.CanBeGeneratedByModifiers => true`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:576`。如果某张 Curse 只能来自固定来源，就像 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/AscendersBane.cs:20` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/BadLuck.cs:26` 一样覆写为 `false`。

## 手牌回合结束处罚

需要留手惩罚时，复用 `HasTurnEndInHandEffect` + `OnTurnEndInHand(...)`：

- `Decay`: 普通自伤，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Decay.cs:44`
- `BadLuck`: `HpLossVar` + 不可格挡伤害，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/BadLuck.cs:42`

## 卡池

Curse 是否被系统认作标准诅咒，以 `CurseCardPool` 为准。池定义见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardPools/CurseCardPool.cs:13`，当前列表见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardPools/CurseCardPool.cs:25`。

## 易错点

- Curse 的身份优先靠关键词和生命周期 hook 表达，不要靠奇怪的 target 或资源条件间接实现。
- 需要固定来源时别忘了关掉 `CanBeGeneratedByModifiers`。
