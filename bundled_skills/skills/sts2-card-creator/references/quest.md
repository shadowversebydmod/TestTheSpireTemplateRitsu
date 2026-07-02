# Quest 牌

`${STS_SOURCE_PATH}` 表示当前项目的 STS2 source root；TestTheSpireTemplateRitsu 默认使用 `local/sts2-decompiled/`。

## 最小骨架

Quest 牌不是常规战斗牌。现有样板几乎都从 `: base(-1, CardType.Quest, CardRarity.Quest, ...)` 开始，并且默认 `MaxUpgradeLevel => 0` + `CardKeyword.Unplayable`，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/ByrdonisEgg.cs:17`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/LanternKey.cs:19`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/SpoilsMap.cs:25`。

```csharp
public sealed class MyQuest : CardModel
{
  public MyQuest()
    : base(-1, CardType.Quest, CardRarity.Quest, TargetType.Self)
  {
  }

  public override int MaxUpgradeLevel => 0;

  public override IEnumerable<CardKeyword> CanonicalKeywords =>
    new[] { CardKeyword.Unplayable };
}
```

## 什么时候加状态字段

任务进度、目标 act、坐标等需要持久化的状态，放在 `[SavedProperty]` 字段/属性里，并在 setter 里调用 `AssertMutable()`，模板看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/SpoilsMap.cs:48`。只在创建时初始化一次的任务状态，可以放进 `AfterCreated()`，基类入口见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:850`，示例见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/SpoilsMap.cs:61`。

## 常见 Quest Hook

- 营火 / 休息点选项：`TryModifyRestSiteOptions(...)`，模板看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/ByrdonisEgg.cs:32`
- 事件 / 地图未知点改写：`ModifyUnknownMapPointRoomTypes(...)`、`ModifyNextEvent(...)`，模板看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/LanternKey.cs:34`
- 地图生成与挂点：`ModifyGeneratedMap(...)`、`ModifyGeneratedMapLate(...)`、`AfterMapGenerated(...)`，模板看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/SpoilsMap.cs:63`
- 移除前清理：`BeforeCardRemoved(...)`，模板看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/SpoilsMap.cs:102`

如果你的 Quest 需要影响地图、事件或营地，就从最接近的例子开始裁剪，不要凭空组合多个 hook。

## 完成流程

Quest 完成时，先发奖励，再标记完成，最后从牌组移除。`SpoilsMap.OnQuestComplete()` 的顺序很适合作为默认模板：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/SpoilsMap.cs:110`。

## 卡池

Quest 牌池定义在 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardPools/QuestCardPool.cs:13`，当前只收录 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/ByrdonisEgg.cs:15`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/LanternKey.cs:15`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/SpoilsMap.cs:21` 三张任务牌。新 Quest 要进系统标准列表，就必须加入这个池。

## 易错点

- Quest 牌默认不是可打出的战斗牌；先想清楚它是“挂在牌组中的状态型任务”，还是“需要真正打出的特殊卡”。
- 需要跨 act 或跨地图持久化时，不要偷懒放临时字段；用 `[SavedProperty]`。
- 地图型任务在移除时要同步清理挂点，否则容易留下脏状态。
