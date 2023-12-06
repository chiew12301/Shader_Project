using System.Collections;
using System.Collections.Generic;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;

using KC_Custom;

namespace ASSETSBUNDLE
{
    public class AssetsBundleHandler : MonobehaviourSingleton<AssetsBundleHandler>
    {
        [Header("Will be overwrite")]
        [SerializeField] private string m_bundleURL = "";
        
        [Header("Bundles Link")]
        [SerializeField] private string m_androidBundle = "https://firebasestorage.googleapis.com/v0/b/hk-asset-bundles.appspot.com/o/Android%2Fhkpdassetbundle?alt=media&token=da989ce7-55bd-4278-baa6-7a72e0c0f3e4";
        [SerializeField] private string m_iosBundle = "https://firebasestorage.googleapis.com/v0/b/hk-asset-bundles.appspot.com/o/IOS%2Fhkpdassetbundle?alt=media&token=3f08b14f-8671-4d04-90a1-cc2fd0e30aa7";

        private AssetBundle m_downloadedBundle = null;

        private Coroutine m_downloadBundleCO = null;
        private Coroutine m_loadsGameObjectCO = null;
        private Coroutine m_loadsAllGOCO = null;

        private List<GameObject> m_objectsLoadedFromBundlelist = new List<GameObject>();
        private string[] m_allScenesFromBundle;

        //================================================

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitBeforeSceneLoad()
        {
            AssetsBundleHandler obj = new GameObject("AssetsBundleHandler").AddComponent<AssetsBundleHandler>();
            DontDestroyOnLoad(obj.gameObject);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            //this.FetchNow();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            KC_Custom.EventManager.AddListener<EventRemoteConfigCompletedAssignData>(this.OnRemoteConfigCompleted);
        }

        protected override void OnDisable()
        {
            KC_Custom.EventManager.RemoveListener<EventRemoteConfigCompletedAssignData>(this.OnRemoteConfigCompleted);
            if (this.m_downloadedBundle != null)
            {
                this.m_objectsLoadedFromBundlelist.Clear();
                this.m_downloadedBundle.Unload(true);
                this.m_downloadedBundle = null;
            }
            base.OnDisable();
        }

        protected override void OnApplicationQuit()
        {
            if (this.m_downloadedBundle != null)
            {
                this.m_objectsLoadedFromBundlelist.Clear();
                this.m_downloadedBundle.Unload(true);
                this.m_downloadedBundle = null;
            }
            base.OnApplicationQuit();
        }
        //================================================

        public List<GameObject> GetLoadedObjects() => this.m_objectsLoadedFromBundlelist;
        public string[] GetSceneArray() => this.m_allScenesFromBundle;

        /// <summary>
        /// This method is to load specific game object. Gameobject will return base on event.
        /// Please add listener to retrieve the object.
        /// </summary>
        /// <param name="assetsName">The asset name</param>
        /// <returns></returns>
        public bool LoadSpecificGameObjectFromAssetsBundle(string assetsName)
        {
            if (this.m_downloadedBundle == null) return false;

            if(null != this.m_loadsGameObjectCO)
            {
                this.StopCoroutine(this.m_loadsGameObjectCO);
                this.m_loadsGameObjectCO = null;
            }

            this.m_loadsGameObjectCO = this.StartCoroutine(this.LoadGameObjectFromBundle(assetsName));
            return true;
        }

        public void BNMPOCLoad()
        {
#if UNITY_ANDROID
            this.m_bundleURL = this.m_androidBundle;
#elif UNITY_IOS 
            this.m_bundleURL = this.m_iosBundle;
#endif
            this.UpdateLoadingScreen("Starting Load Bundle.", 0.0f);

            if (null != this.m_downloadBundleCO)
            {
                this.StopCoroutine(this.m_downloadBundleCO);
                this.m_downloadBundleCO = null;
            }
            this.m_downloadBundleCO = this.StartCoroutine(this.LoadAssetsBundle());
        }

        public void FetchNow()
        {
            if(this.m_downloadedBundle != null) return;
            this.BNMPOCLoad();
        }

