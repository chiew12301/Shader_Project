using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Camera))]
public class CameraToSpecificColor : MonoBehaviour
{
    [SerializeField] private Color m_targetedColor = Color.white;
    private Camera m_cam = null;

    //=========================================

    //=========================================

    [ContextMenu("Set To Green Screen")]
    public void SetCameraToGreenScreen()
    {
        this.CheckCam();

        this.m_cam.backgroundColor = new Color(0.0f, 177.0f / 255.0f, 64.0f / 255.0f, 255.0f / 255.0f);
    }

    [ContextMenu("Set To Black Screen")]
    public void SetCameraToBlackScreen()
    {
        this.CheckCam();

        this.m_cam.backgroundColor = Color.black;
    }

    [ContextMenu("Set To Specific Color")]
    public void SetCameraToSpecificColor()
    {
        this.CheckCam();

        this.m_cam.backgroundColor = this.m_targetedColor;
    }


    private void CheckCam()
    {
        if (this.m_cam == null)
        {
            this.m_cam = this.GetComponent<Camera>();
        }
    }

    //=========================================
}
