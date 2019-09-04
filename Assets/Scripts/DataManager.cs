using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public enum Mode
{
    Edit,
    Play
}

public enum MissionType
{
    None,
    RescuePeople,
    DestroyEnemies
}

[DefaultExecutionOrder(-1000)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;

    private Mode m_mode = Mode.Edit;
    public Mode mode {
        get
        {
            return m_mode;
        }
        set
        {
            if(value != m_mode)
            {
                m_mode = value;
                onModeChange.Invoke();
            }
        }
    }

    [SerializeField] public List<Part> prefabParts;
    [SerializeField] public Debris prefabDebris;
    [SerializeField] public GameObject prefabExplosion;
    [SerializeField] public UnityEvent onModeChange = new UnityEvent();
    [SerializeField] public UnityEvent onPartChange = new UnityEvent();
    JsonSerializer serializer = new JsonSerializer();

    private int m_currentPartIndex = -1;
    public int currentPartIndex
    {
        get { return m_currentPartIndex; }
        set
        {
            if(m_currentPartIndex != value)
            {
                m_currentPartIndex = value;
                onPartChange.Invoke();
            }
        }
    }

    public MissionType missionType = MissionType.None;
    public bool missionEnded = false;
    public float missionTimer = -1.0f;
    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

			// Load shared assets
			AssetBundle shared = AssetBundleManager.LoadBundle("shared");
			shared.LoadAllAssets();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetCurrentPartName()
    {
        if (m_currentPartIndex >= 0 && m_currentPartIndex < prefabParts.Count)
            return prefabParts[m_currentPartIndex].name;
        else
            return "None";
    }

    public Part GetPrefabFromName(string _name)
    {
        Part prefab = null;

        foreach(Part part in prefabParts)
        {
            if(part.name == _name)
            {
                prefab = part;
            }
        }

        return prefab;
    }

    public void LoadPartBundles()
    {
        prefabParts.Clear();

        // Lood all parts
        List<BundleFile> bundles = AssetBundleManager.ListBundles("Parts");
        foreach(BundleFile file in bundles)
        {
            Debug.Log("Bundle: " + file.name);
            AssetBundle bundle = AssetBundleManager.LoadBundle(file);
            Object[] assets = bundle.LoadAllAssets();
            foreach(Object asset in assets)
            {
                GameObject obj = asset as GameObject;
                if(obj != null)
                {
                    Part part = obj.GetComponent<Part>();
                    if(part != null)
                    {
                        prefabParts.Add(part);
                    }
                }
            }
        }
    }

    public void SaveVehicle(VehicleEditorController _vehicle, string _filename)
    {
        string path = Path.Combine(DirectoryManager.GetDirectory("Vehicles").FullName, _filename + ".vehicle");

        using (FileStream file = new FileStream(path, File.Exists(path) ? FileMode.Truncate : FileMode.Create))
        using (StreamWriter writer = new StreamWriter(file))
        //using (JsonWriter json = new JsonTextWriter(writer))
        {
            //json.Formatting = Formatting.Indented;
            //serializer.Serialize(json, _vehicle.Serialize());
            writer.Write(JsonUtility.ToJson(_vehicle.Serialize(), true));
        }
    }
    
    public void LoadVehicle(VehicleEditorController _vehicle, string _filename)
    {
        string path = Path.Combine(DirectoryManager.GetDirectory("Vehicles").FullName, _filename + ".vehicle");
        if (!File.Exists(path))
        { Debug.Log("Vehicle not found"); return; }
        using (FileStream file = new FileStream(path, FileMode.Open))
        using (StreamReader reader = new StreamReader(file))
        //using (JsonReader json = new JsonTextReader(reader))
        {
            //_vehicle.Deserialize(serializer.Deserialize<JSONVehicle>(json));
            _vehicle.Deserialize(JsonUtility.FromJson<JSONVehicle>(reader.ReadToEnd()));
        }
    }
}

