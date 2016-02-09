using UnityEngine;
using System.Collections;

/// <summary>
/// Apply the difference between the wanted translation and the real translation 
/// of the controller on object when controller is colliding.
/// Usefull to simulate collision in a none constraint environnement (eg. use in a CAVE).
/// </summary>
public class CollisionOffsetFromController : MonoBehaviour 
{
	public CharacterController character;

	public GameObject head;
	public GameObject objectToMove;

	Vector3 previousPosition;
	Vector3 previousControllerPosition;

    /// <summary>
    /// Is object to move giving current character orientation ?
    /// </summary>
    bool ObjectToMoveMode
    {
        get { return VRTools.GetMode() != VRTools.MIDDLEVR; }
    }

	void Start () 
	{
		previousPosition = head.transform.position;
		previousControllerPosition = character.transform.localPosition;
	}

	void Update () 
	{
        Vector3 offsetHead = head.transform.position - previousPosition;
        offsetHead.y = 0; //Ignore height.

        Vector3 characterMovement = (ObjectToMoveMode ? objectToMove.transform.rotation : character.transform.rotation) * offsetHead;

        if(character.enabled)
            character.Move(characterMovement);
        else
            character.transform.localPosition += characterMovement;

        Vector3 offsetController = character.transform.localPosition - previousControllerPosition;
        Vector3 differenceHeadController = (ObjectToMoveMode ? objectToMove.transform.rotation : character.transform.rotation) * offsetHead - offsetController;
        if (VRTools.GetMode() == VRTools.VICONSDK)
            objectToMove.transform.localPosition -= differenceHeadController;
        //else
        //    objectToMove.transform.localPosition = character.transform.localPosition;// new Vector3(character.transform.localPosition.x, objectToMove.transform.localPosition.y, character.transform.localPosition.z);
	}

    void LateUpdate()
	{
		previousPosition = head.transform.position;
		previousControllerPosition = character.transform.localPosition;
	}
}
