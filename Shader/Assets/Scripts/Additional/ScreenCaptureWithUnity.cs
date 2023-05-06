using UnityEngine;
using System.IO;

public class ScreenCaptureWithUnity : MonoBehaviour
{
    public string nameofImage = "SomeLevel";
    public string format = "png";


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            this.Capture();
        }
    }

    [ContextMenu("Render")]
    public void CaptureScreen()
    {
        this.Capture();
    }

    private void Capture()
    {
        string folderPath = Directory.GetCurrentDirectory() + "/Screenshots/";

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string name = Path.Combine(folderPath, nameofImage + "." + format);

        ScreenCapture.CaptureScreenshot(name);
        Debug.Log(name);
    }
}
