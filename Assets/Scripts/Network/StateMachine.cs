using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using Random = UnityEngine.Random;

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

        EventManager.Trigger(new WaitingForPlayersEvent());
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
        EventManager.Trigger(new RoundStartingEvent(3));
        yield return new WaitForSeconds(1);
        EventManager.Trigger(new RoundStartingEvent(2));
        yield return new WaitForSeconds(1);
        EventManager.Trigger(new RoundStartingEvent(1));
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

        // Unlock clients
        foreach (var client in clients)
        {
            connectors.Add(client.PlayerObject.GetComponent<PlayerConnector>());
            connectors.Last().SetLocked(false);
            connectors.Last().Heal();
        }

        // Starts snowball spawning
        GameCanvasManager.Singleton.StartCoroutine(SpawnPickups());

        EventManager.Trigger(new RoundStartedEvent());
    }

    public override void OnExitState()
    {
        GameCanvasManager.Singleton.StopCoroutine(SpawnPickups());

        for (int i = 0; i < connectors.Count; i++)
        {
            connectors[i].SetLocked(false);
            connectors[i].Heal();
            connectors[i].transform.position = EnviromentManager.Singleton.GetSpawnPosition(i);
        }

        connectors.Clear();
    }

    public IEnumerator<WaitForSeconds> SpawnPickups()
    {
        GameSpawnManager.SpawnPickup(EnviromentManager.Singleton.GetPickupSpawnCenterPosition());

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

            GameSpawnManager.SpawnPickup(EnviromentManager.Singleton.GetPickupSpawnPosition());

            yield return null;
        }
    }
}