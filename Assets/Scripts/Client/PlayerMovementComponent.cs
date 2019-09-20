using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;

public class PlayerMovementComponent : BasePlayerComponent
{
    [SerializeField] PlayerInputComponent input;
    readonly float speed = 5;

    void Update()
    {
        if (locked.Value) return;

        Vector2 moveDirection = Vector2.zero;

        float vertical = input.VerticalMovement;
        float horizontal = input.HorizontalMovement;

        if (horizontal > 0)
        {
            moveDirection.x = 1;
        }
        else if (horizontal < 0)
        {
            moveDirection.x = -1;
        }
        else if (vertical > 0)
        {
            moveDirection.y = 1;
        }
        else if (vertical < 0)
        {
            moveDirection.y = -1;
        }
        
        Vector3 movement = (moveDirection * speed * Time.deltaTime);

        transform.position += movement;
    }
}
