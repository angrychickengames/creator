using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
    public class PartGroup : MonoBehaviour
    {
        /// <summary>
        /// PackageItem's template.
        /// </summary>
        [Tooltip("PackageItem's template")]
        public PackageItem tPackage;

        /// <summary>
        /// PartItem's template.
        /// </summary>
        [Tooltip("PartItem's template")]
        public PartItem tPartItem;

        /// <summary>
        /// Current selected PartItem.
        /// </summary>
        [Tooltip("Current selected PartItem")]
        [ReadOnly]
        public PartItem selectedItem;

        /// <summary>
        /// SlotCategory loaded by this PartGroup.
        /// </summary>
        [Tooltip("SlotCategory loaded by this PartGroup")]
        public SlotCategory slotCategory;

        /// <summary>
        /// Transform object used for highlighting current selected PartItem. 
        /// </summary>
        [Tooltip("Transform object used for highlighting current selected PartItem")]
        public Transform selectionObj;

        /// <summary>
        /// UICreator found in this PartGroup's parent Transform.
        /// </summary>
        public UICreator CreatorUI { get; private set; }

        private List<Color> _partcolors = new List<Color>();

        /// <summary>
        /// Initialize PartGroup and populate the items.
        /// </summary>
        public void Initialize()
        {
            this.CreatorUI = this.transform.GetComponentInParent<UICreator>();
            if (this.CreatorUI == null)
                return;

            Transform itemparent = getItemParent();
            Dictionary<string, List<Part>> parts = getAvailableParts();
            Part assignedpart = this.CreatorUI.character.GetAssignedPart(slotCategory);

            clearItems();

            //..add null
            PartItem partitem = Instantiate<PartItem>(tPartItem, itemparent);
            partitem.Initialize(null);
            if (assignedpart == null)
            {
                selectedItem = partitem;
                if (this.gameObject.activeInHierarchy)
                {
                    StopCoroutine("ie_initselected");
                    StartCoroutine("ie_initselected", selectedItem);
                }
            }
            //add null..

            foreach (string packagename in parts.Keys)
            {
                List<WeaponCategory> weaponcats = new List<WeaponCategory>();
                PackageItem packitem = Instantiate<PackageItem>(tPackage, itemparent);
                packitem.Initialize(packagename);

                foreach (Part part in parts[packagename])
                {
                    if (part is Weapon)
                    {
                        Weapon weapon = (Weapon)part;
                        if (!weaponcats.Contains(weapon.weaponCategory))
                        {
                            packitem = Instantiate<PackageItem>(tPackage, itemparent);
                            packitem.Initialize(getWeaponTag(weapon.weaponCategory));
                            weaponcats.Add(weapon.weaponCategory);
                        }
                    }

                    partitem = Instantiate<PartItem>(tPartItem, itemparent);
                    partitem.Initialize(part);
                    if (part == assignedpart)
                    {
                        selectedItem = partitem;
                        if (this.gameObject.activeInHierarchy)
                        {
                            StopCoroutine("ie_initselected");
                            StartCoroutine("ie_initselected", selectedItem);
                        }
                    }
                }
            }

            _partcolors = getPartColors();
        }

        private Transform getItemParent()
        {
            Transform val = this.transform.Find("Part List/Content");
            return val != null ? val : this.transform;
        }

        private string getWeaponTag(WeaponCategory weaponCategory)
        {
            switch (weaponCategory)
            {
                case WeaponCategory.Bow:
                    return "Bow";
                case WeaponCategory.OneHanded:
                    return "One Handed";
                case WeaponCategory.Shield:
                    return "Shield";
                case WeaponCategory.TwoHanded:
                    return "Two Handed";
                default:
                    return "";
            }
        }

        void OnEnable()
        {
            Initialize();
        }

        IEnumerator ie_initselected(PartItem item)
        {
            yield return null;
            SelectItem(item);
        }

        private Dictionary<string, List<Part>> getAvailableParts()
        {
            try
            {
                Dictionary<string, List<Part>> val = new Dictionary<string, List<Part>>();
                List<Part> parts = PartList.Static.FindParts(slotCategory);
                foreach (Part part in parts)
                {
                    if (!part.supportedBody.Contains(CreatorUI.character.bodyType))
                        continue;

                    if (!val.ContainsKey(part.packageName))
                        val.Add(part.packageName, new List<Part>());

                    val[part.packageName].Add(part);
                }

                return val;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return new Dictionary<string, List<Part>>();
            }
        }

        /// <summary>
        /// Updates selected PartItem and UICreator's character part according to the given value.
        /// </summary>
        /// <param name="item">PartItem to be selected.</param>
        public void SelectItem(PartItem item)
        {
            selectedItem = item;
            selectionObj.position = item.transform.position;

            Part part = CreatorUI.character.GetAssignedPart(slotCategory);
            if (part != item.part)
                CreatorUI.character.EquipPart(slotCategory, item.part);
        }

        /// <summary>
        /// Updates selected PartItem to the item whose part value is 'null' and unequip UICreator's character part in the corresponded PartSlot.
        /// </summary>
        public void UnequipItem()
        {
            PartItem[] items = this.transform.GetComponentsInChildren<PartItem>(true);
            foreach (PartItem i in items)
            {
                if (i.part == null)
                {
                    SelectItem(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Assign selectedItem with random PartItem found in this Transform's children.
        /// </summary>
        public void RandomizeItem()
        {
            PartItem[] items = this.transform.GetComponentsInChildren<PartItem>(true);
            if (items.Length > 1)
                SelectItem(items[Random.Range(1, items.Length)]);
            if (_partcolors.Count > 0)
            {
                Color tcolor = _partcolors[Random.Range(0, _partcolors.Count)];
                CreatorUI.character.SetPartColor(this.slotCategory, ColorCode.Color1, tcolor);

                if (this.slotCategory == SlotCategory.SkinDetails)
                {
                    UIBodyColor uibodycolor = CreatorUI.GetComponentInChildren<UIBodyColor>(true);
                    if (uibodycolor != null)
                        uibodycolor.skinDetailsImg.color = tcolor;
                }
                else if (!SetupData.colorableSpriteLinks.ContainsKey(this.slotCategory))
                {
                    tcolor = _partcolors[Random.Range(0, _partcolors.Count)];
                    CreatorUI.character.SetPartColor(this.slotCategory, ColorCode.Color2, tcolor);

                    tcolor = _partcolors[Random.Range(0, _partcolors.Count)];
                    CreatorUI.character.SetPartColor(this.slotCategory, ColorCode.Color3, tcolor);
                }
            }
        }

        private void clearItems()
        {
            PackageItem[] packages = this.transform.GetComponentsInChildren<PackageItem>(true);
            for (int i = packages.Length - 1; i >= 0; i--)
            {
                GameObject go = packages[i].gameObject;
                packages[i] = null;
                Destroy(go);
            }

            PartItem[] items = this.transform.GetComponentsInChildren<PartItem>(true);
            for (int i = items.Length - 1; i >= 0; i--)
            {
                GameObject go = items[i].gameObject;
                items[i] = null;
                Destroy(go);
            }
        }

        private List<Color> getPartColors()
        {
            List<Color> val = new List<Color>();
            foreach (ColorPalette p in CreatorUI.colorUI.colorPalette.colorPalettes)
            {
                foreach (Color c in p.colors)
                    val.Add(c);
            }

            if (CreatorUI.colorUI.colorPalette.customPalette != null)
            {
                foreach (Color c in CreatorUI.colorUI.colorPalette.customPalette.colors)
                    val.Add(c);
            }

            return val;
        }
    }
}