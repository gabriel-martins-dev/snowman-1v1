using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Spawning;
using UnityEngine;

public class NetworkingServer : MonoBehaviour
{
    ServerStateMachine stateMachine;

    public void Initialize(bool isHost)
    {
        NetworkingManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

        stateMachine = new ServerStateMachine();
        stateMachine.ChangeState(ServerModeType.Waiting);

        if (isHost) NetworkingManager.Singleton.StartHost(EnviromentManager.Singleton.GetSpawnPosition(0));
        else NetworkingManager.Singleton.StartServer();
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkingManager.ConnectionApprovedDelegate callback)
    {
        int clients = NetworkingManager.Singleton.ConnectedClientsList.Count;
        bool approve = clients < 2;

        callback(true, EnviromentManager.Singleton.GetPlayerHash(clients), approve, EnviromentManager.Singleton.GetSpawnPosition(clients), null);
    }
}