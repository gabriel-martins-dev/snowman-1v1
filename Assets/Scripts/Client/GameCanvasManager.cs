using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization.Pooled;
using System.Text;
using System.Linq;

public static class GameStateMessages
{
    public static string Waiting()
    {
        return "Waiting for players";
    }
}

public class GameCanvasManager : NetworkedBehaviour
{
    [Header("Game State")]
    [SerializeField] RectTransform gameStatePanel;
    [SerializeField] Text gameStateText;

    private static GameCanvasManager singleton;

    public static GameCanvasManager Singleton
    {
        get
        {
            if (singleton != null) return singleton;
            else
            {
                singleton = FindObjectOfType<GameCanvasManager>();

                return singleton;
            }
        }
    }


    public void SetGameStateText(bool value, string text = "")
    {
        if (IsClient) Debug.Log("SetGameStateText: ["  + value + "] - " + text);
        gameStatePanel.gameObject.SetActive(value);
        gameStateText.text = text;
    }

    /*
    public override void NetworkStart()
    {
        CustomMessagingManager.RegisterNamedMessageHandler("myMessageName", 
            (senderClientId, stream) => 
        {
            using (PooledBitReader reader = PooledBitReader.Get(stream))
            {
                StringBuilder stringBuilder = reader.ReadString(); //Example
                string message = stringBuilder.ToString();
                Debug.Log("WHAT: " + message);
            }
        });
    }

    public void RunTest()
    {
        InvokeServerRpc(TEST);
    }

    [ServerRPC]
    public void TEST()
    {
        Debug.Log("TEST");
    }

    [ClientRPC]
    public void TEST2()
    {
        Debug.Log("TEST2");

        SetGameStateText(true, "BUNDA");
        gameStateText.color = Color.magenta;
    }
    */

    [ClientRPC]
    public void TriggerGameStateText(bool value, string text)
    {
        SetGameStateText(value, text);
    }
}
