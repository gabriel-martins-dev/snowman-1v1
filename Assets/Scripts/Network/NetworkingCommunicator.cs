using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class NetworkingCommunicator : NetworkedBehaviour
{
    [ClientRPC]
    private void TEST()
    {
        Debug.Log("TEST!!!!!");

        GetComponent<SpriteRenderer>().color = Color.green;
    }
}
