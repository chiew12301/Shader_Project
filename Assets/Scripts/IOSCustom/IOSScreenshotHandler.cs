using UnityEngine;
using System.Runtime.InteropServices;

namespace IOSCUSTOM
{
    public class IOSScreenshotHandler : MonoBehaviour
    {
        //=========================================

        // Import the native iOS methods
        [DllImport("__Internal")]
        private static extern void startObservingScreenshotNotification();

        [DllImport("__Internal")]
        private static extern void stopObservingScreenshotNotification();

        private void OnEnable()
        {
            startObservingScreenshotNotification();
        }

        private void OnDisable()
        {
            stopObservingScreenshotNotification();
        }


        private void OnScreenshotTaken()
        {
            // Handle screenshot taken event
        }

        //=========================================
    }
}