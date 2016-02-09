using UnityEngine;
using System.Collections;

public class AssignTrackerTransformRelativeToCharacter : MonoBehaviour 
{
    public GameObject tracked;

	public GameObject character;
	public GameObject characterOrientationObject;
	public GameObject head;

    bool ApplyCharacterRotationMode
    {
        get { return VRTools.GetMode() == VRTools.MIDDLEVR || VRTools.GetMode() == VRTools.VICONSDK; }
    }
    void Start()
    {
        if (character == null)
            character = FindObjectOfType<CharacterController>().gameObject;

    }

	void Update ()
	{
		Vector3 offsetHeadTracker = tracked.transform.position - head.transform.position;

        float height = character.transform.position.y + tracked.transform.position.y;

        transform.position = character.transform.position + (ApplyCharacterRotationMode ? characterOrientationObject : character).transform.rotation * offsetHeadTracker;
		transform.position = new Vector3(transform.position.x, height, transform.position.z);

        //if (ApplyCharacterRotationMode)
            transform.rotation = characterOrientationObject.transform.rotation * tracked.transform.rotation;
	}
}
