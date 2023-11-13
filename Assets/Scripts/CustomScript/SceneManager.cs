using UnityEngine;
using UnityEngine.Internal;
using SceneManagement = UnityEngine.SceneManagement;
using UnityEvent = UnityEngine.Events;

namespace KC_Custom
{
    /// <summary>
    /// a wrapper class for UnityEngine.SceneManagement.SceneManager
    /// All users are recommended to use this wrapper class instead of directly using the Unity SceneManager
    /// 
    /// In Unity SceneManager, we cannot find an event that could perform "before LoadScene notification"
    /// Hence, we created this wrapper to do so.
    /// </summary>
    public sealed class SceneManager
    {
        //=======================================================

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitBeforeSceneLoad()
        {
            SceneManagement.SceneManager.sceneLoaded += sceneLoaded;
            SceneManagement.SceneManager.sceneUnloaded += sceneUnloaded;
            SceneManagement.SceneManager.activeSceneChanged += activeSceneChanged;
        }

        //=======================================================

        public static int sceneCountInBuildSettings { get { return SceneManagement.SceneManager.sceneCountInBuildSettings; } }
        public static int sceneCount { get { return SceneManagement.SceneManager.sceneCount; } }

        public static event UnityEvent.UnityAction<SceneManagement.LoadSceneMode> beforeLoadScene = (mode) => { }; // this is a custom event

        public static event UnityEvent.UnityAction<SceneManagement.Scene, SceneManagement.LoadSceneMode> sceneLoaded = (scene, mode) => { };
        public static event UnityEvent.UnityAction<SceneManagement.Scene> sceneUnloaded = (scene) => { };
        public static event UnityEvent.UnityAction<SceneManagement.Scene, SceneManagement.Scene> activeSceneChanged = (scene, currentScene) => { };

        //=======================================================

        public static SceneManagement.Scene CreateScene(string sceneName, SceneManagement.CreateSceneParameters parameters)
        {
            return SceneManagement.SceneManager.CreateScene(sceneName, parameters);
        }

        public static SceneManagement.Scene CreateScene(string sceneName)
        {
            return SceneManagement.SceneManager.CreateScene(sceneName);
        }

        public static SceneManagement.Scene GetActiveScene()
        {
            return SceneManagement.SceneManager.GetActiveScene();
        }

        public static SceneManagement.Scene GetSceneAt(int index)
        {
            return SceneManagement.SceneManager.GetSceneAt(index);
        }

        public static SceneManagement.Scene GetSceneByBuildIndex(int buildIndex)
        {
            return SceneManagement.SceneManager.GetSceneByBuildIndex(buildIndex);
        }

        public static SceneManagement.Scene GetSceneByName(string name)
        {
            return SceneManagement.SceneManager.GetSceneByName(name);
        }

        public static SceneManagement.Scene GetSceneByPath(string scenePath)
        {
            return SceneManagement.SceneManager.GetSceneByPath(scenePath);
        }

        //========================================================

        #region load scene

        public static void LoadScene(string sceneName)
        {
            beforeLoadScene(SceneManagement.LoadSceneMode.Single);
            SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] SceneManagement.LoadSceneMode mode)
        {
            beforeLoadScene(mode);
            SceneManagement.SceneManager.LoadScene(sceneName, mode);
        }

        public static SceneManagement.Scene LoadScene(string sceneName, SceneManagement.LoadSceneParameters parameters)
        {
            beforeLoadScene(SceneManagement.LoadSceneMode.Single);
            return SceneManagement.SceneManager.LoadScene(sceneName, parameters);
        }

        public static void LoadScene(int sceneBuildIndex)
        {
            beforeLoadScene(SceneManagement.LoadSceneMode.Single);
            SceneManagement.SceneManager.LoadScene(sceneBuildIndex);
        }

        public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] SceneManagement.LoadSceneMode mode)
        {
            beforeLoadScene(mode);
            SceneManagement.SceneManager.LoadScene(sceneBuildIndex, mode);

        }

        public static SceneManagement.Scene LoadScene(int sceneBuildIndex, SceneManagement.LoadSceneParameters parameters)
        {
            beforeLoadScene(SceneManagement.LoadSceneMode.Single);
            return SceneManagement.SceneManager.LoadScene(sceneBuildIndex, parameters);
        }

        #endregion load scene

        //=======================================================


        #region load scene async

        public static AsyncOperation LoadSceneAsync(string sceneName)
        {
            beforeLoadScene(SceneManagement.LoadSceneMode.Single);
            return SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] SceneManagement.LoadSceneMode mode)
        {
            beforeLoadScene(mode);
            return SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName, SceneManagement.LoadSceneParameters parameters)
        {
            beforeLoadScene(SceneManagement.LoadSceneMode.Single);
            return SceneManagement.SceneManager.LoadSceneAsync(sceneName, parameters);
        }

        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
        {
            beforeLoadScene(SceneManagement.LoadSceneMode.Single);
            return SceneManagement.SceneManager.LoadSceneAsync(sceneBuildIndex);
        }

        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] SceneManagement.LoadSceneMode mode)
        {
            beforeLoadScene(mode);
            return SceneManagement.SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
        }

        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, SceneManagement.LoadSceneParameters parameters)
        {
            beforeLoadScene(SceneManagement.LoadSceneMode.Single);
            return SceneManagement.SceneManager.LoadSceneAsync(sceneBuildIndex, parameters);
        }

        #endregion load scene async

        //=======================================================

        #region other

        public static void MergeScenes(SceneManagement.Scene sourceScene, SceneManagement.Scene destinationScene)
        {
            SceneManagement.SceneManager.MergeScenes(sourceScene, destinationScene);
        }

        public static void MoveGameObjectToScene(GameObject go, SceneManagement.Scene scene)
        {
            SceneManagement.SceneManager.MoveGameObjectToScene(go, scene);
        }

        public static bool SetActiveScene(SceneManagement.Scene scene)
        {
            return SceneManagement.SceneManager.SetActiveScene(scene);
        }

        public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
        {
            return SceneManagement.SceneManager.UnloadSceneAsync(sceneBuildIndex);
        }

        public static AsyncOperation UnloadSceneAsync(string sceneName)
        {
            return SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }

        public static AsyncOperation UnloadSceneAsync(SceneManagement.Scene scene)
        {
            return SceneManagement.SceneManager.UnloadSceneAsync(scene);
        }

        public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex, SceneManagement.UnloadSceneOptions options)
        {
            return SceneManagement.SceneManager.UnloadSceneAsync(sceneBuildIndex, options);
        }

        public static AsyncOperation UnloadSceneAsync(string sceneName, SceneManagement.UnloadSceneOptions options)
        {
            return SceneManagement.SceneManager.UnloadSceneAsync(sceneName, options);
        }

        public static AsyncOperation UnloadSceneAsync(SceneManagement.Scene scene, SceneManagement.UnloadSceneOptions options)
        {
            return SceneManagement.SceneManager.UnloadSceneAsync(scene, options);
        }

        #endregion other

        //=======================================================


    }

}