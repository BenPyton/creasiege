using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : Part
{
    [SerializeField] float power = 500.0f;
    [SerializeField] ParticleSystem particles;
    Rigidbody rb;
    Vector3 direction;

    Ref<KeyCode> key = new Ref<KeyCode>(KeyCode.None);

    protected override void Awake()
    {
        base.Awake();
        properties.Add(new Property("Key Binding", key));
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null || transform.root.name != "Vehicle")
            return;

        if (rb == null)
            rb = transform.parent.GetComponent<Rigidbody>();

        if (DataManager.instance.mode != Mode.Play)
        {
            particles.enableEmission = parentJoint == null;

            return;
        }

        bool thrust = false;
        if (key.asValue == KeyCode.None)
        {
            Quaternion from = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            Quaternion to = Quaternion.LookRotation(transform.parent.forward, transform.parent.up);

            Quaternion quat = to * Quaternion.Inverse(from);

            direction = quat * new Vector3(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical"),
                Input.GetAxis("Applicate")
                ).normalized;
            thrust = Vector3.Dot(transform.up, direction) > 0.5f;
        }
        else
        {
            thrust = Input.GetKey(key.asValue);
        }
        
        if (thrust)
        {
            rb.AddForceAtPosition(transform.up * power, transform.position);
            if(particles != null)
            {
                particles.enableEmission = true;
            }
        }
        else
        {
            if (particles != null)
            {
                particles.enableEmission = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + 100.0f * transform.up);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + 100.0f * direction);
    }
}
