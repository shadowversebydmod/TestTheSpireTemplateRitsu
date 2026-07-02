using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using CharModCharacter = CharMod.CharModCode.Character.CharMod;
using CharModCardPool = CharMod.CharModCode.Character.CharModCardPool;
using CharModPotionPool = CharMod.CharModCode.Character.CharModPotionPool;
using CharModRelicPool = CharMod.CharModCode.Character.CharModRelicPool;
using SampleEnhanceStrike = CharMod.CharModCode.Cards.SampleEnhanceStrike;

namespace CharMod.CharModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "CharMod"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());

        Harmony harmony = new(ModId);

        harmony.PatchAll();

        RitsuLibFramework.CreateContentPack(ModId)
            .SharedCardPool<CharModCardPool>()
            .SharedRelicPool<CharModRelicPool>()
            .SharedPotionPool<CharModPotionPool>()
            .Character<CharModCharacter>()
            .Card<CharModCardPool, SampleEnhanceStrike>()
            .CardKeywordOwnedByLocNamespace("ENHANCE")
            .Apply();
    }
}
