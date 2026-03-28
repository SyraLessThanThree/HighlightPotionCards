using System.Reflection;
using Godot;
using HarmonyLib;
using HighlightPotionCards.HighlightPotionCards;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.RichTextTags;

namespace HighlightPotionCards.HighlightPotionCardsCode.Core.RichTextTags;

[GlobalClass]
[Tool]
public partial class RichTexGray : AbstractMegaRichTextEffect
{
    public new string bbcode = "gray";

    protected override string Bbcode => bbcode;

    public override bool _ProcessCustomFX(CharFXTransform charFx)
    {
        charFx.Color = StsColors.gray;
        return true;
    }
}

[HarmonyPatch(typeof(MegaRichTextLabel), "InstallEffectsIfNeeded")]
public static class GrayEffectPatch1 {
    static void Postfix(MegaRichTextLabel __instance) {
        __instance.CustomEffects.Add(new RichTexGray());
    }
}

[HarmonyPatch(typeof(NMainMenu), nameof(NMainMenu._Ready))]
public static class GrayEffectPatch2 {
    static void Postfix() {
        return;
        MainFile.Logger.Info($"NMainMenu._Ready Postfix");
        var engTables = Traverse.Create(LocManager.Instance).Field<Dictionary<string, LocTable>>("_tables").Value;
        MainFile.Logger.Info($"NMainMenu._Ready Postfix3");
        var strToCheck = "colorless";
        MainFile.Logger.Info($"Showing Eng LocStrings with \"{strToCheck}\"");
        foreach (var tablePair in engTables) {
            var table = tablePair.Value;
            foreach (var key in table.Keys) {
                var value = table.GetLocString(key);
                if (value.GetRawText().Contains(strToCheck, StringComparison.InvariantCultureIgnoreCase)) {
                    MainFile.Logger.Info($"{key}:{value.GetRawText()}");
                }
            }
        }
        MainFile.Logger.Info("Adding RichTexGray");
        var texteffectsArrField =
            Traverse.Create<MegaRichTextLabel>().Field<AbstractMegaRichTextEffect[]>("_textEffects");
        var texteffectsArrField2 =
            Assembly.GetAssembly(typeof(MegaRichTextLabel)).GetType(nameof(MegaRichTextLabel)).GetField("_textEffects", BindingFlags.NonPublic | BindingFlags.Static);
        
        if(!texteffectsArrField.Value.Any((e)=> e is RichTexGray)){
            var newVal = texteffectsArrField.Value.AddItem(new RichTexGray()).ToArray();
            texteffectsArrField.Value = newVal;
            texteffectsArrField2.SetValue(null,newVal);
        }

    }
}