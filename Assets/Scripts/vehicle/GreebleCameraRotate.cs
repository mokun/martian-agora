using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreebleCameraRotate : MonoBehaviour {
		private enum CameraStatus
		{
				rotating,
				waiting
		}
		CameraStatus status;

		private float secondsLeft=0;
		private float rotateMultiplier=1;

	void Update () {
				secondsLeft -= Time.deltaTime;
				if (secondsLeft < 0) {
						secondsLeft = Random.Range (0.2f, 2f) * Random.Range (0.2f, 2f);

						if (status == CameraStatus.rotating) {
								status = CameraStatus.waiting;
								secondsLeft /= 2f;
						} else {
								status = CameraStatus.rotating;
								rotateMultiplier = Random.Range (20f, 80f);
								if (Random.Range (0, 2) < 1)
										rotateMultiplier *= -1;
						}
						
				}

				if (status == CameraStatus.rotating) {
						transform.RotateAround (transform.position, Vector3.up, Time.deltaTime * rotateMultiplier);
				}
	}
}
