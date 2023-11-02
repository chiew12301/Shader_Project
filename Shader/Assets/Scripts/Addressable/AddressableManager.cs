using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid) { }
}

public class AddressableManager : MonoBehaviour
{
    [SerializeField] private AssetReference m_AssetReference;

    [SerializeField] private AssetReferenceAudioClip m_musicReferences;

    [SerializeField] private AssetReferenceTexture2D m_textureReferences;

    [SerializeField] private RawImage m_image = null;

    private GameObject m_spawnedObject = null;

    private void Start()
    {
        Addressables.InitializeAsync().Completed += AddressableManagerCompleted;
    }

    private void AddressableManagerCompleted(AsyncOperationHandle<IResourceLocator> obj)
    {
        //Loading.GetInstance().UIStatus(true);

        this.m_AssetReference.LoadAssetAsync<GameObject>().Completed += (loadedAssetsOBJData) =>
        {
            Debug.Log("loading");
            //Loading.GetInstance().UpdateText("Loading Assets...");

            this.m_AssetReference.InstantiateAsync().Completed += (loadedAssetsOBJ) =>
            {
                Debug.Log("loaded");
                //Loading.GetInstance().UpdateText("Loaded and Created!");

                //instantiate object
                this.m_spawnedObject = loadedAssetsOBJ.Result;
            };
        };

        //Loading.GetInstance().UpdateText("Loading Music");

        this.m_musicReferences.LoadAssetAsync<AudioClip>().Completed += (clip) =>
        {
            var audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip.Result;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.Play();
            Debug.Log("Loaded Music");
            //Loading.GetInstance().UpdateText("Loaded Music");
        };

        //Loading.GetInstance().UpdateText("Loading texture");

        this.m_textureReferences.LoadAssetAsync<Texture2D>().Completed += (textureLoaded) =>
        {
            Debug.Log("Loaded Texture and assigning Texture");
            //Loading.GetInstance().UpdateText("Loaded Texture and assigning Texture");

            this.m_image.texture = this.m_textureReferences.Asset as Texture2D;
            Color currentColor = this.m_image.color;
            currentColor.a = 1.0f;
            this.m_image.color = currentColor;
        };

        //Loading.GetInstance().UIStatus(false);
    }

    private void ReleaseAddressable()
    {
        this.m_musicReferences.ReleaseAsset();
        this.m_AssetReference.ReleaseInstance(this.m_spawnedObject);
        this.m_textureReferences.ReleaseAsset();
    }

    private void OnDestroy()
    {
        this.ReleaseAddressable();
    }
}
