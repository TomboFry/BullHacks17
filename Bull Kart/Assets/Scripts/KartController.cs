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

	private bool canIncrementLap = false;
	private bool boost = false;
	private float velocity = 0.0f;
	private float boostVelocity = 0.0f;
	private float lastSplinePosition = 0.0f;

	private Camera camera;

	void Start() {
		var rigidbody = GetComponent<Rigidbody>();
		rigidbody.centerOfMass = new Vector3 (0, -0.75f, 0);

		camera = GetComponentInChildren<Camera>();
	}

	void OnCollisionStay(Collision collisionInfo) {
		SpeedBooster booster = collisionInfo.gameObject.GetComponent<SpeedBooster>();

		if (booster) {
			boost = true;
		}
	}

	void OnCollisionExit() {
		boost = false;
	}

	void Update() {
		if (Input.GetButtonDown("reset_" + playerNumber)) {
			transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
			velocity = 0.0f;

			transform.position = playerController.spline.GetClosestPoint(transform.position) + (Vector3.up * 2.0f);
			return;
		}

		float throttleInput = -Input.GetAxis("throttle_" + playerNumber);
		float turnInput = Input.GetAxis("steering_" + playerNumber);

		if (Mathf.Abs(throttleInput) > 0.05f) {
			velocity += throttleInput * acceleration;
		} else {
			velocity = Mathf.Lerp(velocity, 0, 0.05f);
		}

		boostVelocity = boost ? 200.0f : Mathf.Lerp(boostVelocity, 0.0f, 0.05f);
		camera.fieldOfView = 60.0f + (boostVelocity / 5.0f);

		velocity = Mathf.Clamp(velocity, (-maxSpeed / 4), maxSpeed);
		transform.Translate(Vector3.forward * ((velocity + boostVelocity) / 3.6f) * Time.deltaTime);

		Quaternion rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, Time.deltaTime * 20.0f);
		transform.rotation = Quaternion.Euler(rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rotation.eulerAngles.z);

		float steeringAngle = turnInput * (((maxSpeed - velocity) / maxSpeed) + 1.0f) * turnSpeed;
		transform.Rotate(Vector3.up * steeringAngle * Time.deltaTime);

		float splinePosition = SplinePosition;

		if (lastSplinePosition > 0.45f && lastSplinePosition <= 0.5f && splinePosition >= 0.5f && splinePosition < 0.55f) {
			canIncrementLap = true;
		} else if (canIncrementLap && lastSplinePosition > 0.95f && lastSplinePosition <= 1.0f && splinePosition >= 0.0f && splinePosition < 0.05f) {
			Lap += 1;
			canIncrementLap = false;
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
