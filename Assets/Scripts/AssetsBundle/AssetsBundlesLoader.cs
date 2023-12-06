using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

using KC_Custom;

namespace ASSETSBUNDLE
{
    public class AssetsBundlesLoader : MonobehaviourSingleton<AssetsBundlesLoader>
    {
        [Header("Remote")]
        [SerializeField] private string m_bundleURL = "";

        [Header("Local")]
        [SerializeField] private string m_bundleName = "agmobundle";

        [Header("Object in Bundle")]
        [SerializeField] private string m_assetName = "pikachu";

        private Coroutine m_asLoaderLocalCO = null;
        private Coroutine m_asLoaderRemoteCO = null;

        //===============================================================

        //===============================================================

        [ContextMenu("Test Load Local")]
        public void StartLoadAssetsFromBundle()
        {
            if (this.m_asLoaderLocalCO != null)
            {
                this.StopCoroutine(this.m_asLoaderLocalCO);
                this.m_asLoaderLocalCO = null;
            }

            this.m_asLoaderLocalCO = this.StartCoroutine(this.LocalLoadAssetsFromBundle());
        }

        [ContextMenu("Test Load Online")]
        public void StartLoadAssetsFromBundleOnline()
        {
            if (this.m_asLoaderRemoteCO != null)
            {
                this.StopCoroutine(this.m_asLoaderRemoteCO);
                this.m_asLoaderRemoteCO = null;
            }
// #if UNITY_ANDROID
//             this.m_bundleURL = RemoteConfigHandler.GetInstance().GetRemoteConfigData().androidAssetBundleLink;
// #endif

//             this.m_assetName = RemoteConfigHandler.GetInstance().GetRemoteConfigData().assetsName;
            this.m_asLoaderRemoteCO = this.StartCoroutine(this.RemoteLoadAssetsFromBundle(this.m_assetName));
        }

        /// <summary>
        /// Be aware that the method is not check couroutine, it can be called multiple times!
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="bundleURL">Bundle URL, default will retrieve from remote config</param>
        public void LoadSpecificAssetsFromBundleOnline(string name, string bundleURL = null)
        {
            if(bundleURL == null)
            {
                //Set Default
#if UNITY_ANDROID
                this.m_bundleURL = RemoteConfigHandler.GetInstance().GetRemoteConfigData().androidAssetBundleLink;
#endif
            }

            this.m_assetName = name;
            this.StartCoroutine(this.RemoteLoadAssetsFromBundle(this.m_assetName));
        }

        private IEnumerator LocalLoadAssetsFromBundle()
        {
            AssetBundleCreateRequest asyncBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, this.m_bundleName));
            AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Getting Bundle from Local", asyncBundleRequest.progress);
            yield return asyncBundleRequest;

            AssetBundle localAssetBundle = asyncBundleRequest.assetBundle;

            if (localAssetBundle == null)
            {
                Debug.LogError("Failed To Load Local " + this.m_bundleName + " performing break loading.");
                AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Failed Load Bundle", 1.0f);
                yield break;
            }

            AssetBundleRequest assetRequest = localAssetBundle.LoadAssetAsync<GameObject>(this.m_assetName);
            AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Extracting Bundle Object", assetRequest.progress);
            yield return assetRequest;

            GameObject prefab = assetRequest.asset as GameObject;
            //EventManager.AddEvent(new EventAssetsBundleObjectLoaded(prefab));
            AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Compeleted Loaded", 1.0f);
            localAssetBundle.Unload(false);
        }

        private IEnumerator RemoteLoadAssetsFromBundle(string assetsName)
        {
            using (UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(this.m_bundleURL))
            {
                AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Loading Bundle Remote", req.downloadProgress);

                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(req.error);
                    AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Failed load bundle remotely.", 1.0f);
                }
                else
                {
                    AssetBundle remoteAssetBundle = DownloadHandlerAssetBundle.GetContent(req);

                    if (remoteAssetBundle == null)
                    {
                        Debug.LogWarning("Failed To Load Remote " + this.m_bundleName + " performing break loading/");
                        AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Loaded Bundle, but assets is empty.", 1.0f);
                        yield break;
                    }
                    AssetBundleRequest assetReq = remoteAssetBundle.LoadAssetAsync<GameObject>(assetsName);
                    AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Extracting Bundle Object.", assetReq.progress);
                    yield return assetReq;

                    if(assetReq.asset != null)
                    {
                        GameObject prefab = assetReq.asset as GameObject;
                        //EventManager.AddEvent(new EventAssetsBundleObjectLoaded(prefab));
                        Instantiate(prefab);
                        AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Spawned Object!", 1.0f);
                    }
                    else
                    {
                        Debug.LogWarning("Assets Not Found.");
                        AssetsBundlerLoadingScreen.GetInstance().UpdateLoading("Object Not Found!", 1.0f);
                    }
                    remoteAssetBundle.Unload(false);
                }
                req.Dispose();
            }
        }

        //===============================================================
    }
}