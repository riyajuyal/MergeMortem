using System.IO;
using UnityEditor;
using UnityEngine;

namespace MergeMortemEditor
{
    public static class BuildScript
    {
        const string ScenePath = "Assets/Scenes/Main.unity";
        const string OutputDir = "docs";

        public static void BuildWebGL()
        {
            if (!File.Exists(ScenePath))
            {
                SceneSetup.CreateMainScene();
            }

            PlayerSettings.productName = "MergeMortem";
            PlayerSettings.companyName = "MergeMortem";

            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;

            EditorUserBuildSettings.development = false;

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

            if (Directory.Exists(OutputDir))
            {
                Directory.Delete(OutputDir, true);
            }
            Directory.CreateDirectory(OutputDir);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputDir,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(options);

            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError("WebGL build failed: " + report.summary.result);
                EditorApplication.Exit(1);
            }
            else
            {
                Debug.Log("WebGL build succeeded. Output: " + OutputDir);
                EditorApplication.Exit(0);
            }
        }
    }
}
