using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CharacterCreator2D;

namespace CharacterEditor2D
{
    public class WizardPart : ScriptableWizard
    {
        protected struct TemplateOption
        {
            public string spriteName;
            public bool option;
        }

        protected Texture2D _texture;
        protected Texture2D _mask;
        protected List<PartTemplate> _templates = new List<PartTemplate>();
        protected int[] _templateint = new int[0];
        protected string[] _templatestring = new string[0];
        protected Dictionary<string, TemplateOption[]> _tempopt = new Dictionary<string, TemplateOption[]>();
        protected BodyType[] _bodytype = new BodyType[0];
        protected bool[] _bodyopt = new bool[0];
        protected int[] _packint = new int[0];
        protected string[] _packstring = new string[0];
        protected string _partname = "";

        protected int _stemp = 0;
        protected string skey
        {
            get { return _templatestring[_stemp]; }
        }

        protected int _spack;

        void OnEnable()
        {
            initializeBodyOpt();
            initializeTemplate();
            initializePackages();
        }

        private void initializeBodyOpt()
        {
            _bodytype = (BodyType[])Enum.GetValues(typeof(BodyType));
            _bodyopt = new bool[_bodytype.Length];
            for (int i = 0; i < _bodyopt.Length; i++)
                _bodyopt[i] = true;
        }

        private void initializeTemplate()
        {
            _templates = EditorUtils.GetScriptables<PartTemplate>(WizardUtils.PartTemplateFolder);
            _stemp = 0;
            _tempopt = new Dictionary<string, TemplateOption[]>();
            if (_templates.Count <= 0)
            {
                _templateint = new int[0];
                _templatestring = new string[0];
                _tempopt = new Dictionary<string, TemplateOption[]>();
                return;
            }

            _templateint = new int[_templates.Count];
            _templatestring = new string[_templates.Count];
            for (int i = 0; i < _templates.Count; i++)
            {
                _templateint[i] = i;
                _templatestring[i] = _templates[i].ToString();
                _tempopt.Add(_templates[i].ToString(),
                    new TemplateOption[_templates[i].spriteNames.Count]);
                for (int j = 0; j < _templates[i].spriteNames.Count; j++)
                {
                    _tempopt[_templates[i].ToString()][j].spriteName = _templates[i].spriteNames[j];
                    _tempopt[_templates[i].ToString()][j].option = true;
                }
            }
        }

        protected virtual void initializePackages()
        {
            _spack = 0;
            _packint = new int[] { 0 };
            _packstring = new string[] { "Custom" };
        }

        protected override bool DrawWizardGUI()
        {
            bool val = base.DrawWizardGUI();

            //..draw name
            EditorGUILayout.BeginVertical(WizardUtils.BGStyle);
            _partname = EditorGUILayout.TextField("Name", _partname);
            EditorGUILayout.EndVertical();
            //draw name

            EditorGUILayout.Space();

            //..draw package
            EditorGUILayout.BeginVertical(WizardUtils.BGStyle);
            _spack = EditorGUILayout.IntPopup("Package", _spack, _packstring, _packint);
            EditorGUILayout.EndVertical();
            //draw package..

            EditorGUILayout.Space();

            //..draw texture
            EditorGUILayout.BeginVertical(WizardUtils.BGStyle);
            EditorGUILayout.LabelField("Texture", WizardUtils.BoldTextStyle);
            _texture = (Texture2D)EditorGUILayout.ObjectField(_texture, typeof(Texture2D), false);
            EditorGUILayout.EndVertical();
            //draw texture..

            EditorGUILayout.Space();

            //..draw mask
            EditorGUILayout.BeginVertical(WizardUtils.BGStyle);
            EditorGUILayout.LabelField("Color Mask (Optional)", WizardUtils.BoldTextStyle);
            _mask = (Texture2D)EditorGUILayout.ObjectField(_mask, typeof(Texture2D), false);
            EditorGUILayout.EndVertical();
            //draw mask..

            EditorGUILayout.Space();

            //..draw template category
            EditorGUILayout.BeginVertical(WizardUtils.BGStyle);
            EditorGUILayout.LabelField("Template", WizardUtils.BoldTextStyle);
            _stemp = EditorGUILayout.IntPopup(_stemp, _templatestring, _templateint);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Used Parts", WizardUtils.BoldTextStyle);
            for (int i = 0; i < _tempopt[skey].Length; i++)
                _tempopt[skey][i].option =
                    EditorGUILayout.Toggle(_tempopt[skey][i].spriteName, _tempopt[skey][i].option);
            EditorGUILayout.EndVertical();
            //draw template category..

            EditorGUILayout.Space();

            //..draw body type
            EditorGUILayout.BeginVertical(WizardUtils.BGStyle);
            EditorGUILayout.LabelField("Compatible Body Type", WizardUtils.BoldTextStyle);
            for (int i = 0; i < _bodyopt.Length; i++)
                _bodyopt[i] = EditorGUILayout.Toggle(_bodytype[i].ToString(), _bodyopt[i]);
            EditorGUILayout.EndVertical();
            //draw body type..

            return val;
        }

