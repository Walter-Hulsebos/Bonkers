using System;
using System.Threading.Tasks;

using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [Header(header: "References")]
    [SerializeField] private ServerSingleton serverPrefab;

    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton   hostSingleton;

    private       ApplicationData appData;
    public static Boolean         IsServer;

    private async void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(target: gameObject);

        await LaunchInMode(isServer: SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(Boolean isServer)
    {
        appData  = new ApplicationData();
        IsServer = isServer;

        if (isServer)
        {
            ServerSingleton serverSingleton = Instantiate(original: serverPrefab);
            await serverSingleton.CreateServer();

            GameInfo defaultGameInfo = new()  { gameMode = GameMode.Default, map = Map.Default, gameQueue = GameQueue.Casual, };

            await serverSingleton.Manager.StartGameServerAsync(startingGameInfo: defaultGameInfo);
        }
        else
        {
            ClientSingleton clientSingleton = Instantiate(original: clientPrefab);
            Instantiate(original: hostSingleton);

            await clientSingleton.CreateClient();

            clientSingleton.Manager.ToMainMenu();
        }
    }
}