using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CharacterCreator2D.UI
{
    public class BodyTypeItem : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// BodyType value loaded by this BodyTypeItem.
        /// </summary>
        public BodyType bodyType;

        private BodyTypeGroup _group;

        void Awake()
        {
            _group = this.transform.GetComponentInParent<BodyTypeGroup>();
        }
        
        /// <summary>
        /// Initialize BodyTypeItem with a given BodyType value.
        /// </summary>
        /// <param name="bodyType"></param>
        public void Initialize(BodyType bodyType)
        {
            this.bodyType = bodyType;
            this.transform.name = "" + bodyType;
            this.transform.Find("Text").GetComponent<Text>().text = "" + bodyType;
        }
                
        public void OnPointerClick(PointerEventData eventData)
        {
            _group.SelectItem(this);
        }
    }
}