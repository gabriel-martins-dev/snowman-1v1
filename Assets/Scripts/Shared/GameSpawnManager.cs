using System;
using System.Linq;
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

    public List<object> pool;

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
        pool = new List<object>();
    }

    private T Get<T>() where T : UnityEngine.Object
    {
        var item = pool.Find(obj => obj.GetType() == typeof(T));

        if (item != null) pool.Remove(item);

        return (T) item;
    }

    public static void SpawnBullet(PlayerWeaponComponent weapon,Vector3 position, Vector3 rotation)
    {
        Singleton.InvokeServerRpc(Singleton.TriggerSpawnBullet, weapon, position, rotation);
    }

    public static void SpawnPickup(Vector3 position)
    {
        var pickup = Singleton.Get<AmmoPickup>();

        if(pickup == null)
        {
            pickup = Instantiate(Singleton.pickupPrefab, position, Quaternion.identity);
            pickup.GetComponent<NetworkedObject>().Spawn();
        }
        else
        {
            pickup.transform.position = position;
            pickup.gameObject.SetActive(true);
        }
    }

    public void Despawn<T>(T obj) where T : MonoBehaviour
    {
        pool.Add(obj);
        obj.gameObject.SetActive(false);
    }

    public void DespawnAll<T>()
    {
 
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
