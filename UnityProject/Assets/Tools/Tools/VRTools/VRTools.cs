using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

#if MIDDLEVR
using MiddleVR_Unity3D;
#endif

public enum VRToolsMode
{
    UNITY,
#if MIDDLEVR
    MIDDLEVR,
#endif
#if VICONSDK
    VICONSDK
#endif
}

/// <summary>
/// MVR tools. Provides abstraction to interface with MiddleVR creating an
/// abstraction layer between MiddleVR API and the Unity app.
/// 
/// It provides: 
/// 
/// Methods to acces input data from MVR devices (buttons, axis, keyboard).
/// Methods to cluster methods like isMaster() isClient()
/// Methods to create sync objects (buttons, axis)
/// 
/// </summary>
public class VRTools : Singleton<VRTools>
{

    static VRToolsMode mode;
    public const string UNITY = "UNITY";
    public const string MIDDLEVR = "MIDDLEVR";
    public const string VICONSDK = "VICONSDK";

    static IVRTools modeInstance;

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Non-Static
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Change current VRTools mode.
    /// <param name="vrMode">New VRTools mode.</param>
    /// </summary>
    public static void SetVRToolsMode(VRToolsMode vrMode)
    {
        mode = vrMode;
        ChangeMode();
    }

    /// <summary>
    /// Intializes all the devices defined in the MiddleVR configuration
    /// </summary>
    static VRTools()
    {
        ChangeMode();
    }

    static void ChangeMode()
    {
           modeInstance = UnityVRTools.Instance;

#if MIDDLEVR
        if (mode == VRToolsMode.MIDDLEVR)
            modeInstance = MiddleVR_VRTools.Instance;
#endif

#if VICONSDK
        if (mode == VRToolsMode.VICONSDK)
            modeInstance = ViconSDK_VRTools.Instance;
#endif
    }


    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Button Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns whether a button is pressed or not
    /// </summary>
    /// <returns><c>true</c>, the button was pressed, <c>false</c> otherwise.</returns>
    /// <param name="button">Button Id</param>
    public static bool IsButtonPressed(uint button, int wand = 0)
    {
        return modeInstance.IsButtonPressed(button, wand);
    }

    /// <summary>
    /// Returns whether the toggle status of the button has changed or not
    /// </summary>
    /// <returns><c>true</c>, if button was toggled, <c>false</c> otherwise.</returns>
    /// <param name="button">Button Id.</param>
    /// <param name="wand">The interaction wand, 0 by defalt.</param>
    public static bool IsButtonToggled(uint button, int wand = 0)
    {
        return IsButtonToggled(button, false, wand);
    }

    /// <summary>
    /// Returns changes in the toogle state of the given button
    /// </summary>
    /// <returns><c>True</c>, if the toggled state has changed, <c>false</c> otherwise.</returns>
    /// <param name="button">Id button</param>
    /// <param name="pressed">Allows to detect when the button has been pressed (true) or released (false)</param>
    /// <param name="wand">The interaction wand, 0 by defalt.</param>
    public static bool IsButtonToggled(uint button, bool pressed, int wand = 0)
    {
        return modeInstance.IsButtonToggled(button, pressed, wand);
    }

    /// <summary>
    /// Returns whether the i-th button of a specific device is pressed.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool IsButtonPressed(string device, uint button)
    {
        return modeInstance.IsButtonPressed(device, button);
    }

    /// <summary>
    /// Returns the list of all the buttons pressed in the button device
    /// </summary>
    /// <param name="device"></param>
    public static List<uint> IsButtonPressed(string device)
    {
        return modeInstance.IsButtonPressed(device);
    }

#if MIDDLEVR
    /// <summary>
    /// Creates a new button device for synchorizing values among clients </summary>
    /// <returns>The pointer to the device</returns>
    /// <param name="name">Name of the device.</param>
    /// <param name="numButtons">Number of buttons of the device.</param>
    public static vrButtons CreateSyncButtonDevice(string name, uint numButtons)
    {
        if (MiddleVR.VRDeviceMgr == null) return null;

        vrButtons vrb = MiddleVR.VRDeviceMgr.CreateButtons(name);

        if (vrb != null)
        {
            vrb.SetButtonsNb(numButtons);

            MiddleVRTools.Log("[+] Created shared event button " + name);

            MiddleVR.VRClusterMgr.AddSynchronizedObject(vrb, 0);
        }
        else
        {
            MiddleVRTools.Log("[!] Error creating a shared event button " + name);
        }

        return vrb;
    }

