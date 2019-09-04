using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartJoint : MonoBehaviour
{
    public bool isAttached { get; private set; } = false;

    // reference to the joint this joint is connected to
    public PartJoint connection { get; private set; } = null;

    // position and rotation of the connection
    public Vector3 position { get; private set; } = Vector3.zero;
    public Quaternion rotation { get; private set; } = Quaternion.identity;
    
    // reference to the part containing this joint
    public Part part = null;

    private SphereCollider m_collider = null;
    private SphereCollider coll
    {
        get
        {
            if (m_collider == null)
            {
                m_collider = GetComponent<SphereCollider>();
            }
            return m_collider;
        }
    }

    public int id
    {
        get
        {
            if (part != null)
            {
                return part.GetJointID(this);
            }
            return -1;
        }
    }

    public void AttachTo(PartJoint _joint, bool dontDisableOther = false)
    {
        Detach();
        if (_joint != null && !_joint.isAttached)
        {
            // attach this to other
            isAttached = true;
            connection = _joint;
            coll.enabled = false;

            // attach other to this
            connection.isAttached = true;
            if (!dontDisableOther)
            {
                connection.connection = this;
                connection.coll.enabled = false;
            }

            // reference position and rotation is other
            position = connection.transform.position;
            rotation = connection.transform.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
    }

    public void Detach()
    {
        if(connection != null)
        {
            // detach other from this
            connection.isAttached = false;
            connection.coll.enabled = true;
            connection.connection = null;

            // detach this from other
            connection = null;
            isAttached = false;
            coll.enabled = true;
        }
    }
}
