using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenu : MonoBehaviour
{
    [SerializeField] float speed = 0.8f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation *= Quaternion.Euler(20 * speed * Time.deltaTime, speed * 10 * Mathf.Sin(0.2f*Time.time) * Time.deltaTime + 0 * speed * Time.deltaTime, 40*Mathf.Cos(0.1f * Time.time) * speed * Time.deltaTime + 10.0f* speed * Time.deltaTime);
    }
}
