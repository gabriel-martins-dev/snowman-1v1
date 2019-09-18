using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
public enum ServerModeType
{
    Waiting,
    StartRound,
    Round,
    EndRound,
    EndGame
}

public class ServerStateMachine
{
    Dictionary<ServerModeType, ServerMode> states;
    ServerMode currentState;

    public ServerStateMachine()
    {
        states = new Dictionary<ServerModeType, ServerMode>();
        states.Add(ServerModeType.Waiting, new WaitingMode(ChangeState));
        states.Add(ServerModeType.StartRound, new StartRoundMode(ChangeState));
        states.Add(ServerModeType.Round, new RoundMode(ChangeState));
    }

    public void ChangeState(ServerModeType state)
    {
        currentState?.OnExitState();
        currentState = states[state];
        currentState.OnEnterState();
    }
}

public abstract class ServerMode
{
    protected Action<ServerModeType> to;

    public ServerMode(Action<ServerModeType> to)
    {
        this.to = to;
    }

    public abstract void OnEnterState();
    public abstract void OnExitState();
}

public class WaitingMode : ServerMode
{
    private List<PlayerConnector> connectors;

    public WaitingMode(Action<ServerModeType> to) : base(to)
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
        connectors.Clear();

        NetworkingManager.Singleton.OnServerStarted -= ServerStartedHandler;
        NetworkingManager.Singleton.OnClientConnectedCallback -= ClientConnectedHandler;
    }

    private void ServerStartedHandler()
    {
        Debug.Log("Server Started");

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

        if (HasAllPlayers())
        {
            to(ServerModeType.StartRound);
        }

        ServerEventManager.TriggerEvent(ServerEventName.WaitingForPlayers);
        // GameCanvasManager.Singleton.InvokeServerRpc(GameCanvasManager.Singleton.TEST);
        //GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, true, "Bunda");
    }

    private bool HasAllPlayers()
    {
        // value should come from a setting, not hard codded
        return NetworkingManager.Singleton.ConnectedClientsList.Count >= 2;
    }
}

public class StartRoundMode : ServerMode
{
    public StartRoundMode(Action<ServerModeType> to) : base(to)
    {

    }

    public override void OnEnterState()
    {
        GameCanvasManager.Singleton.StartCoroutine(CountDownRoutine());
    }

    public override void OnExitState()
    {

    }

    public IEnumerator<WaitForSeconds> CountDownRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        ServerEventManager.TriggerEvent(ServerEventName.RoundStarting(3));
        yield return new WaitForSeconds(1);
        ServerEventManager.TriggerEvent(ServerEventName.RoundStarting(2));
        yield return new WaitForSeconds(1);
        ServerEventManager.TriggerEvent(ServerEventName.RoundStarting(1));
        yield return new WaitForSeconds(1);

        to(ServerModeType.Round);
    }
}

public class RoundMode : ServerMode
{
    private List<PlayerConnector> connectors;

    public RoundMode(Action<ServerModeType> to) : base(to)
    {

    }

    public override void OnEnterState()
    {
        connectors = new List<PlayerConnector>();

        var clients = NetworkingManager.Singleton.ConnectedClientsList;

        foreach (var client in clients)
        {
            connectors.Add(client.PlayerObject.GetComponent<PlayerConnector>());
            connectors.Last().SetLocked(false);
        }

        ServerEventManager.TriggerEvent(ServerEventName.RoundStarted);

    }

    public override void OnExitState()
    {

    }

    public IEnumerator<WaitForSeconds> CountDownRoutine()
    {
        yield return new WaitForSeconds(2f);
        ServerEventManager.TriggerEvent(ServerEventName.RoundStarted);
        GameCanvasManager.Singleton.TriggerGameStateText(true, "GO!");

    }
}