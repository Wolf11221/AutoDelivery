using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace AutoDelivery.Patches;

[HarmonyPatch(typeof(ItemDropship))]
public class ItemDropshipPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void OnStart()
    {
        Main.ItemDropship = GameObject.FindObjectOfType<ItemDropship>();
    }

    [HarmonyPatch("OpenShipDoorsOnServer")]
    [HarmonyPrefix]
    public static bool OpenShipDoorsOnServer(ItemDropship __instance)
    {
        if (__instance.shipLanded && !__instance.shipDoorsOpened)
        {
            FieldInfo fieldInfo = typeof(ItemDropship).GetField("itemsToDeliver", BindingFlags.NonPublic | BindingFlags.Instance);
            List<int> itemsToDeliver = (List<int>)fieldInfo.GetValue(__instance);

            for (int i = 0; i < itemsToDeliver.Count; i++)
            {
                GameObject obj = GameObject.Instantiate(Main.Terminal.buyableItemsList[itemsToDeliver[i]].spawnPrefab, Main.GetItemDropLocation(), Quaternion.identity, StartOfRound.Instance.elevatorTransform);
                obj.GetComponent<GrabbableObject>().fallTime = 1f;
                obj.GetComponent<GrabbableObject>().hasHitGround = true;
                obj.GetComponent<GrabbableObject>().isInShipRoom = true;
                obj.GetComponent<GrabbableObject>().isInElevator = true;
                obj.GetComponent<NetworkObject>().Spawn();
            }

            itemsToDeliver.Clear();
            __instance.OpenShipClientRpc();
        }

        return true;
    }
}