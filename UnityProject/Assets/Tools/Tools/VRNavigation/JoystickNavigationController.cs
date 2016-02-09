using UnityEngine;

using System;
using System.Collections;

/**
 * Generic joystick character controller using VRTools.
 * Navigation direction should follow the joystick tracker or the head tracker forward direction
 * depending in how is configured.
 */
public class JoystickNavigationController : MonoBehaviour
{
	public double rotateSpeed = 50;
	public double translateSpeed = 1;

    /// <summary>
    /// Is character controller via its transform or via Move() method.
    /// </summary>
    public bool directTranslateMode = false;

    /// <summary>
    /// Constraint movement to a fix Y plane. (Jittering with Character Controller without).
    /// </summary>
    public bool fixedHeight = true;

	/// <summary>
	/// Move only when input value are above the threshold. [0..1].
	/// </summary>
	public double inputThreshold = 0.2;


	public CharacterController character;
    public GameObject objectToRotate;

    /// <summary>
    /// For techviz camera, need to rotate around the current character position.
    /// </summary>
    bool RotateAroundMode
    {
        get { return VRTools.GetMode() == VRTools.VICONSDK; }
    }

    /// <summary>
    /// For MiddleVR camera.
    /// </summary>
    bool RotateCharacterMode
    {
        get { return VRTools.GetMode() == VRTools.MIDDLEVR; }
    }

	/// <summary>
	/// Mostly joystick tracker.
	/// </summary>
    public GameObject objectDirectionToFollowStandard;
    public GameObject objectDirectionToFollowExtend;

    GameObject objectDirectionToFollow;
	
    public uint xIndex = 0;
    public uint yIndex = 1;

    public uint changeObjectToFollowIndex = 2;

    double x;
    double y;

	public GameObject pivot;

	void Start()
	{
		if(character == null)
			character = GetComponent<CharacterController>();
	}

	void Update ()
    {
		objectDirectionToFollow = VRTools.IsButtonPressed(changeObjectToFollowIndex) ?
			objectDirectionToFollowExtend : objectDirectionToFollowStandard;

        x = VRTools.GetWandAxisValue(xIndex);
		y = VRTools.GetWandAxisValue(yIndex);
        
		if (Math.Abs(x) < inputThreshold)
			x = 0;

		if (Math.Abs(y) < inputThreshold)
			y = 0;

		Vector3 pivotPoint = character.transform.position;
		pivotPoint.y = 0;

        if (RotateAroundMode)
            objectToRotate.transform.RotateAround(pivotPoint, Vector3.up, (float)(x * rotateSpeed * VRTools.GetDeltaTime()));
        else
        {
            objectToRotate.transform.Rotate(Vector3.up, (float)(x * rotateSpeed * VRTools.GetDeltaTime()));
            if (RotateCharacterMode)
                character.transform.Rotate(Vector3.up, (float)(x * rotateSpeed * VRTools.GetDeltaTime()));
        }
        Vector3 translation = objectDirectionToFollow.transform.forward * VRTools.GetDeltaTime() * (float)(y * translateSpeed);
        if (fixedHeight)
            translation.y = 0;

		Vector3 currentCharacterPosition = character.transform.position;
        if (directTranslateMode)
            character.transform.localPosition += translation;
        else
            character.Move(translation);

		Vector3 realTranslation = character.transform.position - currentCharacterPosition;
		objectToRotate.transform.position += realTranslation;
    }

    void LateUpdate()
    {
        if (VRTools.GetMode() == VRTools.UNITY)
            character.transform.localRotation = Quaternion.identity;
    }
}
