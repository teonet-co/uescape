using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class VirtualJoystick2 : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image BackGroundImage;
	private Image joystickImage;

	public static Vector3 InputDirection2 { set; get;}
	// Use this for initialization
	private void Start () 
	{
		BackGroundImage = GetComponent<Image> ();
		joystickImage = transform.GetChild(0).GetComponent<Image> ();
		InputDirection2 = Vector3.zero;
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
			InputDirection2 = new Vector3 (x, y, 0);
			InputDirection2 = (InputDirection2.magnitude > 1) ? InputDirection2.normalized : InputDirection2;

			joystickImage.rectTransform.anchoredPosition = new Vector3 (InputDirection2.x * (BackGroundImage.rectTransform.sizeDelta.x / 3), InputDirection2.y * (BackGroundImage.rectTransform.sizeDelta.y / 3));
		
		}
	}

	public virtual void OnPointerDown (PointerEventData ped)
	{
		OnDrag (ped);
	}

	public virtual void OnPointerUp (PointerEventData ped)
	{
		InputDirection2 = Vector3.zero;
		joystickImage.rectTransform.anchoredPosition = Vector3.zero;
	}
}