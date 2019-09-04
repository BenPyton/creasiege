using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref<T>
{
    private T[] m_ref;

    public Ref(T _value)
    {
		m_ref = new T[] {_value};
    }

    // copy the value, so no modification on this value
    public T asValue
    {
        get{return m_ref[0];}
        set{m_ref[0] = value;}
    }

    public T[] asRef
    {
        get{return m_ref;}
    }
}

public class Property
{
    public enum Type
    {
        None,
        Bool,
        Float,
        Key
    }

    public Type type = Type.None;

    public object reference = null;
    public string name = "";

    public Property(string _name, Ref<bool> _ref)
    {
        reference = _ref;
        type = Type.Bool;
        name = _name;
    }

    public Property(string _name, Ref<float> _ref)
    {
        reference = _ref;
        type = Type.Float;
        name = _name;
    }

    public Property(string _name, Ref<KeyCode> _ref)
    {
        reference = _ref;
        type = Type.Key;
        name = _name;
    }

    public JSONProperty Serialize()
    {
        JSONProperty json = new JSONProperty { type = type };
        switch (type)
        {
            case Type.Bool:
                json.boolValue = (reference as Ref<bool>).asValue;
                break;
            case Type.Float:
                json.floatValue = (reference as Ref<float>).asValue;
                break;
            case Type.Key:
                json.keyValue = (int)(reference as Ref<KeyCode>).asValue;
                break;
            default:
                break;
        }
        return json;
    }

    public void Deserialize(JSONProperty _json)
    {
        if(type == _json.type)
        {
            switch (type)
            {
                case Type.Bool:
                    (reference as Ref<bool>).asRef[0] = _json.boolValue;
                    break;
                case Type.Float:
                    (reference as Ref<float>).asRef[0] = _json.floatValue;
                    break;
                case Type.Key:
                    (reference as Ref<KeyCode>).asRef[0] = (KeyCode)_json.keyValue;
                    break;
                default:
                    break;
            }
        }
    }
}

[Serializable]
public struct JSONProperty
{
    public Property.Type type;
    public int keyValue;
    public float floatValue;
    public bool boolValue;
}