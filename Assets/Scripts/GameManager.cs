using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Joysticks")] 
	public Joystick MovementJoystick;
	public Joystick AimingJoystick;
	
	[Header("Player Variables")] 
	public int MoveSpeed;
	public Transform PlayerTransform;

	private void Awake()
	{
		Observable.EveryUpdate()
			.Subscribe(_ =>
			           {
				           UpdateMovementJoystick();
				           UpdateAimingJoystick();
			           }).AddTo(this);
	}

	private void UpdateMovementJoystick()
	{
		var moveVector = PlayerTransform.right * MovementJoystick.Horizontal + PlayerTransform.forward * MovementJoystick.Vertical;

		if (moveVector == Vector3.zero)
			return;

		PlayerTransform.Translate(moveVector * MoveSpeed * Time.deltaTime, Space.World);
	}

	private void UpdateAimingJoystick()
	{
		var aimVector = Vector3.right * AimingJoystick.Horizontal + Vector3.forward * AimingJoystick.Vertical;

		if (aimVector == Vector3.zero)
			return;

		PlayerTransform.rotation = Quaternion.LookRotation(aimVector);
	}
}