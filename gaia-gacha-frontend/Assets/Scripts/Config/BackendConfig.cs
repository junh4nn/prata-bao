using UnityEngine;

// Resolves the backend base URL: the local Express server inside the Unity Editor,
// the deployed fly.io server in any compiled build (including the WebGL build on itch.io).
[CreateAssetMenu(fileName = "BackendConfig", menuName = "GaiaGacha/Backend Config")]
public class BackendConfig : ScriptableObject
{
    [SerializeField] private string localBaseUrl = "http://localhost:3000";
    [SerializeField] private string productionBaseUrl = "https://gaia-gacha-backend.fly.dev";

    public string baseUrl =>
#if UNITY_EDITOR
        localBaseUrl;
#else
        productionBaseUrl;
#endif

    private static BackendConfig _instance;
    public static BackendConfig Instance => _instance ??= Resources.Load<BackendConfig>("BackendConfig");
}
