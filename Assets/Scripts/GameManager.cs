using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
		[SerializeField]
		private GameObject player;

		public static float RecalculateWaitSeconds = 1;

		//real world seconds = game hours * timeFactor
		public const float timeFactor = 1;
		private bool isSetup = false;
		private Crew crew;

		void Start ()
		{
				Setup ();
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
				foreach (StructureController sc in GameObject.FindObjectsOfType<StructureController>())
						sc.SetupRealStructure ();

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
				foreach (Camera c in gameObject.GetComponentsInChildren<Camera>(true))
						return c;
				Debug.LogError (this + " failed to find camera.");
				isSetup = false;
				return null;
		}
}
