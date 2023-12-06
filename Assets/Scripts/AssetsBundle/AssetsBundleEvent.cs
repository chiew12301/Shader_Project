using UnityEngine;
using KC_Custom;

namespace ASSETSBUNDLE
{
    public class EventAssetsBundleObjectLoaded : KC_Custom.AnEvent
    {
        public GameObject loadedObject;

        public EventAssetsBundleObjectLoaded(GameObject obj)
        {
            loadedObject = obj;
        }
    }

    public class EventOnSceneLoaded : KC_Custom.AnEvent { public EventOnSceneLoaded() { } }

    //For assetsbundle
    public class EventAssetsBundleCompleted : KC_Custom.AnEvent{public EventAssetsBundleCompleted() { }}

    public class EventAssetsBundleStarted : KC_Custom.AnEvent{public EventAssetsBundleStarted() { }}

    public class EventAssetsBundleAssetsLoaded : KC_Custom.AnEvent { public EventAssetsBundleAssetsLoaded() { } }

    public class EventRemoteConfigCompletedAssignData : KC_Custom.AnEvent {public EventRemoteConfigCompletedAssignData(){}}
}