using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace AutoDelivery;

public class Main : MonoBehaviour
{
    private static Terminal _terminal;
    private static ItemDropship _itemDropship;

    private void Update()
    {
        if(StartOfRound.Instance == null) return;
        
        if (_terminal == null) { _terminal = FindObjectOfType<Terminal>(); }
        if (_itemDropship == null) { _itemDropship = FindObjectOfType<ItemDropship>(); }

        if (!StartOfRound.Instance.inShipPhase && _terminal.orderedItemsFromTerminal.Count > 0)
        {
            FieldInfo fieldInfo = typeof(ItemDropship).GetField("itemsToDeliver", BindingFlags.NonPublic | BindingFlags.Instance);
            List<int> itemsToDeliver = (List<int>)fieldInfo.GetValue(_itemDropship);
            
            _itemDropship.deliveringOrder = true;
            _itemDropship.shipLanded = true;
            _itemDropship.shipTimer = 0f;
            itemsToDeliver.Clear();
            itemsToDeliver.AddRange(_terminal.orderedItemsFromTerminal);
            _terminal.orderedItemsFromTerminal.Clear();
            _itemDropship.playersFirstOrder = false;
            
            _itemDropship.LandShipClientRpc();
            
            _itemDropship.TryOpeningShip();
            
            _itemDropship.ShipLeave();
        }
    }

    private static Vector3 GetItemDropLocation()
    {
        Vector3 pos = GameObject.Find("/Environment/HangarShip").transform.position;
        pos.x += 3.90f;
        pos.y += 1f;
        pos.z += -8.67f;
        return pos;
    }
    
    [HarmonyPatch(typeof(ItemDropship), "OpenShipDoorsOnServer")]
    public class StartOfRoundPatch
    {
        private static bool Prefix(ItemDropship __instance)
        {
            if (__instance.shipLanded && !__instance.shipDoorsOpened)
            {
                FieldInfo fieldInfo = typeof(ItemDropship).GetField("itemsToDeliver", BindingFlags.NonPublic | BindingFlags.Instance);
                List<int> itemsToDeliver = (List<int>)fieldInfo.GetValue(__instance);
                
                int num = 0;
                for (int i = 0; i < itemsToDeliver.Count; i++)
                {
                    GameObject obj = Instantiate(_terminal.buyableItemsList[itemsToDeliver[i]].spawnPrefab, GetItemDropLocation(), Quaternion.identity, StartOfRound.Instance.elevatorTransform);
                    obj.GetComponent<GrabbableObject>().fallTime = 0f;
                    obj.GetComponent<NetworkObject>().Spawn();
                    num = ((num < 3) ? (num + 1) : 0);
                }
                itemsToDeliver.Clear();
                __instance.OpenShipClientRpc();
            }
            
            return true;
        }
    }
}