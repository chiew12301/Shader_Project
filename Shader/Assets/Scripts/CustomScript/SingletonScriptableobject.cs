using UnityEngine;

public class SingletonScriptableobject<T> : ScriptableObject where T : SingletonScriptableobject<T>
{
    private static T _INSTANCE;
    //public static T INSTANCE
    //{
    //    get
    //    {
    //        return GetInstance();
    //    }
    //    private set { }
    //}

    public static T GetInstance(bool debug = false)
    {
        if (_INSTANCE == null)
        {
            T[] assetsCreated = Resources.LoadAll<T>("");
            if (assetsCreated == null || assetsCreated.Length <= 0)
            {
                if(debug)
                {
                    throw new System.Exception("No Singleton is created, could not find any scriptable object in the resources");
                }
                return null;
            }
            else if (assetsCreated.Length > 1)
            {
                Debug.LogWarning("Multiple instances of singleton scriptable object found in the resources");
            }
            _INSTANCE = assetsCreated[0];
        }
        return _INSTANCE;
    }
}
