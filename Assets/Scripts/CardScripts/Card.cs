using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {
    [HideInInspector] public UnityEvent PointerUpEvent;
    
    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Debug.Log("Is Clicked");
        PointerUpEvent.Invoke();
        
        // destroy self
        Destroy(gameObject);
    }
    
    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //Debug.Log("Pointer Down");
        //PointerUpEvent.Invoke(this);
    }
}