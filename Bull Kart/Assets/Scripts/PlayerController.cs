using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum State {
	PRE,
	RACE,
	FINISH
}

public class PlayerController : MonoBehaviour {

	public Spline spline;
	public KartController[] karts;

	public int laps = 3;

	private State state = State.PRE;
	private KartController winner;

	void Update() {
		int position = 1;

		foreach (KartController kart in karts.OrderByDescending(kart => kart.Lap).ThenByDescending(kart => kart.SplinePosition)) {
			string positionSuffix;

			switch (position++) {
			case 1:
				positionSuffix = "st";
				break;
			case 2:
				positionSuffix = "nd";
				break;
			case 3:
				positionSuffix = "rd";
				break;
			default:
				positionSuffix = "th";
				break;
			}

			// TODO: Update kart position and lap in UI

			if (kart.Lap >= laps && winner == null) {
				winner = kart;
				state = State.FINISH;
			}
		}
	}
}
