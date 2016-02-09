using UnityEngine;
using System.Collections;

/// <summary>
/// Change character height according to the head position.
/// </summary>
public class AutoHeightNavigation : MonoBehaviour
{
	CharacterController character;
    NavMeshAgent navMeshAgent;

	/// <summary>
	/// Tracked head user.
	/// </summary>
	public GameObject head;

	/// <summary>
	/// Minimal character controller height.  
	/// </summary>
	public float minHeight = 0.4f;

	/// <summary>
	/// If present, correctly display the character controller renderer. 
	/// </summary>
	public Renderer characterRenderer;

	void Start ()
	{
		character = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
	}

	void Update ()
	{
		float currentHeight = head.transform.position.y;

		character.height = Mathf.Max(minHeight, currentHeight);
		character.center = new Vector3(0, character.height / 2.0f, 0);

        navMeshAgent.height = character.height;

		if(characterRenderer != null)
		{
			characterRenderer.transform.localScale = new Vector3(characterRenderer.transform.localScale.x,
			                                                     character.center.y, 
			                                                     characterRenderer.transform.localScale.z);

			characterRenderer.transform.localPosition = new Vector3(0, character.height / 2 , 0);
		}
	}
}
