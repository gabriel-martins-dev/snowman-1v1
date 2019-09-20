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
    }

    private bool HasAllPlayers()
    {
        // value should come from a setting, not hard codded
        return NetworkingManager.Singleton.ConnectedClientsList.Count >= 1;
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
    private class Team
    {
        public int Score;
    }

    private List<PlayerConnector> connectors;
    private bool stopSpawning;
    private int redTeam, blueTeam = 0;

    public RoundMode(Action<ServerModeType> to) : base(to)
    {

    }

    public override void OnEnterState()
    {
        EventManager.Listen<DeathEvent>(DeathEventHandler);
        stopSpawning = false;
        connectors = new List<PlayerConnector>();

        var clients = NetworkingManager.Singleton.ConnectedClientsList;

        // Unlock clients
        foreach (var client in clients)
        {
            connectors.Add(client.PlayerObject.GetComponent<PlayerConnector>());
            connectors.Last().SetLocked(false);
            connectors.Last().ResetComponents();
            //connectors.Last().InvokeClientRpcOnEveryone(connectors.Last().Teleport, EnviromentManager.Singleton.GetSpawnPosition(connectors.Count - 1));
            //connectors.Last().transform.position = EnviromentManager.Singleton.GetSpawnPosition(connectors.Count-1);
        }

        // Starts snowball spawning
        GameCanvasManager.Singleton.StartCoroutine(SpawnPickups());

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
        stopSpawning = true;

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

                if(redTeam > 1 || blueTeam > 1)
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

    public IEnumerator<WaitForSeconds> SpawnPickups()
    {
        GameSpawnManager.SpawnPickup(EnviromentManager.Singleton.GetPickupSpawnCenterPosition());

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

            if (stopSpawning) break;

            GameSpawnManager.SpawnPickup(EnviromentManager.Singleton.GetPickupSpawnPosition());

            yield return null;
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
        GameCanvasManager.Singleton.StartCoroutine(KillGame());
    }

    public override void OnExitState()
    {

    }

    public IEnumerator<WaitForSeconds> KillGame()
    {
        yield return new WaitForSeconds(4f);

        NetworkingManager.Singleton.StopServer();

        EventManager.Trigger(new RestartEvent());

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}