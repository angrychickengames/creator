using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CharacterCreator2D;

namespace CharacterEditor2D
{
    [CustomEditor(typeof(CharacterViewer))]
    public class CharacterViewerEditor : Editor
    {
        private CharacterViewer _character;
        private SerializedProperty _instancemat;
        private SerializedProperty _tintcolor;
        private SerializedProperty _bodytype;
        private SerializedProperty _skincolor;
        private SerializedProperty _slots;

        private readonly GUIContent _isinstgui = new GUIContent()
        {
            text = "Instantiate Material",
            tooltip = "'true' if you want to instantiate materials for each character when the application is playing"
        };
        private readonly GUIContent _tintgui = new GUIContent()
        {
            text = "Tint Color",
            tooltip = "Character's tint color"
        };
        private readonly GUIContent _bodygui = new GUIContent()
        {
            text = "Body Type",
            tooltip = "Character's body type"
        };
        private readonly GUIContent _skingui = new GUIContent()
        {
            text = "Skin Color",
            tooltip = "Character's skin color"
        };
        private readonly GUIContent _slotsgui = new GUIContent()
        {
            text = "Slots",
            tooltip = "Display current parts used by the Character"
        };

        void OnEnable()
        {
            _character = (CharacterViewer)target;
            _bodytype = serializedObject.FindProperty("bodyType");
            _tintcolor = serializedObject.FindProperty("_tintcolor");
            _skincolor = serializedObject.FindProperty("_skincolor");
            _instancemat = serializedObject.FindProperty("instanceMaterials");
            _slots = serializedObject.FindProperty("slots");
            HideInHierarchy(_character);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Color tcolor = _character.TintColor;
            Color scolor = _character.SkinColor;
            EditorGUILayout.PropertyField(_instancemat, _isinstgui);
            EditorGUILayout.PropertyField(_tintcolor, _tintgui);
            EditorGUILayout.LabelField("Character's Data", WizardUtils.BoldTextStyle);
            EditorGUILayout.PropertyField(_bodytype, _bodygui);
            EditorGUILayout.PropertyField(_skincolor, _skingui);
            EditorGUILayout.PropertyField(_slots, _slotsgui, true);

            if (GUILayout.Button("Refresh"))
                _character.Initialize();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Child"))
                ShowInHierarchy(_character);
            if (GUILayout.Button("Hide Child"))
                HideInHierarchy(_character);
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();

            if (tcolor != _tintcolor.colorValue)
                _character.RepaintTintColor();

            if (scolor != _skincolor.colorValue)
                _character.RepaintSkinColor();
        }

        void ShowInHierarchy(CharacterViewer character)
        {
            Transform[] child = character.GetComponentsInChildren<Transform>();
            foreach (Transform c in child)
            {
                c.gameObject.hideFlags = HideFlags.None;
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        void HideInHierarchy(CharacterViewer character)
        {
            Transform[] child = character.GetComponentsInChildren<Transform>();
            foreach (Transform c in child)
            {
                if (c != child[0])
                {
                    c.gameObject.hideFlags = HideFlags.HideInInspector;
                    c.gameObject.hideFlags ^= HideFlags.HideInHierarchy;
                }
            }
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.DirtyHierarchyWindowSorting();
        }
    }
}