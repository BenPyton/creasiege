using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoolProperty : UIProperty
{
    [SerializeField] Toggle toggle;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(toggle != null && property != null && property.type == Property.Type.Bool)
        {
            toggle.isOn = (property.reference as Ref<bool>).asValue;
            toggle.onValueChanged.AddListener((bool _value) =>
            {
                Debug.Log("Toggle changed! Value: " + _value + " | Property: " + property.name + " | Type: " + property.type + " | Value: " + (property.reference as Ref<bool>).asRef[0]);
                //(property.reference as Ref<bool>).asRef[0] = _value;
                    (property.reference as Ref<bool>).asRef[0] = _value;
                //}
            });
        }
    }
}
