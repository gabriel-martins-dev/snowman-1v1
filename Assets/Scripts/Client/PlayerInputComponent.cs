using MLAPI;
using UnityEngine;

public class PlayerInputComponent : NetworkedBehaviour
{
    public float verticalMovement { get; private set; }
    public float horizontalMovement { get; private set; }

    void Update()
    {
        verticalMovement = IsLocalPlayer
            ? Input.GetAxis("Vertical")
            : 0;

        horizontalMovement = IsLocalPlayer
            ? Input.GetAxis("Horizontal")
            : 0;
    }
}
