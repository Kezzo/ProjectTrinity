using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
	Vector2 lastDirection = Vector2.zero;
    Vector2 startBackgroundPosition;

	private void Start()
	{
        startBackgroundPosition = background.position;
        background.gameObject.SetActive(true);
	}

    public override void OnDrag(PointerEventData eventData)
	{
        lastDirection = eventData.position - ((Vector2) background.position);
		inputVector = lastDirection.magnitude > background.sizeDelta.x / 2f ? lastDirection.normalized : lastDirection / (background.sizeDelta.x / 2f);
		ClampJoystick();
		handle.anchoredPosition = inputVector * background.sizeDelta.x / 2f;

        background.position = eventData.position - handle.anchoredPosition;
    }

	public override void OnPointerDown(PointerEventData eventData)
	{
        base.OnPointerDown(eventData);

		background.position = eventData.position;
		handle.anchoredPosition = Vector2.zero;
    }

	public override void OnPointerUp(PointerEventData eventData)
	{
        base.OnPointerUp(eventData);

		inputVector = Vector2.zero;
        background.position = startBackgroundPosition;
        handle.anchoredPosition = Vector2.zero;
    }
}