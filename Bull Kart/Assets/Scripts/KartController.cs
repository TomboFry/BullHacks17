using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour {

	public float acceleration = 1.0f;
	public float maxSpeed = 20.0f;
	public float turnSpeed = 5.0f;
    public int playernumber = 0;

	private float velocity;
	private float steeringAngle;
	private float throttleInput;
	private float turnInput;
	private Rigidbody rigidBody;

	void Update() {
		throttleInput = -Input.GetAxis("throttle_" + playernumber);

		print (throttleInput);

		turnInput = Input.GetAxis("steering_" + playernumber);

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
	}
}
