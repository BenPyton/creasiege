using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] GameObject prefabProjectile;
    Transform target;
    bool shot = false;
    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<VehicleEditorController>().transform;
        StartCoroutine(ShotCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (DataManager.instance.mode != Mode.Play)
        {
            shot = false;
            return;
        }

        if (target != null)
        {
            transform.rotation = Quaternion.LookRotation((target.position - transform.position).normalized, transform.up);
            shot = true;
        }
        else
        {
            shot = false;
        }
    }

    IEnumerator ShotCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.value);
            if(shot)
            {
                Debug.Log("Shot");
                Instantiate(prefabProjectile, transform.position, transform.rotation * Quaternion.Euler(Random.Range(-2, 2), Random.Range(-2, 2), 0.0f));
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
}
