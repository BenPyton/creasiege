using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFX : MonoBehaviour
{
    [SerializeField] Rigidbody target;
    ParticleSystem ps = null;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion quat = Quaternion.FromToRotation(-Vector3.forward, target.velocity.normalized);
        transform.rotation = quat;

        transform.position = target.transform.position + target.velocity * 3f;

        ParticleSystem.MainModule main = ps.main;
        main.startSpeedMultiplier = target.velocity.magnitude;
    }
}
