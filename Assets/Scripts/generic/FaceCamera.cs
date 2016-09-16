using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {
	void Update () {
        transform.LookAt(CrewManager.GetActiveCrewCamera().transform);
	}
}
