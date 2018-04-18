using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class ZoomPreview : MonoBehaviour
    {
        /// <summary>
        /// UI Slider used for adjusting zoom value.
        /// </summary>
        [Tooltip("UI Slider used for adjusting zoom value")]
        public Slider zoomSlider;

        /// <summary>
        /// How much zoom value will be incresed/decreased.
        /// </summary>
        [Tooltip("How much zoom value will be increased/decreased")]
        public float zoomIncrement = 0.5f;

        /// <summary>
        /// Preview camera managed by this ZoomPreview.
        /// </summary>
        [Tooltip("Preview camera managed by this ZoomPreview")]
        public Camera previewCam;

        /// <summary>
        /// Minimum value of previewCam's orthographicSize. 
        /// </summary>
        [Tooltip("Minimum value of previewCam's orthographicSize")]
        public float minCamSize;

        /// <summary>
        /// Maximum value of previewCam's orthographicSize.
        /// </summary>
        [Tooltip("Maximum value of previewCam's orthographicSize")]
        public float maxCamSize;

        /// <summary>
        /// Maximum Y position of previewCam.
        /// </summary>
        [Tooltip("Maximum Y position of previewCam")]
        public float minY;

        /// <summary>
        /// Maximum X position of previewCam.
        /// </summary>
        [Tooltip("Maximum X position of previewCam")]
        public float maxY;

        private float _defcamsize;
        
        void Start()
        {
            zoomSlider.minValue = minCamSize;
            zoomSlider.maxValue = maxCamSize;
            zoomSlider.value = _defcamsize = previewCam.orthographicSize;
            UpdateZoom();
        }

        /// <summary>
        /// Update previewCam's zoom value.
        /// </summary>
        public void UpdateZoom()
        {
            previewCam.orthographicSize = zoomSlider.value;
            Vector3 newPos = previewCam.transform.localPosition;
            if (zoomSlider.normalizedValue < 0.5f)
                newPos.y = Mathf.Lerp(maxY, minY, zoomSlider.normalizedValue * 2);
            else
                newPos.y = minY;
            previewCam.transform.localPosition = newPos;
        }

        /// <summary>
        /// Zoom in previewCam.
        /// </summary>
        public void ZoomIn()
        {
            zoomSlider.value -= zoomIncrement;
            UpdateZoom();
        }

        /// <summary>
        /// Zoom out previewCam.
        /// </summary>
        public void ZoomOut()
        {
            zoomSlider.value += zoomIncrement;
            UpdateZoom();
        }

        /// <summary>
        /// Reset previewCam's zoom value to defaultCamSize.
        /// </summary>
        public void Reset()
        {
            zoomSlider.value = _defcamsize;
            UpdateZoom();
        }
    }
}