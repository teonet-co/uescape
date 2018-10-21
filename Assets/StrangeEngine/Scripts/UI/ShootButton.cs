using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using StrangeEngine;
public class ShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public bool buttonDown;

    public void OnPointerDown(PointerEventData data)
    {
        buttonDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonDown = false;
    }
}