using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityMake;

[CreateAssetMenu(fileName = "ItchIoUploadBuild", menuName = "UMake/Action/ItchIoUploadBuild")]
public class ItchIoUploadBuild : UMakeBuildAction
{
    public string contentSubFolder = "Windows";
    public string Channel = "windows";
    public string UserName = "cottoncatstudios";
    public string ProjectName = "neo-neon-goddess-closed";

    public override void Execute(UMake umake, UMakeTarget target)
    {
        try
        {
            string buildPath = UMake.GetBuildPath() + "/" + contentSubFolder;
            var proc1 = new ProcessStartInfo();
            proc1.UseShellExecute = true;
            proc1.WorkingDirectory = @"C:\Windows\System32";
            proc1.FileName = "cmd.exe";
            proc1.Arguments = $"/c butler push {buildPath} {UserName}/{ProjectName}:{Channel}";
            proc1.WindowStyle = ProcessWindowStyle.Normal;
            var process = new Process();
            process.StartInfo = proc1;
            process.Start();
            process.WaitForExit();
            process.Close();
            Debug.LogFormat("Uploading Build from path: \"{0}\"...", buildPath);
        }
        catch (System.Exception e)
        {
            Debug.Log("Upload to Itch Io failed.");
            Debug.LogException(e);
        }
    }
}