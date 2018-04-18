using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
    public class UICreator : MonoBehaviour
    {
        /// <summary>
        /// CharacterViewer displayed by this UICreator.
        /// </summary>
        [Tooltip("CharacterViewer displayed by this UICreator")]
        public CharacterViewer character;

        /// <summary>
        /// UIColor managed by this UICreator.
        /// </summary>
        [Tooltip("UIColor managed by this UICreator")]
        public UIColor colorUI;

        /// <summary>
        /// RuntimeDialog used by this UICreator.
        /// </summary>
        [Tooltip("RuntimeDialog used by this UICreator")]
        public RuntimeDialog dialog;

        /// <summary>
        /// JSON file location of this character in Build mode.
        /// </summary>
        [Tooltip("JSON file location of this character in build mode")]
        public string JSONRuntimePath;

        private bool _processing = false;

        void Awake()
        {
            _processing = false;
            BodyTypeGroup bodygroup = this.transform.GetComponentInChildren<BodyTypeGroup>(true);
            bodygroup.Initialize();
            PartGroup[] partgroups = this.transform.GetComponentsInChildren<PartGroup>(true);
            foreach (PartGroup g in partgroups)
                g.Initialize();
        }

        /// <summary>
        /// [EDITOR] Save character as a prefab in desired path.
        /// </summary>
        public void SaveCharacterAsPrefab()
        {
            if (this.character == null || _processing)
                return;

            StartCoroutine("ie_savecharaasprefab");
        }

        IEnumerator ie_savecharaasprefab()
        {
            _processing = true;
            yield return null;
#if UNITY_EDITOR
            string path = CharacterUtils.ShowSaveFileDialog("Save Character", "New Character", "prefab");
            if (!string.IsNullOrEmpty(path))
            {
                CharacterViewer tcharacter = CharacterUtils.SpawnCharacter(this.character, path);
                yield return null;
                yield return null;
                CharacterUtils.SaveCharacterToPrefab(tcharacter, path);
                yield return null;
                yield return null;
                Destroy(tcharacter.gameObject);
                dialog.Show("Save Character", "'" + tcharacter.name + "' is saved");
            }
#endif
            _processing = false;
        }

        /// <summary>
        /// [EDITOR] Load character from a prefab.
        /// </summary>
        public void LoadCharacterFromPrefab()
        {
            _processing = true;
#if UNITY_EDITOR
            string path = CharacterUtils.ShowOpenFileDialog("Load Character", "prefab");
            if (!string.IsNullOrEmpty(path))
            {
                CharacterViewer tcharacter = CharacterUtils.LoadCharacterFromPrefab(path);
                if (tcharacter == null)
                    return;

                CharacterData data = tcharacter.GenerateCharacterData();
                this.character.AssignCharacterData(data);
                dialog.Show("Load Character", "'" + tcharacter.name + "' is loaded");
            }
#endif
            _processing = false;
        }

        /// <summary>
        /// Save character's data as JSON file. Calling this function will save character's data with path defined in JSONRuntimePath field.
        /// </summary>
        public void SaveCharacterToJSON()
        {
            _processing = true;
            string path = "";

#if UNITY_EDITOR
            path = CharacterUtils.ShowSaveFileDialog("Save Character", "New Character Data", "json");
#else
			path = this.JSONRuntimePath;
#endif

            if (!string.IsNullOrEmpty(path))
            {
                this.character.SaveToJSON(path);
                dialog.Show("Save Character", "'" + System.IO.Path.GetFileNameWithoutExtension(path) + "' is saved");
            }
            _processing = false;
        }

        /// <summary>
        /// Load character from JSON file's data. Calling this function will try to load character's data from a path defined in JSONRuntimePath field.
        /// </summary>
        public void LoadCharacterFromJSON()
        {
            _processing = true;
            string path = "";

#if UNITY_EDITOR
            path = CharacterUtils.ShowOpenFileDialog("Load Character", "json");
#else
			path = this.JSONRuntimePath;
#endif

            if (!string.IsNullOrEmpty(path))
            {
                this.character.LoadFromJSON(path);
                dialog.Show("Load Character", "'" + System.IO.Path.GetFileNameWithoutExtension(path) + "' is loaded");
            }
            _processing = false;
        }
    }
}