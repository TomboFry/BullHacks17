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
	public int countdown = 3;

	private State state = State.PRE;
	private KartController winner;

	void Update() {
		switch (state) {
		case State.PRE:
			state = State.RACE;
			break;
		case State.RACE:
			int position = 0;

			foreach (KartController kart in karts.OrderByDescending(kart => kart.Lap).ThenByDescending(kart => kart.SplinePosition)) {
				string positionSuffix;
                
				switch (++position) {
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

				kart.lapNumText.text = "Lap " + (kart.Lap + 1) + " / " + laps;
                kart.PlayerPosTxt.text = position.ToString();
                kart.PlayerPosSuffixTxt.text = positionSuffix;

				if (kart.Lap >= laps && winner == null) {
					winner = kart;
					state = State.FINISH;
				}
			}
			break;
		case State.FINISH:
			break;
		}
	}
}
