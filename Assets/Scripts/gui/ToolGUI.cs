using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolGUI : MonoBehaviour
{
		//ToolGUI controls models that appear in front of the camera as though you are holding them
		private bool isMoving;
		private float moveTimer;
		private float moveDuration;
		private GameObject tool, player;
		private ThingTypes selectedThingType;

		private Transform toolUpTransform, toolDownTransform;

		private static Dictionary<ThingTypes, GameObject> toolGUIPrefabs;

		void Start ()
		{
				moveDuration = 0.5f;

				GameManager gameManager = FindObjectOfType<GameManager> ();
				player = gameManager.GetPlayer ();
				foreach (GameObject child in ParentChildFunctions.GetAllChildren(player)) {
						if (child.name.Equals ("tool up"))
								toolUpTransform = child.transform;
						if (child.name.Equals ("tool down"))
								toolDownTransform = child.transform;
				}
				if (toolUpTransform == null || toolDownTransform == null)
						Debug.LogError ("ToolGUI failed to find 'tool down' or 'tool up'. up=" + toolUpTransform + " down=" + toolDownTransform);

				toolGUIPrefabs = new Dictionary<ThingTypes, GameObject> ();
		}

		void Update ()
		{
				if (isMoving && tool != null) {
						moveTimer += Time.deltaTime;
						if (moveTimer < moveDuration) {
								float ratio = moveTimer / moveDuration;
								tool.transform.position = Vector3.Lerp (toolDownTransform.position, toolUpTransform.position, ratio);
								tool.transform.rotation = Quaternion.Slerp (toolDownTransform.rotation, toolUpTransform.rotation, ratio);
						} else {
								tool.transform.position = toolUpTransform.position;
								tool.transform.rotation = toolUpTransform.rotation;
								isMoving = false;
						}
				}
		}

		private static GameObject GetToolGUIPrefab (ThingTypes thingType)
		{
				if (!toolGUIPrefabs.ContainsKey (thingType)) {
						string path = "gui/" + ThingFactory.GetKeyFromThingType (thingType);
						GameObject toolGUIPrefab = Resources.Load (path) as GameObject;

						if (toolGUIPrefab == null)
								Debug.LogError ("GetToolGUIPrefab failed to load prefab: " + path);
						toolGUIPrefabs.Add (thingType, toolGUIPrefab);
				}
				return toolGUIPrefabs [thingType];
		}

		public void UnselectTool ()
		{
				Destroy (tool);
		}

		public void SelectTool (ThingTypes thingType, bool animateIfIdenticalThingType)
		{
				if (selectedThingType == thingType && !animateIfIdenticalThingType)
						return;

				if (tool != null)
						Destroy (tool);
        
				if (!ThingFactory.IsTool (thingType)) {
						Debug.LogError ("ToolGUI.SelectTool got non-tool. " + thingType);
						return;
				}

				selectedThingType = thingType;
				isMoving = true;
				moveTimer = 0;
				tool = Instantiate (GetToolGUIPrefab (thingType)) as GameObject;
				tool.transform.parent = player.transform;

				tool.transform.position = toolDownTransform.position;
				tool.transform.rotation = toolDownTransform.rotation;
		}
}
