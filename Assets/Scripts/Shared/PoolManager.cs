using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;
using MLAPI.Messaging;

[Serializable]
public class Pool
{
    [SerializeField] public NetworkedObject pickupPrefab;

    List<NetworkedObject> active;
    List<NetworkedObject> inactive;
}


public class PoolManager : NetworkedBehaviour
{
    [SerializeField] List<Pool> pools;
    [SerializeField] BulletComponent bulletPrefab;
    [SerializeField] AmmoPickup pickupPrefab;

    public List<NetworkedBehaviour> activePool;
    public List<NetworkedBehaviour> inactivePool;

    private static PoolManager singleton;

    public static PoolManager Singleton
    {
        get
        {
            if (!singleton)
            {
                singleton = FindObjectOfType(typeof(PoolManager)) as PoolManager;

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
        activePool = new List<NetworkedBehaviour>();
        inactivePool = new List<NetworkedBehaviour>();
    }

    private T Get<T>() where T : NetworkedBehaviour
    {
        var item = inactivePool.Find(obj => obj.GetType() == typeof(T));

        if (item != null)
        {
            inactivePool.Remove(item);

            if (!activePool.Contains(item))  activePool.Add(item);

            return item.GetComponent<T>();
        }

        return null;
    }

    public T Push<T>(T obj) where T : NetworkedBehaviour
    {
        obj.NetworkedObject.Spawn();

        if (!activePool.Contains(obj))  activePool.Add(obj);

        return obj;
    }

    public void Pull<T>(T obj, bool remove = true) where T : NetworkedBehaviour
    {
        obj.transform.position = EnviromentManager.Singleton.GetPoolInactivePosition();
        if(remove) activePool.Remove(obj);

        if(!inactivePool.Contains(obj)) inactivePool.Add(obj);
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
            pickup = Singleton.Push(Instantiate(Singleton.pickupPrefab, position, Quaternion.identity));
        }
        else
        {
            pickup.transform.position = position;
        }
    }

    public void PullAll()
    {
        activePool.ForEach(obj => Singleton.Pull(obj, false));
        activePool.Clear();
    }

    [ServerRPC(RequireOwnership = false)]
    public void TriggerSpawnBullet(PlayerWeaponComponent weapon, Vector3 position, Vector3 rotation)
    {
        if(weapon.HasAmmo())
        {
            weapon.SpentAmmo();

            var bullet = Singleton.Get<BulletComponent>();

            if (bullet == null)
            {
                Singleton.Push(Instantiate(Singleton.bulletPrefab, position, Quaternion.Euler(rotation)));
            }
            else
            {
                bullet.transform.position = position;
                bullet.transform.rotation = Quaternion.Euler(rotation);
            }
        }
    }
}
