using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CharacterCreator2D.UI
{
    public class PartItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// Part loaded by this PartItem.
        /// </summary>
        [Tooltip("Part loaded by this PartItem")]
        [ReadOnly]
        public Part part;

        private PartGroup _group;
        private Part _otherweapon;

        void Awake()
        {
            _group = this.transform.GetComponentInParent<PartGroup>();
        }

        void OnEnable()
        {
            if (_group.slotCategory == SlotCategory.MainHand)
                _otherweapon = _group.CreatorUI.character.slots.GetSlot(SlotCategory.OffHand).assignedPart;
            else if (_group.slotCategory == SlotCategory.OffHand)
                _otherweapon = _group.CreatorUI.character.slots.GetSlot(SlotCategory.MainHand).assignedPart;
        }

        /// <summary>
        /// Initialize PartItem according to a given Part.
        /// </summary>
        /// <param name="part">Part to be loaded.</param>
        public void Initialize(Part part)
        {
            this.part = part;
            this.transform.name = part == null ? "NULL" : part.packageName + "_" + part.name;
            this.transform.Find("Text").GetComponent<Text>().text = part == null ? "NULL" : part.name;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_group.selectedItem != null && _group.selectedItem == this)
                return;

            _group.CreatorUI.character.EquipPart(_group.slotCategory, part);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_group.selectedItem != null && _group.selectedItem == this)
                return;

            if (_group.slotCategory == SlotCategory.MainHand)
            {
                _group.CreatorUI.character.EquipPart(SlotCategory.OffHand, _otherweapon);
                _group.CreatorUI.character.EquipPart(SlotCategory.MainHand, _group.selectedItem.part);
            }
            else if (_group.slotCategory == SlotCategory.OffHand)
            {
                _group.CreatorUI.character.EquipPart(SlotCategory.MainHand, _otherweapon);
                _group.CreatorUI.character.EquipPart(SlotCategory.OffHand, _group.selectedItem.part);
            }
            else
                _group.CreatorUI.character.EquipPart(_group.slotCategory, _group.selectedItem.part);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_group.selectedItem != null && _group.selectedItem == this)
                return;

            _group.SelectItem(this);
        }
    }
}