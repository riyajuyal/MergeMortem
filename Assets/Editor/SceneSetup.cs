using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using MergeMortem;

namespace MergeMortemEditor
{
    public static class SceneSetup
    {
        const string ScenePath = "Assets/Scenes/Main.unity";

        [MenuItem("MergeMortem/Create Main Scene")]
        public static void CreateMainScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            Camera cam = cameraGO.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.29f, 0.33f, 0.41f, 1f);
            cam.orthographic = true;
            cameraGO.tag = "MainCamera";

            GameObject esGO = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            GameObject bootstrapGO = new GameObject("Bootstrap", typeof(Bootstrap));

            string dir = Path.GetDirectoryName(ScenePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();

            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = scenes;

            Debug.Log("Main scene created at " + ScenePath);
        }
    }
}
