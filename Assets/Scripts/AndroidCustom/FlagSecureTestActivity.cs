using UnityEngine;

namespace ANDROIDCUSTOM
{
    public class FlagSecureTestActivity : MonoBehaviour
    {
        void Start()
        {
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            this.SetFlagSecure();
        }

        void SetFlagSecure()
        {
            using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaClass secureFlagHelperClass = new AndroidJavaClass("com.hkpolice.unityapp.SecureFlagHelper");
                    secureFlagHelperClass.CallStatic("setFlagSecure", activity);
                }));
            }
        }
    }
}