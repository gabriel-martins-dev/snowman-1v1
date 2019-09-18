using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
