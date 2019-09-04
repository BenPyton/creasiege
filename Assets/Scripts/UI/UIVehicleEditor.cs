using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIVehicleEditor : MonoBehaviour
{
    [SerializeField] VehicleEditorController vehicleEditor;
    [SerializeField] GameObject editorUI;
    [SerializeField] GameObject playUI;
    [SerializeField] GameObject pauseUI;
    [SerializeField] Text debugText;
    //[SerializeField] Button switchModeButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] GameObject partSelector;
    [SerializeField] Button prefabButton;
    [SerializeField] GameObject partContent;
    [SerializeField] Button placeButton;
    [SerializeField] Button selectButton;
    [SerializeField] Button eraseButton;
    [SerializeField] Button clearButton;
    [SerializeField] VehiclePanel vehiclePanel;
    [SerializeField] InputField vehicleNameInput;
    [SerializeField] Text missionText;
    [SerializeField] Text missionStatusText;
    [SerializeField] RescueMission rescueMission;
	[SerializeField] UIPartProperties partProperties;


    // Start is called before the first frame update
    void Start()
    {
        DataManager.instance.LoadPartBundles();
        //if (switchModeButton)
        //{
        //    switchModeButton.onClick.AddListener(() =>
        //    {
        //        switch (DataManager.instance.mode)
        //        {
        //            case Mode.Edit:
        //                DataManager.instance.mode = Mode.Play;
        //                switchModeButton.GetComponentInChildren<Text>().text = "Edit";
        //                editorUI.SetActive(false);
        //                playUI.SetActive(true);
        //                switch (DataManager.instance.missionType)
        //                {
        //                    case MissionType.DestroyEnemies:
        //                        missionText.text = "Kill all enemies !";
        //                        break;
        //                    case MissionType.RescuePeople:
        //                        missionText.text = "Rescue people !";
        //                        break;
        //                    default:
        //                        missionText.text = "No mission";
        //                        break;
        //                }
        //                break;
        //            case Mode.Play:
        //                DataManager.instance.mode = Mode.Edit;
        //                switchModeButton.GetComponentInChildren<Text>().text = "Play";
        //                editorUI.SetActive(true);
        //                playUI.SetActive(false);
        //                break;
        //            default:
        //                break;
        //        }
        //    });
        //}

        DataManager.instance.onModeChange.AddListener(() =>
        {
            switch (DataManager.instance.mode)
            {
                case Mode.Edit:
                    partSelector.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    editorUI.SetActive(true);
                    playUI.SetActive(false);
                    //switchModeButton.gameObject.SetActive(true);
                    break;
                case Mode.Play:
                    partSelector.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    //switchModeButton.gameObject.SetActive(false);
                    editorUI.SetActive(false);
                    playUI.SetActive(true);
                    switch (DataManager.instance.missionType)
                    {
                        case MissionType.DestroyEnemies:
                            missionText.text = "Kill all enemies !";
                            break;
                        case MissionType.RescuePeople:
                            missionText.text = "Rescue people !";
                            break;
                        default:
                            missionText.text = "No mission";
                            break;
                    }
                    break;
                default:
                    break;
            }
        });


        for(int i = 0; i < DataManager.instance.prefabParts.Count; i++)
        {
            GameObject button = Instantiate(prefabButton.gameObject, partContent.transform);
            int tmp = i;
            button.GetComponentInChildren<Text>().text = DataManager.instance.prefabParts[i].name;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                DataManager.instance.currentPartIndex = tmp;
                vehicleEditor.currentTool = VehicleEditorController.Tool.Place;
            });
        }

        saveButton.onClick.AddListener(() =>
        {
            if (vehicleNameInput.text != "")
            {
                vehicleEditor.vehicleName = vehicleNameInput.text;
                DataManager.instance.SaveVehicle(vehicleEditor, vehicleEditor.vehicleName);
            }
        });

        loadButton.onClick.AddListener(() =>
        {
            if(null != vehiclePanel)
            {
                vehiclePanel.gameObject.SetActive(!vehiclePanel.gameObject.activeInHierarchy);
            }
        });

        placeButton.onClick.AddListener(() => {
            vehicleEditor.currentTool = VehicleEditorController.Tool.Place;
        });

        selectButton.onClick.AddListener(() => {
            vehicleEditor.currentTool = VehicleEditorController.Tool.Select;
        });

        eraseButton.onClick.AddListener(() => {
            vehicleEditor.currentTool = VehicleEditorController.Tool.Erase;
        });

        clearButton.onClick.AddListener(() => {
            vehicleEditor.Clear();
        });

        vehiclePanel.onVehicleLoaded.AddListener(() =>
        {
            vehicleNameInput.text = vehicleEditor.vehicleName;
        });

        vehicleEditor.onPartSelected.AddListener(() => {
			partProperties.SetProperties(vehicleEditor.selectedParts);
        });


        DataManager.instance.mode = Mode.Edit;
    }

    //string FormatName(string _name)
    //{
    //    string newName = _name.Replace(" ", "_");
    //    return newName.ToLower();
    //}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!DataManager.instance.isPaused);
        }

        if(debugText != null)
        {
            if (DataManager.instance.mode == Mode.Edit)
            {
                debugText.text = "Escape: Pause\n" +
                    "ScrollWheel: Zoom Camera\n" +
                    "ScrollWheel button: Move Camera\n" +
                    "Right click: Rotate Camera\n" +
                    "ZQSD/Arrows: Move forward and right\n" +
                    "Space,Left Shift: Move up,down\n" +
                    "R: Rotate part\n" +
                    "F: Change attached joint\n" +
                    "Left click: " + vehicleEditor.currentTool + "\n" +
                    "Current part: " + DataManager.instance.GetCurrentPartName() + "\n" +
                    "Selected parts: (ctrl for multiple)\n";

                for (int i = 0; i < vehicleEditor.selectedParts.Count; i++)
                {
                    if (vehicleEditor.selectedParts[i] != null)
                    {
                        debugText.text += vehicleEditor.selectedParts[i].name + "\n";
                    }
                }
            }
            else
            {
                debugText.text = "Escape: Pause\n" + 
                    "ScrollWheel: Zoom Camera\n" +
                    "ScrollWheel button: Toggle Camera Mode\n" +
                    "Right click: Rotate Camera\n" +
                    "ZQSD/Arrows: Move updward and right\n" +
                    "Space,Left Shift: Move forward,backward\n" +
                    "Hold left ctrl: Stabilize vehicle\n" +
                    "Left click: Shot\n";
            }

            if(Input.GetKeyDown(KeyCode.F1))
            {
                debugText.gameObject.SetActive(!debugText.gameObject.activeInHierarchy);
            }
        }

        //if (Input.GetKeyDown(KeyCode.Escape) && DataManager.instance.mode == Mode.Play)
        //{
        //    Cursor.lockState = switchModeButton.gameObject.activeInHierarchy ? CursorLockMode.Locked : CursorLockMode.None;
        //    switchModeButton.gameObject.SetActive(!switchModeButton.gameObject.activeInHierarchy);
        //}

        if(missionStatusText != null)
        {
            if(DataManager.instance.missionEnded)
            {
                missionStatusText.text = "Mission Complete !";
                missionStatusText.gameObject.SetActive(true);
            }
            else
            {
                missionStatusText.gameObject.SetActive(DataManager.instance.missionTimer >= 0);
                missionStatusText.text = String.Format("{0:F2}s", DataManager.instance.missionTimer);
            }
        }
        else
        {
            missionStatusText.gameObject.SetActive(false);
        }
    }


    public void Pause(bool _pause)
    {
        pauseUI.SetActive(_pause);
        Time.timeScale = _pause ? 0.0f: 1.0f;
        DataManager.instance.isPaused = _pause;
        if (_pause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if(DataManager.instance.mode == Mode.Play)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void GoBackToMenu()
    {
        Pause(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
    }

    public void ChangeMode(string _mode)
    {
        switch(_mode.ToLower())
        {
            case "edit":
                DataManager.instance.mode = Mode.Edit;
                Cursor.visible = true;
                break;
            case "play":
                DataManager.instance.mode = Mode.Play;
                Cursor.visible = false;
                break;
            default:
                Debug.LogWarning("Warning: " + _mode + " is not a valid mode.");
                break;
        }
        
    }
}
