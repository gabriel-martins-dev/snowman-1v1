using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public enum ServerModeType
{
    Waiting,
    StartRound,
    Round,
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
        states.Add(ServerModeType.EndGame, new EndGameMode(ChangeState));
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
        connectors.Add(
            NetworkingManager.Singleton.ConnectedClients[client]
            .PlayerObject.GetComponent<PlayerConnector>());
        connectors.Last().SetLocked(true);

        if (HasAllPlayers())
        {
            to(ServerModeType.StartRound);
        }

        EventManager.Trigger(new WaitingForPlayersEvent());
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
        PoolManager.Singleton.PullAll();

        Sequence countDownSequence = DOTween.Sequence();
        countDownSequence
            .AppendInterval(0.1f)
            .AppendCallback(() => EventManager.Trigger(new RoundStartingEvent(3)))
            .AppendInterval(1f)
            .AppendCallback(() => EventManager.Trigger(new RoundStartingEvent(2)))
            .AppendInterval(1f)
            .AppendCallback(() => EventManager.Trigger(new RoundStartingEvent(1)))
            .AppendInterval(1f)
            .AppendCallback(() => to(ServerModeType.Round));
    }

    public override void OnExitState()
    {

    }
}

public class RoundMode : ServerMode
{
    private List<PlayerConnector> connectors;
    private Sequence spawnSequence;
    private int redTeam, blueTeam = 0;

    public RoundMode(Action<ServerModeType> to) : base(to)
    {
        spawnSequence = DOTween.Sequence()
            .AppendInterval(Random.Range(0.5f, 1.5f))
            .AppendCallback(() => PoolManager.SpawnPickup(EnviromentManager.Singleton.GetPickupSpawnPosition()))
            .SetLoops(-1);
        spawnSequence.Pause();
    }

    public override void OnEnterState()
    {
        EventManager.Listen<DeathEvent>(DeathEventHandler);
        connectors = new List<PlayerConnector>();

        var clients = NetworkingManager.Singleton.ConnectedClientsList;

        // Unlock clients
        foreach (var client in clients)
        {
            connectors.Add(client.PlayerObject.GetComponent<PlayerConnector>());
            connectors.Last().SetLocked(false);
            connectors.Last().ResetComponents();
        }

        // Starts snowball spawning
        spawnSequence.Restart();

        EventManager.Trigger(new RoundStartedEvent());
    }

    public override void OnExitState()
    {
        EventManager.Clear<DeathEvent>(DeathEventHandler);

        for (int i = 0; i < connectors.Count; i++)
        {
            connectors[i].SetLocked(true);
            connectors[i].ResetComponents();
            connectors[i].InvokeClientRpcOnEveryone(connectors[i].Teleport, EnviromentManager.Singleton.GetSpawnPosition(i));
        }

        connectors.Clear();
    }

    void DeathEventHandler(DeathEvent e)
    {
        spawnSequence.Pause();

        string winner = string.Empty;

        for (int i = 0; i < connectors.Count; i++)
        {
            if(connectors[i].OwnerClientId == e.ClientId)
            {
                if (i < 1)
                {
                    redTeam++;
                }
                else
                {
                    blueTeam++;
                }

                if(redTeam > 2 || blueTeam > 2)
                {
                    winner = (redTeam > blueTeam ? "Red" : "Blue") + " Player";
                }
                break;
            }
        }

        if(!string.IsNullOrEmpty(winner))
        {
            EventManager.Trigger(new GameEndedEvent(winner));
            to(ServerModeType.EndGame);
        }
        else
        {
            to(ServerModeType.StartRound);
        }
    }
}

public class EndGameMode : ServerMode
{
    public EndGameMode(Action<ServerModeType> to) : base(to)
    {

    }

    public override void OnEnterState()
    {
        PoolManager.Singleton.PullAll();

        DOTween.Sequence()
            .AppendInterval(4f)
            .AppendCallback(() => NetworkingManager.Singleton.StopServer())
            .AppendCallback(() => EventManager.Trigger(new RestartEvent()))
            .AppendCallback(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
    }

    public override void OnExitState()
    {

    }
}