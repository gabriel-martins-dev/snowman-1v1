using MLAPI;
using MLAPI.NetworkedVar;
using UnityEngine;

public class PlayerInputComponent : NetworkedBehaviour
{
    public float verticalMovement { get; private set; }
    public float horizontalMovement { get; private set; }

    public NetworkedVar<bool> locked = new NetworkedVar<bool>(false);

    void Update()
    {
        if (locked.Value) return;

        verticalMovement = IsLocalPlayer
            ? Input.GetAxis("Vertical")
            : 0;

        horizontalMovement = IsLocalPlayer
            ? Input.GetAxis("Horizontal")
            : 0;
    }

    public void SetLock(bool value)
    {
        locked.Value = value;
    }
}
