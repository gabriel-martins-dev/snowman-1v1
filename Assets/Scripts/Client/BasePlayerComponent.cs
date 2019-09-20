using UnityEngine;
using System.Collections;
using MLAPI;
using MLAPI.NetworkedVar;

public class BasePlayerComponent : NetworkedBehaviour
{
    protected NetworkedVar<bool> locked = new NetworkedVar<bool>(true);

    public void SetLock(bool value)
    {
        locked.Value = value;
    }
}
