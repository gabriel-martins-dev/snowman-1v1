using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class AmmoPickup : NetworkedBehaviour
{
    public int Amount { get; private set; } = 1;

    private void Start()
    {
        StartCoroutine(Grow());
    }

    public IEnumerator Grow()
    {
        float time = Time.time + 1;
        float scale = 0.1f;

        while (scale < 1f)
        {
            scale += 5f * Time.deltaTime;
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventManager.Trigger(new AmmoTriggerEvent(collision, this));
    }
}
