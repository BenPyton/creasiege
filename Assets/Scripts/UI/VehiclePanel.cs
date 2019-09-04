using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VehiclePanel : MonoBehaviour
{
    [SerializeField] GameObject prefabButton;
    [SerializeField] Transform content;
    [SerializeField] VehicleEditorController vehicleEditor;

    public UnityEvent onVehicleLoaded;
    public UnityEvent beforeVehicleLoad;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        DirectoryManager.GetDirectory("Vehicles").GetFiles()
            .Where((FileInfo file) => file.Name.EndsWith(".vehicle")).ToList()
            .ForEach((FileInfo file) =>
        {
            Button button = Instantiate(prefabButton, content).GetComponent<Button>();
            string vehicleName = file.Name.Substring(0, file.Name.IndexOf('.'));
            button.GetComponentInChildren<Text>().text = vehicleName;
            button.onClick.AddListener(() =>
            {
                beforeVehicleLoad.Invoke();
                DataManager.instance.LoadVehicle(vehicleEditor, vehicleName);
                gameObject.SetActive(false);
                onVehicleLoaded.Invoke();
            });
        });
    }
}

