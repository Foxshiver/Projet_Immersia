using UnityEngine;
using System.Collections;

public class SetUpUnityCamera : MonoBehaviour 
{
    public GameObject referenceObject;
    public float height = 1.5f;
    public bool isCrouch = false;

    KeyCode crouchKey = KeyCode.C;

    void OnDisable()
    {
        resetCameraSettings();
    }

    void OnEnable()
    {
        changeHeight();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
            changeHeight();
    }

    void Update()
    {
        if (VRTools.GetKeyDown(crouchKey))
        {
            isCrouch = !isCrouch;
        }
        changeHeight();
    }

    void changeHeight()
    {

        transform.localPosition = referenceObject.transform.localPosition + new Vector3(0,
            isCrouch ? height / 2 : height,
            0);
    }

    void resetCameraSettings()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        transform.localRotation = Quaternion.identity;
    }

}
