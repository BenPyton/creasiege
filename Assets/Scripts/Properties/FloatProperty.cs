using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatProperty : UIProperty
{
    [SerializeField] InputField input;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(input != null && property != null && property.type == Property.Type.Float)
        {
            input.text = (property.reference as Ref<float>).asValue.ToString();
            input.contentType = InputField.ContentType.DecimalNumber;
            input.onValueChanged.AddListener((string _value) =>
            {
            float value = 0.0f;
            float.TryParse(_value, out value);
                Debug.Log("Float value: " + value);

                //Debug.Log("Toggle changed! Value: " + _value + " | Property: " + property.name + " | Type: " + property.type + " | Value: " + (property.reference as Ref<float>).asRef[0]);
                //(property.reference as Ref<bool>).asRef[0] = _value;
                    (property.reference as Ref<float>).asRef[0] = value;
                //}
            });
        }
    }
}
