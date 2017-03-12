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
<<<<<<< HEAD
=======
	public int countdown = 3;
>>>>>>> 5f0d79619630ff8f407181a2d7f3eb6ab4b485f5

	private State state = State.PRE;
	private KartController winner;

	void Update() {
		switch (state) {
		case State.PRE:
			break;
		case State.RACE:
			int position = 1;

<<<<<<< HEAD
			// TODO: Update kart position and lap in UI

			if (kart.Lap >= laps && winner == null) {
				winner = kart;
				state = State.FINISH;
			}
=======
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
			break;
		case State.FINISH:
			break;
>>>>>>> 5f0d79619630ff8f407181a2d7f3eb6ab4b485f5
		}
	}
}
