# Attack 牌

`${STS_SOURCE_PATH}` 表示当前项目的 STS2 source root；TestTheSpireTemplateRitsu 默认使用 `local/sts2-decompiled/`。

## 最小骨架

Attack 牌的构造器只需要确定四件事：能量、`CardType.Attack`、稀有度、目标类型。单体、群体、随机攻击的基本骨架分别可以从 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Bash.cs:24`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Whirlwind.cs:30`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Volley.cs:21` 开始抄。

```csharp
public sealed class Foo : CardModel
{
  public Foo()
    : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
  {
  }

  protected override IEnumerable<DynamicVar> CanonicalVars
  {
    get
    {
      return new[] { (DynamicVar)new DamageVar(6M, ValueProp.Move) };
    }
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
    await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
      .FromCard(this)
      .Targeting(cardPlay.Target)
      .Execute(choiceContext);
  }

  protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3M);
}
```

## 常见模式

- 普通单体攻击：`DamageVar` + `cardPlay.Target` 判空 + `.Targeting(cardPlay.Target)`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Bash.cs:37` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Bash.cs:49`。
- 群体攻击：`TargetType.AllEnemies` + `.TargetingAllOpponents(card.CombatState)`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/AstralPulse.cs:21` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/AstralPulse.cs:36`。
- 随机多段：`TargetType.RandomEnemy` + `.WithHitCount(...)` + `.TargetingRandomOpponents(card.CombatState)`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Volley.cs:22` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Volley.cs:36`。
- 计算型伤害：用 `CalculationBaseVar`、`ExtraDamageVar`、`CalculatedDamageVar` 组合，再把 `DynamicVars.CalculatedDamage` 交给 `DamageCmd.Attack(...)`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/BodySlam.cs:29` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/BodySlam.cs:50`。
- 额外说明：需要卡面额外解释时再加 `ExtraHoverTips`，例如 Bash 的 Vulnerable 提示和 Body Slam 的 Block 提示，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Bash.cs:29` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/BodySlam.cs:42`。

## X 费用与星能

- 能量 X：覆写 `HasEnergyCostX => true`，在结算时调用 `ResolveEnergyXValue()`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:369`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:391`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Whirlwind.cs:35`。
- 固定星能：覆写 `CanonicalStarCost`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:418` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/AstralPulse.cs:26`。
- 星能 X：覆写 `HasStarCostX => true`，在结算时调用 `ResolveStarXValue()`，看 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:468`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:480`、`${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Stardust.cs:26`。

## 升级套路

- 纯数值牌：`DynamicVars.Damage.UpgradeValueBy(...)`，如 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Whirlwind.cs:64`。
- 混合效果牌：一次升多个变量，如 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/Bash.cs:57`。
- 成本升级：直接改 `EnergyCost.UpgradeBy(-1)`，如 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/Cards/BodySlam.cs:57`。

## 易错点

- `AnyEnemy` 牌不要忘记判空 `cardPlay.Target`；`AllEnemies` 和 `RandomEnemy` 不要误用单体 target。
- `ResolveEnergyXValue()` / `ResolveStarXValue()` 只能在对应 X 牌里调用，否则会抛异常，见 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:391` 和 `${STS_SOURCE_PATH}/MegaCrit/Sts2/Core/Models/CardModel.cs:480`。
- 计算型攻击应升级输入变量，不要只改最终显示值，否则卡面和实际数值容易漂移。
