using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIProperty : MonoBehaviour
{
    [SerializeField] Text propertyNameText;

    [HideInInspector] public Property property = null;

    protected virtual void Start()
    {
        if(property != null && propertyNameText != null)
        {
            propertyNameText.text = property.name;
        }
        else
        {
            Debug.LogWarning("Property name text is null.");
        }
    }
}
