using Unity.VisualScripting;
using UnityEngine;

namespace KC_Custom
{
    public class SingletonScriptableobject<T> : ScriptableObject where T : SingletonScriptableobject<T>
    {
        private static T _INSTANCE;
        private static object _PADLOCK = new object();

        public static T GetInstance(bool debug = false)
        {
            lock(_PADLOCK)
            {
                if (_INSTANCE == null)
                {
                    _INSTANCE = FindInstancesInProject();

                    if (null == _INSTANCE)
                    {
                        if (debug)
                        {
                            Debug.LogError("No Singleton is created, could not find any monobehavioursingleton object in the hierarchy");
                        }
                    }
                }

                FindInstancesInProject();
                return _INSTANCE;
            }
        }

        private static T FindInstancesInProject()
        {
            T[] assetsCreated = Resources.LoadAll<T>("");

            if (assetsCreated.Length > 1)
                Debug.LogWarning("[Singleton WARNING] More than 1 SingletonScriptableobject of type : '" + typeof(T).ToString() + "' in the scene.");

            if (assetsCreated.Length != 0)
                return assetsCreated[0];

            return null;
        }
    }

}