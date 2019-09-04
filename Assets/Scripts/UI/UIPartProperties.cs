using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartProperties : MonoBehaviour
{
	[SerializeField] Transform container;
    [SerializeField] UIProperty boolPrefab;
    [SerializeField] UIProperty floatPrefab;
    [SerializeField] UIProperty keyPrefab;

    public void Clear()
    {
        for (int i = 1; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    public void SetProperties(List<Part> _parts)
    {

        //Debug.Log("Set property panel");

        //for (int i = 1; i < container.childCount; i++)
        //{
        //    Destroy(container.GetChild(i).gameObject);
        //}
        Clear();

        if (_parts.Count > 1)
        {
            Debug.Log("To many selected part");
        }
        else if (_parts.Count <= 0)
        {
            Debug.Log("No part selected");
        }
        else // only one part selected
        {
            //Debug.Log("Part: " + _parts[0].name + " | Nb property: " + _parts[0].properties.Count);
            foreach (Property prop in _parts[0].properties)
            {
                UIProperty obj = null;
                switch (prop.type)
                {
                    case Property.Type.Bool:
                        obj = Instantiate(boolPrefab.gameObject, container).GetComponent<UIProperty>();
                        break;
                    case Property.Type.Float:
                        obj = Instantiate(floatPrefab.gameObject, container).GetComponent<UIProperty>();
                        break;
                    case Property.Type.Key:
                        obj = Instantiate(keyPrefab.gameObject, container).GetComponent<UIProperty>();
                        break;
                    default: break;
                }
                if (obj != null)
                {
                    obj.property = prop;
                }
            }
        }
    }
}
