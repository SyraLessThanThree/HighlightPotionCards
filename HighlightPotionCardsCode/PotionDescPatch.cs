using System.Collections.Generic;
using System.Text.RegularExpressions;
using HarmonyLib;
using HighlightPotionCards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MainFile = HighlightPotionCards.HighlightPotionCards.MainFile;

namespace HighlightPotionHands.HighlightPotionHandsCode;


[HarmonyPatch(typeof(LocString),nameof(LocString.GetFormattedText))]
public static class PotionDescPatch {
    static void Postfix(ref string __result, LocString __instance) {
        if (__instance.LocTable.Equals("potions") && __instance.LocEntryKey.EndsWith(".description")) {
            MainFile.Logger.Info("Starting Potion Highlighting");
            Dictionary<string,string> toHighlight = [];
            string unformattedRegex = "(?<![\\]_]){0}(?![\\[_])";
            
            toHighlight.AddHighlight("gameplay_ui", "DISCARD_PILE");
            toHighlight.AddHighlight("gameplay_ui", "DRAW_PILE");
            toHighlight.AddHighlight("gameplay_ui", "EXHAUST_PILE","[purple]{0}[/purple]");
            
            toHighlight.AddHighlight("extensions", "EXTENSION.card.humanizedCardTypes.attack","[red]{0}[/red]");
            toHighlight.AddHighlight("extensions", "EXTENSION.card.humanizedCardTypes.curse","[purple]{0}[/purple]");
            toHighlight.AddHighlight("extensions", "EXTENSION.card.humanizedCardTypes.power","[blue]{0}[/blue]");
            toHighlight.AddHighlight("extensions", "EXTENSION.card.humanizedCardTypes.quest","[green]{0}[/green]");
            toHighlight.AddHighlight("extensions", "EXTENSION.card.humanizedCardTypes.skill","[green]{0}[/green]");
            toHighlight.AddHighlight("extensions", "EXTENSION.card.humanizedCardTypes.status","[gray]{0}[/gray]");
            
            toHighlight.AddHighlight("gameplay_ui", "CARD_TYPE.ATTACK","[red]{0}[/red]");
            toHighlight.AddHighlight("gameplay_ui", "CARD_TYPE.CURSE","[purple]{0}[/purple]");
            toHighlight.AddHighlight("gameplay_ui", "CARD_TYPE.POWER","[blue]{0}[/blue]");
            toHighlight.AddHighlight("gameplay_ui", "CARD_TYPE.QUEST","[green]{0}[/green]");
            toHighlight.AddHighlight("gameplay_ui", "CARD_TYPE.SKILL","[green]{0}[/green]");
            toHighlight.AddHighlight("gameplay_ui", "CARD_TYPE.STATUS","[gray]{0}[/gray]");

            //TODO: i cant find the "colorless" or "cards" word to get "colorless" from "POOL_COLORLESS_TIP
            
            string? colorless = TryGetLocString("card_library", "POOL_COLORLESS_TIP");
            if (colorless != null) {
                colorless = Regex.Replace(colorless, "\\.$", "");
                toHighlight.TryAdd(colorless,"[gray]{0}[/gray]");
                
                List<string> format1SupportedLangs = ["eng","deu"];
                if(format1SupportedLangs.Any((l)=>LocManager.Instance.Language.Equals(l)))
                    colorless = Regex.Replace(colorless, " [A-Za-z0-9]*$", "");
                
                List<string> format2SupportedLangs = ["fra","ita","ptb","spa"];
                if(format2SupportedLangs.Any((l)=>LocManager.Instance.Language.Equals(l)))
                    colorless = Regex.Replace(colorless, "^[A-Za-z0-9]* ", "");
                
                toHighlight.TryAdd(colorless,"[gray]{0}[/gray]");
            }
            
            foreach (var element in toHighlight) {
                string regex = string.Format(unformattedRegex, element.Key);
                var highlights = element.Value;
                string newStr = string.Format(highlights,element.Key);
                __result = Regex.Replace(__result, regex, newStr, RegexOptions.IgnoreCase);
            }
        }
    }

    static void AddHighlight(this Dictionary<string,string> toHighlight, string table, string key, string? highlightFormat = null,bool forceInsert = false) {

        string? entry = TryGetLocString(table, key);
        if(entry == null) return;
        
        highlightFormat ??= "[gold]{0}[/gold]";
        if(!toHighlight.TryAdd(entry, highlightFormat) && forceInsert) {
            toHighlight.Remove(entry);
            toHighlight.Add(entry, highlightFormat);
        }
    }

    static string? TryGetLocString(string table, string key) {
        string? entry = null;
        try {
            entry = new LocString(table, key).GetFormattedText();
        }
        catch (LocException e) {
            MainFile.Logger.Warn(e.Message);
        }
        return entry;
    }
}