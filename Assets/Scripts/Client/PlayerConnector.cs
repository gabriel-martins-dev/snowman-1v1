using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerConnector : NetworkedBehaviour
{
    [SerializeField] PlayerInputComponent input;

    public void SetLocked(bool value)
    {
        input.SetLock(value);
    }
}
