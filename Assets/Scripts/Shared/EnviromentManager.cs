using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Spawning;


public class EnviromentManager : MonoBehaviour
{
    private static EnviromentManager singleton;
    [SerializeField] Transform[] spawns;
    [SerializeField] Transform pickupAreaLeftUpperCorner;
    [SerializeField] Transform pickupAreaRightLowerCorner;
    [SerializeField] Transform poolInactivePosition;

    public static EnviromentManager Singleton
    {
        get
        {
            if (singleton != null) return singleton;
            else
            {
                singleton = FindObjectOfType<EnviromentManager>();

                return singleton;
            }
        }
    }

    public Vector3 GetSpawnPosition(int index)
    {
        return spawns[index].position;
    }

    public ulong? GetPlayerHash(int index)
    {
        string name = index == 0 ? "BluePlayer" : "RedPlayer";
        ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator(name);

        return prefabHash;
    }

    public Vector3 GetPickupSpawnPosition()
    {
        return new Vector3(
            Random.Range(pickupAreaLeftUpperCorner.position.x, pickupAreaRightLowerCorner.position.x),
            Random.Range(pickupAreaRightLowerCorner.position.y, pickupAreaLeftUpperCorner.position.y),
            0);
    }

    public Vector3 GetPickupSpawnCenterPosition()
    {
        return (pickupAreaLeftUpperCorner.position + pickupAreaRightLowerCorner.position) / 2;
    }

    public Vector3 GetPoolInactivePosition()
    {
        return poolInactivePosition.position;
    }
}
