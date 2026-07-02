using CharMod.CharModCode.CardKeywords;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MinionLib.Component;
using MinionLib.Component.Core;
using MinionLib.Component.Interfaces;

namespace CharMod.CharModCode.Components.Enhances;

public sealed partial class Enhance : CardComponent
{
    public static Color EnhancedColor = Colors.Gold;

    [ComponentState] public int Cost { get; private set; }
    [LocArg] public bool IsActive { get; private set; }
    [ComponentState] [NotLocArg] private string TextName { get; set; } = "";

    private LocString CapturedDescription => new("cards", ResolveContentLocKey());

    protected override LocString PrefixLocString => new("cards", "CharMod.Enhance.prefix");

    protected override string FormatPrefix(LocString loc)
    {
        var captured = CapturedDescription;
        var prefix = PrefixLocString;

        captured.AddVariablesFrom(loc);
        prefix.AddVariablesFrom(loc);
        captured.Add("Cost", Cost);
        captured.Add("IsActive", IsActive);
        prefix.Add("IsActive", IsActive);

        if (Card != null)
            captured.Add("energyPrefix", EnergyIconHelper.GetPrefix(Card));

        Card?.DynamicVars?.AddTo(captured);
        prefix.Add("CapturedDescription", captured);

        return prefix.GetFormattedText();
    }

    public override Color? GlowColor => IsActive ? EnhancedColor : null;

    public override IEnumerable<IHoverTip> HoverTips =>
    [
        HoverTipFactory.FromKeyword(CharModKeywords.Enhance)
    ];

    public Enhance(int cost, string textName)
    {
        if (cost < 0)
            throw new ArgumentOutOfRangeException(nameof(cost), "Enhance cost cannot be negative.");
        if (string.IsNullOrWhiteSpace(textName))
            throw new ArgumentException("Enhance text name cannot be empty.", nameof(textName));

        Cost = cost;
        TextName = textName;
    }

    private string ResolveContentLocKey()
    {
        if (TextName.Contains('.') || TextName.StartsWith("CHARMOD-", StringComparison.Ordinal))
            return TextName;

        return Card == null ? TextName : $"{StringHelper.SnakeCase(Card.GetType().Name)}.{TextName}";
    }

    private void Refresh()
    {
        if (Card?.Owner.PlayerCombatState == null)
            return;
        if (ShouldKeepCurrentEnhanceState())
            return;

        bool changed;
        switch (Card.Pile?.Type)
        {
            case PileType.Hand:
                var canEnhance = CanActivate();
                changed = IsActive != canEnhance;
                IsActive = canEnhance;
                break;
            case PileType.Play:
                return;
            default:
                changed = IsActive;
                IsActive = false;
                break;
        }

        if (changed)
            Card.InvokeEnergyCostChanged();
    }

    private bool ShouldKeepCurrentEnhanceState()
    {
        return RunManager.Instance.ActionExecutor.CurrentlyRunningAction is PlayCardAction playCardAction
               && ReferenceEquals(playCardAction.NetCombatCard.ToCardModelOrNull(), Card);
    }

    private bool CanActivate()
    {
        return Card != null && CanEnhance(Card, Cost);
    }

    public static bool CanEnhance(CardModel card, int enhanceCost)
    {
        if (enhanceCost < 0 || card.EnergyCost.CostsX)
            return false;

        using var _ = new EnhanceSuppressionScope();
        return CanEnhance(card, enhanceCost, card.EnergyCost.GetResolved());
    }

    private static bool CanEnhance(CardModel card, int enhanceCost, decimal currentCost)
    {
        var ownerState = card.Owner.PlayerCombatState;
        if (ownerState == null)
            return false;

        return currentCost <= ownerState.Energy
               && currentCost < enhanceCost
               && enhanceCost <= ownerState.Energy;
    }

    internal static int? ResolveActiveCost(CardModel card)
    {
        if (card is not IComponentsCardModel componentsCard)
            return null;

        using var _ = new EnhanceSuppressionScope();
        var currentCost = card.EnergyCost.GetResolved();

        int? activeCost = null;
        foreach (var enhance in componentsCard.Components.OfType<Enhance>())
            if (CanEnhance(card, enhance.Cost, currentCost))
                activeCost = Math.Max(activeCost ?? enhance.Cost, enhance.Cost);

        return activeCost;
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        if (EnhanceSuppressionScope.IsSuppressed)
        {
            modifiedCost = originalCost;
            return false;
        }

        Refresh();

        if (card == Card && ResolveActiveCost(card) is { } activeCost)
        {
            modifiedCost = activeCost;
            return true;
        }

        modifiedCost = originalCost;
        return false;
    }

    public override Task AfterCardPlayedLatePostfix(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        ComponentContext componentContext)
    {
        Refresh();
        return Task.CompletedTask;
    }

    public override Task AfterEnergyResetLatePostfix(Player player, ComponentContext componentContext)
    {
        Refresh();
        return Task.CompletedTask;
    }

    public override Task AfterEnergySpentPostfix(CardModel card, int amount, ComponentContext componentContext)
    {
        if (card != Card)
            Refresh();

        return Task.CompletedTask;
    }

    public override Task AfterModifyingEnergyGainPostfix(ComponentContext componentContext)
    {
        Refresh();
        return Task.CompletedTask;
    }
}
