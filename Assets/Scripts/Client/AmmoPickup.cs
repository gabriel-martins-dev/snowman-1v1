using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using DG.Tweening;

public class AmmoPickup : NetworkedBehaviour
{
    public int Amount { get; private set; } = 1;

    private void Start()
    {
        transform.localScale = Vector3.one * 0.1f;
        transform.DOScale(1f, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventManager.Trigger(new AmmoTriggerEvent(collision, this));
    }
}