    /// <summary>
    /// Enables the change of the button state of a vrButton. It should be used
    /// only for synch purposes. If the device does not exist it will create a
    /// new shared button device
    /// </summary>
    /// <param name="device">The string representing the device.</param>
    /// <param name="button">The button id which has to be updated.</param>
    /// <param name="state">The new state of the button</param>
    public static void SetButtonState(string device, uint button, bool state)
    {
        if (MiddleVR.VRDeviceMgr == null) return;

        vrButtons vrb = MiddleVR.VRDeviceMgr.GetButtons(device);

        if (vrb != null) vrb.SetPressedState(button, state);
    }

#endif

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Axis Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    public static float GetWandAxisValue(uint axis, int wand = 0)
    {
        return modeInstance.GetWandAxisValue(axis, wand);
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Keyboard Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns whether the key has been pressed
    /// </summary>
    /// <returns><c>true</c>, when the key has been pressed, <c>false</c> otherwise.</returns>
    /// <param name="key">Unity keycode</param>
    public static bool GetKeyDown(KeyCode key)
    {
        return modeInstance.GetKeyDown(key);
    }

    /// <summary>
    /// Returns whether the key has been released
    /// </summary>
    /// <returns><c>true</c>, the key has been released, <c>false</c> otherwise.</returns>
    /// <param name="key">Unity keycode</param>
    public static bool GetKeyUp(KeyCode key)
    {
        return modeInstance.GetKeyUp(key);
    }

    /// <summary>
    /// Returns whether the key is currently pressed
    /// </summary>
    /// <returns><c>true</c>, if the key is down, <c>false</c> otherwise.</returns>
    /// <param name="key">Unity keycode</param>
    public static bool GetKeyPressed(KeyCode key)
    {
        return modeInstance.GetKeyPressed(key);
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Static
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    public static VRTools GetInstance()
    {
        return Instance;
    }

    /// <summary>
    /// Calls the given callback with the instance when it's initialized.
    /// </summary>
    public static void GetInstance(System.Action<IVRTools> callback)
    {
        GetInstance().StartCoroutine(modeInstance.InitRoutine(callback));
    }

    /// <summary>
    /// Returns true if the unity client is a cluster client.
    /// </summary>
    /// <returns><c>true</c>, if client, <c>false</c> otherwise.</returns>
    public static bool IsClient()
    {
        return modeInstance.IsClient();
    }

    /// <summary>
    /// Returns true if the unity client is the master server or there is no cluster.
    /// </summary>
    /// <returns><c>true</c>, if client, <c>false</c> otherwise.</returns>
    public static bool IsMaster()
    {
        return modeInstance.IsMaster();
    }


    /// <summary>
    /// Returns true if the unity client is in cluster mode.
    /// </summary>
    /// <returns><c>true</c>, if cluster, <c>false</c> otherwise.</returns>
    public static bool IsCluster()
    {
        return modeInstance.IsCluster();
    }

    /// <summary>
    /// Gets the delta time. Must be used instead of Time.delaTime!
    /// </summary>
    /// <returns>The delta time.</returns>
    public static float GetDeltaTime()
    {
        return modeInstance.GetDeltaTime();
    }

    /// <summary>
    /// Gets the time at the beginning of the last frame. Must be used instead of Time.time!
    /// </summary>
    /// <returns>The delta time.</returns>
    public static float GetTime()
    {
        return modeInstance.GetTime();
    }

    /// <summary>
    /// The total number of frames that have passed since the beginning of the application.
    /// </summary>
    /// <returns>Frame number.</returns>
    public static uint GetFrameCount()
    {
        return modeInstance.GetFrameCount();
    }

    /// <summary>
    /// Get tracker absolute positions in meter if available.
    /// </summary>
    /// <param name="trackerName">Tracker name.</param>
    /// <returns>Tracker positions in meter or Vector3.zero if not available.</returns>
    public static Vector3 GetTrackerPosition(string trackerName)
    {
        return modeInstance.GetTrackerPosition(trackerName);
    }

    /// <summary>
    /// Get tracker absolute positions in meter if available.
    /// </summary>
    /// <param name="trackerName">Tracker name.</param>
    /// <param name="segmentName">Segment name (For Immermove Apex).</param>
    /// <returns>Tracker positions in meter or Vector3.zero if not available.</returns>
    public static Vector3 GetTrackerPosition(string trackerName, string segmentName)
    {
        return modeInstance.GetTrackerPosition(trackerName, segmentName);
    }

    /// <summary>
    /// Get tracker absolute orientation if available.
    /// </summary>
    /// <param name="trackerName">Tracker name.</param>
    /// <returns>Tracker orientation or Quaternion.identity if not available.</returns>
    public static Quaternion GetTrackerRotation(string trackerName)
    {
        return modeInstance.GetTrackerRotation(trackerName);
    }

    /// <summary>
    /// Get tracker absolute orientation if available.
    /// </summary>
    /// <param name="trackerName">Tracker name.</param>
    /// <param name="segmentName">Segment name (For Immermove Apex).</param>
    /// <returns>Tracker orientation or Quaternion.identity if not available.</returns>
    public static Quaternion GetTrackerRotation(string trackerName, string segmentName)
    {
        return modeInstance.GetTrackerRotation(trackerName, segmentName);
    }

    /// <summary>
    /// Get active mode.
    /// </summary>
    /// <returns>UNITY/MIDDLEVR/VICONSDK</returns>
    public static string GetMode()
    {
        return mode.ToString();
    }

    /// <summary>
    /// Use VRTools.WaitForSeconds instead of regular WaitForSeconds.
    /// 
    /// Usage:
    /// yield return StartCoroutine(VRTools.WaitForSeconds(5))
    /// Instead of:
    /// yield return new WaitForSeconds(5);
    /// </summary>
    /// <param name="timeToWait">Time to wait, in second.</param>
    /// <returns></returns>
    public static IEnumerator WaitForSeconds(float timeToWait)
    {
        float t = 0;
        while (t < timeToWait)
        {
            t += VRTools.GetDeltaTime();
            yield return null;
        }
    }
}

/// <summary>
/// Interface for VRTools features.
/// <see cref="VRTools">
/// </summary>
public interface IVRTools
{
    /// <see cref="VRTools.IsButtonPressed(uint, int)">
    bool IsButtonPressed(uint button, int wand = 0);

    /// <see cref="VRTools.IsButtonToggled(uint, int)">
    bool IsButtonToggled(uint button, int wand = 0);

    /// <see cref="VRTools.IsButtonToggled(uint, bool, int)">
    bool IsButtonToggled(uint button, bool pressed, int wand = 0);

    /// <see cref="VRTools.IsButtonPressed(string, uint)">
    bool IsButtonPressed(string device, uint button);

    /// <see cref="VRTools.IsButtonPressed(string)">
    List<uint> IsButtonPressed(string device);

    /// <see cref="VRTools.GetWandAxisValue(uint, int)">
    float GetWandAxisValue(uint axis, int wand = 0);

    /// <see cref="VRTools.GetKeyDown(UnityEngine.KeyCode)">
    bool GetKeyDown(KeyCode key);

    /// <see cref="VRTools.GetKeyUp(UnityEngine.KeyCode)">
    bool GetKeyUp(KeyCode key);

    /// <see cref="VRTools.GetInstance(System.Action<IVRTools>)">
    void GetInstance(System.Action<IVRTools> callback);

    /// <summary>
    /// Call callback only when fully initialized.
    /// </summary>
    /// <param name="resultCallback"></param>
    /// <returns></returns>
    IEnumerator InitRoutine(Action<IVRTools> resultCallback);

    /// <see cref="VRTools.GetKeyPressed(UnityEngine.KeyCode)">
    bool GetKeyPressed(KeyCode key);

    /// <see cref="VRTools.IsClient">
    bool IsClient();

    /// <see cref="VRTools.IsMaster">
    bool IsMaster();

    /// <see cref="VRTools.IsCluster">
    bool IsCluster();

    /// <see cref="VRTools.GetDeltaTime">
    float GetDeltaTime();

    /// <see cref="VRTools.GetTime">
    float GetTime();

    /// <see cref="VRTools.GetFrameCount">
    uint GetFrameCount();

    /// <see cref="VRTools.GetTrackerPosition(string)">
    Vector3 GetTrackerPosition(string trackerName);

    /// <see cref="VRTools.GetTrackerPosition(string, string)">
    Vector3 GetTrackerPosition(string trackerName, string segmentName);

    /// <see cref="VRTools.GetTrackerRotation(string)">
    Quaternion GetTrackerRotation(string trackerName);

    /// <see cref="VRTools.GetTrackerRotation(string, string)">
    Quaternion GetTrackerRotation(string trackerName, string segmentName);
}



#if MIDDLEVR

/// <summary>
/// MVR tools. Provides abstraction to interface with MiddleVR creating an
/// abstraction layer between MiddleVR API and the Unity app.
/// 
/// It provides: 
/// 
/// Methods to acces input data from MVR devices (buttons, axis, keyboard).
/// Methods to cluster methods like isMaster() isClient()
/// Methods to create sync objects (buttons, axis)
/// 
/// </summary>
public class MiddleVR_VRTools : Singleton<MiddleVR_VRTools>, IVRTools
{
    private static vrKeyboard _kb = null;
    private static List<vrButtons> _buttons = new List<vrButtons>();
    private static List<vrAxis> _axis = new List<vrAxis>();
    private static List<vrJoystick> _joysticks = new List<vrJoystick>();

    /// <summary>
    /// Controls whether the singleton has been initialized or not
    /// </summary>
    private static bool _init = false;


    /// <summary>
    /// Call Init() unity middlevr is fully configured. Wait one frame between each call.
    /// </summary>
    void Awake()
    {
        Instance.StartCoroutine(Instance.InitRoutine());
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Non-Static
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
   
    /// <summary>
    /// Intializes all the devices defined in the MiddleVR configuration
    /// </summary>
    private void Init()
    {
        bool initialisationIncomplete = false;
        if (MiddleVR.VRDeviceMgr != null)
        {
            for (uint w = 0; w < MiddleVR.VRDeviceMgr.GetWandsNb(); w++)
            {
                vrWand wand = MiddleVR.VRDeviceMgr.GetWandByIndex(w);

                vrButtons b = wand.GetButtons();

                if (b != null)
                {
                    _buttons.Add(b);

                    Debug.Log("[MVRTools] Add button device: " + b.GetName() + " with " + b.GetButtonsNb() + " buttons");
                }
                else
                {
                    Debug.LogWarningFormat("Could not find wand {0} buttons", wand.GetName());
                    initialisationIncomplete = true;
                }

                vrAxis axis = wand.GetAxis();

                if (axis != null)
                {
                    _axis.Add(axis);

                    Debug.Log("[MVRTools] Add axis device: " + axis.GetName() + " with " + axis.GetAxisNb() + " axes");
                }
                else
                {
                    Debug.LogWarningFormat("Could not find wand {0} axis", wand.GetName());
                    initialisationIncomplete = true;
                }
            }

            _kb = MiddleVR.VRDeviceMgr.GetKeyboard();
            if (_kb == null)
            {
                Debug.LogWarning("Could not find keyboard");
                initialisationIncomplete = true;
            }

            if (initialisationIncomplete)
                Debug.LogWarning("VRTool initialization incomplete. Maybe you tried to call a method too soon. Change script execution order or use GetInstance(Action callback)");
            else
                _init = true;
        }


    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Button Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool IsButtonPressed(uint button, int wand = 0)
    {
        if (wand < _buttons.Count)
            return _buttons[wand].IsPressed(button);
        else
            return false;
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, int wand = 0)
    {
        return IsButtonToggled(button, false, wand);
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, bool pressed, int wand = 0)
    {
        if (wand < _buttons.Count)
            return _buttons[wand].IsToggled(button, pressed);
        else
            return false;
    }

    /// <inheritdoc/>
    public bool IsButtonPressed(string device, uint button)
    {
        vrButtons vrb = MiddleVR.VRDeviceMgr.GetButtons(device);
        if (vrb != null)
            return vrb.IsPressed(button);    

        return false;
    }

    /// <inheritdoc/>
    public List<uint> IsButtonPressed(string device)
    {
        List<uint> pressedButtons = new List<uint>();

        vrButtons vrb = MiddleVR.VRDeviceMgr.GetButtons(device);
        if (vrb != null)
        {
            for (uint i = 0; i < vrb.GetButtonsNb(); i++)
            {
                if (vrb.IsPressed(i)) pressedButtons.Add(i);
            }
        }
        return pressedButtons;
    }

    /// <summary>
    /// Creates a new button device for synchorizing values among clients </summary>
    /// <returns>The pointer to the device</returns>
    /// <param name="name">Name of the device.</param>
    /// <param name="numButtons">Number of buttons of the device.</param>
    public vrButtons CreateSyncButtonDevice(string name, uint numButtons)
    {
        vrButtons vrb = MiddleVR.VRDeviceMgr.CreateButtons(name);

        if (vrb != null)
        {
            vrb.SetButtonsNb(numButtons);

            MiddleVRTools.Log("[+] Created shared event button " + name);

            MiddleVR.VRClusterMgr.AddSynchronizedObject(vrb, 0);
        }
        else
        {
            MiddleVRTools.Log("[!] Error creating a shared event button " + name);
        }

        return vrb;
    }

    /// <summary>
    /// Enables the change of the button state of a vrButton. It should be used
    /// only for synch purposes. If the device does not exist it will create a
    /// new shared button device
    /// </summary>
    /// <param name="device">The string representing the device.</param>
    /// <param name="button">The button id which has to be updated.</param>
    /// <param name="state">The new state of the button</param>
    public void SetButtonState(string device, uint button, bool state)
    {
        vrButtons vrb = MiddleVR.VRDeviceMgr.GetButtons(device);
        if (vrb != null) 
            vrb.SetPressedState(button, state);
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Axis Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public float GetWandAxisValue(uint axis, int wand = 0)
    {
        if (_axis.Count > 0 && _axis.Count > wand)
        {
            return _axis[wand].GetValue(axis);
        }
        else if (_joysticks.Count != 0)
        {
            return _joysticks[wand].GetAxisValue(axis);
        }
        return 0;
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Keyboard Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool GetKeyDown(KeyCode key)
    {
        if (_kb == null)
            return MiddleVR.VRDeviceMgr.IsKeyPressed(translateKeyCode(key)) &&
                   MiddleVR.VRDeviceMgr.IsKeyToggled(translateKeyCode(key));
        return _kb.IsKeyToggled(translateKeyCode(key));
    }

    /// <inheritdoc/>
    public bool GetKeyUp(KeyCode key)
    {
        if (_kb == null)
            return !MiddleVR.VRDeviceMgr.IsKeyPressed(translateKeyCode(key)) &&
                   MiddleVR.VRDeviceMgr.IsKeyToggled(translateKeyCode(key));
        return _kb.IsKeyToggled(translateKeyCode(key), false);
    }

    /// <inheritdoc/>
    public bool GetKeyPressed(KeyCode key)
    {
        if (_kb == null)
            return MiddleVR.VRDeviceMgr.IsKeyPressed(translateKeyCode(key));
        return _kb.IsKeyPressed(translateKeyCode(key));
    }

    /// <inheritdoc/>
    public bool IsClient()
    {
        return MiddleVR.VRClusterMgr.IsCluster() && MiddleVR.VRClusterMgr.IsClient();
    }

    /// <inheritdoc/>
    public bool IsMaster()
    {
        return !MiddleVR.VRClusterMgr.IsCluster() || MiddleVR.VRClusterMgr.IsServer();
    }

    /// <inheritdoc/>
    public bool IsCluster()
    {
        return MiddleVR.VRClusterMgr.IsCluster();
    }

    /// <inheritdoc/>
    public float GetDeltaTime()
    {
        return (float)(MiddleVR.VRKernel.GetDeltaTime());
    }

    /// <inheritdoc/>
    public float GetTime()
    {
        return (float)(MiddleVR.VRKernel.GetTime() / 1000);
    }

    /// <inheritdoc/>
    public uint GetFrameCount()
    {
        return MiddleVR.VRKernel.GetFrame();
    }

    /// <inheritdoc/>
    public Vector3 GetTrackerPosition(string trackerName)
    {
        vrNode3D node = MiddleVR.VRDisplayMgr.GetNode(trackerName);
        if (node == null)
            return Vector3.zero;
        return MVRTools.ToUnity(node.GetPositionVRSystemWorld());
    }

    /// <inheritdoc/>
    public Vector3 GetTrackerPosition(string trackerName, string segmentName)
    {
        return GetTrackerPosition(trackerName);
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName)
    {
        vrNode3D node = MiddleVR.VRDisplayMgr.GetNode(trackerName);
        if (node == null)
            return Quaternion.identity;
        return MVRTools.ToUnity(node.GetOrientationVRSystemWorld());
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName, string segmentName)
    {
        return GetTrackerRotation(trackerName);
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    public static MiddleVR_VRTools GetInstance()
    {
        return Instance;
    }

    /// <summary>
    /// Calls the given callback with the instance when it's initialized.
    /// </summary>
    public void GetInstance(System.Action<IVRTools> callback)
    {
        GetInstance().StartCoroutine(GetInstance().InitRoutine(callback));
    }

    /// <see cref="Instance"/>
    public IEnumerator InitRoutine(Action<IVRTools> resultCallback)
    {
        while (!_init)
        {
            yield return null;
            Init();
        }
        resultCallback(this);
    }

    /// <see cref="Instance"/>
    IEnumerator InitRoutine()
    {
        while (!_init)
        {
            yield return null;
            Init();
        }
    }

    /// <summary>
    /// Translates Unity key code to the MiddleVR KeyCode with respect to azerty keyboard
    /// </summary>
    /// <returns>The MiddleVR key code.</returns>
    /// <param name="k">Unity Keycode to translate</param>
    private static uint translateKeyCode(KeyCode k)
    {
        switch (k)
        {
            case KeyCode.A: return MiddleVR.VRK_Q;
            case KeyCode.B: return MiddleVR.VRK_B;
            case KeyCode.C: return MiddleVR.VRK_C;
            case KeyCode.D: return MiddleVR.VRK_D;
            case KeyCode.E: return MiddleVR.VRK_E;
            case KeyCode.F: return MiddleVR.VRK_F;
            case KeyCode.G: return MiddleVR.VRK_G;
            case KeyCode.H: return MiddleVR.VRK_H;
            case KeyCode.I: return MiddleVR.VRK_I;
            case KeyCode.J: return MiddleVR.VRK_J;
            case KeyCode.K: return MiddleVR.VRK_K;
            case KeyCode.L: return MiddleVR.VRK_L;
            case KeyCode.M: return MiddleVR.VRK_SEMICOLON;
            case KeyCode.N: return MiddleVR.VRK_N;
            case KeyCode.O: return MiddleVR.VRK_O;
            case KeyCode.P: return MiddleVR.VRK_P;
            case KeyCode.Q: return MiddleVR.VRK_A;
            case KeyCode.R: return MiddleVR.VRK_R;
            case KeyCode.S: return MiddleVR.VRK_S;
            case KeyCode.T: return MiddleVR.VRK_T;
            case KeyCode.U: return MiddleVR.VRK_U;
            case KeyCode.V: return MiddleVR.VRK_V;
            case KeyCode.W: return MiddleVR.VRK_Z;
            case KeyCode.X: return MiddleVR.VRK_X;
            case KeyCode.Y: return MiddleVR.VRK_Y;
            case KeyCode.Z: return MiddleVR.VRK_W;
            case KeyCode.Alpha1: return MiddleVR.VRK_1;
            case KeyCode.Alpha2: return MiddleVR.VRK_2;
            case KeyCode.Alpha3: return MiddleVR.VRK_3;
            case KeyCode.Alpha4: return MiddleVR.VRK_4;
            case KeyCode.Alpha5: return MiddleVR.VRK_5;
            case KeyCode.Alpha6: return MiddleVR.VRK_6;
            case KeyCode.Alpha7: return MiddleVR.VRK_7;
            case KeyCode.Alpha8: return MiddleVR.VRK_8;
            case KeyCode.Alpha9: return MiddleVR.VRK_9;
            case KeyCode.Alpha0: return MiddleVR.VRK_0;
            case KeyCode.Space: return MiddleVR.VRK_SPACE;
            case KeyCode.UpArrow: return MiddleVR.VRK_UP;
            case KeyCode.DownArrow: return MiddleVR.VRK_DOWN;
            case KeyCode.LeftArrow: return MiddleVR.VRK_LEFT;
            case KeyCode.RightArrow: return MiddleVR.VRK_RIGHT;
            case KeyCode.Keypad0: return MiddleVR.VRK_NUMPAD0;
            case KeyCode.Keypad1: return MiddleVR.VRK_NUMPAD1;
            case KeyCode.Keypad2: return MiddleVR.VRK_NUMPAD2;
            case KeyCode.Keypad3: return MiddleVR.VRK_NUMPAD3;
            case KeyCode.Keypad4: return MiddleVR.VRK_NUMPAD4;
            case KeyCode.Keypad5: return MiddleVR.VRK_NUMPAD5;
            case KeyCode.Keypad6: return MiddleVR.VRK_NUMPAD6;
            case KeyCode.Keypad7: return MiddleVR.VRK_NUMPAD7;
            case KeyCode.Keypad8: return MiddleVR.VRK_NUMPAD8;
            case KeyCode.Keypad9: return MiddleVR.VRK_NUMPAD9;
            case KeyCode.KeypadDivide: return MiddleVR.VRK_DIVIDE;
            case KeyCode.KeypadMultiply: return MiddleVR.VRK_MULTIPLY;
            case KeyCode.KeypadMinus: return MiddleVR.VRK_SUBTRACT;
            case KeyCode.KeypadPlus: return MiddleVR.VRK_ADD;
            case KeyCode.KeypadEnter: return MiddleVR.VRK_NUMPADENTER;
            case KeyCode.KeypadPeriod: return MiddleVR.VRK_DECIMAL;
            case KeyCode.Insert: return MiddleVR.VRK_INSERT;
            case KeyCode.Delete: return MiddleVR.VRK_DELETE;
            case KeyCode.Home: return MiddleVR.VRK_HOME;
            case KeyCode.End: return MiddleVR.VRK_END;
            case KeyCode.PageUp: return MiddleVR.VRK_PRIOR;
            case KeyCode.PageDown: return MiddleVR.VRK_NEXT;
            case KeyCode.Escape: return MiddleVR.VRK_ESCAPE;
            case KeyCode.LeftControl: return MiddleVR.VRK_LCONTROL;
            case KeyCode.RightControl: return MiddleVR.VRK_RCONTROL;
            case KeyCode.LeftAlt: return MiddleVR.VRK_ALTLEFT;
            case KeyCode.RightAlt: return MiddleVR.VRK_ALTRIGHT;
            case KeyCode.LeftShift: return MiddleVR.VRK_LSHIFT;
            case KeyCode.RightShift: return MiddleVR.VRK_RSHIFT;
            case KeyCode.Less: return MiddleVR.VRK_OEM_102;
            case KeyCode.Comma: return MiddleVR.VRK_M;
            case KeyCode.Semicolon: return MiddleVR.VRK_COMMA;
            case KeyCode.Colon: return MiddleVR.VRK_PERIOD;
            case KeyCode.Exclaim: return MiddleVR.VRK_SLASH;
            case KeyCode.Return: return MiddleVR.VRK_RETURN;
            case KeyCode.Equals: return MiddleVR.VRK_EQUALS;
            case KeyCode.Backspace: return MiddleVR.VRK_BACK;
            case KeyCode.RightBracket: return MiddleVR.VRK_MINUS;
            case KeyCode.Tab: return MiddleVR.VRK_TAB;
            case KeyCode.F1: return MiddleVR.VRK_F1;
            case KeyCode.F2: return MiddleVR.VRK_F2;
            case KeyCode.F3: return MiddleVR.VRK_F3;
            case KeyCode.F4: return MiddleVR.VRK_F4;
            case KeyCode.F5: return MiddleVR.VRK_F5;
            case KeyCode.F6: return MiddleVR.VRK_F6;
            case KeyCode.F7: return MiddleVR.VRK_F7;
            case KeyCode.F8: return MiddleVR.VRK_F8;
            case KeyCode.F9: return MiddleVR.VRK_F9;
            case KeyCode.F10: return MiddleVR.VRK_F10;
            case KeyCode.F11: return MiddleVR.VRK_F11;
            case KeyCode.F12: return MiddleVR.VRK_F12;
            case KeyCode.ScrollLock: return MiddleVR.VRK_SCROLL;
            case KeyCode.Pause: return MiddleVR.VRK_PAUSE;
            default:
                //Debug.Log("Unknown key asked : " + k);
                return MiddleVR.VRK_ESCAPE;
        }
    }
}

#endif

/// <inheritdoc/>
public class UnityVRTools : Singleton<UnityVRTools>, IVRTools
{
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Button Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool IsButtonPressed(uint button, int wand = 0)
    {
        return Input.GetMouseButton((int)button);
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, int wand = 0)
    {
        return IsButtonToggled(button, false, wand);
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, bool pressed, int wand = 0)
    {
        if (pressed)
            return Input.GetMouseButtonDown((int)button);
        else
            return Input.GetMouseButtonUp((int)button);
    }

    /// <inheritdoc/>
    public bool IsButtonPressed(string device, uint button)
    {
        if (device == "mouse" || device == "MOUSE")
        {
            return IsButtonPressed(button);
        }
        else
        {
            Debug.LogError("[VRTools] No device in VRTools Unity mode.");
            return false;
        }
    }

    /// <inheritdoc/>
    public List<uint> IsButtonPressed(string device)
    {
        Debug.LogError("[VRTools] No device in VRTools Unity mode.");
        List<uint> pressedButtons = new List<uint>();
        return pressedButtons;
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Axis Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public float GetWandAxisValue(uint axis, int wand = 0)
    {
        if (axis == 1)
            return Input.GetAxis("Vertical");
        else if (axis == 0)
            return Input.GetAxis("Horizontal");
        else if (axis == 2)
            return Input.GetAxis("Gear");
        else
            return 0;
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Keyboard Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    /// <inheritdoc/>
    public bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    /// <inheritdoc/>
    public bool GetKeyPressed(KeyCode key)
    {
        return Input.GetKey(key);
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Static
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public void GetInstance(System.Action<IVRTools> callback)
    {
        Instance.StartCoroutine(Instance.InitRoutine(callback));
    }

    /// <inheritdoc/>
    public IEnumerator InitRoutine(Action<IVRTools> resultCallback)
    {
        resultCallback(this);
        yield return null;
    }

    /// <inheritdoc/>
    public bool IsClient()
    {
        return false;
    }

    /// <inheritdoc/>
    public bool IsMaster()
    {
        return true;
    }

    /// <inheritdoc/>
    public bool IsCluster()
    {
        return false;
    }

    /// <inheritdoc/>
    public float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    /// <inheritdoc/>
    public float GetTime()
    {
        return Time.time;
    }

    /// <inheritdoc/>
    public uint GetFrameCount()
    {
        return (uint)Time.frameCount;
    }

    /// <inheritdoc/>
    public Vector3 GetTrackerPosition(string trackerName)
    {
        return Vector3.zero;
    }

    /// <inheritdoc/>
    public Vector3 GetTrackerPosition(string trackerName, string segmentName)
    {
        return Vector3.zero;
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName)
    {
        return Quaternion.identity;
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName, string segmentName)
    {
        return Quaternion.identity;
    }
}


#if VICONSDK

public class ViconSDK_VRTools : Singleton<ViconSDK_VRTools>, IVRTools
{
    ViconSDKManager viconSDKManager;
    void Awake()
    {
        viconSDKManager = GameObject.FindObjectOfType<ViconSDKManager>();
        if (viconSDKManager == null)
            Debug.LogError("[VRTools] Cannot find ViconTrackingManager gameobject.");
    }

    /// <inheritdoc/>
    public bool IsButtonPressed(uint button, int wand = 0)
    {
        return viconSDKManager.IsButtonPressed(button, wand);
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, int wand = 0)
    {
        return viconSDKManager.IsButtonToggled(button, wand);
    }

    /// <inheritdoc/>
    public bool IsButtonToggled(uint button, bool pressed, int wand = 0)
    {
        return viconSDKManager.IsButtonToggled(button, pressed, wand);
    }

    /// <inheritdoc/>
    public bool IsButtonPressed(string device, uint button)
    {
        return viconSDKManager.IsDeviceButtonPressed(device, button);
    }

    /// <inheritdoc/>
    public List<uint> IsButtonPressed(string device)
    {
        List<uint> pressedButtons = new List<uint>();
        return pressedButtons;
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Axis Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public float GetWandAxisValue(uint axis, int wand = 0)
    {
        return (float) viconSDKManager.getDeviceValue((uint)wand, axis);
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Keyboard Handling
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    /// <inheritdoc/>
    public bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    /// <inheritdoc/>
    public bool GetKeyPressed(KeyCode key)
    {
        return Input.GetKey(key);
    }

    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////
    /// Static
    /////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////

    /// <inheritdoc/>
    public void GetInstance(System.Action<IVRTools> callback)
    {
        Instance.StartCoroutine(Instance.InitRoutine(callback));
    }

    /// <inheritdoc/>
    public IEnumerator InitRoutine(Action<IVRTools> resultCallback)
    {
        //TODO use vicon sdk connexion status.
        resultCallback(this);
        yield return null;
    }

    /// <inheritdoc/>
    public bool IsClient()
    {
        return false;
    }

    /// <inheritdoc/>
    public bool IsMaster()
    {
        return true;
    }

    /// <inheritdoc/>
    public bool IsCluster()
    {
        return false;
    }

    /// <inheritdoc/>
    public float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    /// <inheritdoc/>
    public float GetTime()
    {
        return Time.time;
    }

    /// <inheritdoc/>
    public uint GetFrameCount()
    {
        return (uint)Time.frameCount;
    }

    /// <inheritdoc/>
    public Vector3 GetTrackerPosition(string trackerName)
    {
        return viconSDKManager.GetTrackerPosition(trackerName);
    }

    /// <inheritdoc/>
    public Vector3 GetTrackerPosition(string trackerName, string segmentName)
    {
        return viconSDKManager.GetTrackerPosition(trackerName, segmentName);
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName)
    {
        return viconSDKManager.GetTrackerRotation(trackerName);
    }

    /// <inheritdoc/>
    public Quaternion GetTrackerRotation(string trackerName, string segmentName)
    {
        return viconSDKManager.GetTrackerRotation(trackerName, segmentName);
    }
}

#endif
