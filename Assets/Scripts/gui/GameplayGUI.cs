using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GameManager))]
public class GameplayGUI : MonoBehaviour
{
		//gui stuff during gameplay. this class also manages input that will change the gameplay gui like item selection.
		public int selectedSlotIndex = 1;
		public PersonalToolbar personalToolbar;

		private Thing selectedThing;
		private Rect selectedThingRect, selectedLabelRect, selectedNameRect;

		private float iconSize;

		private Blueprint cursorBlueprint;
		private BlueprintNode lastClickedBlueprintNode;
		private Thing selectedBlueprintThing;
		private ToolGUI toolGUI;
		private GameObject lastHoverObject, activeHoverObject;
		private GameManager gameManager;

		private GUIStyle textSplashStyle;

		private bool isSetup = false;

		void Start ()
		{
				Setup ();
		}

		private void Setup ()
		{
				if (isSetup)
						return;

				gameManager = FindObjectOfType<GameManager> ();
				selectedSlotIndex = 1;
				iconSize = Screen.height * 0.1f;
				selectedThingRect = new Rect (Screen.width / 2 - iconSize / 2,
						Screen.height - iconSize * 2, iconSize, iconSize);
				selectedLabelRect = new Rect (selectedThingRect.x + iconSize, selectedThingRect.y,
						selectedThingRect.width, selectedThingRect.height);
				selectedNameRect = new Rect (0,
						Screen.height - iconSize, Screen.width, iconSize);

				GetToolGUI ();

				isSetup = true;
		}

		private Thing GetSelectedBlueprintThing ()
		{
				BlueprintDesign bd = Toolbar.GetBlueprintWindow ().GetSelectedBlueprintDesign ();
				return ThingFactory.MakeThing (bd.thingType);
		}

		private ToolGUI GetToolGUI ()
		{
				if (toolGUI == null)
						toolGUI = gameObject.AddComponent<ToolGUI> ();
				return toolGUI;
		}

		void Update ()
		{
				if (Input.anyKeyDown) {
						int alphaKey = GetAlphaKey (1, 8);
						if (alphaKey != -1)
								SelectSlotIndex (alphaKey);
				}

				if (Input.GetMouseButtonDown (0) && cursorBlueprint != null &&
				    cursorBlueprint.CanBePlacedHere ()) {
						cursorBlueprint.DeployHere ();
						Debug.Log ("123");
						cursorBlueprint = BlueprintFactory.MakeNewCursorBlueprint ();
						SelectSlotIndex (selectedSlotIndex);
				}

				if (Input.GetMouseButtonDown (1))
						TryClickBlueprintNodes ();

				if (Input.GetMouseButton (0))
						TryHoldClickBlueprintNodes ();

				if (Input.GetMouseButtonUp (0) && lastClickedBlueprintNode != null)
						lastClickedBlueprintNode.UnholdClick ();
		}

		private Blueprint GetCursorBlueprint ()
		{
				if (cursorBlueprint == null)
						cursorBlueprint = BlueprintFactory.MakeNewCursorBlueprint ();
				return cursorBlueprint;
		}

		private BlueprintNode GetClickedBlueprintNode ()
		{
				Vector3 pos = gameManager.GetPlayer ().transform.position;
				RaycastHit rayCastHit = new RaycastHit ();
				if (Physics.Linecast (pos, pos + gameManager.GetPlayerCamera ().transform.forward * gameManager.GetCrew ().reachRange, out rayCastHit, 1)) {
						GameObject hitObject = rayCastHit.transform.gameObject;

						if (hitObject.transform.parent != null)
								return hitObject.transform.parent.GetComponent<BlueprintNode> ();
				}
				return null;
		}

		private void TryHoldClickBlueprintNodes ()
		{
				BlueprintNode blueprintNode = GetClickedBlueprintNode ();
				if (blueprintNode == null) {
						if (lastClickedBlueprintNode != null)
								lastClickedBlueprintNode.UnholdClick ();
				} else {
						lastClickedBlueprintNode = blueprintNode;
						blueprintNode.HoldClickedByPlayer (selectedThing, gameManager.GetCrew ());
				}

				SelectSlotIndex (selectedSlotIndex);
		}

		private void TryClickBlueprintNodes ()
		{
				BlueprintNode blueprintNode = GetClickedBlueprintNode ();
				if (blueprintNode != null)
						blueprintNode.ClickedByPlayer (gameManager.GetCrew ());

				SelectSlotIndex (selectedSlotIndex);
		}

		public void SelectSlotIndex (int slotIndex)
		{
				//Debug.Log ("SelectSlotIndex " + slotIndex);
				if (!isSetup)
						Setup ();
				if (!gameManager.GetCrew ().IsSlotIndexValid (slotIndex))
						Debug.LogError ("SelectSlotIndex given bad slotIndex. slotIndex=" + slotIndex);
				selectedSlotIndex = slotIndex;
				selectedThing = gameManager.GetCrew ().GetThingFromSlotIndex (slotIndex);
				if (selectedThing != null) {
						if (ThingFactory.IsBlueprintVisor (selectedThing.thingType)) {
								Debug.Log ("ThingFactory.IsBlueprintVisor(selectedThing.thingType)");
								GetCursorBlueprint ().StartBlueprint (GetSelectedBlueprintThing ());
						} else
								GetCursorBlueprint ().StopBlueprint ();

						if (selectedThing.IsTool ())
								GetToolGUI ().SelectTool (selectedThing.thingType, false);
						else
								GetToolGUI ().UnselectTool ();
				} else {
						SelectNull ();
				}
		}

		private void SelectNull ()
		{
				GetCursorBlueprint ().StopBlueprint ();
				GetToolGUI ().UnselectTool ();
		}

		private int GetAlphaKey (int minimum, int maximum)
		{
				//returns the int matching they key that was pressed. No key pressed returns -1.
				int magicAsciiNumber = 48;

				for (int i = magicAsciiNumber + minimum; i <= magicAsciiNumber + maximum; i++)
						if (Input.GetKeyDown ((KeyCode)i))
								return i - magicAsciiNumber;
				return -1;
		}

		void OnGUI ()
		{
				//draw reticle
				Vector2 size = GUI.skin.label.CalcSize (new GUIContent ("+"));
				Rect reticleRect = new Rect (0, 0, size.x, size.y);
				reticleRect.center = new Vector2 (Screen.width, Screen.height) / 2f;
				GUI.Label (reticleRect, "+");

				if (selectedThing != null) {
						GUIFunctions.DrawThing (selectedThingRect, selectedThing, true);
						GUI.Label (selectedLabelRect, selectedSlotIndex.ToString (), GUIFunctions.hotkeyStyle);
						GUI.Label (selectedNameRect, selectedThing.longName, GUIFunctions.hotkeyStyle);
				}
		}
}
