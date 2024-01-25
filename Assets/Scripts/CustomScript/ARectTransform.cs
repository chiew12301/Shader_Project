using UnityEngine;
using UnityEngine.UI;

namespace KC_Custom
{
    [RequireComponent( typeof( RectTransform ) )]
    public abstract class ARectTransform : MonoBehaviour
	{
        private RectTransform m_rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if ( null == this.m_rectTransform )
                    this.m_rectTransform = transform as RectTransform;
                return this.m_rectTransform;
            }
        }

        public Rect rect
        {
            get
            {
                return this.rectTransform.rect;
            }
        }
        
        private Canvas m_canvas;
        public Canvas canvas
        {
            get
            {
                if ( null == this.m_canvas )
                    this.m_canvas = this.GetComponent<Canvas>();

                if ( null == this.m_canvas )
                {
                    this.m_canvas = this.gameObject.AddComponent<Canvas>();
                    this.gameObject.AddComponent<GraphicRaycaster>();
                }

                return this.m_canvas;
            }
        }

        public int sortingOrder
        {
            set
            {
                if ( null == this.canvas )
                    return;

                this.canvas.overrideSorting = true;
                this.canvas.sortingOrder = value;
            }
        }

        public bool overrideSorting
        {
            set
            {
                if( null == this.canvas )
                    return;

                this.canvas.overrideSorting = false;
            }
        }

        //--------------------------------------------------------------------------------------------------------

        #region MonoBehaviour

        virtual protected void Awake()      {}

        virtual protected void OnEnable()   {}
        virtual protected void Start()      {}

        virtual protected void Update()     {}
        virtual protected void OnDisable()  {}
        virtual protected void OnDestroy()  {}

        virtual protected void OnApplicationQuit() {}
        virtual protected void OnApplicationPause( bool isPaused ) {}

		#endregion MonoBehaviour
	}
}