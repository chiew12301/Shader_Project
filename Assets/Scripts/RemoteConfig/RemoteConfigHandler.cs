using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using KC_Custom;

namespace ASSETSBUNDLE
{
    [System.Serializable]
    public class RemoteConfigData
    {
        public string androidAssetBundleLink = "";
        public string IOSAssetBundleLink = "";
    }


    public class RemoteConfigHandler : MonobehaviourSingleton<RemoteConfigHandler>
    {
        public struct userAttributes { }
        public struct appAttributes { }

        private RemoteConfigData m_remoteConfigData = null;

        //===============================================================
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // private static void InitBeforeSceneLoad()
        // {
        //     RemoteConfigHandler obj = new GameObject("RemoteConfigHandler").AddComponent<RemoteConfigHandler>();
        // }


        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }

        protected override void Start()
        {
            base.Start();
            //this.StartFetchRemoteConfig();
        }

        //===============================================================

        public RemoteConfigData GetRemoteConfigData() => this.m_remoteConfigData;

        public void RefetchRC()
        {
            RemoteConfigService.Instance.FetchCompleted -= this.ApplyRemoteSettings;
            //this.StartFetchRemoteConfig();
        }

        public void StartFetchRemoteConfig()
        {
            this.UpdateLoadingScreen("Starting Fetch RC", 0.0f);
            //this.StartCoroutine(this.InitRCCO());
            //this.ServiceFetch();
            var task = this.StartFetch();
        }


        private IEnumerator InitRCCO()
        {
            this.UpdateLoadingScreen("Checking game services.", 0.0f);
            var taskServiceInit = UnityServices.InitializeAsync();
            yield return new WaitUntil(() => taskServiceInit.IsCompleted);
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                this.UpdateLoadingScreen("Sign in.", 0.0f);
                var login =  AuthenticationService.Instance.SignInAnonymouslyAsync();
                yield return new WaitUntil(() => login.IsCompleted);
            }

            this.UpdateLoadingScreen("Waiting To Fetch RC", 0.0f);
            yield return new WaitForSeconds(5.0f); 

            this.UpdateLoadingScreen("Fetching RC", 0.0f);
            RemoteConfigService.Instance.FetchCompleted += this.ApplyRemoteSettings;
            var fetchTask = RemoteConfigService.Instance.FetchConfigsAsync<userAttributes, appAttributes>(new userAttributes(){}, new appAttributes(){});
            yield return new WaitUntil(() => fetchTask.IsCompleted);
            this.UpdateLoadingScreen("Fetch Completed", 1.0f);
        }

        private async Task InitializeRemoteConfigAsync()
        {
            this.UpdateLoadingScreen("Checking game services.", 0.0f);
            // initialize handlers for unity game services
            await UnityServices.InitializeAsync();

            // remote config requires authentication for managing environment information
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                this.UpdateLoadingScreen("Sign in.", 0.0f);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private async Task StartFetch()
        {
            this.UpdateLoadingScreen("Checking Internet Conncetion.", 0.0f);
            // initialize Unity's authentication and core services, however check for internet connection
            // in order to fail gracefully without throwing exception if connection does not exist
            if (Utilities.CheckForInternetConnection())
            {
                await InitializeRemoteConfigAsync();
            }

            this.UpdateLoadingScreen("Start Fetching RC.", 0.0f);
            try{
                RemoteConfigService.Instance.FetchCompleted += this.ApplyRemoteSettings;
                RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
            }
            catch(Exception e)
            {
                this.UpdateLoadingScreen(e.ToString(), 1.0f);
            }
        }

        private void ServiceFetch()
        {
            this.UpdateLoadingScreen("Start Fetching RC.", 0.0f);
            RemoteConfigService.Instance.FetchCompleted += this.ApplyRemoteSettings;
            RemoteConfigService.Instance.FetchConfigs<userAttributes, appAttributes>(new userAttributes(){}, new appAttributes(){});
        }

        private void ApplyRemoteSettings(ConfigResponse response)
        {
            if(null == this.m_remoteConfigData)
            {
                this.m_remoteConfigData = new RemoteConfigData();
            }
            this.UpdateLoadingScreen("Fetched Assigning RC!", 1.0f);
            Debug.Log( RemoteConfigService.Instance.appConfig.GetString("AndroidLink"));
            Debug.Log( RemoteConfigService.Instance.appConfig.GetString("IOSLink"));
            this.m_remoteConfigData.androidAssetBundleLink = RemoteConfigService.Instance.appConfig.GetString("AndroidLink");
            this.m_remoteConfigData.IOSAssetBundleLink = RemoteConfigService.Instance.appConfig.GetString("IOSLink");
            this.UpdateLoadingScreen("Fire Event To Handler.", 0.0f);
            AssetsBundleHandler.GetInstance().OnRemoteConfigCompleted(new EventRemoteConfigCompletedAssignData());
            //KC_Custom.EventManager.AddEvent(new EventRemoteConfigCompletedAssignData());
        }

        private void UpdateLoadingScreen(string text, float progress)
        {
            if (AssetsBundlerLoadingScreen.GetInstance() == null) return;

            AssetsBundlerLoadingScreen.GetInstance().UpdateLoading(text, progress);
        }
        //===============================================================

    }
}