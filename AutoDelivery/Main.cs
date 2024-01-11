using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AutoDelivery;

public class Main : MonoBehaviour
{
    public static Terminal Terminal;
    public static ItemDropship ItemDropship;

    private void Update()
    {
        if(StartOfRound.Instance == null) return;
        if (StartOfRound.Instance.inShipPhase || Terminal.orderedItemsFromTerminal.Count <= 0) return;
        
        FieldInfo fieldInfo = typeof(ItemDropship).GetField("itemsToDeliver", BindingFlags.NonPublic | BindingFlags.Instance);
        List<int> itemsToDeliver = (List<int>)fieldInfo.GetValue(ItemDropship);
            
        // modified LandShipOnServer
        ItemDropship.deliveringOrder = true;
        ItemDropship.shipLanded = true;
        ItemDropship.shipTimer = 0f;
        itemsToDeliver.Clear();
        itemsToDeliver.AddRange(Terminal.orderedItemsFromTerminal);
        Terminal.orderedItemsFromTerminal.Clear();
        ItemDropship.playersFirstOrder = false;
            
        ItemDropship.LandShipClientRpc();
            
        ItemDropship.TryOpeningShip();
            
        ItemDropship.ShipLeaveClientRpc();
    }

    public static Vector3 GetItemDropLocation()
    {
        Vector3 pos = GameObject.Find("/Environment/HangarShip").transform.position;
        pos.x += 3.90f;
        pos.y += 1f;
        pos.z += -8.67f;
        return pos;
    }
}