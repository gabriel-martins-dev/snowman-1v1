using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerFlowManager : MonoBehaviour
{
    private void Awake()
    {
        ServerEventManager.Listen(ServerEventName.WaitingForPlayers, WaitingForPlayersHandler);
        ServerEventManager.Listen(ServerEventName.RoundStarting(3), () => RoundStartingHandler(3));
        ServerEventManager.Listen(ServerEventName.RoundStarting(2), () => RoundStartingHandler(2));
        ServerEventManager.Listen(ServerEventName.RoundStarting(1), () => RoundStartingHandler(1));
        ServerEventManager.Listen(ServerEventName.RoundStarted, RoundStarted);
    }

    private void WaitingForPlayersHandler()
    {
        GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, true, GameStateMessages.Waiting());
    }

    private void RoundStartingHandler(int time)
    {
        GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, true, time.ToString());
    }

    private void RoundStarted()
    {
        StartCoroutine(GoRoutine());
    }

    public IEnumerator<WaitForSeconds> GoRoutine()
    {
        GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, true, "GO!");

        yield return new WaitForSeconds(1);

        GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, false, "");
    }
}
