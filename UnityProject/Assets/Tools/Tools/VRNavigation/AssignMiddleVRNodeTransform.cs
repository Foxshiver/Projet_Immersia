using UnityEngine;
using System.Collections;

/// <summary>
/// Force MiddleVR node position.
/// Should be call directly after VRManagerScript
/// </summary>
public class AssignMiddleVRNodeTransform : MonoBehaviour
{
    public string MiddleVRHeadNodeName = "HeadNode";
    GameObject middleVRHeadNode;

    public string MiddleVRRootNode = "CenterNode";
    GameObject middleVRRootNode;

    public CharacterController characterController;
    public GameObject HeadNode;

    void Reset()
    {
        MiddleVRHeadNodeName = "HeadNode";
        MiddleVRRootNode = "CenterNode";
        characterController = FindObjectOfType<CharacterController>();
        HeadNode = gameObject;
    }

	void OnEnable()
    {
        middleVRHeadNode = GameObject.Find(MiddleVRHeadNodeName);
        middleVRRootNode = GameObject.Find(MiddleVRRootNode);
        //foreach (Transform t in node.transform)
        //    t.parent = HeadNode.transform;
	}

    void Update()
    {
        middleVRRootNode.transform.rotation = characterController.transform.rotation;
        middleVRHeadNode.transform.position = HeadNode.transform.position;
        //foreach (Transform t in HeadNode.transform)
        //{
        //    t.localPosition = Vector3.zero;
        //    t.localRotation = Quaternion.identity;
        //}
    }

}
