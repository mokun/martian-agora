using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {
		private GameObject player;
		void Start(){
				GameManager gm = FindObjectOfType<GameManager> ();
				player = gm.GetPlayer ();
		}
		void Update () {
				transform.LookAt(player.transform);
		}
}