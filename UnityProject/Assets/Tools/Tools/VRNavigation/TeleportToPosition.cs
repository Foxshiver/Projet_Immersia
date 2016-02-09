using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeleportToPosition : MonoBehaviour
{
    public Transform ReferenceObjectToTeleport;
    public Transform[] OffsetObjectToTeleport;

    public List<Transform> Positions = new List<Transform>();

    protected vrKeyboard keyb = null;

    int currentPosition = 0;

    public KeyCode NextPositionKey = KeyCode.N;
    public KeyCode PreviousPositionKey = KeyCode.P;

    public uint NextPositionButton = 1;
    public uint PreviousPositionButton = 2;

    public int WandID = 0;

    public bool UsePositionsRotation = true;

    bool UseRotateAround
    {
        get { return VRTools.GetMode() == VRTools.VICONSDK; }
    }

    // Update is called once per frame
    void Update()
    {
        if (VRTools.GetKeyDown(NextPositionKey) || VRTools.IsButtonToggled(NextPositionButton))
        {
            currentPosition++;
            if (currentPosition >= Positions.Count)
                currentPosition = 0;

            changeTransform();     
        }
        if (VRTools.GetKeyDown(PreviousPositionKey) || VRTools.IsButtonToggled(PreviousPositionButton))
        {
            currentPosition--;
            if (currentPosition < 0)
                currentPosition = Positions.Count - 1;

            changeTransform();
        }
    }

    void changeTransform()
    {
        setNewPosition(Positions[currentPosition].position);
        if (UsePositionsRotation)
            setNewRotation(Positions[currentPosition].rotation);
    }

    void setNewPosition(Vector3 position)
    {
        Vector3 offsetPosition = position - ReferenceObjectToTeleport.transform.position;

        for (int t = 0; t < OffsetObjectToTeleport.Length; t++)
            OffsetObjectToTeleport[t].position += offsetPosition;

        ReferenceObjectToTeleport.transform.position = position;
    }

    void setNewRotation(Quaternion rotation)
    {
        for (int t = 0; t < OffsetObjectToTeleport.Length; t++)
        {
            if (UseRotateAround)
                OffsetObjectToTeleport[t].RotateAround(ReferenceObjectToTeleport.transform.position, Vector3.up, rotation.eulerAngles.y - ReferenceObjectToTeleport.transform.rotation.eulerAngles.y);
            else
                OffsetObjectToTeleport[t].rotation = rotation;
        }
      
        ReferenceObjectToTeleport.transform.rotation = rotation;
    }
}
