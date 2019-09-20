using System;
using UnityEngine;

public class NetworkingManagerView : MonoBehaviour
{
    public Action onStartClient;
    public Action onStartServer;
    public Action onStartHost;

    bool visible { get; set; } = true;

    public void Hide()
    {
        visible = false;
    }

    public void Show()
    {
        visible = true;
    }

    private void OnGUI()
    {
        if (!visible) return;

        if (GUI.Button(new Rect(20, 20, 100, 20), "Start client"))
        {
            onStartClient?.Invoke();
        }

        // Disasbled
        //if (GUI.Button(new Rect(20, 70, 100, 20), "Start server"))
        //{
        //    onStartServer?.Invoke();
        //}

        if (GUI.Button(new Rect(20, 120, 100, 20), "Start host"))
        {
            onStartHost?.Invoke();
        }
    }
}
