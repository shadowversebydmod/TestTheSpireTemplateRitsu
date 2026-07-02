# Status 牌

`${STS_SOURCE_PATH}` 表示当前项目的 STS2 source root；TestTheSpireTemplateRitsu 默认使用 `local/sts2-decompiled/`。

## 基线行为

Status 牌属于独立的 `StatusCardPool`，不是普通无色池。池定义见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardPools/StatusCardPool.cs:13`，收录列表见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardPools/StatusCardPool.cs:25`。

不要假设所有 Status 都自动 `Unplayable`。现有样板里：

- `Wound`: `Unplayable`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Wound.cs:22`
- `Burn`: `Unplayable`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Burn.cs:40`
- `Void`: `Unplayable + Ethereal`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Void.cs:35`
- `Toxic`: 只有 `Exhaust`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Toxic.cs:28`

## 常见形状

- 纯死牌状态：`-1` 费用、`TargetType.None`、`MaxUpgradeLevel => 0`、`Unplayable`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Wound.cs:15`
- 手牌回合结束惩罚：覆写 `HasTurnEndInHandEffect` 和 `OnTurnEndInHand(...)`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Burn.cs:48` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Toxic.cs:44`
- 抽到即触发：覆写 `AfterCardDrawn(...)`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Void.cs:47`

## 手牌回合结束触发

基类默认 `HasTurnEndInHandEffect => false`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:1196`。只有显式返回 `true`，状态卡才会在回合结束留手时跑逻辑。

- Burn：带伤害动态变量、VFX、音效，再对持有者造成伤害，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Burn.cs:32` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Burn.cs:52`
- Toxic：结构更轻，只做伤害，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Toxic.cs:36` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Toxic.cs:46`

## 抽牌触发

`Void` 展示了“抽到自身时触发”的标准写法：先判断 `card == this`，再执行惩罚，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Void.cs:47`。如果你的状态是在抽牌瞬间处罚玩家，优先复用这个模式。

## 能量与可打出性

状态卡可以有能量相关动态变量，但不等于它能像普通牌一样打出。`CardModel.CanPlay()` 会同时检查关键词和资源，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:1204`。`Void` 就有 `EnergyVar(1)`，但它的用途是“抽到后失去 1 点能量”，不是打出成本，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Void.cs:27`。

## 易错点

- 新状态卡不进 `StatusCardPool` 就不会进入标准状态来源。
- 不要把“状态牌”简化成固定模板；先判断它是死牌、留手处罚、抽到触发，还是可打出的特殊状态。
