using UnityEngine;

public class VRHand : Grabber
{

    #region Attributes

    public enum GrabMode { GrabWhilePressed = 0, GrabWhenToggled, TryGrabWhenToggled }

    /// <summary>
    /// Grab Mode
    /// </summary>
    [Tooltip("Grab mode : \n" +
        "GrabWhilePressed : Keep the object in the hand while the button is pressed or the threshold is met\n" +
        "GrabWhenToggled : When the grabbing command is activated, take an object if one is available, activate the command again to release\n" +
        "TryGrabWhenToggled : When the grabbing command is activated, take an object as soon as one is available, activate the command again to release")]
    public GrabMode grabMode = GrabMode.GrabWhilePressed;

    [Tooltip("Use an axis value for the grab command")]
    public bool UseAxis = true;
    [Tooltip("Axis value to use for the grab command (if UseAxis is activated")]
    public uint GrabAxis = 2;
    [Tooltip("Use a button for the grab command")]
    public bool UseButton = true;
    [Tooltip("Button to use for the grab command (if UseButton is activated")]
    public uint GrabButton;
    [Tooltip("Device to use for the command (for both axis and button)")]
    public int DeviceID = 0;

    //[Range(0,1)]
    //public float emulationValue = 0;

    [Range(0, 1)]
    [Tooltip("Threshold to use for the grabbin gcommand when using axis")]
    public float GrabThreshold = 0.5f;

    [Tooltip("Grabbing animation speed when using button")]
    public float GrabSpeed = 1.0f;

    [Tooltip("The animator containing the grab/release animations (can be discovered at launch if it's a child component")]
    public Animator Animator;

    #endregion

    protected bool handOpen = true;
    protected bool toggled = false;



    // Use this for initialization
    void Start()
    {

        if (Animator == null)
            Animator = GetComponentInChildren<Animator>();
        if (Animator == null)
            Debug.LogWarning("Could not find an Animator component for the hand", this);
        else
        {
            Debug.Log("Using Animator component on " + Animator.name + " for the hand", this);
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (UseButton)
        {
            ProcessButton();
        }
        if (UseAxis)
        {
            ProcessAxis();
        }

    }

    private void ProcessButton()
    {
        switch (grabMode)
        {
            case GrabMode.GrabWhilePressed:
                if (VRTools.IsButtonPressed(GrabButton, DeviceID))
                {

                    CloseHand();
                    if (!grabbing)
                    {
                        Grab();
                    }
                }

                else
                {
                    OpenHand();
                    if (grabbing)
                    {
                        Release();
                    }
                }
                break;
            case GrabMode.GrabWhenToggled:
                // take the command into account when we release the button
                if (VRTools.IsButtonToggled(GrabButton, false, DeviceID))
                {
                    if (!grabbing && CanGrab())
                    {
                        CloseHand();
                        Grab();
                    }
                    else if (!handOpen)
                    {
                        OpenHand();
                        Release();
                    }
                }
                break;
            case GrabMode.TryGrabWhenToggled:
                if (!grabbing && !handOpen)
                {
                    Grab();
                }
                if (grabbing && handOpen)
                {
                    Release();
                }
                // take the command into account when we release the button
                if (VRTools.IsButtonToggled(GrabButton, false, DeviceID))
                {
                    if (handOpen)
                        CloseHand();
                    else
                        OpenHand();
                }
                break;
        }
    }


    private void ProcessAxis()
    {

        float axisValue = Mathf.Clamp(VRTools.GetWandAxisValue(GrabAxis, DeviceID), 0.0f, 1.0f);
        //axisValue = emulationValue;
        Animator.SetBool("CloseHand", true);
        Animator.Play("CloseHandAnim", 0, axisValue);
        switch (grabMode)
        {
            case GrabMode.GrabWhilePressed:
                if (axisValue >= GrabThreshold)
                {
                    if (!grabbing)
                    {
                        Grab();
                    }
                }
                else
                {
                    if (grabbing)
                    {
                        Release();
                    }
                }
                break;
            case GrabMode.GrabWhenToggled:
                if (axisValue < GrabThreshold)
                {
                    toggled = true;
                }
                else
                {
                    if (toggled)
                    {
                        if (!grabbing)
                        {
                            Grab();
                        }
                        else
                        {
                            Release();
                        }
                        toggled = false;
                    }
                }
                break;
            case GrabMode.TryGrabWhenToggled:
                if (axisValue < GrabThreshold)
                {
                    if (grabbing)
                    {
                        Release();
                    }
                }
                else
                {
                    if (!grabbing)
                        Grab();
                }
                break;
        }
    }


    protected void CloseHand()
    {
        Animator.SetBool("CloseHand", true);
        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("CloseHandAnim"))
            Animator.Play("CloseHandAnim", 0, 1 - Mathf.Clamp01(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
        handOpen = false;
    }

    protected void OpenHand()
    {
        Animator.SetBool("CloseHand", false);
        if (!Animator.GetCurrentAnimatorStateInfo(0).IsName("OpenHandAnim"))
            Animator.Play("OpenHandAnim", 0, 1 - Mathf.Clamp01(Animator.GetCurrentAnimatorStateInfo(0).normalizedTime));
        handOpen = true;
    }
}
