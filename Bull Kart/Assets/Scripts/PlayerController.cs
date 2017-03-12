﻿using System.Collections;
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

	private float lastCountdownTick;

	void Start() {
		lastCountdownTick = Time.timeSinceLevelLoad;

		foreach (KartController kart in karts) {
			kart.disableInput = true;
		}
	}

	void Update() {
		switch (state) {
		case State.PRE:
			if (countdown < 0) {
				state = State.RACE;

				foreach (KartController kart in karts) {
					kart.disableInput = false;
				}
			} else if (Time.timeSinceLevelLoad - lastCountdownTick > 1.0f) {
				lastCountdownTick = Time.timeSinceLevelLoad;
				print(countdown--);
			}
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

				kart.lapNumText.text = "Lap " + Mathf.Clamp(kart.Lap + 1, 1, laps) + " / " + laps;
                kart.PlayerPosTxt.text = position.ToString();
                kart.PlayerPosSuffixTxt.text = positionSuffix;

				if (kart.Lap >= laps && winner == null) {
					winner = kart;
					state = State.FINISH;

					foreach (KartController k in karts) {
						k.disableInput = true;
					}
				}
			}
			break;
		case State.FINISH:
			break;
		}
	}
}
