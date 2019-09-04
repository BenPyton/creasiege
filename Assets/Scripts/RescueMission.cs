using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueMission : MonoBehaviour
{
    [SerializeField] private float timerDuration = 3.0f;
    public bool playerNear { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DataManager.instance.missionEnded)
            return;

        if(playerNear)
        {
            if(DataManager.instance.missionTimer == -1)
            {
                DataManager.instance.missionTimer = timerDuration;
            }

            DataManager.instance.missionTimer -= Time.deltaTime;
            if(DataManager.instance.missionTimer <= 0.0f)
            {
                DataManager.instance.missionEnded = true;
            }
        }
        else
        {
            DataManager.instance.missionTimer = -1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
        if(other.transform.root.tag == "Player")
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger enter");
        if (other.transform.root.tag == "Player")
        {
            playerNear = false;
        }
    }
}
