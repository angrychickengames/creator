using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharacterCreator2D
{
    public class CharacterViewer : MonoBehaviour
    {
        /// <summary>
        /// CharacterViewer's setup data.
        /// </summary>
        [Tooltip("CharacterViewer's setup data")]
        public SetupData setupData;

        /// <summary>
        /// CharacterViewer's body type.
        /// </summary>
        [Tooltip("CharacterViewer's body type")] 
        public BodyType bodyType;

        /// <summary>
        /// List of all PartSlots supported by CharacterViewer.
        /// </summary>
        [Tooltip("List of all PartSlots supported by CharacterViewer")]
        public SlotList slots;

        /// <summary>
        /// Should the materials are instantiated at Runtime?
        /// </summary>
        [Tooltip("Should the materials are instantiated at Runtime?")]
        public bool instanceMaterials = true;

        /// <summary>
        /// List of all sprites.
        /// </summary>
        [Tooltip("List of all sprites")]
        public List<SpriteRenderer> sprites;

        /// <summary>
        /// List CharacterViewer's skins.
        /// </summary>
        [Tooltip("List of the skins of this CharacterViewer")]
        public List<SpriteRenderer> skins;

        [SerializeField]
        private Color _skincolor = Color.gray;
        [SerializeField]
        private Color _tintcolor = Color.white;

        /// <summary>
        /// CharacterViewer's skin color.
        /// </summary>
        public Color SkinColor
        {
            get { return _skincolor; }
            set
            {
                _skincolor = value;
                RepaintSkinColor();
            }
        }

        /// <summary>
        /// CharacterViewer's tint color. Default value is 'Color.white'.
        /// </summary>
        public Color TintColor
        {
            get { return _tintcolor; }
            set
            {
                _tintcolor = value;
                RepaintSkinColor();
            }
        }

        void Awake()
        {
            if (instanceMaterials)
                instantiateSlotsMaterial();
            Initialize();
        }

        /// <summary>
        /// Initialize CharacterViewer according to the corresponded parts and settings.
        /// </summary>
        public void Initialize()
        {
            if (setupData == null)
                setupData = Resources.Load<SetupData>("CC2D_SetupData");

            refreshBodyType();
            sprites = getSprites(setupData.order);
            skins = getSprites(setupData.skin);
            sortSprites();
            refreshSortingGroup();
            RelinkMaterials();
            RepaintSkinColor();
            RepaintTintColor();
            refreshPartSlots();
        }

        private void instantiateSlotsMaterial()
        {
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = this.slots.GetSlot(cat);
                if (slot == null || slot.material == null)
                    continue;

                if (!materials.ContainsKey(slot.material.name))
                {
                    Material tmat = Instantiate<Material>(slot.material);
                    tmat.name = slot.material.name;
                    materials.Add(tmat.name, tmat);
                    slot.material = tmat;
                }
                else
                {
                    slot.material = materials[slot.material.name];
                }
            }
        }

        /// <summary>
        /// Relink all materials used by this CharacterViewer to the value in each of its slots.
        /// </summary>
		public void RelinkMaterials()
        {
            SpriteRenderer[] renderers = this.transform.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer r in renderers)
            {
                if (r.sharedMaterial == null)
                    continue;

                PartSlot slot = slots.GetSlot(getMaterialsCategory(r.sharedMaterial));
                if (slot == null || slot.material == null)
                    continue;

                if (Application.isPlaying)
                    r.material = slot.material;
                else
                    r.sharedMaterial = slot.material;
            }
        }

        private SlotCategory getMaterialsCategory(Material material)
        {
            foreach (SlotCategory s in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = slots.GetSlot(s);
                if (slot == null || slot.material == null)
                    continue;

                if (material.name.Contains(slot.material.name))
                    return s;
            }

            foreach (MaterialData md in setupData.defaultMaterials)
            {
                if (material.name.Contains(md.material.name))
                    return md.slotCategory;
            }
            return SlotCategory.Nose;
        }

        private List<SpriteRenderer> getSprites(List<string> nameList)
        {
            SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>(true);
            List<SpriteRenderer> outputSprites = new List<SpriteRenderer>();
            for (int n = 0; n < nameList.Count; n++)
            {
                for (int s = 0; s < allSprites.Length; s++)
                {
                    if (allSprites[s].name == nameList[n])
                    {
                        outputSprites.Add(allSprites[s]);
                        break;
                    }
                }
            }
            return outputSprites;
        }

        private void sortSprites()
        {
            for (int a = 0; a < setupData.order.Count; a++)
            {
                for (int s = 0; s < sprites.Count; s++)
                {
                    if (sprites[s].name == setupData.order[a])
                    {
                        sprites[s].sortingOrder = a;
                        break;
                    }
                }
            }
        }

        private void refreshSortingGroup()
        {
            if (Application.isPlaying)
            {
                StopCoroutine("ie_refreshsorting");
                StartCoroutine("ie_refreshsorting");
            }
        }

        IEnumerator ie_refreshsorting()
        {
            SortingGroup sortinggroup = this.GetComponent<SortingGroup>();
            sortinggroup.enabled = false;
            yield return null;
            sortinggroup.enabled = true;
        }

        /// <summary>
        /// Repaint skin color according to the current value.
        /// </summary>
        public void RepaintSkinColor()
        {
            if (skins == null || skins.Count <= 0)
                return;

            foreach (SpriteRenderer s in skins)
            {
                if (s != null)
                    s.color = _skincolor;
            }
        }

        /// <summary>
        /// Reset TintColor to its default color.
        /// </summary>
		public void ResetTintColor()
        {
            TintColor = Color.white;
        }

        /// <summary>
        /// Repaint tint color according to the current value.
        /// </summary>
        public void RepaintTintColor()
        {
            foreach (SlotCategory c in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = slots.GetSlot(c);
                if (slot.material == null)
                    continue;

                if (slot.material.HasProperty("_Color"))
                    slot.material.SetColor("_Color", _tintcolor);
            }
        }

        private void refreshPartSlots()
        {
            foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot s = this.slots.GetSlot(cat);
                EquipPart(cat, s.assignedPart);
                if (!SetupData.colorableSpriteLinks.ContainsKey(cat))
                {
                    SetPartColor(cat, ColorCode.Color1, s.color1);
                    SetPartColor(cat, ColorCode.Color2, s.color2);
                    SetPartColor(cat, ColorCode.Color3, s.color3);
                }
            }
        }

        /// <summary>
        /// Returns the assigned part of a given SlotCategory. Returns 'null' if there is no part assigned.
        /// </summary>
        /// <param name="category">Given SlotCategory.</param>
        /// <returns>Assigned part of the given SlotCategory if there is any, otherwise returns 'null'</returns>
        public Part GetAssignedPart(SlotCategory category)
        {
            PartSlot slot = this.slots.GetSlot(category);
            if (slot == null)
                return null;
            return slot.assignedPart;
        }

        /// <summary>
        /// Assign/unassign part to/from desired slot of this CharacterViewer.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="partName">The name of the part. It will assign the first part found if there are more than one part with the same name in different packages. Will unassign if 'null' or 'empty'.</param>
        public void EquipPart(SlotCategory slotCategory, string partName)
        {
            EquipPart(slotCategory, partName, "");
        }

        /// <summary>
        /// Assign/unassign part to/from desired slot of this CharacterViewer.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="partName">The name of the part. Will unassign if 'null' or 'empty'.</param>
        /// <param name="partPackage">The package name of the part. Will assign the first part found if 'null' or 'empty'</param>
        public void EquipPart(SlotCategory slotCategory, string partName, string partPackage)
        {
            if (string.IsNullOrEmpty(partName))
            {
                EquipPart(slotCategory, (Part)null);
                return;
            }

            Part part = PartList.Static.FindPart(partName, partPackage, slotCategory);
            if (part == null)
            {
                Debug.Log("can't find part: " + partName);
                return;
            }

            EquipPart(slotCategory, part);
        }

        /// <summary>
        /// Assign/unassign part to/from desired slot of this CharacterViewer.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="part">Desired Part. Will unassigned if 'null'.</param>
        public void EquipPart(SlotCategory slotCategory, Part part)
        {
            //..find alternative if part doesn't support this character body type
            if (part != null && !part.supportedBody.Contains(this.bodyType))
            {
                Part altpart = getAlternatePart(part);
                if (altpart == null)
                    Debug.Log("part: " + part + " doesn't support body type: " + this.bodyType);
                else
                    Debug.Log("part: " + part + " doesn't support body type: " + this.bodyType +
                        ". it will be switched by: " + altpart.name);
                part = altpart;
            }

            if (slotCategory == SlotCategory.MainHand)
            {
                Weapon mainweapon = (Weapon)part;
                Weapon offweapon = (Weapon)this.slots.GetSlot(SlotCategory.OffHand).assignedPart;
                if (mainweapon != null && mainweapon.weaponCategory == WeaponCategory.TwoHanded)
                    equipWeapon(SlotCategory.OffHand, null);
                else if (mainweapon != null && offweapon != null && offweapon.weaponCategory == WeaponCategory.Bow)
                    equipWeapon(SlotCategory.OffHand, null);
                equipWeapon(SlotCategory.MainHand, mainweapon);
            }
            else if (slotCategory == SlotCategory.OffHand)
            {
                Weapon mainweapon = (Weapon)this.slots.GetSlot(SlotCategory.MainHand).assignedPart;
                Weapon offweapon = (Weapon)part;
                if (offweapon != null && offweapon.weaponCategory == WeaponCategory.Bow)
                    equipWeapon(SlotCategory.MainHand, null);
                else if (offweapon != null && mainweapon != null && mainweapon.weaponCategory == WeaponCategory.TwoHanded)
                    equipWeapon(SlotCategory.MainHand, null);
                equipWeapon(SlotCategory.OffHand, offweapon);
            }
            else if (slotCategory == SlotCategory.SkinDetails)
                equipSkinDetails(slotCategory, part);
            else
                equipPart(slotCategory, part);
        }

        private Part getAlternatePart(Part part)
        {
            List<Part> parts = PartList.Static.FindParts(part.category);
            int sid = parts.FindIndex(x => x == part);
            int inc = this.bodyType == BodyType.Male ? 1 : -1;

            for (int i = sid; (i >= 0) && (i < parts.Count); i += inc)
            {
                if (parts[i].supportedBody.Contains(this.bodyType))
                    return parts[i];
            }

            foreach (Part p in parts) //..search for any
            {
                if (p.supportedBody.Contains(this.bodyType))
                    return p;
            }

            return null;
        }

        private void equipWeapon(SlotCategory slotCategory, Weapon weapon)
        {
            PartSlot slot = this.slots.GetSlot(slotCategory);
            if (slot == null)
                return;

            resetWeaponRenderer(slot);
            if (weapon == null)
            {
                slot.assignedPart = null;
                return;
            }

            Dictionary<string, string> links = new Dictionary<string, string>();
            switch (slotCategory)
            {
                case SlotCategory.MainHand:
                    if (weapon.weaponCategory == WeaponCategory.OneHanded ||
                        weapon.weaponCategory == WeaponCategory.TwoHanded)
                        links = SetupData.rWeaponLink;
                    break;
                case SlotCategory.OffHand:
                    if (weapon.weaponCategory == WeaponCategory.Bow)
                        links = SetupData.bowLink;
                    else if (weapon.weaponCategory == WeaponCategory.Shield)
                        links = SetupData.shieldLink;
                    else if (weapon.weaponCategory == WeaponCategory.OneHanded)
                        links = SetupData.lWeaponLink;
                    break;
                default:
                    break;
            }

            slot.assignedPart = weapon;
            if (slot.material != null)
                slot.material.SetTexture("_ColorMask", weapon.colorMask);
            foreach (Sprite s in weapon.sprites)
                this.transform.Find(links[s.name]).GetComponent<SpriteRenderer>().sprite = s;
            if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
                SetPartColor(slotCategory, ColorCode.Color1, slot.color1);
        }

        private void resetWeaponRenderer(PartSlot slot)
        {
            Weapon weapon = (Weapon)slot.assignedPart;
            if (weapon == null)
                return;

            if (slot == this.slots.GetSlot(SlotCategory.MainHand))
            {
                foreach (string k in SetupData.rWeaponLink.Keys)
                    this.transform.Find(SetupData.rWeaponLink[k]).GetComponent<SpriteRenderer>().sprite = null;
            }
            else if (slot == this.slots.GetSlot(SlotCategory.OffHand))
            {
                foreach (string k in SetupData.bowLink.Keys)
                    this.transform.Find(SetupData.bowLink[k]).GetComponent<SpriteRenderer>().sprite = null;
                foreach (string k in SetupData.shieldLink.Keys)
                    this.transform.Find(SetupData.shieldLink[k]).GetComponent<SpriteRenderer>().sprite = null;
                foreach (string k in SetupData.lWeaponLink.Keys)
                    this.transform.Find(SetupData.lWeaponLink[k]).GetComponent<SpriteRenderer>().sprite = null;
            }
        }

        private void equipSkinDetails(SlotCategory slotCategory, Part part)
        {
            if ((part != null && part.category != PartCategory.SkinDetails) ||
                slotCategory != SlotCategory.SkinDetails)
                return;

            PartSlot slot = this.slots.GetSlot(slotCategory);
            if (slot == null)
                return;

            slot.assignedPart = part;
            if (part == null)
                slot.material.SetTexture("_Details", null);
            else
                slot.material.SetTexture("_Details", part.texture);
        }

        private void equipPart(SlotCategory slotCategory, Part part)
        {
            if (!SetupData.partLinks.ContainsKey(slotCategory))
                return;

            Dictionary<string, string> links = SetupData.partLinks[slotCategory];
            PartSlot slot = this.slots.GetSlot(slotCategory);

            if (links == null || slot == null)
                return;

            if (part != null && (int)slotCategory != (int)part.category)
            {
                Debug.Log("can't equip " + part.name + ". part doesn't match with slot category");
                return;
            }

            //..reset part
            foreach (string k in links.Keys)
                this.transform.Find(links[k]).GetComponent<SpriteRenderer>().sprite = null;

            if (part == null)
            {
                slot.assignedPart = null;
                if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
                    SetPartColor(slotCategory, ColorCode.Color1, slot.color1);
                return;
            }

            slot.assignedPart = part;
            if (slot.material != null)
                slot.material.SetTexture("_ColorMask", part.colorMask);
            foreach (Sprite s in part.sprites)
                this.transform.Find(links[s.name]).GetComponent<SpriteRenderer>().sprite = s;
            if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
                SetPartColor(slotCategory, ColorCode.Color1, slot.color1);
        }

        /// <summary>
        /// Returns Part's Color from desired PartSlot.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory</param>
        /// <param name="colorCode">Represent the desired color order of the part. Use 'CharacterCreator2D.ColorCode'.</param>
        /// <returns>Part's Color. Returns 'Color.clear' if colorCode doesn't match with any value in 'CharacterCreator2D.ColorCode'.</returns>
        public Color GetPartColor(SlotCategory slotCategory, string colorCode)
        {
            PartSlot slot = slots.GetSlot(slotCategory);
            if (slot != null)
            {
                switch (colorCode)
                {
                    case ColorCode.Color1:
                        return slot.color1;
                    case ColorCode.Color2:
                        return slot.color2;
                    case ColorCode.Color3:
                        return slot.color3;
                    default:
                        return Color.clear;
                }
            }
            return Color.clear;
        }

        private Color getSkinDetailsColor()
        {
            PartSlot slot = this.slots.GetSlot(SlotCategory.SkinDetails);
            if (slot != null && slot.material.HasProperty("_DetailsColor"))
                return slot.material.GetColor("_DetailsColor");
            else
                return Color.clear;
        }

        private Color getPartColor(SlotCategory slotCategory, string colorCode)
        {
            PartSlot slot = slots.GetSlot(slotCategory);
            if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
                return this.transform.Find(SetupData.colorableSpriteLinks[slotCategory][0]).GetComponent<SpriteRenderer>().color;
            else if (slot != null && slot.material != null)
                return slot.material.GetColor(colorCode);
            return Color.clear;
        }

        /// <summary>
        /// Modifies Part's Color. 
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="colorCode">Represent the desired color order of the part. Use 'CharacterCreator2D.ColorCode'.</param>
        /// <param name="color">Desired value of the color.</param>
        public void SetPartColor(SlotCategory slotCategory, string colorCode, Color color)
        {
            if (slotCategory == SlotCategory.MainHand || slotCategory == SlotCategory.OffHand)
                setWeaponColor(slotCategory, colorCode, color);
            else if (slotCategory == SlotCategory.SkinDetails)
                setSkinDetailsColor(color);
            else
                setPartColor(slotCategory, colorCode, color);
        }

        private void setWeaponColor(SlotCategory slotCategory, string colorCode, Color color)
        {
            if (colorCode == ColorCode.Color1)
            {
                if (slotCategory == SlotCategory.MainHand)
                    this.transform.Find(SetupData.weaponFXLinks[SlotCategory.MainHand]).GetComponent<SpriteRenderer>().color = color;
                else if (slotCategory == SlotCategory.OffHand)
                    this.transform.Find(SetupData.weaponFXLinks[SlotCategory.OffHand]).GetComponent<SpriteRenderer>().color = color;
            }

            setPartColor(slotCategory, colorCode, color);
        }

        private void setSkinDetailsColor(Color color)
        {
            PartSlot slot = this.slots.GetSlot(SlotCategory.SkinDetails);
            slot.material.SetColor("_DetailsColor", color);
            slot.color1 = slot.color2 = slot.color3 = color;
        }

        private void setPartColor(SlotCategory slotCategory, string colorCode, Color color)
        {
            PartSlot slot = this.slots.GetSlot(slotCategory);
            if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
            {
                List<string> links = SetupData.colorableSpriteLinks[slotCategory];
                foreach (string l in links)
                    this.transform.Find(l).GetComponent<SpriteRenderer>().color = color;
                slot.color1 = slot.color2 = slot.color3 = color;
                return;
            }

            if (slot == null || slot.material == null)
                return;

            slot.material.SetColor(colorCode, color);
            switch (colorCode)
            {
                case ColorCode.Color1:
                    slot.color1 = color;
                    break;
                case ColorCode.Color2:
                    slot.color2 = color;
                    break;
                case ColorCode.Color3:
                    slot.color3 = color;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Change this CharacterViewer's BodyType.
        /// </summary>
        /// <param name="bodyType">Desired BodyType value.</param>
        public void SetBodyType(BodyType bodyType)
        {
            this.bodyType = bodyType;
            Initialize();
        }

        private void refreshBodyType()
        {
            if (!this.gameObject.activeInHierarchy)
                return;

            Transform bodyobj = setupData.GetBodyPrefab(bodyType);
            if (bodyobj == null)
                return;

            Animator animator = this.GetComponent<Animator>();
            RuntimeAnimatorController controller = animator.runtimeAnimatorController;
            string curranim = "";
            if (Application.isPlaying && animator.GetCurrentAnimatorClipInfo(0).Length > 0)
                curranim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

            animator.enabled = false;
            animator.runtimeAnimatorController = null;

            Transform oldbody = this.transform.Find("Root");
            bodyobj = Instantiate(bodyobj, this.transform.position, Quaternion.identity, this.transform);
            bodyobj.name = "Root";
            if (oldbody != null)
                DestroyImmediate(oldbody.gameObject);

            animator.runtimeAnimatorController = controller;
            animator.enabled = true;
            animator.Play(curranim);
        }

        #region JSON
        /// <summary>
        /// Generate CharacterData of CharacterViewer.
        /// </summary>
        /// <returns>Generated CharacterData of CharacterViewer.</returns>
        public CharacterData GenerateCharacterData()
        {
            CharacterData val = new CharacterData();
            val.bodyType = this.bodyType;
            val.skinColor = _skincolor;
            val.tintColor = _tintcolor;

            val.slotData = new List<PartSlotData>();
            foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = this.slots.GetSlot(cat);
                val.slotData.Add(new PartSlotData()
                {
                    category = cat,
                    partName = slot.assignedPart == null ? "" : slot.assignedPart.name,
                    partPackage = slot.assignedPart == null ? "" : slot.assignedPart.packageName,
                    color1 = slot.color1,
                    color2 = slot.color2,
                    color3 = slot.color3
                });
            }

            return val;
        }

        /// <summary>
        /// Assign and initialize this CharacterViewer according to a given CharacterData.
        /// </summary>
        /// <param name="data">CharacterData to be assigned from.</param>
        public void AssignCharacterData(CharacterData data)
        {
            this.bodyType = data.bodyType;
            _skincolor = data.skinColor;
            _tintcolor = data.tintColor;
            Initialize();

            foreach (PartSlotData sd in data.slotData)
            {
                PartSlot slot = this.slots.GetSlot(sd.category);
                if (slot == null)
                    continue;

                if (string.IsNullOrEmpty(sd.partName))
                {
                    EquipPart(sd.category, (Part)null);
                    continue;
                }

                Part part = PartList.Static.FindPart(sd.partName, sd.partPackage, sd.category);
                if (part == null)
                    continue;

                EquipPart(sd.category, part);
                SetPartColor(sd.category, ColorCode.Color1, sd.color1);
                SetPartColor(sd.category, ColorCode.Color2, sd.color2);
                SetPartColor(sd.category, ColorCode.Color3, sd.color3);
            }
        }

        /// <summary>
        /// Save CharacterViewer's data as JSON file on a given path.
        /// </summary>
        /// <param name="filePath">Desired file path.</param>
        public void SaveToJSON(string filePath)
        {
            try
            {
                CharacterData data = GenerateCharacterData();
                string content = JsonUtility.ToJson(data, false);
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                File.WriteAllText(filePath, content);
            }
            catch (Exception e)
            {
                Debug.LogError("error on save to JSON:\n" + e.ToString());
            }
        }

        /// <summary>
        /// Load and assign data from a JSON file in a given path to CharacterViewer.
        /// </summary>
        /// <param name="filePath">Desired file path.</param>
        public void LoadFromJSON(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("load CharacterViewer '" + this.name + "' from JSON's failed: file doesn't exist");
                return;
            }

            try
            {
                CharacterData data = JsonUtility.FromJson<CharacterData>(File.ReadAllText(filePath));
                AssignCharacterData(data);
            }
            catch (Exception e)
            {
                Debug.LogError("error on load from JSON:\n" + e.ToString());
            }
        }
        #endregion
    }
}