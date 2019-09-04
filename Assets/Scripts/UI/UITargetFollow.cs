using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITargetFollow : MonoBehaviour
{
    [SerializeField] Transform targetUI;
    [SerializeField] Transform outsideUI;
    Transform target = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Finish");
        if (obj != null)
        {
            target = obj.transform;
        }

		targetUI.gameObject.SetActive(false);
		outsideUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(target != null)
        {
            Vector3 viewPos = Camera.main.WorldToViewportPoint(target.position);
            if(viewPos.x >= 0 && viewPos.y >=0 && viewPos.x <= 1.0f && viewPos.y <= 1.0f && viewPos.z >= 0)
            {
                targetUI.gameObject.SetActive(true);
                outsideUI.gameObject.SetActive(false);
            }
            else
            {
                targetUI.gameObject.SetActive(false);
                outsideUI.gameObject.SetActive(true);
            }


            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
            targetUI.position = screenPos;
            
            Vector3 targetDirection = Vector3.Scale(new Vector3(1, 1, 0), Camera.main.transform.worldToLocalMatrix.MultiplyPoint(target.position)).normalized;
            outsideUI.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg);
        }
        else
        {
            targetUI.gameObject.SetActive(false);
            outsideUI.gameObject.SetActive(false);
        }
    }
}
