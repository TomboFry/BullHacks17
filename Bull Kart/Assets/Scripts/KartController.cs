using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KartController : MonoBehaviour {

	public PlayerController playerController;
	public float acceleration = 1.0f;
	public float maxSpeed = 20.0f;
	public float turnSpeed = 5.0f;
    public int playerNumber = 1;

	public float SplinePosition {
		get {
			return playerController.spline.GetPosition(transform.position);
		}
	}

	public int Lap { get; private set; }

	private float velocity;
	private float steeringAngle;
	private float throttleInput;
	private float turnInput;
	private float lastSplinePosition = 0.0f;

	void Update() {
		throttleInput = -Input.GetAxis("throttle_" + playerNumber);
		turnInput = Input.GetAxis("steering_" + playerNumber);

		if (Mathf.Abs(throttleInput) > 0.05f) {
			velocity += throttleInput * acceleration;
		} else {
			velocity = Mathf.Lerp(velocity, 0, 0.05f);
		}

		velocity = Mathf.Clamp(velocity, (-maxSpeed / 4), maxSpeed);
		transform.Translate(Vector3.forward * (velocity / 3.6f) * Time.deltaTime);

		Quaternion rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, Time.deltaTime * 20.0f);
		transform.rotation = Quaternion.Euler(rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rotation.eulerAngles.z);

		steeringAngle = turnInput * (((maxSpeed - velocity) / maxSpeed) + 1.0f) * turnSpeed;
		transform.Rotate(Vector3.up * steeringAngle * Time.deltaTime);

		float splinePosition = SplinePosition;

		if (lastSplinePosition > 0.95f && splinePosition < 0.05f) {
			Lap += 1;
		}

		lastSplinePosition = splinePosition;
	}
}

[CustomEditor(typeof(KartController))]
public class KartControllerInspector : Editor {

	void OnSceneGUI() {
		KartController kartController = target as KartController;

		Spline spline = kartController.playerController.spline;
		Vector3 kartPosition = kartController.transform.position;

		Handles.color = Color.cyan;
		Handles.DrawLine(kartPosition, spline.GetClosestPoint(kartPosition));
	}
}
