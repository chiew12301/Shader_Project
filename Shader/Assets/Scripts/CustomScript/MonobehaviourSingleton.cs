using UnityEngine;

public class MonobehaviourSingleton<T> : MonoBehaviour where T : MonobehaviourSingleton<T>
{
    private static T _INSTANCE;
    //public static T INSTANCE
    //{
    //    get
    //    {
    //        return GetInstance(true, false);
    //    }
    //    private set { }
    //}

    public static T GetInstance(bool createNewIfNull = false, bool debug = false, bool dontDestroy = false)
    {
        if (applicationIsQuitting) return null;

        if (_INSTANCE == null)
        {
            T[] assetsCreated = FindObjectsOfType<T>();
            if (assetsCreated == null || assetsCreated.Length <= 0)
            {
                if(debug)
                {
                    throw new System.Exception("No Singleton is created, could not find any monobehavioursingleton object in the hierarchy");
                }

                if (createNewIfNull)
                {
                    GameObject newSingleton = new GameObject();
                    newSingleton.AddComponent<T>();
                    _INSTANCE = newSingleton.GetComponent<T>();
                    _INSTANCE.gameObject.name = newSingleton.GetComponent<T>().GetType().ToString();

                    if(dontDestroy)
                    {
                        DontDestroyOnLoad(_INSTANCE);
                    }
                    return _INSTANCE;
                }
                return null;
            }
            else if (assetsCreated.Length > 1)
            {
                Debug.LogWarning("Multiple instances of monobehavioursingleton object found in the hierarchy");
            }
            _INSTANCE = assetsCreated[0];
            if (dontDestroy)
            {
                DontDestroyOnLoad(_INSTANCE);
            }
        }
        return _INSTANCE;
    }

    private static bool applicationIsQuitting = false;

    //=======================================================

    #region MONOBEHAVIOUR

    protected void OnDestroy()
    {
        //Prevent Recreating During Scene Unload
        applicationIsQuitting = true;
    }

    protected virtual void OnApplicationPause(bool pause) { }

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
