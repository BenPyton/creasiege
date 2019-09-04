using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BeltWindow : EditorWindow
{
    [MenuItem("Window/Belt")]
    public static void OpenWindow()
    {
        EditorWindow window = GetWindow<BeltWindow>();
    }

    List<GameObject> prefabs = null;
    Vector3 range = Vector3.zero;
    int number = 1;
    float scaleRange = 0.0f;
    Transform container = null;

    private void OnEnable()
    {
        if (prefabs == null)
        {
            prefabs = new List<GameObject>();
            prefabs.Add(null);
            prefabs.Add(null);
            prefabs.Add(null);
        }
    }

    private void OnGUI()
    {

        container = (Transform)EditorGUILayout.ObjectField("Container", container, typeof(Transform), true);

        prefabs[0] = (GameObject)EditorGUILayout.ObjectField(prefabs[0], typeof(GameObject), false);
        prefabs[1] = (GameObject)EditorGUILayout.ObjectField(prefabs[1], typeof(GameObject), false);
        prefabs[2] = (GameObject)EditorGUILayout.ObjectField(prefabs[2], typeof(GameObject), false);

        range = EditorGUILayout.Vector3Field("Position Range", range);
        scaleRange = EditorGUILayout.FloatField("Scale Range", scaleRange);

        number = EditorGUILayout.IntField("Number", number);

        if (GUILayout.Button("Generate"))
        {
            GameObject obj = null;
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            for(int i = 0; i < number; i++)
            {
                obj = prefabs[Random.Range(0, prefabs.Count)];
                pos = new Vector3(2.0f * (Random.value-0.5f) * range.x, 2.0f * (Random.value - 0.5f) * range.y, 2.0f * (Random.value - 0.5f) * range.z);
                rot = Random.rotationUniform;
                GameObject go;
                if (container)
                {
                    go = Instantiate(obj, pos, rot, container);
                }
                else
                {
                    go = Instantiate(obj, pos, rot);
                }
                go.transform.localScale = (0.8f + Random.value * 0.4f) * scaleRange * Vector3.one;
            }
        }

        //List<Item> items = DataManager.instance.GetAll<Item>();

        //EditorGUILayout.BeginHorizontal();
        //int tmpIndex = selectedItem;
        //selectedItem = EditorGUILayout.Popup(selectedItem, items.Select(x => x.id).ToArray(), GUILayout.Width(200));
        //if (tmpIndex != selectedItem)
        //{
        //    item = new Item(items[selectedItem]);
        //    //item.id = items[selectedItem].id;
        //    //item.name = items[selectedItem].name;
        //    //item.price = items[selectedItem].price;
        //}

        //newId = EditorGUILayout.TextField(newId);

        //if (GUILayout.Button("+"))
        //{
        //    item = new Item();
        //    item.id = newId;
        //}

        //EditorGUILayout.EndHorizontal();

        //if (null != item)
        //{
        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.PrefixLabel("Id: ");
        //    EditorGUILayout.LabelField(item.id);
        //    EditorGUILayout.EndHorizontal();
        //    item.name = EditorGUILayout.TextField("Name: ", item.name);
        //    item.price = EditorGUILayout.IntField("Price: ", item.price);
        //    item.consummable = EditorGUILayout.Toggle("Consumable: ", item.consummable);
        //    item.type = (Item.Type)EditorGUILayout.EnumPopup("Type: ", item.type);
        //}


        //if (GUILayout.Button("Save"))
        //{
        //    DataManager.instance.SetData(item.id, item);
        //    DataManager.instance.SaveDatabase<Item>();
        //}

        //if (GUILayout.Button("Picker Item"))
        //{
        //    //DatabasePickerWrapper.OpenWindows<Item>(GetItem);
        //    DatabasePickerWrapper.OpenWindows<Item>(GetItems);
        //}
    }
}
