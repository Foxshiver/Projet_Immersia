using UnityEngine;
using System.Collections;

public class iTween_elevator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
    /*
    void OnCollisionEnter()
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("elevatorUp"), "time", 10));
    }*/

    void OnTriggerEnter()
    {
        if(this.GetComponent<iTween>() == null)
            iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("elevatorUp"), "time", 10));
    }
	
	// Update is called once per frame
	void Update () {

        
     }
}
