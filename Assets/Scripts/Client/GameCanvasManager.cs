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
        gameStatePanel.gameObject.SetActive(value);
        gameStateText.text = text;
    }

    [ClientRPC]
    public void TriggerGameStateText(bool value, string text)
    {
        SetGameStateText(value, text);
    }
}