        public void OnRemoteConfigCompleted(EventRemoteConfigCompletedAssignData evt)
        {
            //this.m_bundleURL = RemoteConfigHandler.GetInstance().GetRemoteConfigData().androidAssetBundleLink;
            this.UpdateLoadingScreen("Loading Bundle.", 0.0f);
            this.m_androidBundle = RemoteConfigHandler.GetInstance().GetRemoteConfigData().androidAssetBundleLink;
            this.m_iosBundle = RemoteConfigHandler.GetInstance().GetRemoteConfigData().IOSAssetBundleLink;
            this.BNMPOCLoad();
        }

        private IEnumerator LoadAssetsBundle()
        {
            //KC_Custom.EventManager.AddEvent(new EventAssetsBundleStarted());

            using (UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(this.m_bundleURL, (uint)1, 0))
            {
                this.UpdateLoadingScreen("Starting Fetch Bundle Remote...", req.downloadProgress);
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(req.error);
                    this.UpdateLoadingScreen("Bundle download failed!", 1.0f);
                }
                else
                {
                    this.UpdateLoadingScreen("Bundle download Completed!", req.downloadProgress);
                    Debug.Log("Bundle Assigned");
                    this.m_downloadedBundle = DownloadHandlerAssetBundle.GetContent(req);
                }
                req.Dispose();
            }
            //KC_Custom.EventManager.AddEvent(new EventAssetsBundleCompleted());
            if(this.m_loadsAllGOCO != null)
            {
                this.StopCoroutine(this.m_loadsAllGOCO);
                this.m_loadsAllGOCO = null;
            }
            this.m_loadsAllGOCO = this.StartCoroutine(this.LoadAllGOFromBundle());
        }

        private IEnumerator LoadGameObjectFromBundle(string assetsName)
        {
            if (this.m_downloadedBundle == null) { Debug.LogWarning("Bundle Not Downloaded/Exist!"); yield break; }

            AssetBundleRequest assetReq = this.m_downloadedBundle.LoadAssetAsync<GameObject>(assetsName);
            this.UpdateLoadingScreen("Extracting Bundle Object!", assetReq.progress);
            yield return assetReq;

            if (assetReq.asset == null) { Debug.LogWarning("Assets Not Exist!"); yield break; }

            this.UpdateLoadingScreen("Assets Loaded!", assetReq.progress);
            GameObject assetsFromBundle = assetReq.asset as GameObject;
            if(!this.m_objectsLoadedFromBundlelist.Contains(assetsFromBundle))
            {
                this.m_objectsLoadedFromBundlelist.Add(assetsFromBundle);
            }
            KC_Custom.EventManager.AddEvent(new EventAssetsBundleAssetsLoaded());
        }

        private IEnumerator LoadAllGOFromBundle()
        {
            if (this.m_downloadedBundle == null) { Debug.LogWarning("Bundle Not Downloaded/Exist!"); yield break; }

            AssetBundleRequest bundleReq = this.m_downloadedBundle.LoadAllAssetsAsync<GameObject>();

            this.UpdateLoadingScreen("Extracting all objects!", bundleReq.progress); ;
            yield return bundleReq;

            if (bundleReq == null) { Debug.LogWarning("Assets unable to request!"); KC_Custom.EventManager.AddEvent(new EventAssetsBundleAssetsLoaded()); yield break; }
            this.UpdateLoadingScreen("Assets loaded!", bundleReq.progress);
            List<Object> assets = bundleReq.allAssets.ToList();

            foreach (Object obj in assets)
            {
                GameObject converttogo = obj as GameObject;
                if(converttogo != null)
                {
                    this.m_objectsLoadedFromBundlelist.Add(converttogo);
                    Debug.Log("Converted");
                }
            }
            KC_Custom.EventManager.AddEvent(new EventAssetsBundleAssetsLoaded());
        }

        private void UpdateLoadingScreen(string text, float progress)
        {
            if (AssetsBundlerLoadingScreen.GetInstance() == null) return;

            AssetsBundlerLoadingScreen.GetInstance().UpdateLoading(text, progress);
        }

        [ContextMenu("Clear Cache")]
        private void ClearCache()
        {
            if(Caching.ClearCache())
            {
                Debug.Log("Cleared Cache");
            }
            else{
                Debug.Log("Cache is begin used");
            }
        }

        //================================================

    }
}