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
		private Rover rover=null;

		void Start(){
				Transform parent = transform;
				while (parent != null && parent.gameObject.GetComponent<Rover> () == null)
						parent = parent.transform.parent;

				if(parent!=null)
						rover = parent.gameObject.GetComponent<Rover> ();
		}

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

				if (status == CameraStatus.rotating && rover!=null) {
						transform.RotateAround (transform.position, rover.transform.up, Time.deltaTime * rotateMultiplier);
				}
	}
}
