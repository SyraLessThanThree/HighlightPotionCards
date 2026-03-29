using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace HighlightPotionCards.HighlightPotionCards;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node {
    public const string
        ModId = "HighlightPotionCards"; //At the moment, this is used only for the Logger and harmony names.
    
    public static bool Experimental = true;

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize() {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
    }
}