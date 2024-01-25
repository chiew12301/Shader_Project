using System;
using System.Collections.Generic;
using UnityEngine;

namespace KC_Custom
{
    public class MenuHandler : MonobehaviourSingleton<MenuHandler>
    {
        private const string GROUP_MENU_NAME = "Group_Menu";
        private const string MAIN_CANVAS_NAME = "MainCanvas";

        private SO_Menu m_soMenu;

        private Queue<Action> m_pendingMenu = new Queue<Action>();
        private Stack<AMenu> m_menuStack = new Stack<AMenu>();
        private Transform m_groupMenuTransform;
        public Transform groupMenuTransform
        {
            get
            {
                if ( null == this.m_groupMenuTransform )
                {
                    this.InitGroupMenu();
                }

                return this.m_groupMenuTransform;
            }
        }


        public bool hasPendingMenus { get { return this.m_pendingMenu.Count > 0; } }

        public bool isTopMenuClosing { get; set; }

        public int menuCount { get { return this.m_menuStack.Count; } }

        // =================================================================================================================

        #region Singleton Init

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void InitBeforeSplashScreen()
        {
            MenuHandler obj = new GameObject("MenuHandler").AddComponent<MenuHandler>();
            DontDestroyOnLoad(obj.gameObject);
        }

        #endregion Singleton Init

        // =================================================================================================================

        // ======================================================================================================

        #region Monobehavior

        protected override void Awake()
        {
            base.Awake();
            this.InitMenuData();
        }

        protected override void Update()
        {
            base.Update();

            if( this.m_menuStack.Count > 0 && this.OnBackPressedDown() )
            {
                this.m_menuStack.Peek().OnBackPressed();
            }
        }

        protected override void OnDestroy()
        {
            this.m_menuStack.Clear();
            this.m_menuStack = null;

            this.m_pendingMenu.Clear();
            this.m_pendingMenu = null;

            base.OnDestroy();
        }

        protected virtual bool OnBackPressedDown()
        {
            return Input.GetKeyDown( KeyCode.Escape );
        }

        #endregion

        // ======================================================================================================

        private void InitMenuData()
        {
            this.m_soMenu = SO_Menu.GetInstance();
        }

        private void InitGroupMenu()
        {
            Canvas[] allCanvasAvailable = FindObjectsOfType<Canvas>();           
            Canvas mainCanvas = null;

            if(null != allCanvasAvailable)
            {
                for(int i = 0; i < allCanvasAvailable.Length; ++i)
                {
                    if(allCanvasAvailable[i].name == MAIN_CANVAS_NAME)
                    {
                        mainCanvas = allCanvasAvailable[i];
                        break;
                    }
                }
            }

            this.m_groupMenuTransform = new GameObject( GROUP_MENU_NAME, typeof(RectTransform) ).transform;

            if(null == mainCanvas)
            {
                GameObject generatedNewMainCanvas = new GameObject();    
                generatedNewMainCanvas.layer = 5; //UI Layer
                mainCanvas = generatedNewMainCanvas.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                UnityEngine.UI.CanvasScaler cs = generatedNewMainCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                cs.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = new Vector2(1980.0f, 1080.0f);
                cs.matchWidthOrHeight = 0.5f;
                generatedNewMainCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();     
                mainCanvas.name = MAIN_CANVAS_NAME;

                var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
                if(null == eventSystem)
                {
                    eventSystem = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule)).GetComponent<UnityEngine.EventSystems.EventSystem>();
                }
            }

            this.m_groupMenuTransform.parent = mainCanvas.transform;
            this.groupMenuTransform.localScale = Vector3.one;
            RectTransform castRTFromGroupMenu = this.m_groupMenuTransform.GetComponent<RectTransform>();

            castRTFromGroupMenu.anchorMax = new Vector2(1.0f, 1.0f);
            castRTFromGroupMenu.anchorMin = new Vector2(0.0f, 0.0f);   
            castRTFromGroupMenu.offsetMax = new Vector2(0.0f, 0.0f);
            castRTFromGroupMenu.offsetMin = new Vector2(0.0f, 0.0f);
        }

        public T CreateInstance<T>() where T : AMenu
        {
            var prefab = this.m_soMenu.GetMenuPrefab<T>();
            return Instantiate(prefab, this.groupMenuTransform);
        }

        public void OpenMenu( AMenu instance )
        {
            // De-activate top menu
            if( this.m_menuStack.Count > 0 )
            {
                if( instance.DisableMenusUnderneath )
                {
                    foreach( AMenu menu in this.m_menuStack )
                    {
                        menu.gameObject.SetActive( false );

                        if( menu.DisableMenusUnderneath )
                            break;
                    }
                }

                Canvas topCanvas = instance.canvas;
                instance.EnableRaycaster( true );

                Canvas prevCanvas = this.m_menuStack.Peek().canvas;
                this.m_menuStack.Peek().EnableRaycaster( false );

                if( topCanvas != null && prevCanvas != null )
                    topCanvas.sortingOrder = prevCanvas.sortingOrder + 1;
            }
            this.m_menuStack.Push( instance );
        }

        public void CloseMenu( AMenu menu )
        {
            if( this.m_menuStack.Count == 0 )
            {
                Debug.LogError( string.Format( "{0} cannot be closed because menu stack is empty", menu.GetType() ) );
                return;
            }

            if( this.m_menuStack.Peek() != menu )
            {
                Debug.LogError( string.Format( "{0} cannot be closed because it is not on top of the stack", menu.GetType() ) );
                return;
            }

            this.ClosedTopMenu();
        }

        public AMenu GetTopMenu()
        {
            return this.m_menuStack.Peek();
        }

        private void ClosedTopMenu()
        {
            this.isTopMenuClosing = false;

            AMenu instance = this.m_menuStack.Pop();

            if( instance.DestroyWhenClosed )
                Destroy( instance.gameObject );
            else
            {
                instance.EnableRaycaster( false );
                instance.gameObject.SetActive( false );
            }

            if( this.m_pendingMenu.Count > 0 )
            {
                if ( this.m_pendingMenu.Dequeue() != null )
                {
                    this.m_pendingMenu.Dequeue().Invoke();
                }
                return;
            }

            // Re-activate top menu
            // If a re-activated menu is an overlay we need to activate the menu under it
            foreach( AMenu menu in this.m_menuStack )
            {
                menu.gameObject.SetActive( true );

                if( menu.DisableMenusUnderneath )
                    break;
            }

            // Only enable the raycast for the top menu
            if( this.m_menuStack.Count > 0 && this.m_menuStack.Peek() != null )
                this.m_menuStack.Peek().EnableRaycaster( true );
        }

        // ======================================================================================================

        public void AppendMenu( Action openMenuAction )
        {
            this.m_pendingMenu.Enqueue( openMenuAction );
        }

        // ======================================================================================================

    }
}