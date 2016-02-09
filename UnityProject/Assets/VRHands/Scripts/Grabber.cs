using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Grabber : MonoBehaviour
{

    /// <summary>
    /// Transform to which grabbed item will be added as a child
    /// </summary>
    [Tooltip("The transform to use as a grab point")]
    public Transform GrabPoint;

    [HideInInspector]
    public Grabbable GrabbedObject = null;

    protected bool grabbing = false;

    private HashSet<Grabbable> _PotentialGrabbables = new HashSet<Grabbable>();

    #region Events
    public event EventHandler GrabEvent;
    public event EventHandler ReleaseEvent;
    #endregion

    public virtual void Awake()
    {
        if (GrabPoint == null)
            GrabPoint = this.transform;
    }

    void OnTriggerEnter(Collider other)
    {
        Grabbable grabbable = other.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            _PotentialGrabbables.Add(grabbable);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Grabbable grabbable = other.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            _PotentialGrabbables.Remove(grabbable);
        }
    }

    protected virtual void Grab()
    {
        if (_PotentialGrabbables.Count > 0)
        {
            Grabbable grabbable = _PotentialGrabbables.First();
            grabbable.Grabbed(this);
            GrabbedObject = grabbable;
            if (GrabEvent != null)
                GrabEvent(this, EventArgs.Empty);
            grabbing = true;
        }
    }

    protected virtual void Release()
    {
        GrabbedObject.Released();
        GrabbedObject = null;
        if (ReleaseEvent != null)
            ReleaseEvent(this, EventArgs.Empty);
        grabbing = false;
    }

    public bool CanGrab()
    {
        return _PotentialGrabbables.Count > 0;
    }
}
