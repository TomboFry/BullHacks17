using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour {

	public float acceleration = 1.0f;
	public float maxSpeed = 20.0f;
	public float turnSpeed = 5.0f;

	private float velocity;
	private float steeringAngle;
	private float throttleInput;
	private float turnInput;
	private Rigidbody rigidBody;

	void Update() {
		throttleInput = Input.GetAxis("Vertical");
		turnInput = Input.GetAxis("Horizontal");

		if (Mathf.Abs(throttleInput) > 0.05f) {
			velocity += throttleInput * acceleration;
		} else {
			velocity = Mathf.Lerp(velocity, 0, 0.05f);
		}

		velocity = Mathf.Clamp(velocity, (-maxSpeed / 4), maxSpeed);
		steeringAngle = turnInput * (((maxSpeed - velocity) / maxSpeed) + 1.0f) * turnSpeed;

		print(velocity + ", " + steeringAngle);

		transform.Translate(Vector3.forward * (velocity / 3.6f) * Time.deltaTime);
		transform.Rotate(Vector3.up * steeringAngle * Time.deltaTime);
	}
}
