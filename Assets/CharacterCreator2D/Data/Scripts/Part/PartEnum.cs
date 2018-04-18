using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public enum BodyType
    {
        Male = 0,
        Female = 1
    }

    public enum PartCategory
    {
        Armor = 0,
        Boots = 1,
        Ear = 2,
        Eyebrow = 3,
        Eyes = 4,
        FacialHair = 5,
        Gloves = 6,
        Hair = 7,
        Helmet = 8,
        Mouth = 9,
        Nose = 10,
        Pants = 11,
        SkinDetails = 12,
        Weapon = 13
    }
    
    public enum SlotCategory
    {
        Armor = 0,
        Boots = 1,
        Ear = 2,
        Eyebrow = 3,
        Eyes = 4,
        FacialHair = 5,
        Gloves = 6,
        Hair = 7,
        Helmet = 8,
        Mouth = 9,
        Nose = 10,
        Pants = 11,
        SkinDetails = 12,
        MainHand = 13,
        OffHand = 14
    }

    public enum WeaponCategory
    {
        OneHanded,
        TwoHanded,
        Bow,
        Shield
    }

    public class ColorCode
    {
        /// <summary>
        /// First Color
        /// </summary>
        public const string Color1 = "_Color1";

        /// <summary>
        /// Second Color
        /// </summary>
        public const string Color2 = "_Color2";

        /// <summary>
        /// Third Color
        /// </summary>
        public const string Color3 = "_Color3";
    }
}