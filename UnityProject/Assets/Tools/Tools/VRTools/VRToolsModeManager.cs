using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Change VRTools mode in runtime. Disable unused dependant scripts (MiddleVR | ViconSDK | Unity).
/// All dependant scripts must be activated in order to be disable and reactivated.
/// Inactive scripts are discarded. Reactive scripts are recorded during the desactivation.
/// 
/// Mode priority : 
/// - Executable Argument (UNITY | MIDDLEVR | VICONSDK)
/// - Start with MiddleVR (--config)
/// - Select mode on Editor
/// 
///  Script execution order (at least for initialization)
///  - Before VRManagerScript and every script which use MiddleVR
///  - Before ViconSDKManager and every script which use ViconSDK 
/// </summary>
public class VRToolsModeManager : MonoBehaviour
{
    public VRToolsMode vrToolsMode;

    List<MonoBehaviour> middleVRComponents = new List<MonoBehaviour>();
    List<MonoBehaviour> viconSDKComponents = new List<MonoBehaviour>();
    List<MonoBehaviour> unityComponents = new List<MonoBehaviour>();

    bool middleVRComponentsState = true;
    bool viconSDKComponentsState = true;
    bool unityComponentsState = true;

    GameObject middleVRManagerGameObject;
    GameObject middleVRSystemCenterGameObject;
    GameObject middleVRWandGameObject;
    GameObject viconSDKgameObject;
    Camera mainCamera;

    const string UNITY = "UNITY";
    const string MIDDLEVR = "MIDDLEVR";
    const string VICONSDK = "VICONSDK";

    [ContextMenu("Force Update Mode")]
    void OnValidate()
    {
        //Only on runtime.
        if (Application.isPlaying)
        {
            DisableUnusedEnableUsedScripts();
            VRTools.SetVRToolsMode(vrToolsMode);
        }
    }

    void Awake()
    {
        mainCamera = Camera.main;

        if (SetModeFromExecutableArgument()) { }

#if MIDDLEVR
        else if (isStartWithMiddleVR())
        {
            Debug.Log("[VRTools] Applicatio started with MiddelVR. Change VRTools mode to MiddleVR.");
            vrToolsMode = VRToolsMode.MIDDLEVR;
            VRTools.SetVRToolsMode(vrToolsMode);
        }
#endif
        DisableUnusedEnableUsedScripts();
        VRTools.SetVRToolsMode(vrToolsMode);
    }

    bool SetModeFromExecutableArgument()
    {
        HashSet<string> arguments = new HashSet<string>(System.Environment.GetCommandLineArgs());

        if (arguments.Contains(UNITY))
        {
            ChangeMode(VRToolsMode.UNITY);
            return true;
        }
#if MIDDLEVR
        else if (arguments.Contains(MIDDLEVR))
        {
            ChangeMode(VRToolsMode.MIDDLEVR);
            return true;
        }
#endif
#if VICONSDK
        else if (arguments.Contains(VICONSDK))
        {
            ChangeMode(VRToolsMode.VICONSDK);
            return true;
        }
#endif
        
        return false;
    }

    void ChangeMode(VRToolsMode vrToolsMode)
    {
        this.vrToolsMode = vrToolsMode;
        VRTools.SetVRToolsMode(vrToolsMode);
        DisableUnusedEnableUsedScripts();
    }

    void DisableUnusedEnableUsedScripts()
    {
#if MIDDLEVR
        FindMiddleVRGameObject();
        if (vrToolsMode == VRToolsMode.MIDDLEVR)
            EnableMiddleVRScripts();
        else
            DisableMiddleVRScripts();
#endif

#if VICONSDK
        FindViconSDKScript();
        if (vrToolsMode == VRToolsMode.VICONSDK)
            EnableViconSDKScripts();
        else
            DisableViconSDKScripts();
#endif

        if (vrToolsMode == VRToolsMode.UNITY)
            EnableUnityScripts();
        else
            DisableUnityScripts();
    }


#if MIDDLEVR
    /// <summary>
    /// Search if the application was started with MiddleVR.
    /// </summary>
    /// <returns>True if started with MiddleVR</returns>
    bool isStartWithMiddleVR()
    {
        HashSet<string> parameters = new HashSet<string>(System.Environment.GetCommandLineArgs());
        return parameters.Contains("--config");
    }

    void FindMiddleVRGameObject()
    {
        VRManagerScript vrManager = FindObjectOfType<VRManagerScript>();
        if (vrManager != null)
        {
            middleVRManagerGameObject = vrManager.gameObject;
            middleVRSystemCenterGameObject = (vrManager.VRSystemCenterNode != null) ?
                vrManager.VRSystemCenterNode :
                GameObject.Find("VRSystemCenterNode");
     
            VRWand vrWand = FindObjectOfType<VRWand>();
            if (vrWand != null)
                middleVRWandGameObject = vrWand.gameObject;
        }
    }

