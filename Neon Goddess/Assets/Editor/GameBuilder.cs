using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class GameBuilder : MonoBehaviour
{
    [MenuItem("Build/Build Windows")]
    public static void PerformWindowsBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = new[] {"Assets/_Prototypes/2. Movement/Scenes/MovementPrototype.unity"};
        buildPlayerOptions.locationPathName = "build/Windows/NeonGoddess/NeonGoddess.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build Succeeded: " + summary.totalSize + " bytes");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build Failed");
        }
    }
}
