using MLAPI;
using UnityEngine;

public class NetworkingModeComponent : MonoBehaviour
{
    [SerializeField] NetworkingManagerView view;
    [SerializeField] NetworkingServer serverPrefab;

    void Start()
    {
        view.onStartClient += StartClient;
        view.onStartServer += StartServer;
        view.onStartHost += StartHost;

        EventManager.Listen<RestartEvent>(Restart);
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
        Instantiate(serverPrefab).GetComponent<NetworkingServer>().Initialize(isHost);
    }

    void Restart(RestartEvent e)
    {
        view.Show();
    }
}
