using UnityEngine;
using System.Collections;
using MLAPI;
using MLAPI.Spawning;


public static class Extensions
{
    public static PlayerConnector GetPlayerConnector(this Component component)
    {
        var netObject = component.GetComponent<NetworkedObject>();

        if (netObject != null)
        {
            var player = SpawnManager.GetPlayerObject(netObject.OwnerClientId);

            if (player != null) return player.GetComponent<PlayerConnector>();
        }

        return null;
    }
}
