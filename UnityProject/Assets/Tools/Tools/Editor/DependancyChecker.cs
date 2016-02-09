using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// Add define symbol to unity if the dependancy are found for conditionnal code.
/// Based on folder name in Assets folder.
/// </summary>
class DependancyChecker : AssetPostprocessor
{
    const string ASSETS = "Assets/";
    const string MIDDLEVR = "MiddleVR";
    const string VICON_SDK = "ViconSDK";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (AssetDatabase.IsValidFolder(ASSETS + MIDDLEVR))
            addDefine(MIDDLEVR.ToUpper());

        if (AssetDatabase.IsValidFolder(ASSETS+ VICON_SDK))
            addDefine(VICON_SDK.ToUpper());
    } 

    static void addDefine(string define)
    {
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (!defineSymbols.Contains(define))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSymbols + ";" + define);
            Debug.Log("[VRTools] Add define " + define + " symbol to group : " + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
        }
    }
}