using HarmonyLib;
using UnityEngine;

namespace AutoDelivery.Patches;

[HarmonyPatch(typeof(Terminal))]
public class TerminalPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void OnStart()
    {
        Main.Terminal = GameObject.FindObjectOfType<Terminal>();
    }
}