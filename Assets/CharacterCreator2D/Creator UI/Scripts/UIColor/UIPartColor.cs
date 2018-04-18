using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UIPartColor : MonoBehaviour
    {
        /// <summary>
        /// Current state of UIPartColor.
        /// </summary>
        [Tooltip("Current state of UIPartColor")]
        [ReadOnly]
        public UIPartColorState state;

        /// <summary>
        /// Current mode of UIPartColor.
        /// </summary>
        [Tooltip("Current mode of UIPartColor")]
        [ReadOnly]
        public UIPartColorMode mode;

        /// <summary>
        /// SlotCategory displayed by this UIPartColor.
        /// </summary>
        [Tooltip("SlotCategory displayed by this UIPartColor")]
        [ReadOnly]
        public SlotCategory slotCategory;

        /// <summary>
        /// Image preview of the color of a part
        /// </summary>
        [Tooltip("Image preview of the first color of a part")]
        public Image color1Img;

        /// <summary>
        /// Image preview of the second color of a part
        /// </summary>
        [Tooltip("Image preview of the second color of a part")]
        public Image color2Img;

        /// <summary>
        /// Image preview of the third color of a part
        /// </summary>
        [Tooltip("Image preview of the third color of a part")]
        public Image color3Img;

        /// <summary>
        /// Text message that tells if a part can't be colored.
        /// </summary>
        [Tooltip("Text message that tells if a part can't be colored")]
        public Text uncolorableText;

        private UICreator _uicreator;
        private string _colorcode;

        void Awake()
        {
            _uicreator = this.transform.GetComponentInParent<UICreator>();
        }

        void OnEnable()
        {
            setState(UIPartColorState.None);
        }
        
        /// <summary>
        /// Initialize UIPartColor according to SlotCategoy value.
        /// </summary>
        /// <param name="slotCatInt">An integer value of SlotCategory.</param>
        public void Initialize(int slotCatInt)
        {
            SlotCategory slotcat = (SlotCategory)slotCatInt;
            Initialize(slotcat);
        }

        /// <summary>
        /// Initialize UIPartColor according to SlotCategory value.
        /// </summary>
        /// <param name="slotCategory">SlotCategory value.</param>
        public void Initialize(SlotCategory slotCategory)
        {
            if (_uicreator == null)
                _uicreator = this.transform.GetComponentInParent<UICreator>();
            this.slotCategory = slotCategory;
            if (_uicreator.character.slots.GetSlot(slotCategory).material == null)
                this.mode = UIPartColorMode.Uncolorable;
            else if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
                this.mode = UIPartColorMode.OneColor;
            else
                this.mode = UIPartColorMode.ThreeColor;
        }

        /// <summary>
        /// Request to open UIColor and edit the first color of selected part.
        /// </summary>
        public void EditColor1()
        {
            setState(UIPartColorState.Color1);
        }

        /// <summary>
        /// Request to open UIColor and edit the second color of selected part.
        /// </summary>
        public void EditColor2()
        {
            setState(UIPartColorState.Color2);
        }

        /// <summary>
        /// Request to open UIColor and edit the third color of selected part.
        /// </summary>
        public void EditColor3()
        {
            setState(UIPartColorState.Color3);
        }

        private void setState(UIPartColorState state)
        {
            if (_uicreator == null)
                return;

            this.state = state;
            switch (this.state)
            {
                case UIPartColorState.None:
                    refreshImgColors();
                    _uicreator.colorUI.Close();
                    _colorcode = "";
                    setChildActive(true);
                    break;
                case UIPartColorState.Color1:
                    _uicreator.colorUI.Show(_uicreator.character.GetPartColor(slotCategory, ColorCode.Color1));
                    _colorcode = ColorCode.Color1;
                    setChildActive(false);
                    break;
                case UIPartColorState.Color2:
                    _uicreator.colorUI.Show(_uicreator.character.GetPartColor(slotCategory, ColorCode.Color2));
                    _colorcode = ColorCode.Color2;
                    setChildActive(false);
                    break;
                case UIPartColorState.Color3:
                    _uicreator.colorUI.Show(_uicreator.character.GetPartColor(slotCategory, ColorCode.Color3));
                    _colorcode = ColorCode.Color3;
                    setChildActive(false);
                    break;
                default:
                    break;
            }
        }

        private void refreshImgColors()
        {
            if (_uicreator == null)
                return;
            switch (this.mode)
            {
                case UIPartColorMode.OneColor:
                    color1Img.color = _uicreator.character.GetPartColor(slotCategory, ColorCode.Color1);
                    break;
                case UIPartColorMode.ThreeColor:
                    color1Img.color = _uicreator.character.GetPartColor(slotCategory, ColorCode.Color1);
                    color2Img.color = _uicreator.character.GetPartColor(slotCategory, ColorCode.Color2);
                    color3Img.color = _uicreator.character.GetPartColor(slotCategory, ColorCode.Color3);
                    break;
                default:
                    break;
            }
        }

        private void setChildActive(bool isActive)
        {
            for (int i = 0; i < this.transform.childCount; i++)
                this.transform.GetChild(i).gameObject.SetActive(isActive);

            if (!isActive)
                return;

            switch (mode)
            {
                case UIPartColorMode.OneColor:
                    color1Img.gameObject.SetActive(true);
                    color2Img.gameObject.SetActive(false);
                    color3Img.gameObject.SetActive(false);
                    uncolorableText.gameObject.SetActive(false);
                    break;
                case UIPartColorMode.ThreeColor:
                    color1Img.gameObject.SetActive(true);
                    color2Img.gameObject.SetActive(true);
                    color3Img.gameObject.SetActive(true);
                    uncolorableText.gameObject.SetActive(false);
                    break;
                case UIPartColorMode.Uncolorable:
                    color1Img.gameObject.SetActive(false);
                    color2Img.gameObject.SetActive(false);
                    color3Img.gameObject.SetActive(false);
                    uncolorableText.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        void Update()
        {
            if (state == UIPartColorState.None)
                return;

            if (_uicreator.colorUI.gameObject.activeInHierarchy)
                _uicreator.character.SetPartColor(slotCategory, _colorcode, _uicreator.colorUI.selectedColor);
            else
                setState(UIPartColorState.None);
        }
    }
}
