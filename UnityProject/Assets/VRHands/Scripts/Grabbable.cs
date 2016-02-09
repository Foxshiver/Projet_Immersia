using UnityEngine;

[RequireComponent(typeof(Velocity))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour {

	/// <summary>
	/// Reference Offset is the position where we would like this to be grabbed.
	/// </summary>
    public Transform ReferenceOffset;
    public bool IsGrabbable = true;
	public bool IsGrabbed { get { return CurrentGrabber != null; } }
	public Velocity Velocity { get; private set; }
	public Rigidbody Rigidbody { get; private set; }
	public Collider Collider { get; private set; }

	private Vector3 _OriginalWorldPos;
	private Quaternion _OriginalWorldOri;
    public Grabber CurrentGrabber;

    protected bool wasTrigger;
    protected bool wasKinematic;
    protected bool usedGravity;

    private bool _Expulse = false;

    void Start()
    {
		Velocity = GetComponent<Velocity>();
		Rigidbody = GetComponent<Rigidbody>();
		Collider = GetComponent<Collider>();
		if (ReferenceOffset == null)
		{
			ReferenceOffset = transform;
		}
    }

	void FixedUpdate()
	{
		if (_Expulse)
		{
			Rigidbody.AddForce(Velocity.Value, ForceMode.Impulse);
			_Expulse = false;
		}
	}

	#region Public Methods
	public void Grabbed(Grabber pGrabber)
    {
		_OriginalWorldPos = transform.position;
		_OriginalWorldOri = transform.rotation;

		ReferenceOffset.parent = null;
		transform.parent = ReferenceOffset;
		ReferenceOffset.parent = pGrabber.GrabPoint;
		ReferenceOffset.localPosition = Vector3.zero;
		ReferenceOffset.localRotation = Quaternion.identity;
        wasTrigger = Collider.isTrigger;
        wasKinematic = Rigidbody.isKinematic;
        usedGravity = Rigidbody.useGravity;
        Rigidbody.useGravity = false;
		Rigidbody.isKinematic = true;
		Collider.isTrigger = true;

		CurrentGrabber = pGrabber;
    }

	public void Released()
	{
        transform.parent = null;
        ReferenceOffset.parent = transform;

        Collider.isTrigger = wasTrigger;
        Rigidbody.isKinematic = wasKinematic;
        Rigidbody.useGravity = usedGravity;
        CurrentGrabber = null;
	}
	#endregion

	#region Private Methods
	private void SimpleReleased()
	{
		transform.parent = null;
		ReferenceOffset.parent = transform;

		Collider.isTrigger = false;
		Rigidbody.isKinematic = false;
		Rigidbody.useGravity = true;
	}

	private void GoBackReleased()
	{
		SimpleReleased();
		transform.position = _OriginalWorldPos;
		transform.rotation = _OriginalWorldOri;
	}

	private void ExpulseReleased()
	{
		SimpleReleased();
		_Expulse = true;
	}

	#endregion
}
