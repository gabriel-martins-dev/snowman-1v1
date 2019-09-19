using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class BulletComponent : NetworkedBehaviour
{
    public readonly float Damage = 1f;
    readonly float velocity = 10f;

    private void Update()
    {
        Vector2 dir = DegreeToVector2(transform.eulerAngles.z);

        transform.position += (Vector3)(dir * velocity * Time.deltaTime);
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        EventManager.Trigger(new BulletTriggerEvent(collision, this));
    }
}