using UnityEngine;

public class InternetManager : MonoBehaviour
{
    public static InternetManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// Checks if the device has an active network interface (Wi-Fi or Mobile Data).
    /// </summary>
    public bool IsOnline()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}