using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    Rigidbody rb = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        DataManager.instance.onModeChange.AddListener(() =>
        {
            if (DataManager.instance.mode == Mode.Edit)
            {
                Destroy(gameObject);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
