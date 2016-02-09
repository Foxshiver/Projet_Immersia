using UnityEngine;
using System.Collections;

/// <summary>
/// Use this to compute Velocity of a GameObject.
/// </summary>
public class Velocity : MonoBehaviour
{
	public Vector3 Value 
	{
		get
		{
			if (_NeedUpdate)
			{
				_Velocity = Vector3.zero;
				foreach (Vector3 velo in _VelocityBuffer)
				{
					_Velocity += velo;
				}
				_Velocity = _Velocity / BufferSize;
				_NeedUpdate = false;
			}
			return _Velocity;
		}
	}

	public int BufferSize = 3;
	private Vector3 _PreviousPosition;
	private System.Collections.Generic.List<Vector3> _VelocityBuffer;
	private bool _NeedUpdate = true;
	private Vector3 _Velocity;

	void OnEnable() 
	{
		_VelocityBuffer = new System.Collections.Generic.List<Vector3>();
		_PreviousPosition = transform.position;
	}

	void OnDisable()
	{
		_VelocityBuffer.Clear();
	}
	
	// Update is called once per frame
	void Update()
	{
		float dt = VRTools.GetDeltaTime();
		_VelocityBuffer.Add((transform.position - _PreviousPosition) / dt); // Current Velocity
		_PreviousPosition = transform.position;
		if (_VelocityBuffer.Count > BufferSize)
		{
			_VelocityBuffer.RemoveAt(0);
		}
		_NeedUpdate = true;
	}
}
