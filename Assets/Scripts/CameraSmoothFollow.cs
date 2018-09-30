using UniRx;
using UnityEngine;

 public class CameraSmoothFollow : MonoBehaviour
 {
     // The target we are following
     public Transform Target;
     // The distance in the x-z plane to the target
     public float Distance = 10.0f;
     // the height we want the camera to be above the target
     public float Height = 5.0f;
     // How much we 
     public float HeightDamping = 2.0f;
     public float RotationDamping = 3.0f;

	 private Transform cameraTransform;

	 private void Awake()
	 {
		 cameraTransform = GetComponent<Transform>();
		 Observable.EveryLateUpdate().Subscribe(_ => SmoothFollowTarget()).AddTo(this);
	 }

	 private void  SmoothFollowTarget ()
     {
         // Early out if we don't have a target
         if (!Target)
             return;
     
         // Calculate the current rotation angles
         var wantedRotationAngle = Target.eulerAngles.y;
         var wantedHeight = Target.position.y + Height;
         var currentRotationAngle = cameraTransform.eulerAngles.y;
         var currentHeight = cameraTransform.position.y;
     
         // Damp the rotation around the y-axis
         currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, RotationDamping * Time.deltaTime);
 
         // Damp the height
         currentHeight = Mathf.Lerp (currentHeight, wantedHeight, HeightDamping * Time.deltaTime);
 
         // Convert the angle into a rotation
         var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
     
         // Set the position of the camera on the x-z plane to:
         // distance meters behind the target
	     cameraTransform.position = Target.position;
	     cameraTransform.position -= currentRotation * Vector3.forward * Distance;
 
         // Set the height of the camera
	     cameraTransform.position = new Vector3(cameraTransform.position.x, currentHeight, cameraTransform.position.z);
     
         // Always look at the target
	     cameraTransform.LookAt (Target);
     }
 }