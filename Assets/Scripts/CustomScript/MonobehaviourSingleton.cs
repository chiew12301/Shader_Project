using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace KC_Custom
{
    public class MonobehaviourSingleton<T> : MonoBehaviour where T : MonobehaviourSingleton<T>
    {
        private static T _INSTANCE;
        private static object _PADLOCK = new object();

        public static T GetInstance(bool createNewIfNull = false, bool debug = false, bool dontDestroy = false)
        {
            lock(_PADLOCK )
            {
                if (applicationIsQuitting) return null;

                if (_INSTANCE == null)
                {
                    _INSTANCE = FindInstancesInScene();

                    if(null == _INSTANCE)
                    {
                        if(createNewIfNull)
                        {
                            GameObject newSingleton = new GameObject();
                            newSingleton.AddComponent<T>();
                            _INSTANCE = newSingleton.GetComponent<T>();
                            _INSTANCE.gameObject.name = newSingleton.GetComponent<T>().GetType().ToString();

                            if (dontDestroy)
                            {
                                DontDestroyOnLoad(_INSTANCE);
                            }
                            return _INSTANCE;
                        }
                        else
                        {
                            if(debug)
                            {
                                Debug.LogError("No Singleton is created, could not find any monobehavioursingleton object in the hierarchy");
                            }
                        }
                    }
                }

                FindInstancesInScene();
                return _INSTANCE;
            }
        }

        private static T FindInstancesInScene()
        {
            T[] instancesInScene = FindObjectsOfType<T>();

            if (instancesInScene.Length > 1)
                Debug.LogWarning("[Singleton WARNING] More than 1 SingletonMonoBehaviour of type : '" + typeof(T).ToString() + "' in the scene.");

            if (instancesInScene.Length != 0)
                return instancesInScene[0];

            return null;
        }


        private static bool applicationIsQuitting = false;

        //=======================================================

        #region MONOBEHAVIOUR

        protected virtual void OnDestroy()
        {
            _INSTANCE = null;
        }

        protected virtual void OnApplicationPause(bool pause) { }

        protected virtual void OnApplicationFocus(bool focus) { }

        protected virtual void OnApplicationQuit() { }

        protected virtual void Awake() { }

        protected virtual void Start() { }

        protected virtual void Update() { }

        protected virtual void LateUpdate() { }

        protected virtual void FixedUpdate() { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        #endregion MONOBEHAVIOUR

        //=======================================================
    }

}