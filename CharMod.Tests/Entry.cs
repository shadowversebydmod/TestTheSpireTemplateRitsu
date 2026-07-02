using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using TestTheSpire;

namespace CharMod.Tests;

[ModInitializer("Init")]
public static class Entry
{
    public static void Init()
    {
        CombatTestBootstrap.Initialize(Assembly.GetExecutingAssembly(), new CombatTestOptions
        {
            LogPrefix = "CharMod.Tests"
        });

        Log.Info("[CharMod.Tests] Mod initialized");
    }
}
