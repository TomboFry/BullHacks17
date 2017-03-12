using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType {
	NONE,
	BOOST,
	TRIPLE_BOOST
}

public class PickupBox : MonoBehaviour {

	public PickupType type = PickupType.BOOST;
	public float timeout = 5.0f;

	private float lastPickedUpTime = 0.0f;
	private MeshRenderer meshRenderer;
	private Collider collider;

	void Start() {
		meshRenderer = GetComponent<MeshRenderer>();
		collider = GetComponent<Collider>();
	}

	void Update() {
		if (Time.time - lastPickedUpTime > timeout) {
			meshRenderer.enabled = true;
			collider.enabled = true;
			return;
		}
	}

	void OnCollisionEnter(Collision collisionInfo) {
		KartController kart = collisionInfo.gameObject.GetComponent<KartController>();

		if (kart != null) {
			if (kart.powerup == PickupType.NONE) {
				kart.powerup = type;
				kart.pickupUses = (type == PickupType.TRIPLE_BOOST) ? 3 : 1;
			}

			lastPickedUpTime = Time.time;
			meshRenderer.enabled = false;
			collider.enabled = false;
		}
	}
}
