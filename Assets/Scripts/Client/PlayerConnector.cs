using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class PlayerConnector : NetworkedBehaviour
{
    [SerializeField] PlayerInputComponent input;
    [SerializeField] PlayerWeaponComponent weapon;
    [SerializeField] PlayerHealthComponent health;
    [SerializeField] PlayerMovementComponent movement;

    public void SetLocked(bool value)
    {
        input.SetLock(value);
        movement.SetLock(value);
    }

    public void AddAmmo(int amount)
    {
        weapon.AddAmmo(amount);
    }

    public void Damage(float damage)
    {
        health.Damage(damage);
    }

    public void Heal()
    {
        health.Heal();
    }

    public void ResetComponents()
    {
        health.Heal();
        weapon.ResetAmmo();
    }

    [ClientRPC]
    public void Teleport(Vector3 pos)
    {
        movement.transform.position = pos;
    }
}
