using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UIBodyColor : MonoBehaviour
    {
        /// <summary>
        /// Current state of UIBodyColor.
        /// </summary>
        [Tooltip("Current state of UIBodyColor")]
        [ReadOnly]
        public UIBodyColorState state;

        /// <summary>
        /// Image preview of the skin color.
        /// </summary>
        [Tooltip("Image preview of the skin color")]
        public Image skinColorImg;

        /// <summary>
        /// Image preview of the skin details color.
        /// </summary>
        [Tooltip("Image preview of the skin details color")]
        public Image skinDetailsImg;

        /// <summary>
        /// Slider that adjust skin details' opacity.
        /// </summary>
        [Tooltip("Slider that adjust skin details' opacity")]
        public Slider skinDetailsSlider;

        private UICreator _uicreator;

        void Awake()
        {
            _uicreator = this.transform.GetComponentInParent<UICreator>();
        }

        void OnEnable()
        {
            setState(UIBodyColorState.None);
        }


        /// <summary>
        /// Request to open UIColor and edit current CharacterViewer's skin color.
        /// </summary>
        public void EditSkinColor()
        {
            setState(UIBodyColorState.SkinColor);
        }

        /// <summary>
        /// Request to open UIColor and edit current CharacterViewer's skin details' color.
        /// </summary>
        public void EditDetailsColor()
        {
            setState(UIBodyColorState.DetailsColor);
        }

        /// <summary>
        /// Adjust CharacterViewer's skin detail's opacity.
        /// </summary>
        /// <param name="opacity">Opacity value</param>
        public void SetDetailsOpacity(float opacity)
        {
            Color detailscolor = _uicreator.character.GetPartColor(SlotCategory.SkinDetails, ColorCode.Color1);
            detailscolor.a = Mathf.Clamp01(opacity);
            _uicreator.character.SetPartColor(SlotCategory.SkinDetails, ColorCode.Color1, detailscolor);
        }

        private void setState(UIBodyColorState state)
        {
            this.state = state;

            if (_uicreator == null)
                return;
            
            switch (this.state)
            {
                case UIBodyColorState.None:
                    skinColorImg.color = _uicreator.character.SkinColor;

                    Color detailscolor = _uicreator.character.GetPartColor(SlotCategory.SkinDetails, ColorCode.Color1);
                    skinDetailsImg.color = new Color(detailscolor.r, detailscolor.g, detailscolor.b, 1.0f);
                    skinDetailsSlider.value = detailscolor.a;

                    _uicreator.colorUI.Close();
                    setChildActive(true);
                    break;
                case UIBodyColorState.SkinColor:
                    _uicreator.colorUI.Show(_uicreator.character.SkinColor);
                    setChildActive(false);
                    break;
                case UIBodyColorState.DetailsColor:
                    _uicreator.colorUI.Show(_uicreator.character.GetPartColor(SlotCategory.SkinDetails, ColorCode.Color1));
                    setChildActive(false);
                    break;
                default:
                    break;
            }
        }

        void Update()
        {
            if (state == UIBodyColorState.None)
                return;

            if (!_uicreator.colorUI.gameObject.activeInHierarchy)
            {
                setState(UIBodyColorState.None);
            }
            else
            {
                if (state == UIBodyColorState.SkinColor)
                    _uicreator.character.SkinColor = _uicreator.colorUI.selectedColor;
                else if (state == UIBodyColorState.DetailsColor)
                {
                    Color detailscolor = _uicreator.colorUI.selectedColor;
                    detailscolor.a = skinDetailsSlider.value;
                    _uicreator.character.SetPartColor(SlotCategory.SkinDetails, ColorCode.Color1, detailscolor);
                }
            }
        }

        private void setChildActive(bool isActive)
        {
            for (int i = 0; i < this.transform.childCount; i++)
                this.transform.GetChild(i).gameObject.SetActive(isActive);
        }
    }
}