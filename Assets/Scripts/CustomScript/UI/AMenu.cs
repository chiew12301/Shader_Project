using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KC_Custom
{
    public abstract class AMenu : MonoBehaviour
    {
         [System.Serializable]
        private class MenuSettings
        {
            [Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")]
            public bool destroyWhenClosed;

            [Tooltip("Disable menus that are under this one in the stack")]
            public bool disableMenusUnderneath;

            public MenuSettings()
            {
                this.destroyWhenClosed = true;
                this.disableMenusUnderneath = false;
            }
        }

        // ===========================================================================================================================
        [SerializeField] private MenuSettings m_menuSettings;

        private Canvas m_canvas;
        public Canvas canvas
        {
            get
            {
                if( this.m_canvas == null )
                    this.m_canvas = this.GetComponent<Canvas>();
                return this.m_canvas;
            }
        }

        protected List<GraphicRaycaster> m_raycasterList;
        public virtual List<GraphicRaycaster> raycasterList
        {
            get
            {
                if( this.m_raycasterList == null )
                {
                    this.m_raycasterList = new List<GraphicRaycaster>();
                    this.m_raycasterList.AddRange( this.GetComponentsInChildren<GraphicRaycaster>().ToList() );
                }
                return this.m_raycasterList;
            }
        }

        public bool DestroyWhenClosed { get { return this.m_menuSettings.destroyWhenClosed; } }
        public bool DisableMenusUnderneath { get { return this.m_menuSettings.disableMenusUnderneath; } }

        public virtual void EnableRaycaster( bool enable )
        {
            if( this.raycasterList.Count > 0 )
            {
                int listCount = this.raycasterList.Count;
                for( int i = 0; i < listCount; i++ )
                {
                    if( this.m_raycasterList[i] != null )
                        this.raycasterList[i].enabled = enable;
                }
            }
        }
        
        protected virtual void OnOpenMenu() 
        {
        }

        protected virtual void OnCloseMenu() 
        {
        }

        public abstract void OnBackPressed();
    }

       // ===================================================================================================================================================================

    public abstract class AMenu<T> : AMenu where T : AMenu<T>
    {
        private static T _INSTANCE;
        public static T GetInstance() { return _INSTANCE; }

        // ==============================================================================

        protected virtual void Awake()
        {
            _INSTANCE = this as T;
        }

        protected virtual void OnDestroy()
        {
            _INSTANCE = null;
        }

        // ==============================================================================

        protected static void Open()
        {
            if( MenuHandler.GetInstance().isTopMenuClosing )
                return;

            if( _INSTANCE == null )
                MenuHandler.GetInstance().CreateInstance<T>();
            else
                _INSTANCE.gameObject.SetActive( true );

            MenuHandler.GetInstance().OpenMenu( _INSTANCE );
            _INSTANCE.OnOpenMenu();
        }

        protected static void Close()
        {
            if( _INSTANCE == null )
            {
                Debug.LogError( string.Format( "Trying to close menu {0} but Instance is null", typeof( T ) ) );
                return;
            }

            MenuHandler.GetInstance().CloseMenu( _INSTANCE );
            _INSTANCE.OnCloseMenu();
        }

        public static void Append()
        {
            MenuHandler.GetInstance().AppendMenu( Open );
        }

        public override void OnBackPressed()
        {
            Close();
        }

        // ==============================================================================
    }
}