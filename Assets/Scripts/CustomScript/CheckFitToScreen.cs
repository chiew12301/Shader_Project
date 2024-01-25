using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KC_Custom
{
    [ExecuteInEditMode]
    [RequireComponent( typeof( Canvas ), typeof(CanvasScaler))]
    public class CheckFitToScreen : ARectTransform
    {
        private enum EFACTOR
        {
            SIZE_SAME,
            SIZE_DIFFERENT,
        }

        private CanvasScaler m_canvasScaler;
        public CanvasScaler canvasScaler
        {
            get
            {
                if ( null == this.m_canvasScaler )
                    this.m_canvasScaler = GetComponent<CanvasScaler>();
                return this.m_canvasScaler;
            }
        }

        private EFACTOR m_factor = EFACTOR.SIZE_SAME;
        private float m_currentFactor;

        protected override void Start()
        {
            base.Start();
            this.UpdateFitToScreen();
        }

        protected override void Update()
        {
            base.Update();

            if ( Application.isEditor )
                this.UpdateFitToScreen();
        }

        private void UpdateFitToScreen()
        {
            Vector2 originResolution = this.canvasScaler.referenceResolution;
            Vector2 canvasResolution = this.rectTransform.sizeDelta;

            float originFactor = originResolution.x / originResolution.y;
            float canvasFactor = canvasResolution.x / canvasResolution.y;

            if ( canvasFactor != originFactor )
            {
                float scaleAmount = ( canvasFactor > originFactor )
                    ? canvasResolution.y / originResolution.y
                    : canvasResolution.x / originResolution.x;
                
                FitToScreen[] fits = this.GetComponentsInChildren<FitToScreen>();

                for ( int i = 0; i < fits.Length; ++i )
                {
                    FitToScreen fit = fits[i];
                    if ( fit.updateOnEditorMode )
                        fit.rectTransform.localScale = new Vector3( scaleAmount, scaleAmount, scaleAmount );
                }

                this.m_factor = EFACTOR.SIZE_DIFFERENT;
            }

            else
            {
                if ( EFACTOR.SIZE_SAME != this.m_factor )
                {
                    FitToScreen[] fits = this.GetComponentsInChildren<FitToScreen>();
                    for ( int i = 0; i < fits.Length; ++i )
                    {
                        FitToScreen fit = fits[i];
                        if ( fit.updateOnEditorMode )
                            fit.rectTransform.localScale = Vector3.one;
                    }

                    this.m_factor = EFACTOR.SIZE_SAME;
                }

            }
        }
    }
}