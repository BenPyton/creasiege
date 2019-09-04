using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject prefabExplode;
    Rigidbody rb;

    public int life = 10;

    EnemyWeapon[] weaponList;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        weaponList = GetComponentsInChildren<EnemyWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GetDamage(int _amount)
    {
        life -= _amount;
        if(life <= 0)
        {
            Instantiate(prefabExplode, transform.position, Quaternion.identity);
            DataManager.instance.missionEnded = true;
            Destroy(gameObject);
        }
    }
}
