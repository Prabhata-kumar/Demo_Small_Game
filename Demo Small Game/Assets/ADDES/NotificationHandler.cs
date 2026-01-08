using UnityEngine;
using Unity.Notifications.Android; // Make sure to install this package via Package Manager
using UnityEngine.Android;

public class NotificationHandler : MonoBehaviour
{
    void Start()
    {
        RequestNotificationPermission();
    }

    public void RequestNotificationPermission()
    {
        // 1. Check if the user is on Android 13 or higher (API 33+)
        if (Application.platform == RuntimePlatform.Android)
        {
            // 2. Check if we already have permission
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                // 3. This triggers the actual system pop-up "Allow Game to send notifications?"
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
            else
            {
                Debug.Log("Notification permission already granted.");
                CreateNotificationChannel(); // Setup your notification categories
            }
        }
    }

    // You must create a "Channel" on Android for notifications to appear
    void CreateNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "game_reminders",
            Name = "Game Reminders",
            Importance = Importance.Default,
            Description = "Generic notifications about your progress",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
}