using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;
using MLAPI.Messaging;


public class GameSpawnManager : NetworkedBehaviour
{
    [SerializeField] BulletComponent bulletPrefab;
    [SerializeField] AmmoPickup pickupPrefab;

    private static GameSpawnManager singleton;

    public static GameSpawnManager Singleton
    {
        get
        {
            if (!singleton)
            {
                singleton = FindObjectOfType(typeof(GameSpawnManager)) as GameSpawnManager;

                if (!singleton)
                {
                    Debug.LogError("Missing EventManager");
                }
                else
                {
                    singleton.Initialize();
                }
            }

            return singleton;
        }
    }

    void Initialize()
    {

    }

    public static void SpawnBullet(PlayerWeaponComponent weapon,Vector3 position, Vector3 rotation)
    {
        Singleton.InvokeServerRpc(Singleton.TriggerSpawnBullet, weapon, position, rotation);
    }

    public static void SpawnPickup(Vector3 position)
    {
        Instantiate(Singleton.pickupPrefab, position, Quaternion.identity)
            .GetComponent<NetworkedObject>().Spawn();
    }

    [ServerRPC(RequireOwnership = false)]
    public void TriggerSpawnBullet(PlayerWeaponComponent weapon, Vector3 position, Vector3 rotation)
    {
        if(weapon.HasAmmo())
        {
            weapon.SpentAmmo();

            Instantiate(Singleton.bulletPrefab, position, Quaternion.Euler(rotation))
                .GetComponent<NetworkedObject>().Spawn();
        }
    }
}
