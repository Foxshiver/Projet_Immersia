using UnityEngine;
using System.Collections;

/// <summary>
/// Assign tracker position and rotation to a game object.
/// 
/// Script execution order:
/// - After scripts which update tracker transform (VRManagerScript, ViconSDKManager)
/// - Before scripts using GameObject positions.
/// 
/// </summary>
public class AssignTransformFromTracker : MonoBehaviour 
{
    /// <summary>
    /// All possible names for the tracker.
    /// <example>Immersia : HeadNode, Immermove : HEAD, Immersia : HandNode, Immermove : Vicon001_AP</example>
    /// </summary>
    public string[] trackerNames;

    /// <summary>
    /// Current valid tracker name.
    /// </summary>
    string trackerName = "";
    string segmentName = "";

    /// <summary>
    /// Relative offset.
    /// </summary>
    Vector3 offset;

    void Start()
    {
        SearchTracker();
        offset = transform.localPosition;
    }

	void Update ()
    {
        if (trackerName != "")
        {
            transform.position = VRTools.GetTrackerPosition(trackerName, segmentName) + offset;
            transform.rotation = VRTools.GetTrackerRotation(trackerName, segmentName);
        }
        else
            SearchTracker();
    }

    void SearchTracker()
    {
        foreach (string name in trackerNames)
            if(name.Contains(";"))
            {
                string tName = name.Split(';')[0];
                string sName = name.Split(';')[1];

                if (VRTools.GetTrackerPosition(tName, sName) != Vector3.zero)
                {
                    trackerName = tName;
                    segmentName = sName;
                }
            }
            else if (VRTools.GetTrackerPosition(name) != Vector3.zero)
            {
                trackerName = name;
                segmentName = name;
            }
    }
    
    [ContextMenu("SetHead")]
    void SetHead()
    {
        trackerNames = new string[] { "HeadNode", "HEAD" };
    }

    [ContextMenu("SetHand")]
    void SetHand()
    {
        trackerNames = new string[] { "Hand_HT27", "Hand_D", "RHAND" };
    }

    [ContextMenu("SetWand")]
    void SetWand()
    {
        trackerNames = new string[] { "HandNode", "ViconAP_001;Root" };
    }
}
