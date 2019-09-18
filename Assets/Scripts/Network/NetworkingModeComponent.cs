using MLAPI;
using UnityEngine;

public class NetworkingModeComponent : MonoBehaviour
{
    [SerializeField] NetworkingManagerView view;
    [SerializeField] NetworkingServer server;

    void Start()
    {
        view.onStartClient += StartClient;
        view.onStartServer += StartServer;
        view.onStartHost += StartHost;
    }

    void StartClient()
    {
        NetworkingManager.Singleton.StartClient();
        view.Hide();
    }

    void StartServer()
    {
        SetupServer(false);
        view.Hide();
    }

    void StartHost()
    {
        SetupServer(true);
        view.Hide();
    }

    void SetupServer(bool isHost)
    {
        Instantiate(server).GetComponent<NetworkingServer>().Initialize(isHost);
    }
}
