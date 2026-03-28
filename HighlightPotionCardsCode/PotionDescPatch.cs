using System.Collections.Generic;
using System.Text.RegularExpressions;
using HarmonyLib;
using HighlightPotionCards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace HighlightPotionHands.HighlightPotionHandsCode;


[HarmonyPatch(typeof(LocString),nameof(LocString.GetFormattedText))]
public static class PotionDescPatch {
    static void Postfix(ref string __result, LocString __instance) {
        if (__instance.LocTable.Equals("potions") && __instance.LocEntryKey.EndsWith(".description")) {
            MainFile.Logger.Info("Starting Potion Highlighting");
            Dictionary<string,string> toHighlight = [];
            string unformattedRegex = "(?<!\\]){0}(?!\\[)";
            
            toHighlight.AddHighlight(new LocString("gameplay_ui", "DISCARD_PILE").GetFormattedText());
            toHighlight.AddHighlight(new LocString("gameplay_ui", "DRAW_PILE").GetFormattedText());
            toHighlight.AddHighlight(new LocString("gameplay_ui", "EXHAUST_PILE").GetFormattedText(),"[purple]{0}[/purple]");
            
            toHighlight.AddHighlight(new LocString("extensions", "EXTENSION.card.humanizedCardTypes.attack").GetFormattedText(),"[red]{0}[/red]");
            toHighlight.AddHighlight(new LocString("extensions", "EXTENSION.card.humanizedCardTypes.curse").GetFormattedText(),"[purple]{0}[/purple]");
            toHighlight.AddHighlight(new LocString("extensions", "EXTENSION.card.humanizedCardTypes.power").GetFormattedText(),"[blue]{0}[/blue]");
            toHighlight.AddHighlight(new LocString("extensions", "EXTENSION.card.humanizedCardTypes.quest").GetFormattedText(),"[green]{0}[/green]");
            toHighlight.AddHighlight(new LocString("extensions", "EXTENSION.card.humanizedCardTypes.skill").GetFormattedText(),"[green]{0}[/green]");
            toHighlight.AddHighlight(new LocString("extensions", "EXTENSION.card.humanizedCardTypes.status").GetFormattedText(),"[gray]{0}[/gray]");
            
            toHighlight.AddHighlight(new LocString("gameplay_ui", "CARD_TYPE.ATTACK").GetFormattedText(),"[red]{0}[/red]");
            toHighlight.AddHighlight(new LocString("gameplay_ui", "CARD_TYPE.CURSE").GetFormattedText(),"[purple]{0}[/purple]");
            toHighlight.AddHighlight(new LocString("gameplay_ui", "CARD_TYPE.POWER").GetFormattedText(),"[blue]{0}[/blue]");
            toHighlight.AddHighlight(new LocString("gameplay_ui", "CARD_TYPE.QUEST").GetFormattedText(),"[green]{0}[/green]");
            toHighlight.AddHighlight(new LocString("gameplay_ui", "CARD_TYPE.SKILL").GetFormattedText(),"[green]{0}[/green]");
            toHighlight.AddHighlight(new LocString("gameplay_ui", "CARD_TYPE.STATUS").GetFormattedText(),"[gray]{0}[/gray]");

            /* TODO: i cant find the "colorless" or "cards" word to get "colorless" from "POOL_COLORLESS_TIP
            */
             
            string colorless = new LocString("card_library", "POOL_COLORLESS_TIP").GetFormattedText();
            colorless = Regex.Replace(colorless, "\\.$", "");
            //colorless = Regex.Replace(colorless, " [A-Za-z0-9]*$", "");
            toHighlight.AddHighlight(colorless,"[gray]{0}[/gray]");
            
            return;
            foreach (var element in toHighlight) {
                string regex = string.Format(unformattedRegex, element.Key);
                var highlights = element.Value;
                string newStr = string.Format(highlights,element.Key);
                __result = Regex.Replace(__result, regex, newStr, RegexOptions.IgnoreCase);
            }
        }
    }

    static void AddHighlight(this Dictionary<string,string> toHighlight, string entry, string? highlightFormat = null,bool forceInsert = false) {
        highlightFormat ??= "[gold]{0}[/gold]";
        if(!toHighlight.TryAdd(entry, highlightFormat) && forceInsert) {
            toHighlight.Remove(entry);
            toHighlight.Add(entry, highlightFormat);
        }
    }
}