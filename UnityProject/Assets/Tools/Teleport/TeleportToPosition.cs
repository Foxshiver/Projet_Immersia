using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeleportToPosition : MonoBehaviour
{

    public Transform TransformToTeleport;

    public List<Transform> Positions = new List<Transform>();

    protected vrKeyboard keyb = null;

    int currentPosition = 0;

    public KeyCode NextPositionKey = KeyCode.N;
    public KeyCode PreviousPositionKey = KeyCode.P;

    public uint NextPositionButton = 1;
    public uint PreviousPositionButton = 2;

    public int WandID = 0;

    public bool UsePositionsRotation = true;



    // Update is called once per frame
    void Update()
    {
        if (VRTools.GetInstance().GetKeyDown(NextPositionKey) || VRTools.GetInstance().IsButtonToggled(NextPositionButton, true, WandID))
        {
            currentPosition++;
            if (currentPosition >= Positions.Count)
                currentPosition = 0;
            TransformToTeleport.position = Positions[currentPosition].position;
            if (UsePositionsRotation)
                TransformToTeleport.rotation = Positions[currentPosition].rotation;
        }
        if (VRTools.GetInstance().GetKeyDown(PreviousPositionKey) || VRTools.GetInstance().IsButtonToggled(PreviousPositionButton, true, WandID))
        {
            currentPosition--;
            if (currentPosition < 0)
                currentPosition = Positions.Count - 1;
            TransformToTeleport.position = Positions[currentPosition].position;
            if (UsePositionsRotation)
                TransformToTeleport.rotation = Positions[currentPosition].rotation;
        }
    }

}
