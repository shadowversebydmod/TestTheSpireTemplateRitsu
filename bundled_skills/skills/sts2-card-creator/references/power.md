# Power 牌

`${STS_SOURCE_PATH}` 表示当前项目的 STS2 source root；TestTheSpireTemplateRitsu 默认使用 `local/sts2-decompiled/`。

## 最小骨架

Power 牌最常见的形状是自施加 buff：构造器写成 `CardType.Power` + `TargetType.Self`，`OnPlay` 先播施法动画，再调用 `PowerCmd.Apply<T>()`。最小例子见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Barricade.cs:20` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Aggression.cs:18`。

```csharp
public sealed class Foo : CardModel
{
  public Foo()
    : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
  {
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);
    await PowerCmd.Apply<FooPower>(this.Owner.Creature, 1M, this.Owner.Creature, this);
  }
}
```

## 什么时候需要 DynamicVars

只有卡面文本或逻辑真的需要可变数值时，才定义 `CanonicalVars`。基类会用它初始化 `DynamicVars`，并在描述生成时自动注入，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:516`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:528`。

- Power 数值直接对应某个 Power 的 amount：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Afterimage.cs:26`
- 文本上是“若干张牌”“若干层效果”等抽象计数：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/CallOfTheVoid.cs:26`
- 如果效果恒为 `1M` 且不会升级，不必强行写 `CanonicalVars`，例如 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Aggression.cs:23`

## Hover Tips

Power 牌的额外说明通常放在 `ExtraHoverTips`。基类 `HoverTips` 会自动合并 `ExtraHoverTips`、关键词和其他系统说明，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:745`。

- Block 相关 power：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Barricade.cs:25`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Afterimage.cs:34`
- 关键字提示型 power：`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/CallOfTheVoid.cs:34`

## 升级套路

- 加 `Innate`：非常常见，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Afterimage.cs:49`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/CallOfTheVoid.cs:49`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Aggression.cs:30`
- 降费用：见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Barricade.cs:40`
- 升 amount：如果 `CanonicalVars` 明确承载 Power 数值，就直接升级那个 var

## 易错点

- 不要把固定 `1M` 的 Power 也机械地包装成 `DynamicVar`，除非它需要出现在描述里或要参与升级。
- `PowerCmd.Apply<T>()` 的目标、来源、`cardSource` 通常都和持有者自己有关；先从同类样板抄，再改 amount。
