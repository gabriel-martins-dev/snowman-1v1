using MLAPI;
using UnityEngine;

public class NetworkingModeComponent : MonoBehaviour
{
    [SerializeField] NetworkingManagerView view;

    void Start()
    {
        view.onStartClient += StartClient;
        view.onStartServer += StartServer;
        view.onStartHost += StartHost;

        NetworkingManager.Singleton.OnClientConnectedCallback += o => Debug.Log("Client Connected");
    }

    void StartClient()
    {
        NetworkingManager.Singleton.StartClient();
        view.Hide();
    }

    void StartServer()
    {
        NetworkingManager.Singleton.StartServer();
        view.Hide();
    }

    void StartHost()
    {
        NetworkingManager.Singleton.StartHost();
        view.Hide();
    }
}
