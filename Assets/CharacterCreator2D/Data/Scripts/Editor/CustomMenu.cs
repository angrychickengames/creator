using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using CharacterCreator2D;

namespace CharacterEditor2D
{
    public static class CustomMenu
    {

        // [MenuItem("Window/Character Creator 2D/Setup Data")]
        public static void ShowRuntimeSettings()
        {
            Selection.activeObject = EditorUtils.LoadScriptable<SetupData>(WizardUtils.SetupDataPath);
        }

        // [MenuItem("Window/Character Creator 2D/Part List")]
        public static void ShowPartList()
        {
            Selection.activeObject = EditorUtils.LoadScriptable<PartList>(WizardUtils.PartListPath);
        }

        [MenuItem("Window/Character Creator 2D/Add New Part")]
        public static void CreatePart()
        {
            ScriptableWizard.DisplayWizard<CharacterEditor2D.WizardPart>("Add Part", "Create");
        }
    }

    [InitializeOnLoad]
    public class PlayCreatorScene
    {
        static PlayCreatorScene()
        {
            EditorApplication.playmodeStateChanged += OnPlayModeChanged;
        }

        static string startScene = "Assets/CharacterCreator2D/Creator UI/Creator UI.unity";
        // static public SceneSetup[] sceneSetup;
        static string prevScene = EditorPrefs.GetString("PlayFromStartPrevScene");
        static bool active = EditorPrefs.GetBool("PlayFromStartActive", false);

        [MenuItem("Window/Character Creator 2D/Create Character")]
        static void Play()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // sceneSetup = EditorSceneManager.GetSceneManagerSetup();
                if (PartList.Static != null)
                {
                    InspectorPartList.RefreshPartPackage();
                    InspectorPartList.Refresh(PartList.Static);
                }
                EditorPrefs.SetString("PlayFromStartPrevScene", EditorSceneManager.GetActiveScene().path);
                EditorSceneManager.OpenScene(startScene);
                EditorPrefs.SetBool("PlayFromStartActive", true);
                EditorApplication.isPlaying = true;
            }
        }

        private static void OnPlayModeChanged()
        {
            if (!active) return;
            if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.update += RestoreScene;
            }
        }

        public static void RestoreScene()
        {
            if (EditorApplication.isPlaying) return;
            // if(sceneSetup == null){
            if (prevScene == null || prevScene == "")
            {
                EditorApplication.update -= RestoreScene;
                return;
            }
            // EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
            EditorSceneManager.OpenScene(prevScene);
            EditorPrefs.DeleteKey("PlayFromStartPrevScene");
            EditorPrefs.DeleteKey("PlayFromStartActive");
            EditorApplication.update -= RestoreScene;
        }
    }
}