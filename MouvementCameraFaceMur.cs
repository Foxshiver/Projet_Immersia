using UnityEngine;
using System.Collections;

public class moveCamera : MonoBehaviour {

    public Transform atobject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.Z))
        {
            transform.Translate(transform.forward*0.1f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-transform.forward * 0.1f);
        }
        float distance = (atobject.position.z-transform.position.z);
        if(distance < 10)
        {
            float ratio = -distance / 6.0f + (1.4f);
            transform.rotation = Quaternion.Euler(-Mathf.Lerp(0, 90, ratio), 0.0f, 0.0f);
        }
        
	}
}
