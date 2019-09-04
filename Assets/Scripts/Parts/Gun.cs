using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Part
{
    [SerializeField] GameObject head;
    [SerializeField] Transform shotStart;
    [SerializeField] GameObject prefabBullet;

    Vector3 targetDirection = Vector3.forward;
    Vector3 currentDirection = Vector3.forward;
    Vector3 speedDirection = Vector3.zero;
    float dampDirection = 0.2f;

    bool shoot = false;
    Coroutine shootCoroutine = null;

    Ref<bool> followCamera = new Ref<bool>(true);
    Ref<float> angleA = new Ref<float>(0.0f);
    Ref<float> angleB = new Ref<float>(0.0f);
    Ref<KeyCode> key = new Ref<KeyCode>(KeyCode.None);

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        properties.Add(new Property("Follow Camera", followCamera));
        properties.Add(new Property("Angle A", angleA));
        properties.Add(new Property("Angle B", angleB));
        properties.Add(new Property("Key Binding", key));
        //Debug.Log("Awake gun | Nb Properties: " + properties.Count);
    }

    private void OnEnable()
    {
        targetDirection = transform.rotation * Vector3.forward;
        currentDirection = transform.rotation * Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null || transform.root.name != "Vehicle")
            return;

        Quaternion quat = Quaternion.identity;

        if (DataManager.instance.mode == Mode.Play && followCamera.asValue == true)
        {
            quat = Camera.main.transform.rotation;
        }
        else
        {
            quat = transform.rotation * Quaternion.Euler(-angleB.asValue, angleA.asValue, 0.0f);
        }
        targetDirection = quat * Vector3.forward;
        currentDirection = Vector3.SmoothDamp(currentDirection, targetDirection, ref speedDirection, dampDirection);
        Quaternion worldQuat = Quaternion.LookRotation(currentDirection, transform.up);

        Vector3 localRot = (transform.worldToLocalMatrix.rotation * worldQuat).eulerAngles;
        Quaternion targetQuat = Quaternion.Euler(Mathf.Clamp(localRot.x > 180 ? localRot.x - 360.0f : localRot.x, -85, 30), localRot.y, localRot.z);

        head.transform.localRotation = targetQuat;

        if (DataManager.instance.mode == Mode.Play)
        {
            if(ButtonDown(key.asValue))
            {
                shoot = true;
                if(shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                }
                shootCoroutine = StartCoroutine(ShootCoroutine(0.3f));
            }
        }

        if (ButtonUp(key.asValue) || DataManager.instance.mode != Mode.Play)
        {
            shoot = false;
        }
    }

    bool ButtonDown(KeyCode key)
    {
        return (key == KeyCode.None && Input.GetMouseButtonDown(0)) || (key != KeyCode.None && Input.GetKeyDown(key));
    }

    bool ButtonUp(KeyCode key)
    {
        return (key == KeyCode.None && Input.GetMouseButtonUp(0)) || (key != KeyCode.None && Input.GetKeyUp(key));
    }

    IEnumerator ShootCoroutine(float _delay)
    {
        yield return new WaitForSeconds(Random.value * _delay);
        do
        {
            GameObject obj = Instantiate(prefabBullet);
            obj.transform.position = shotStart.position;
            obj.transform.rotation = shotStart.rotation;
            obj.GetComponent<Projectile>().ally = true;
            yield return new WaitForSeconds(_delay);
        } while (shoot);
        shootCoroutine = null;
    }
}
