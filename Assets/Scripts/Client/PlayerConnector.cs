using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerConnector : NetworkedBehaviour
{
    [SerializeField] PlayerInputComponent input;
    [SerializeField] PlayerWeaponComponent weapon;
    [SerializeField] PlayerHealthComponent health;

    public void SetLocked(bool value)
    {
        input.SetLock(value);
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
}
