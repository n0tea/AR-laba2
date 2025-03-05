using UnityEngine;
using UnityEngine.Android;

public class PermissionRequestService : MonoBehaviour
{
    public static bool HasCameraPermission { get; private set; }

    public static void RequestCameraPermission(System.Action<bool> callback)
    {
#if UNITY_EDITOR 
        HasCameraPermission = true;
        callback(true);
        return;
#endif
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                HasCameraPermission = true;
                callback(true);
            }
            else
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionGranted += (permissionName) =>
                {
                    if (permissionName == Permission.Camera)
                    {
                        HasCameraPermission = true;
                        callback(true);
                    }
                };
                callbacks.PermissionDenied += (permissionName) =>
                {
                    if (permissionName == Permission.Camera)
                    {
                        HasCameraPermission = false;
                        callback(false);
                    }
                };

                Permission.RequestUserPermission(Permission.Camera, callbacks);
            }
        }
        else
        {
            HasCameraPermission = true;
            callback(true);
        }
    }
}
