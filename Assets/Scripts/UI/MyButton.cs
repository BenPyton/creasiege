using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : Button
{
    static BaseEventData m_eventData = null;
    static private BaseEventData eventData {
        get {
            if(m_eventData == null)
            {
                //m_eventData = new BaseEventData(FindObjectOfType<EventSystem>());
				m_eventData = new BaseEventData(EventSystem.current);
            }
            return m_eventData;
        }
    }

    public bool isHighlighted { get { return IsHighlighted(eventData); } }
    public bool isPressed { get { return IsPressed(); } }
    public bool isDisabled { get { return !interactable; } }

    private Text[] m_texts = null;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
		EventSystem.current.SetSelectedGameObject(null);
    }


    public void Update()
    {
        if(m_texts == null)
        {
            m_texts = GetComponentsInChildren<Text>();
        }
        else
        {
            foreach(Text text in m_texts)
            {
                if (isDisabled)
                {
                    text.color = colors.disabledColor;
                }
                else if (isPressed)
                {
                    text.color = colors.pressedColor;
                }
                else if (isHighlighted)
                {
                    text.color = colors.highlightedColor;
                }
                else
                {
                    text.color = colors.normalColor;
                }
            }
        }
    }
}
