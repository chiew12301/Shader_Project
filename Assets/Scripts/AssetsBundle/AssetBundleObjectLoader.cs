using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ASSETSBUNDLE
{
    public class AssetBundleObjectLoader : MonoBehaviour
    {
        [Tooltip("This is the prefab name that packed.")]
        [SerializeField] private string m_assetsName = "";

        private GameObject m_scenePrefab = null;
        private GameObject m_spawnedObj = null;

        //================================================

        private void OnEnable()
        {
            KC_Custom.EventManager.AddListener<ASSETSBUNDLE.EventOnSceneLoaded>(this.StartSpawnPrefab);
        }

        private void OnDisable()
        {
            KC_Custom.EventManager.RemoveListener<ASSETSBUNDLE.EventOnSceneLoaded>(this.StartSpawnPrefab);
            if(this.m_spawnedObj != null)
            {
                Destroy(this.m_spawnedObj);
            }
        }

        //================================================
        
        private void StartSpawnPrefab(ASSETSBUNDLE.EventOnSceneLoaded evt)
        {
            this.OnStartLoaded();
        }

        private void OnStartLoaded()
        {
            foreach(GameObject go in ASSETSBUNDLE.AssetsBundleHandler.GetInstance().GetLoadedObjects())
            {
                if(this.m_assetsName == go.name)
                {
                    this.m_scenePrefab = go;
                    break;
                }
            }

            this.m_spawnedObj = Instantiate(this.m_scenePrefab);
        }

        public void OnRestartCurrentScene()
        {
            Destroy(m_spawnedObj);

            OnStartLoaded();
        }

        //================================================
    }
}
