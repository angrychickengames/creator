using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharacterCreator2D
{
    public static class CharacterUtils
    {
        private static string _deffolder = Application.dataPath;

        /// <summary>
        /// [EDITOR] Display save file dialog.
        /// </summary>
        /// <param name="title">Title to display.</param>
        /// <param name="defaultName">The default name of the file.</param>
        /// <param name="extension">The extension of the file.</param>
        /// <returns>Selected path if there is, otherwise returns 'empty'</returns>
		public static string ShowSaveFileDialog(string title, string defaultName, string extension)
        {
            string val = "";
#if UNITY_EDITOR
            val = EditorUtility.SaveFilePanel(title, _deffolder, defaultName, extension);
            if (string.IsNullOrEmpty(val))
                return "";
            val = getAssetPath(val);
#endif
            return val;
        }

        /// <summary>
        /// [EDITOR] Display open file dialog.
        /// </summary>
        /// <param name="title">Title to display.</param>
        /// <param name="extension">The extension of the file.</param>
        /// <returns>Selected path if there is any, otherwise returns 'empty'</returns>
		public static string ShowOpenFileDialog(string title, string extension)
        {
            string val = "";
#if UNITY_EDITOR
            val = EditorUtility.OpenFilePanel(title, _deffolder, extension);
            if (string.IsNullOrEmpty(val))
                return "";
            val = getAssetPath(val);
#endif
            return val;
        }

        /// <summary>
        /// [Editor] Instantiate a CharacterViewer based on another CharacterViewer.
        /// </summary>
        /// <param name="baseCharacter">CharacterViewer to be copied from.</param>
        /// <param name="prefabPath">Path to the CharacterViewer prefab in Assets directory.</param>
        /// <returns>Instantiated CharacterViewer with the same settings as baseCharacter that linked to prefab in prefabPath. Returns an instantiated object of baseCharacter if there is no CharacterViewer found in prefabPath.</returns>
        public static CharacterViewer SpawnCharacter(CharacterViewer baseCharacter, string prefabPath)
        {
            CharacterViewer val = null;
#if UNITY_EDITOR
            CharacterViewer extcharacter = AssetDatabase.LoadAssetAtPath<CharacterViewer>(prefabPath);
            if (extcharacter != null)
            {
                GameObject tgo = (GameObject)PrefabUtility.InstantiatePrefab(extcharacter);
                val = tgo.GetComponent<CharacterViewer>();
                val.name = Path.GetFileNameWithoutExtension(prefabPath);
                copyCharacter(baseCharacter, val);
                return val;
            }
#endif
            val = GameObject.Instantiate<CharacterViewer>(baseCharacter);
            val.name = Path.GetFileNameWithoutExtension(prefabPath);
            return val;
        }

        /// <summary>
        /// [EDITOR] Save a CharacterViewer as a prefab in a selected path and automatically generate its materials.
        /// </summary>
        /// <param name="character">CharacterViewer to be saved as prefab.</param>
        /// <param name="path">The path must be in Assets directory.</param>
        public static void SaveCharacterToPrefab(CharacterViewer character, string path)
        {
#if UNITY_EDITOR
            try
            {
                List<Material> materials = new List<Material>();

                //..get all character's materials
                SlotCategory[] cats = (SlotCategory[])Enum.GetValues(typeof(SlotCategory));
                foreach (SlotCategory c in cats)
                {
                    PartSlot slot = character.slots.GetSlot(c);
                    if (slot == null || slot.material == null || materials.Contains(slot.material))
                        continue;
                    materials.Add(slot.material);
                }
                //get all character's materials..

                string materialfolder = Path.GetDirectoryName(path) + "/" + character.name + "_materials";
                if (!Directory.Exists(materialfolder))
                {
                    Directory.CreateDirectory(materialfolder);
                    AssetDatabase.Refresh();
                }

                foreach (Material m in materials)
                {
                    string filename = materialfolder + "/" + m.name + ".mat";
                    AssetDatabase.CreateAsset(m, filename);
                }
                AssetDatabase.Refresh();

                SpriteRenderer[] renderers = character.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (SpriteRenderer r in renderers)
                {
                    if (r.material == null)
                        continue;
                }

                character.RelinkMaterials();

                foreach (Material m in materials)
                {
                    string oldfilename = materialfolder + "/" + m.name + ".mat";
                    string newfilename = m.name.Replace("CC2D", character.name);
                    AssetDatabase.RenameAsset(oldfilename, newfilename);
                }
                AssetDatabase.Refresh();

                CharacterViewer extcharacter = AssetDatabase.LoadAssetAtPath<CharacterViewer>(path);
                if (extcharacter != null)
                    PrefabUtility.ReplacePrefab(character.gameObject, extcharacter);
                else
                    PrefabUtility.CreatePrefab(path, character.gameObject, ReplacePrefabOptions.ConnectToPrefab);
            }
            catch (System.Exception e)
            {
                Debug.LogError("error on save character to prefab:\n" + e.ToString());
            }
#endif
        }

        private static void copyCharacter(CharacterViewer sourceChara, CharacterViewer targetChara)
        {
            CharacterData sdata = sourceChara.GenerateCharacterData();
            targetChara.AssignCharacterData(sdata);
        }

        /// <summary>
        /// [EDITOR] Load CharacterViewer from a given path.
        /// </summary>
        /// <param name="path">The path must be in Assets directory.</param>
        /// <returns>'true' on succes, otherwise returns 'false'.</returns>
        public static CharacterViewer LoadCharacterFromPrefab(string path)
        {
            CharacterViewer val = null;
#if UNITY_EDITOR
            try
            {
                val = AssetDatabase.LoadAssetAtPath<CharacterViewer>(path);
            }
            catch (Exception e)
            {
                Debug.LogError("error on load from prefab:\n" + e.ToString());
            }
#endif
            return val;
        }

        private static string getAssetPath(string completePath)
        {
            string val = "";

#if UNITY_EDITOR
            if (completePath.Contains(Application.dataPath)) //..jika path contains project path
            {
                int assetindex = completePath.IndexOf("Assets/");
                val = completePath.Substring(assetindex);
            }
#endif

            return val;
        }
    }
}