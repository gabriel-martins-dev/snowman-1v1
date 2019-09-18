using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementComponent : MonoBehaviour
{
    [SerializeField] PlayerInputComponent input;
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.position);
        Vector2 moveDirection = Vector2.zero;

        float vertical = input.verticalMovement;
        float horizontal = input.horizontalMovement;

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
