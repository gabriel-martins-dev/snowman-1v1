using MLAPI;
using MLAPI.NetworkedVar;
using UnityEngine;

public class PlayerInputComponent : BasePlayerComponent
{
    public float VerticalMovement { get; private set; }
    public float HorizontalMovement { get; private set; }
    public bool Fire { get; private set; }

    void Update()
    {
        if (locked.Value) return;

        VerticalMovement = IsLocalPlayer
            ? Input.GetAxis("Vertical")
            : 0;

        HorizontalMovement = IsLocalPlayer
            ? Input.GetAxis("Horizontal")
            : 0;

        Fire = IsLocalPlayer 
            ? Input.GetButtonDown("Fire1")
            : false;
    }
}