    void DisableMiddleVRScripts()
    {
        if (middleVRComponentsState)
        {
            ChangeGameObjectState(middleVRSystemCenterGameObject, false);
            ChangeGameObjectState(middleVRManagerGameObject, false);
            ChangeGameObjectState(middleVRWandGameObject, false);

            string[] types =
            { 
                "VRClusterObject",  
                "VRApplySharedTransform",
                "VRShareTransform",
                "VRWebView",
                "VRActor",
                "VRAttachToNode",
                "VRPhysicsBody",
                "VRPhysicsBodyManipulatorIPSI",
                "VRPhysicsConstraintBallSocket",
                "VRPhysicsConstraintCylindrical",
                "VRPhysicsConstraintFixed",
                "VRPhysicsConstraintHelical",
                "VRPhysicsConstraintHinge",
                "VRPhysicsConstraintPlanar",
                "VRPhysicsConstraintPrismatic",
                "VRPhysicsConstraintUJoint",
                "VRPhysicsDeactivateAllContacts",
                "VRPhysicsDisableAllCollisions",
                "VRPhysicsDisableCollisions",
                "VRPhysicsEnableCollisions",
                "VRPhysicsShowContacts",
                "VRFPSInputController",
                "AssignMiddleVRNodeTransform",
                "AddHeadNodeAudioListener"
            };

            middleVRComponents = GetAllActivesMBFromTypes(types);

            foreach (MonoBehaviour mb in middleVRComponents)
                mb.enabled = false;

            middleVRComponentsState = false;
            Debug.Log("[VRTools] Disable " + middleVRComponents.Count + " MiddleVR scripts.");
        }
    }

    void EnableMiddleVRScripts()
    {
        if (!middleVRComponentsState)
        {
            ChangeGameObjectState(middleVRManagerGameObject, true);
            ChangeGameObjectState(middleVRSystemCenterGameObject, true);
            ChangeGameObjectState(middleVRWandGameObject, true);

            mainCamera.enabled = false;
            
            foreach (MonoBehaviour mb in middleVRComponents)
                mb.enabled = true;

            middleVRComponentsState = true;
            Debug.Log("[VRTools] Enable " + middleVRComponents.Count + " MiddleVR scripts.");
        }
    }
#endif

#if VICONSDK
    void FindViconSDKScript()
    {
        ViconSDKManager viconManager = FindObjectOfType<ViconSDKManager>();
        if (viconManager != null)
            viconSDKgameObject = viconManager.gameObject;
    }

    void DisableViconSDKScripts()
    {
        if (viconSDKComponentsState)
        {
            ChangeGameObjectState(viconSDKgameObject, false);

            string[] types =
            { 
               "ChangeFOV"
            };

            viconSDKComponents = GetAllActivesMBFromTypes(types);
  
            foreach (MonoBehaviour mb in viconSDKComponents)
                mb.enabled = false;

            viconSDKComponentsState = false;
            Debug.Log("[VRTools] Disable " + viconSDKComponents.Count + " ViconSDK scripts.");
        }
    }

    void EnableViconSDKScripts()
    {
        if (!viconSDKComponentsState)
        {
            ChangeGameObjectState(viconSDKgameObject, true);

            mainCamera.enabled = true;

            foreach (MonoBehaviour mb in viconSDKComponents)
                mb.enabled = true;

            viconSDKComponentsState = true;
            Debug.Log("[VRTools] Enable " + viconSDKComponents.Count + " ViconSDK scripts.");
        }
    }
#endif

    void DisableUnityScripts()
    {
        if (unityComponentsState)
        {
            string[] types =
            { 
                "UnityStandardAssets.Characters.FirstPerson.FirstPersonController",
                "FPSInputController",
                "MouseLook",
                "SetUpUnityCamera"
            };

            unityComponents = GetAllActivesMBFromTypes(types);

            foreach (MonoBehaviour mb in unityComponents)
                mb.enabled = false;

            unityComponentsState = false;
            Debug.Log("[VRTools] Disable " + unityComponents.Count + " Unity scripts.");
        }
    }

    void EnableUnityScripts()
    {
        if (!unityComponentsState)
        {
            mainCamera.enabled = true;

            foreach (MonoBehaviour mb in unityComponents)
                mb.enabled = true;

            unityComponentsState = true;
            Debug.Log("[VRTools] Enable " + unityComponents.Count + " Unity scripts.");
        }
    }

    void ChangeGameObjectState(GameObject go, bool state)
    {
        if (go != null)
            go.SetActive(state);
    }

    List<MonoBehaviour> GetAllActivesMBFromTypes(string[] types)
    {
        List<MonoBehaviour> activesMBs = new List<MonoBehaviour>();
        foreach (string strType in types)
        {
            System.Type type = TypeFromString(strType);
            if (type != null)
            {
                MonoBehaviour[] mbs = FindObjectsOfType(type) as MonoBehaviour[];
                foreach (MonoBehaviour mb in mbs)
                    if (mb.enabled)
                        activesMBs.Add(mb);
            }
        }
        return activesMBs;
    }

    System.Type TypeFromString(string strType)
    {
        Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly asm in assemblies)
        {
            System.Type type = asm.GetType(strType);
            if (type != null)
                return type;
        }
        return null;
    }
}
