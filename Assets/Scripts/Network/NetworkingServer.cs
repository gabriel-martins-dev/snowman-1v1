using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Spawning;
using UnityEngine;

public class NetworkingServer : MonoBehaviour
{
    public NetworkingCommunicator comm;
    ServerStateMachine stateMachine;

    public void Initialize(bool isHost)
    {
        NetworkingManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

        stateMachine = new ServerStateMachine();
        stateMachine.ChangeState(ServerStateType.Waiting);

        if (isHost) NetworkingManager.Singleton.StartHost();
        else NetworkingManager.Singleton.StartServer();
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkingManager.ConnectionApprovedDelegate callback)
    {
        Debug.Log("APROVAL");

        int clients = NetworkingManager.Singleton.ConnectedClientsList.Count;
        bool approve = clients < 2;

        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        callback(true, null, approve, EnviromentManager.Singleton.GetSpawnPosition(clients), null);
    }

    private void OnGUI()
    {

    }
}