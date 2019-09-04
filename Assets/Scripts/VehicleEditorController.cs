using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class VehicleEditorController : MonoBehaviour
{
    public enum Tool
    {
        Place,
        Select,
        Erase,
        Clear
    }

    [SerializeField] Vector3 startPosition;

    [SerializeField] Material transparentMaterial;

    Part partPreview = null;

    Rigidbody rb;

    public List<Part> selectedParts = new List<Part>();

    public string vehicleName = "";

    int m_currentPartID = 0;

    public Tool currentTool = Tool.Place;

    [HideInInspector] float m_partAngle = 0.0f;

    public UnityEvent onPartSelected = new UnityEvent();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();
        DataManager.instance.onModeChange.AddListener(() =>
        {
            switch (DataManager.instance.mode)
            {
                case Mode.Edit:
                    ResetVehicle();
                    rb.isKinematic = true;
                    DataManager.instance.missionEnded = false;
                    DataManager.instance.LoadVehicle(this, "../tmp");
                    break;
                case Mode.Play:
                    DataManager.instance.SaveVehicle(this, "../tmp");
                    rb.isKinematic = false;
                    if (partPreview != null)
                        partPreview.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            ComputeCenterOfMass();
            foreach (Part part in selectedParts)
            {
                part.isSelected = false;
            }
            selectedParts.Clear();
            onPartSelected.Invoke();
        });

        DataManager.instance.onPartChange.AddListener(() => {

            if (partPreview != null)
            {
                partPreview.Detach();
                Destroy(partPreview.gameObject);
            }
            if (DataManager.instance.currentPartIndex >= 0 && DataManager.instance.currentPartIndex < DataManager.instance.prefabParts.Count)
            {
                partPreview = Instantiate(DataManager.instance.prefabParts[DataManager.instance.currentPartIndex].gameObject).GetComponent<Part>();
                foreach(MeshRenderer mesh in partPreview.GetComponentsInChildren<MeshRenderer>())
                {
                    mesh.material = transparentMaterial;
                }
                foreach (Collider c in partPreview.GetComponentsInChildren<Collider>())
                {
                    c.enabled = false;
                }
                partPreview.gameObject.SetActive(currentTool == Tool.Place);
            }
        });

        yield return new WaitForSeconds(0.1f);



        DataManager.instance.LoadVehicle(this, "Basic");
    }

    void ResetVehicle()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.End))
        {
            ResetVehicle();
        }


        rb.drag = 0.0f;
        rb.angularDrag = 0.0f;
        if (DataManager.instance.mode != Mode.Edit)
        {

            if(Input.GetKey(KeyCode.LeftControl))
            {
                rb.drag = 0.8f;
                rb.angularDrag = 0.8f;
            }

            if(Input.GetKeyDown(KeyCode.Delete))
            {
                foreach(Part part in selectedParts)
                {
                    part.Destroy();
                }
            }

            return;
        }

        // Do not do anything if cursor is over UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (partPreview != null)
            {
                partPreview.Detach();
                partPreview.gameObject.SetActive(false);
            }
            return;
        }

        if (currentTool == Tool.Place && DataManager.instance.currentPartIndex >= 0 && DataManager.instance.currentPartIndex < DataManager.instance.prefabParts.Count)
        {
            Vector3 mousePos;
            if (GetMouseWorldPosition(out mousePos))
            {
                PartJoint joint = GetNearestJoint(mousePos);

                if (joint != null && partPreview != null)
                {
                    if (partPreview.parentJoint != joint && !joint.isAttached)
                    {
                        //partPreview.transform.position = joint.transform.position;
                        partPreview.AttachTo(joint, true);
                        partPreview.gameObject.SetActive(true);
                    }
                }
                else if (partPreview != null)
                {
                    partPreview.Detach();
                    partPreview.gameObject.SetActive(false);
                }
            }
            else if (partPreview != null)
            {
                partPreview.Detach();
                partPreview.gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonDown(0) && partPreview.gameObject.activeInHierarchy)
            {
                Part obj = Instantiate(DataManager.instance.prefabParts[DataManager.instance.currentPartIndex].gameObject, transform).GetComponent<Part>();
                PartJoint joint = partPreview.parentJoint;
                obj.angle = partPreview.angle;
                obj.SetRootJoint(partPreview.rootJointId);
                partPreview.Detach();
                obj.AttachTo(joint);
                //obj.id = m_currentPartID;
                obj.prefabName = DataManager.instance.prefabParts[DataManager.instance.currentPartIndex].gameObject.name;
                m_currentPartID++;

                //obj.onDestroy.AddListener(() =>
                //{
                //    ComputeCenterOfMass();
                //    obj.ApplyExplosion(rb);
                //});

                ComputeCenterOfMass();
            }

            // Rotate around the constraint axis
            if(Input.GetKeyDown(KeyCode.R))
            {
                m_partAngle += 90.0f;
                if(m_partAngle >= 360.0f)
                {
                    m_partAngle -= 360.0f;
                }
                partPreview.angle = m_partAngle;
                partPreview.AttachTo(partPreview.parentJoint, true);
            }

            // switch the root joint
            if (Input.GetKeyDown(KeyCode.F))
            {
                PartJoint tmp = partPreview.parentJoint;
                partPreview.Detach();
                partPreview.SetRootJoint((partPreview.rootJointId + 1) % partPreview.nbJoint);
                partPreview.AttachTo(tmp, true);
            }
        }

        if (currentTool == Tool.Select)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                {
                    foreach(Part part in selectedParts)
                    {
                        part.isSelected = false;
                    }
                    selectedParts.Clear();
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200.0f, -1, QueryTriggerInteraction.Ignore))
                {
                    Part part = hit.collider.gameObject.GetComponentInParent<Part>();
                    if (part != null && part.transform != transform.GetChild(0))
                    {
                        if (selectedParts.Contains(part))
                        {
                            selectedParts.Remove(part);
                            part.isSelected = false;
                        }
                        else
                        {
                            selectedParts.Add(part);
                            part.isSelected = true;
                        }
                    }
                }

                onPartSelected.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                foreach (Part part in selectedParts)
                {
                    part.Detach();
                }
                foreach (Part part in selectedParts)
                {
                    part.DestroyRecursively();
                }
                selectedParts.Clear();
            }
        }

        if (currentTool == Tool.Erase)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200.0f, -1, QueryTriggerInteraction.Ignore))
                {
                    Part part = hit.collider.gameObject.GetComponentInParent<Part>();
                    if (part != null && part.transform != transform.GetChild(0))
                    {
                        part.DestroyRecursively();
                        ComputeCenterOfMass();
                    }
                }
            }
        }
    }

    public void ComputeCenterOfMass()
    {
        Vector3 com = Vector3.zero;
        float massSum = 0.0f;
        foreach (Part part in GetComponentsInChildren<Part>())
        {
            massSum += part.mass;
            com += part.mass * part.transform.localPosition;
        }
        rb.centerOfMass = com / massSum;
        rb.mass = massSum;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.localToWorldMatrix.MultiplyPoint(ComputeCenterOfMass()), 0.1f);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.localToWorldMatrix.MultiplyPoint(rb.centerOfMass), 0.2f);
    //}

    public void Clear()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Part>().Detach();
        }

        while (transform.childCount > 1)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
        }
    }

    bool GetMouseWorldPosition(out Vector3 _position)
    {
        bool hasHit = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        _position = Vector3.zero;
        if(Physics.Raycast(ray, out hit, 200.0f, -1, QueryTriggerInteraction.Ignore))
        {
            _position = hit.point;
            hasHit = true;
        }

        return hasHit;
    }

    List<PartJoint> GetAllJointInRadius(Vector3 _center, float _radius)
    {
        Collider[] colliders = Physics.OverlapSphere(_center, _radius, LayerMask.GetMask("Joints"), QueryTriggerInteraction.Collide);
        return colliders.Select((Collider x) => x.GetComponent<PartJoint>()).ToList();
    }

    PartJoint GetNearestJoint(Vector3 _position)
    {
        PartJoint joint = null;

        foreach (PartJoint j in GetAllJointInRadius(_position, 2.0f))
        {
            if(joint == null || (j.transform.position - _position).sqrMagnitude < (joint.transform.position - _position).sqrMagnitude)
            {
                joint = j;
            }
        }

        return joint;
    }

    public Part GetPartByID(int _id)
    {
        Part part = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Part>().id == _id)
            {
                part = transform.GetChild(i).GetComponent<Part>();
                break;
            }
        }
        return part;
    }


    public JSONVehicle Serialize()
    {
        JSONVehicle vehicleJson = new JSONVehicle();
        vehicleJson.version = "0.0.1";
        vehicleJson.name = vehicleName;
        vehicleJson.parts = new List<JSONPart>();
        for (int i = 1; i < transform.childCount; i++)
        {
            Part part = transform.GetChild(i).GetComponent<Part>();
            if (part != null)
            {
                vehicleJson.parts.Add(part.Serialize());
            }
        }

        return vehicleJson;
    }

    public void Deserialize(JSONVehicle _json)
    {
        Clear();
        m_currentPartID = _json.parts.Count;
        vehicleName = _json.name;
        selectedParts.Clear();
        foreach(JSONPart jsonPart in _json.parts)
        {
            Part prefab = DataManager.instance.GetPrefabFromName(jsonPart.prefabName);
            if (prefab != null)
            {
                Part part = Instantiate(prefab.gameObject, transform).GetComponent<Part>();
                part.Deserialize(jsonPart);
                part.AttachTo(GetPartByID(jsonPart.partTargetId).GetJointFromID(jsonPart.constraintJoint));
            }
        }
        ComputeCenterOfMass();
    }

}

[System.Serializable]
public struct JSONVehicle
{
    public string version;
    public string name;
    public List<JSONPart> parts;
}
