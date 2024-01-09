using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace AutoDelivery;

[BepInPlugin(GUID, MODNAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string
        MODNAME = "AutoDelivery",
        AUTHOR = "Nie",
        GUID = AUTHOR + "_" + MODNAME,
        VERSION = "1.0.0";

    public Plugin()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
    }

    public void Start()
    {
        GameObject load = new GameObject("AutoDelivery");
        load.AddComponent<Main>();
        DontDestroyOnLoad(load);
    }
}