using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using KC_Custom;

namespace ASSETSBUNDLE
{
    public class AssetsBundlerLoadingScreen : MonobehaviourSingleton<AssetsBundlerLoadingScreen>
    {
        [Header("Group")]
        [SerializeField] private GameObject m_group = null;

        [Header("Component")]
        [SerializeField] private GameObject m_rayCastBlockPanel = null;

        [Header("Debug")]
        [SerializeField] private Slider m_loadingSlider = null;
        [SerializeField] private TextMeshProUGUI m_loadingText = null;

        [SerializeField] private Button m_fetchOnlineButton = null;
        [SerializeField] private Button m_fetchOfflineButton = null;

        [SerializeField] private Button m_refetchRCButton = null;

        //==============================================================

        protected override void OnEnable()
        {
            base.OnEnable();
            RemoteConfigService.Instance.FetchCompleted += this.OnRCFetchCompleted;
        }

        protected override void OnDisable()
        {
            RemoteConfigService.Instance.FetchCompleted -= this.OnRCFetchCompleted;
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();
            this.OnStartAction();
        }

        //==============================================================

        public void UIStatus(bool status)
        {
            this.m_group.SetActive(status);
        }

        public void UpdateLoading(string text, float value)
        {
            this.m_loadingText.text = text;
            this.m_loadingSlider.value = value;
        }

        private void OnRCFetchCompleted(ConfigResponse response)
        {
            Debug.Log("completed load");
            this.UpdateLoading("Completed Load!", 1.0f);
            this.DebugButtonInteractStatus(true);
            this.m_rayCastBlockPanel.SetActive(false);
        }

        private void OnStartAction()
        {
            this.UpdateLoading("Pending Task.", 0.0f);
            this.DebugButtonInteractStatus(false);
            this.m_rayCastBlockPanel.SetActive(false);
            AssetsBundleHandler.GetInstance().BNMPOCLoad();
        }

        private void DebugButtonInteractStatus(bool status)
        {
            this.m_fetchOfflineButton.interactable = status;
            this.m_fetchOnlineButton.interactable = status;
            this.m_refetchRCButton.interactable = status;
        }

        //==============================================================

    }
}