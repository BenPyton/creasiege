using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Part : MonoBehaviour
{
    [SerializeField] public bool isRoot = false;
    [SerializeField] public float mass = 10.0f;
    [SerializeField] int health = 10;

    List<PartJoint> m_joints = new List<PartJoint>();
    [HideInInspector] public PartJoint parentJoint { get { return m_rootJoint.connection; } }

    // the joint that set position and rotation of the part
    PartJoint m_rootJoint = null;

    [HideInInspector] public int id { get { return transform.GetSiblingIndex(); } }
    [HideInInspector] public int rootJointId { get; private set; } = -1;
    [HideInInspector] public int nbJoint { get { return m_joints.Count; } }
    [HideInInspector] public string prefabName = "Core";

    [HideInInspector] public float angle = 0.0f;

    private bool m_isSelected = false;
    public bool isSelected
    {
        get { return m_isSelected; }
        set
        {
            foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.SetShaderPassEnabled("Always", value);
            }
            m_isSelected = value;
        }
    }

    // Properties
    [HideInInspector] public List<Property> properties = new List<Property>();

    // Callback when destroyed
    public UnityEvent onDestroy = new UnityEvent();

    protected virtual void Awake()
    {
        // Get all joints in the part
        PartJoint joint = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            joint = transform.GetChild(i).GetComponent<PartJoint>();
            if (joint != null)
            {
                joint.part = this;
                m_joints.Add(joint);
            }
        }

        // set root joint if not root part
        if (!isRoot && m_joints.Count > 0)
        {
            rootJointId = 0;
            m_rootJoint = m_joints[0];
        }
        isSelected = false;
    }

    public void SetRootJoint(int _id)
    {
        if(_id >= 0 && _id < m_joints.Count)
        {
            //PartJoint tmp = m_rootJoint.connection;
            //Detach();
            rootJointId = _id;
            m_rootJoint = m_joints[_id];
        }
    }

    public void AttachTo(PartJoint _joint, bool dontDisableOther = false)
    {
        if (m_rootJoint != null && _joint != null)
        {
            // Attach joint and set position and rotation of the part accordingly
            m_rootJoint.AttachTo(_joint, dontDisableOther);
            Vector3 vOffset = m_rootJoint.transform.localPosition;
            Quaternion qOffset = m_rootJoint.transform.localRotation;
            transform.rotation = m_rootJoint.rotation * Quaternion.Euler(0, 0, angle) * Quaternion.Inverse(qOffset);
            transform.position = m_rootJoint.position - transform.rotation * vOffset;
            if (transform.root != transform)
            {
                transform.root.GetComponent<Rigidbody>().mass += mass;
            }
        }
    }

    public void Detach()
    {
        if (m_rootJoint != null)
        {
            m_rootJoint.Detach();
            if (transform.root != transform)
            {
                transform.root.GetComponent<Rigidbody>().mass -= mass;
            }
        }
    }

    public void DeleteAll()
    {
        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                joint.connection.part.DestroyRecursively();
            }
        }
    }

    public void GetDamage(int _amount)
    {
        health -= _amount;
        if(health <= 0)
        {
            Destroy();
        }
    }

    // Destroy only this part and move other parts in new empty gameobjects 
    public void Destroy()
    {
        Detach();
        Debris debris = null;
        Part part = null;

        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                part = joint.connection.part;
                debris = Instantiate(DataManager.instance.prefabDebris, 
                    part.transform.position, part.transform.rotation);
                part.SetParentRecursively(debris.transform);

                debris.ComputeCenterOfMass();
                ApplyExplosion(debris.GetComponent<Rigidbody>());
            }
        }

        VehicleEditorController vehicle = transform.root.GetComponent<VehicleEditorController>();
        Debris deb = transform.root.GetComponent<Debris>();
        if (vehicle != null)
        {
            vehicle.ComputeCenterOfMass();
            ApplyExplosion(vehicle.GetComponent<Rigidbody>());
        }
        else if(deb != null)
        {
            deb.ComputeCenterOfMass();
            ApplyExplosion(deb.GetComponent<Rigidbody>());
        }

        Debug.Log("Explode");
        Instantiate(DataManager.instance.prefabExplosion, transform.position, Quaternion.identity);

        onDestroy.Invoke();
        Destroy(gameObject);
    }

    public void ApplyExplosion(Rigidbody _rb)
    {
        if(_rb != null)
        {
            _rb.AddExplosionForce(2000.0f, transform.position, 10.0f);
        }
    }

    public void SetParentRecursively(Transform _parent)
    {
        transform.SetParent(_parent, true);
        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                joint.connection.part.SetParentRecursively(_parent);
            }
        }
    }

    public void DestroyRecursively()
    {
        Detach();
        foreach (PartJoint joint in m_joints)
        {
            if (joint != m_rootJoint && joint.connection != null && joint.connection.part != null)
            {
                joint.connection.part.DestroyRecursively();
            }
        }
        //DeleteAll();
        Destroy(gameObject);
    }

    public int GetJointID(PartJoint _joint)
    {
        int jointId = -1;
        for(int i = 0; i < m_joints.Count; i++)
        {
            if(m_joints[i] == _joint)
            {
                jointId = i;
                break;
            }
        }
        return jointId;
    }

    public PartJoint GetJointFromID(int _id)
    {
        if (_id >= 0 && _id < m_joints.Count)
            return m_joints[_id];
        return null;
    }

    public JSONPart Serialize()
    {
        JSONPart json = new JSONPart {
            id = id,
            prefabName = prefabName,
            partTargetId = m_rootJoint != null && parentJoint != null ? parentJoint.part.id : -1,
            rootJoint = rootJointId,
            constraintJoint = m_rootJoint != null && parentJoint != null ? parentJoint.id : -1,
            angle = angle,
            properties = new List<JSONProperty>()
        };

        foreach(Property prop in properties)
        {
            json.properties.Add(prop.Serialize());
        }

        return json;
    }

    public void Deserialize(JSONPart _json)
    {
        prefabName = _json.prefabName;
        //id = _json.id;
        angle = _json.angle;
        rootJointId = _json.rootJoint;
        m_rootJoint = m_joints[_json.rootJoint];

        if (_json.properties != null)
        {
            for (int i = 0; i < _json.properties.Count; i++)
            {
                properties[i].Deserialize(_json.properties[i]);
            }
        }
    }

}

[System.Serializable]
public struct JSONPart
{
    public int id;
    public string prefabName;
    public int partTargetId;
    public int rootJoint;
    public int constraintJoint;
    public float angle;
    public List<JSONProperty> properties;
}