        void OnWizardCreate()
        {
            createPart();
        }

        protected void createPart()
        {
            try
            {
                string filename = "";
                if (_templates[_stemp] is WeaponTemplate)
                {
                    WeaponTemplate twtemplate = (WeaponTemplate)_templates[_stemp];
                    filename = WizardUtils.PartFolder + "/" + _packstring[_spack] + "/Weapon/" + getDirectoryName(twtemplate.weaponCategory) + "/" + _partname + ".asset";
                }
                else
                    filename = WizardUtils.PartFolder + "/" + _packstring[_spack] + "/" + getDirectoryName(_templates[_stemp].category) + "/" + _partname + ".asset";

                string directory = Path.GetDirectoryName(filename);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    AssetDatabase.Refresh();
                }

                if (File.Exists(filename))
                {
                    if (!EditorUtility.DisplayDialog("Replace Part", "the part you wish to create is already exist", "Replace", "Cancel"))
                        return;
                }

                Part tpart = _templates[_stemp] is WeaponTemplate ? EditorUtils.LoadScriptable<Weapon>(filename) :
                    EditorUtils.LoadScriptable<Part>(filename);

                //..category
                tpart.category = _templates[_stemp].category;
                //category..

                //..package
                tpart.packageName = getPackageName(filename);
                //package..

                //..bodytype
                tpart.supportedBody = new List<BodyType>();
                for (int i = 0; i < _bodyopt.Length; i++)
                {
                    if (_bodyopt[i])
                        tpart.supportedBody.Add(_bodytype[i]);
                }
                //bodytype..

                //..textures
                tpart.texture = _texture;
                tpart.colorMask = _mask;
                //textures..

                //..sprites
                if (_texture != null && _templates[_stemp].category != PartCategory.SkinDetails) //..skin details doesn't need to be sliced
                {
                    string sourcepath = AssetDatabase.GetAssetPath(_templates[_stemp].texture);
                    string targetpath = AssetDatabase.GetAssetPath(_texture);
                    tpart.sprites = SpriteSlicerUtils.SliceSprite(sourcepath, targetpath, getExcludedSprite());
                }
                else
                    tpart.sprites = new List<Sprite>();
                //sprites..

                if (tpart is Weapon)
                {
                    WeaponTemplate twtemplate = (WeaponTemplate)_templates[_stemp];
                    Weapon tweapon = (Weapon)tpart;
                    tweapon.weaponCategory = twtemplate.weaponCategory;
                    EditorUtility.SetDirty(tweapon);
                }
                else
                    EditorUtility.SetDirty(tpart);

                if (PartList.Static != null)
                    InspectorPartList.Refresh(PartList.Static);

                Selection.activeObject = tpart;

                string message = "part '" + tpart.name + "' has successfully created";
                EditorUtility.DisplayDialog("Success", message, "OK");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", "error on creating part: " + e.ToString(), "OK");
            }
        }

        private string[] getExcludedSprite()
        {
            TemplateOption[] options = _tempopt[skey];
            List<string> val = new List<string>();
            foreach (TemplateOption o in options)
            {
                if (!o.option)
                    val.Add(o.spriteName);
            }

            return val.ToArray();
        }

        private Sprite findSprite(string spriteName, Sprite[] spriteList)
        {
            foreach (Sprite s in spriteList)
            {
                if (s.name == spriteName)
                    return s;
            }
            return null;
        }

        private string getPackageName(string fileName)
        {
            string[] directories = Directory.GetDirectories(WizardUtils.PartFolder);
            foreach (string d in directories)
            {
                string packagename = Path.GetFileNameWithoutExtension(d);
                if (fileName.Contains(packagename))
                    return packagename;
            }

            return "";
        }

        private string getDirectoryName(PartCategory partCategory)
        {
            switch (partCategory)
            {
                case PartCategory.FacialHair:
                    return "Facial Hair";
                case PartCategory.SkinDetails:
                    return "Skin Details";
                default:
                    return "" + partCategory;
            }
        }

        private string getDirectoryName(WeaponCategory weaponCategory)
        {
            switch (weaponCategory)
            {
                case WeaponCategory.OneHanded:
                    return "One Handed";
                case WeaponCategory.TwoHanded:
                    return "Two Handed";
                default:
                    return "" + weaponCategory;
            }
        }
    }
}