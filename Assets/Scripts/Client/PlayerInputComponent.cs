using MLAPI;
using MLAPI.NetworkedVar;
using UnityEngine;

public class PlayerInputComponent : NetworkedBehaviour
{
    public float verticalMovement { get; private set; }
    public float horizontalMovement { get; private set; }
    public bool fire { get; private set; }

    private NetworkedVar<bool> locked = new NetworkedVar<bool>(false);

    void Update()
    {
        if (locked.Value) return;

        verticalMovement = IsLocalPlayer
            ? Input.GetAxis("Vertical")
            : 0;

        horizontalMovement = IsLocalPlayer
            ? Input.GetAxis("Horizontal")
            : 0;

        fire = IsLocalPlayer 
            ? Input.GetButtonDown("Fire1")
            : false;
    }

    public void SetLock(bool value)
    {
        locked.Value = value;
    }
}
