using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppController : MonoBehaviour
{
    [SerializeField] private SingletonClient clientObj;
    [SerializeField] private SingletonHost hostObj;
    [SerializeField] private SingletonServer serverObj;

    private const string GameScene = "GameScene";
    private ApplicationData appData;
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
        await LaunchMachineType(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchMachineType(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            appData = new ApplicationData();
            SingletonServer serverSingleton = Instantiate(serverObj);
            StartCoroutine(LoadGame(serverSingleton));
        }
        else
        {
            //Host Manager olusmadan diger scene yuklenmemeli
            //Bundan dolayi await cagrisindan once calisarak yeterince bekleme suresini sagladim.


            //Host Manager burada uretilir.
            SingletonHost hostSingleton = Instantiate(hostObj);
            hostSingleton.InstantiateHostManager();

            //Client Manager burada uretilir.
            //Instantiate edilmesiyle Auth kontrolu yapilir.
            SingletonClient clientSingleton = Instantiate(clientObj);
            bool isAuthorized = await clientSingleton.InstantiateClientManager();




            //Auth basarili ise menu yuklemesi yapilir
            if (isAuthorized)
            {
                clientSingleton.GameManager.LoadMenuScene();
            }
        }
    }

    IEnumerator LoadGame(SingletonServer serverSingleton)
    {
       AsyncOperation asyncOp = SceneManager.LoadSceneAsync(GameScene);
        while (!asyncOp.isDone)
        {
            yield return null;
        }
        Task instantiateManager=  serverSingleton.InstantiateServerManager();
        yield return new WaitUntil(() => instantiateManager.IsCompleted);
        Task InitiateManager = serverSingleton.GameManager.InitiateGameServer();
        yield return new WaitUntil(() => InitiateManager.IsCompleted);
    }


}
