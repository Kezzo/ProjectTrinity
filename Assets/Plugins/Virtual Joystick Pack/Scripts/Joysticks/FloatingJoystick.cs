using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
	Vector2 joystickCenter = Vector2.zero;
	Vector2 lastDirection = Vector2.zero;
	private bool joystickActive;

	private void Start()
	{
		background.gameObject.SetActive(false);
	}

    private void Update()
    {
        /*
                            * Needs to snap to the center, so if the player drags and stops, so should the controller
                            * Needs to unsnap from sides, so if it hits the outer limits, if the vector is negative, it would move backwards
                            */
        var xBound = background.localPosition.x < 600 && background.localPosition.x > 0;
        var yBound = background.localPosition.y < 900 && background.localPosition.y > 200;
        var movement = new Vector2();
        if (xBound)
            movement = new Vector2(lastDirection.normalized.x, movement.y);

        if (yBound)
            movement = new Vector2(movement.x, lastDirection.normalized.y);

        background.anchoredPosition = movement + background.anchoredPosition;
    }

    public override void OnDrag(PointerEventData eventData)
	{
		lastDirection = eventData.position - joystickCenter;
		inputVector = lastDirection.magnitude > background.sizeDelta.x / 2f ? lastDirection.normalized : lastDirection / (background.sizeDelta.x / 2f);
		ClampJoystick();
		handle.anchoredPosition = inputVector * background.sizeDelta.x / 2f * handleLimit;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		joystickActive = true;
		background.gameObject.SetActive(true);
		background.position = eventData.position;
		handle.anchoredPosition = Vector2.zero;
		joystickCenter = eventData.position;
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		joystickActive = false;
		background.gameObject.SetActive(false);
		inputVector = Vector2.zero;
	}
}