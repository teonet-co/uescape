using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image BackGroundImage;
	private Image joystickImage;

	public static Vector3 InputDirection { set; get;}
	// Use this for initialization
	private void Start () 
	{
		BackGroundImage = GetComponent<Image> ();
		joystickImage = transform.GetChild(0).GetComponent<Image> ();
		InputDirection = Vector3.zero;
	}

	public virtual void OnDrag (PointerEventData ped)
	{
		Vector2 pos = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (BackGroundImage.rectTransform, ped.position, ped.pressEventCamera, out pos)) 
		{
			pos.x = (pos.x / BackGroundImage.rectTransform.sizeDelta.x);
			pos.y = (pos.y / BackGroundImage.rectTransform.sizeDelta.y);

			float x = (BackGroundImage.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
			float y = (BackGroundImage.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;
			InputDirection = new Vector3 (x, y, 0);
			InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

			joystickImage.rectTransform.anchoredPosition = new Vector3 (InputDirection.x * (BackGroundImage.rectTransform.sizeDelta.x / 3), InputDirection.y * (BackGroundImage.rectTransform.sizeDelta.y / 3));
		
		}
	}

	public virtual void OnPointerDown (PointerEventData ped)
	{
		OnDrag (ped);
	}

	public virtual void OnPointerUp (PointerEventData ped)
	{
		InputDirection = Vector3.zero;
		joystickImage.rectTransform.anchoredPosition = Vector3.zero;
	}
}