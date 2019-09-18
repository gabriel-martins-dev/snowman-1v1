using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
public enum ServerStateType
{
    Waiting,
    StartRound,
    Round,
    EndRound,
    EndGame
}

public class ServerStateMachine
{
    Dictionary<ServerStateType, State> states;
    State currentState;

    public ServerStateMachine()
    {
        states = new Dictionary<ServerStateType, State>();
        states.Add(ServerStateType.Waiting, new WaitingState(ChangeState));
        states.Add(ServerStateType.StartRound, new StartRoundState(ChangeState));
    }

    public void ChangeState(ServerStateType state)
    {
        currentState?.OnExitState();
        currentState = states[state];
        currentState.OnEnterState();
    }
}

public abstract class State
{
    protected Action<ServerStateType> to;

    public State(Action<ServerStateType> to)
    {
        this.to = to;
    }

    public abstract void OnEnterState();
    public abstract void OnExitState();
}

public class WaitingState : State
{
    private List<PlayerConnector> connectors;

    public WaitingState(Action<ServerStateType> to) : base(to)
    {

    }

    public override void OnEnterState()
    {
        connectors = new List<PlayerConnector>();

        NetworkingManager.Singleton.OnServerStarted += ServerStartedHandler;
        NetworkingManager.Singleton.OnClientConnectedCallback += ClientConnectedHandler;
    }

    public override void OnExitState()
    {
        Debug.Log("Moved to: " + connectors.Last().transform.position);

        connectors.Clear();

        NetworkingManager.Singleton.OnServerStarted -= ServerStartedHandler;
        NetworkingManager.Singleton.OnClientConnectedCallback -= ClientConnectedHandler;
    }

    private void ServerStartedHandler()
    {
        Debug.Log("Server Started: " + NetworkingManager.Singleton.ConnectedClientsList.Count);

        var clients = NetworkingManager.Singleton.ConnectedClientsList;

        if (clients.Count > 0)
        {
            foreach (var client in clients)
            {
                ClientConnectedHandler(client.ClientId);
            }
        }
    }

    private void ClientConnectedHandler(ulong client)
    {
        Debug.Log("Client Connected");

        connectors.Add(
            NetworkingManager.Singleton.ConnectedClients[client]
            .PlayerObject.GetComponent<PlayerConnector>());
        connectors.Last().SetLocked(true);
        connectors.Last().transform.position = (EnviromentManager.Singleton.GetSpawnPosition(connectors.Count - 1));

        if (HasAllPlayers())
        {
            to(ServerStateType.StartRound);
        }
    }

    private bool HasAllPlayers()
    {
        // value should come from a setting, not hard codded
        return NetworkingManager.Singleton.ConnectedClientsList.Count >= 2;
    }
}

public class StartRoundState : State
{
    public StartRoundState(Action<ServerStateType> to) : base(to)
    {

    }

    public override void OnEnterState()
    {
        Debug.Log("StartRoundState!");
    }

    public override void OnExitState()
    {

    }
}