using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
		[SerializeField]
		private GameObject player;

		[SerializeField]
		private bool allowRestarts=true;

		public static float RecalculateWaitSeconds = 1;

		//real world seconds = game hours * timeFactor
		public const float timeFactor = 1;
		private bool isSetup = false;
		private Crew crew;

		void Start ()
		{
				Setup ();
		}

		void Update(){
				if (allowRestarts && Input.GetKeyDown (KeyCode.R))
						Application.LoadLevel (0);
		}

		private void Setup(){
				if (isSetup)
						return;

				gameObject.AddComponent<GUIManager> ();
				gameObject.AddComponent<ClickController> ();

				crew = player.GetComponent<Crew> ();

				ThingFactory.Setup ();
				GUIFunctions.Setup ();
				BlueprintDesignManager.Setup ();

				//setup structures that are already in scene
				foreach (StructureController sc in GameObject.FindObjectsOfType<StructureController>()) {
						sc.SetupRealStructure ();
						sc.SetAltitudeToMatchTerrain ();
				}

				isSetup = true;
		}

		public Crew GetCrew(){
				return crew;
		}

		public GameObject GetPlayer ()
		{
				return player;
		}

		public Camera GetPlayerCamera(){
				foreach (Camera c in player.GetComponentsInChildren<Camera>(true))
						return c;
				Debug.LogError (this + " failed to find camera.");
				isSetup = false;
				return null;
		}
}
