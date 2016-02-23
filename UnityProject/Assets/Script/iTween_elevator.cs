using UnityEngine;
using System.Collections;

public class iTween_elevator : MonoBehaviour {

	// Use this for initialization
	void Start () {
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("elevator"), "time", 10, "easetype", iTween.EaseType.linear, "loopType", "loop", "delay", 0));
   
	}
	
	// Update is called once per frame
	void Update () {

        
     }
}
