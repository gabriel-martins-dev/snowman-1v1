using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Spawning;


public class EnviromentManager : MonoBehaviour
{
    private static EnviromentManager singleton;
    [SerializeField] Transform[] spawns;

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
}
