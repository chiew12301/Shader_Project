using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraToSpecificColor))]
public class CustomScreenCaptureLayout : Editor
{
    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();

        CameraToSpecificColor camScript = (CameraToSpecificColor)target;
        if(GUILayout.Button("Set To Green Screen"))
        {
            camScript.SetCameraToGreenScreen();
        }

        if(GUILayout.Button("Set To Black Screen"))
        {
            camScript.SetCameraToBlackScreen();
        }

        if(GUILayout.Button("Set To Specific Color"))
        {
            camScript.SetCameraToSpecificColor();
        }

    }
}

[CustomEditor(typeof(ScreenCaptureWithUnity))]
public class CustomScreenCaptureWithUnityLayout : Editor
{
    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();

        ScreenCaptureWithUnity captureScript = (ScreenCaptureWithUnity)target;

        if(GUILayout.Button("Render Screen"))
        {
            captureScript.CaptureScreen();
        }
    }
}
