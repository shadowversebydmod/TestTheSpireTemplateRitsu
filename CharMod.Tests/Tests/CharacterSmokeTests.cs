using CharMod.CharModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using TestTheSpire;
using Xunit;
using TemplateCharacter = global::CharMod.CharModCode.Character.CharMod;

namespace CharMod.Tests.Cases;

public sealed class CharacterSmokeTests : CombatTestSuite
{
    protected override void ConfigureBattle(CombatTestBattleBuilder battle)
    {
        battle
            .Player<TemplateCharacter>()
            .AddEnemy<BigDummy>()
            .WithSeed("charmod-template-smoke");
    }

    [Fact]
    public async Task Template_character_loads_and_can_play_strike()
    {
        Assert.IsType<TemplateCharacter>(Player.Character);
        Assert.Equal(70, Player.Creature.MaxHp);

        var enemy = EnemyAt(0);
        var hpBefore = enemy.CurrentHp;
        var strike = await AddToHand<StrikeIronclad>();

        await PlayerCmd.SetEnergy(10, Player);
        await WaitForIdle();
        await Play(strike, enemy);

        Assert.Equal(hpBefore - 6, enemy.CurrentHp);
    }

    [Fact]
    public async Task Sample_enhance_strike_applies_extra_hit_when_enhanced()
    {
        var enemy = EnemyAt(0);
        var hpBefore = enemy.CurrentHp;
        var strike = await AddToHand<SampleEnhanceStrike>();

        await PlayerCmd.SetEnergy(3, Player);
        await WaitForIdle();
        await Play(strike, enemy);

        Assert.Equal(hpBefore - 12, enemy.CurrentHp);
    }
}
