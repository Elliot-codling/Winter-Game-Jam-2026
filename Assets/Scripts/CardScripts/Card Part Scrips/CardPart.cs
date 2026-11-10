using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CardPart : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
    [HideInInspector] public UnityEvent PointerUpEvent;
    [HideInInspector] public UnityEvent PointerDownEvent;
    
    [HideInInspector] public UnityEvent PointerEnterEvent;
    [HideInInspector] public UnityEvent PointerExitEvent;
    
    private Vector2 OriginalPosition;
    
    private bool isBeingDragged = false;
    private bool shouldResetPosition = false;
    
    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Debug.Log("Pointer Up");
        PointerUpEvent.Invoke();
        
        isBeingDragged = false;
        shouldResetPosition = true;
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    
    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Debug.Log("Pointer Down");
        
        PointerDownEvent.Invoke();
        
        OriginalPosition = transform.position;
        
        isBeingDragged = true;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        PointerEnterEvent.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData) {
        PointerExitEvent.Invoke();
    }

    private void Update() {
        if (isBeingDragged) {
            transform.position = Mouse.current.position.ReadValue();
        }

        if (shouldResetPosition) {
            transform.position = OriginalPosition;
            shouldResetPosition = false;
        }
    }
}
