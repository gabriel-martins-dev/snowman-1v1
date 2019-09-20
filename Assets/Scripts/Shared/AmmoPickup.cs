using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using DG.Tweening;

public class AmmoPickup : NetworkedBehaviour
{
    public int Amount { get; private set; } = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventManager.Trigger(new AmmoTriggerEvent(collision, this));
    }
}
