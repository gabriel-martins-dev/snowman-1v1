using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;
using DG.Tweening;

public class ServerFlowManager : MonoBehaviour
{
    private void Awake()
    {
        // server rules
        EventManager.Listen<WaitingForPlayersEvent>(WaitingForPlayersHandler);
        EventManager.Listen<RoundStartingEvent>(RoundStartingHandler);
        EventManager.Listen<RoundStartedEvent>(RoundStartedHandler);
        EventManager.Listen<GameEndedEvent>(GameEndedEventHandler);

        // gameplay rules
        EventManager.Listen<BulletTriggerEvent>(BulletTriggerHandler);
        EventManager.Listen<AmmoTriggerEvent>(AmmoTriggerHandler);
    }

    void BulletTriggerHandler(BulletTriggerEvent e)
    {
        if (e.Collider.tag == "Wall")
        {
            Destroy(e.Bullet.gameObject);
        }

        if (e.Collider.tag == "Player")
        {
            PlayerConnector connector = e.Collider.GetPlayerConnector();
            if (connector != null)
            {
                connector.Damage(e.Bullet.Damage);
            }

            Destroy(e.Bullet.gameObject);
        }
    }

    void AmmoTriggerHandler(AmmoTriggerEvent e)
    {
        if (e.Collider.tag == "Player")
        {
            var netObject = e.Collider.GetComponent<NetworkedObject>();

            if (netObject != null)
            {
                var p = SpawnManager.GetPlayerObject(netObject.OwnerClientId);

                p.GetComponent<PlayerConnector>().AddAmmo(e.Ammo.Amount);

                Destroy(e.Ammo.gameObject);
            }
        }
    }

    private void WaitingForPlayersHandler(WaitingForPlayersEvent e)
    {
        GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, true, GameStateMessages.Waiting());
    }

    private void RoundStartingHandler(RoundStartingEvent e)
    {
        GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, true, e.Time.ToString());
    }

    private void RoundStartedHandler(RoundStartedEvent e)
    {
        DOTween.Sequence()
            .AppendCallback(() => GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, true, "GO!"))
            .AppendInterval(1f)
            .AppendCallback(() => GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, false, ""));
    }

    private void GameEndedEventHandler(GameEndedEvent e)
    {
        GameCanvasManager.Singleton.InvokeClientRpcOnEveryone(GameCanvasManager.Singleton.TriggerGameStateText, true, "Winner is " + e.Winner);
    }
}