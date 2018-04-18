using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public struct CharacterData
    {
        /// <summary>
        /// Character's BodyType.
        /// </summary>
        public BodyType bodyType;

        /// <summary>
        /// Character's skin color.
        /// </summary>
        public Color skinColor;

        /// <summary>
        /// Character's tint color.
        /// </summary>
        public Color tintColor;

        /// <summary>
        /// List of PartSlotData
        /// </summary>
        public List<PartSlotData> slotData;
    }

    [Serializable]
    public struct PartSlotData
    {
        /// <summary>
        /// Slot category.
        /// </summary>
        public SlotCategory category;

        /// <summary>
        /// Part name.
        /// </summary>
        public string partName;

        /// <summary>
        /// Package name.
        /// </summary>
		public string partPackage;

        /// <summary>
        /// First color.
        /// </summary>
        public Color color1;

        /// <summary>
        /// Second color.
        /// </summary>
        public Color color2;

        /// <summary>
        /// Third color.
        /// </summary>
        public Color color3;
    }
}