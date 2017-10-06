using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blueprint : MonoBehaviour
{
		//a blueprint is the blue rotating structure a player can place on the martian surface.

		//this class controls a blueprint for its whole life: moving/rotating from gameplay gui, blueprint deployment,
		//doing work on blueprint, and applying things to blueprint.

		//this is just like a structure gameobject except it is off, and blue.
		private GameObject blueprintStructure;
		private int blueprintNodeCount = 0;

		private float expandTimer = 0;
		private float blueprintRange;
		//when a blueprint appears, this is how long it takes to go full size.
		private const float expandDuration = 0.5f;
		private const float rotationSpeed = 25f;

		private static Material blueprintMaterial, fadedMaterial;
		private Thing thing;

		//keeps track of all the blueprint nodes that have been made so that CreateThingNode and CreateWorkNode use the same nodes.
		private Dictionary<Vector3, BlueprintNode> blueprintNodes;

		private static GameObject blueprintNodePrefab;
		private TerrainManager terrainManager;
		private ClickController clickController;

		public enum BlueprintModes
		{
				expanding,
				rotating,
				deployed,
				off
		}

		public BlueprintModes blueprintMode;

		void Start ()
		{
				blueprintRange = 100;
				terrainManager = FindObjectOfType<TerrainManager> ();
				clickController = FindObjectOfType<ClickController> ();
		}

		public static Material GetBlueprintMaterial ()
		{
				if (blueprintMaterial == null)
						blueprintMaterial = Resources.Load ("gui/blueprint") as Material;

				return blueprintMaterial;
		}

		public static Material GetFadedMaterial ()
		{
				if (fadedMaterial == null)
						fadedMaterial = Resources.Load ("gui/blueprint faded") as Material;
				return fadedMaterial;
		}

		public void StartBlueprint (Thing thing)
		{
				if (blueprintStructure != null)
						Destroy (blueprintStructure);
				blueprintMode = BlueprintModes.expanding;
				expandTimer = 0;
				this.thing = thing;
				blueprintStructure = StructureFactory.MakeStructure (thing.thingType, true);
				blueprintStructure.transform.parent = gameObject.transform;
		}

		public void StopBlueprint ()
		{
				blueprintMode = BlueprintModes.off;
				Destroy (blueprintStructure);
		}

		public static bool NameMatchesIgnoringNumber (string name1, string name2)
		{
				//used to match which gameobjects should be replaced by nodes.
				//example: returns true for "condenser###"  and "condenser###" where # is any digit, underscores, or nothing.
				if (name1.Length < name2.Length) {
						string temp = name1;
						name1 = name2;
						name2 = temp;
				}

				//short string not found in long string
				if (name1.IndexOf (name2) != 0)
						return false;

				//perfect match
				if (name1.Length == name2.Length)
						return true;

				//remove underscores
				string digits = name1.Substring (name2.Length);
				while (digits.Length > 0 && digits [0].Equals ('_'))
						digits = digits.Substring (1);

				//can digits be parsed as an int?
				int result;
				int.TryParse (digits, out result);
				return result != 0;
		}

		private void AdjustToMinimumTerrainHeight (GameObject nodeGameObject)
		{
				//this takes the coordinates of nodeGameObject and makes sure they are never below the terrain, plus half
				//the height of the nodeGameObject render.
				BlueprintNode bn = nodeGameObject.GetComponent<BlueprintNode> ();
				float terrainHeight = terrainManager.GetHeightAtPoint (nodeGameObject.transform.position);
				float minimumHeight = terrainHeight + bn.GetThingGameObject ().GetComponent<Renderer> ().bounds.size.y / 2;

				if (terrainHeight < minimumHeight)
						nodeGameObject.transform.position = new Vector3 (nodeGameObject.transform.position.x,
								minimumHeight, nodeGameObject.transform.position.z);

		}

		private static GameObject GetBlueprintNodePrefab ()
		{
				if (blueprintNodePrefab == null)
						blueprintNodePrefab = Resources.Load ("gui/blueprint node") as GameObject;
				return blueprintNodePrefab;
		}

		private BlueprintNode GetBlueprintNode (Vector3 positionOffset)
		{
				//creates a blueprint node for this position,
				//or returns the blueprint node that was already created for this position.
				//also ensures a minimum height according to terrain.
				if (!blueprintNodes.ContainsKey (positionOffset)) {
						blueprintNodeCount++;
						GameObject nodeGO = Instantiate (GetBlueprintNodePrefab ()) as GameObject;
						nodeGO.name = "blueprint node " + blueprintNodeCount;
						nodeGO.transform.parent = transform;
						nodeGO.transform.position = blueprintStructure.transform.position + positionOffset;
						AdjustToMinimumTerrainHeight (nodeGO);

						blueprintNodes [positionOffset] = nodeGO.GetComponent<BlueprintNode> ();
				}
				//Debug.Log("GetBlueprintNode returned: " + createdBlueprintNodes[candidate]);
				return blueprintNodes [positionOffset];
		}

		private void CreateThingNode (ThingTypes requiredThingType, int quantityPerNode, Vector3 position)
		{
				//creates a thing blueprint node at position, which is relative to gameobject
				GetBlueprintNode (position).AddRequiredThing (requiredThingType, quantityPerNode);
		}

		private void CreateThingNode (ThingTypes requiredThingType, int quantityPerNode, int replacedNodeCount, string nodeSubstring)
		{
				//creates an thing blueprint node for all children whose name contains substring
				int newNodeCount = 0;
				foreach (GameObject candidate in ParentChildFunctions.GetAllChildren(this.gameObject)) {
						if (NameMatchesIgnoringNumber (candidate.name, nodeSubstring)) {
								Vector3 relativePosition = candidate.transform.position - blueprintStructure.transform.position;
								GetBlueprintNode (relativePosition).AddRequiredThing (requiredThingType, quantityPerNode);
								newNodeCount++;
								if (newNodeCount >= replacedNodeCount)
										break;
						}
				}
				if (newNodeCount < replacedNodeCount)
						Debug.LogError ("CreateThingNode expected to make " + replacedNodeCount + " nodes but only made " + newNodeCount + ". thingType=" + requiredThingType + " substring=" + nodeSubstring + " thingtype=" + thing.thingType);
		}

		private void CreateToolNode (ThingTypes requiredThingType, float energyPerNode, int replacedNodeCount, string nodeSubstring)
		{
				//creates an thing blueprint node for all children whose name contains substring
				int newNodeCount = 0;
				foreach (GameObject candidate in ParentChildFunctions.GetAllChildren(this.gameObject)) {
						if (NameMatchesIgnoringNumber (candidate.name, nodeSubstring)) {
								Vector3 relativePosition = candidate.transform.position - blueprintStructure.transform.position;
								GetBlueprintNode (relativePosition).AddRequiredTool (requiredThingType, energyPerNode);
								newNodeCount++;
								if (newNodeCount >= replacedNodeCount)
										break;
						}
				}
				if (newNodeCount < replacedNodeCount)
						Debug.LogError ("CreateThingNode expected to make " + replacedNodeCount + " nodes but only made " + newNodeCount + ". thingType=" + requiredThingType + " substring=" + nodeSubstring);
		}

		private void CreateAllNodes ()
		{
				//creates all work nodes and thing nodes, depending on the thingType of this blueprint.
				if (blueprintNodes != null)
						Debug.LogError ("CreateAllNodes was run more than once. (it shouldn't be)");
				blueprintNodes = new Dictionary<Vector3, BlueprintNode> ();

				foreach (DesignRequirement dr in thing.GetBlueprintDesign().GetDesignRequirements()) {
						if (!dr.IsSet ()) {
								Debug.LogError ("CreateAllNodes got an unset DesignRequirement.");
								continue;
						}

						if (dr.isLocationSubstring) {
								if (dr.isThingRequirement)
										CreateThingNode (dr.requiredThingType, dr.quantityPerNode, dr.replacedNodeCount, dr.nodeSubstring);
								else
										CreateToolNode (dr.requiredThingType, dr.energyPerNode, dr.replacedNodeCount, dr.nodeSubstring);
						} else if (dr.isLocationVector) {
								if (dr.isThingRequirement)
										CreateThingNode (dr.requiredThingType, dr.quantityPerNode, dr.position);
								else
										Debug.LogError ("not implemented.");
						}
				}

				AdjustLocationOfNodes ();
		}

		private void AdjustLocationOfNodes ()
		{
				//sometimes nodes need to be lowered or drifted apart instead of being perfectly on top of the parts of the model
				if (thing.thingType == ThingTypes.windturbine)
						HeightMultiplyNodes (0.5f);
				if (thing.thingType == ThingTypes.solarpanel)
						DriftApartAllNodes (1.5f);
				if (thing.thingType == ThingTypes.chargingstation)
						DriftApartAllNodes (2);
		}

		private float GetLowestNodeHeight ()
		{
				float lowest = Mathf.Infinity;
				foreach (BlueprintNode bn in blueprintNodes.Values) {
						if (bn.gameObject.transform.position.y < lowest)
								lowest = bn.gameObject.transform.position.y;
				}
				if (lowest == Mathf.Infinity)
						Debug.LogError ("FindHeightOfLowestNode returned infinity!");
				return lowest;
		}

		private void HeightMultiplyNodes (float heightFraction)
		{
				//multiplies the height of all nodes by heightFraction so they can be reached from the ground.
				float lowestHeight = GetLowestNodeHeight ();
				foreach (BlueprintNode bn in blueprintNodes.Values) {
						Vector3 newPosition = bn.gameObject.transform.position;
						newPosition.y = (newPosition.y - lowestHeight) * heightFraction + lowestHeight;
						bn.gameObject.transform.position = newPosition;
				}
		}

		private void DriftApartAllNodes (float ratio)
		{
				//scales the positions of all nodes away from the center. for small structures with packed together nodes.
				Vector3 origin = blueprintStructure.transform.position;
				foreach (BlueprintNode bn in blueprintNodes.Values) {
						Vector3 newPosition = bn.gameObject.transform.position;
						newPosition = (newPosition - origin) * ratio + origin;
						bn.gameObject.transform.position = newPosition;
				}
		}

		private void SetFadedAppearance ()
		{
				foreach (GameObject child in ParentChildFunctions.GetAllChildren(this.gameObject)) {
						if (child.GetComponent<Renderer> () != null)
								child.GetComponent<Renderer> ().material = GetFadedMaterial ();
				}
		}

		public void MakeIfConstructed ()
		{
				//this turns the blueprint into a real structure. removes nodes, deletes itself, and activates structure.
				if (IsConstructed ()) {
						GameObject realStructure = StructureFactory.MakeStructure (thing.thingType, false);
						StructureController structureController = realStructure.GetComponent<StructureController> ();
						realStructure.transform.position = blueprintStructure.transform.position;
						realStructure.transform.rotation = blueprintStructure.transform.rotation;
						Destroy (gameObject);
				}
		}

		public bool IsConstructed ()
		{
				//checks if all BlueprintNodes are completed.
				foreach (GameObject child in ParentChildFunctions.GetAllChildren(gameObject)) {
						BlueprintNode node = child.GetComponent<BlueprintNode> ();
						if (node != null && !node.IsConstructed ())
								return false;
				}
				return true;
		}

		public void DeployHere ()
		{
				Debug.Log ("DeployHere");
				//the blueprint will stay at this position/rotation and create its BlueprintNodes.
				if (!CanBePlacedHere ())
						Debug.LogError ("DeployHere happened when not CanBePlacedHere.");

				blueprintMode = BlueprintModes.deployed;
				SetFadedAppearance ();
				CreateAllNodes ();
		}

		public bool CanBePlacedHere ()
		{
				return blueprintMode == BlueprintModes.rotating;
		}

		void Update ()
		{
				if (blueprintMode == BlueprintModes.rotating)
						blueprintStructure.transform.Rotate (new Vector3 (0, 1, 0), Time.deltaTime * rotationSpeed);

				if (blueprintMode == BlueprintModes.expanding) {
						expandTimer += Time.deltaTime;
						if (expandTimer > expandDuration) {
								blueprintMode = BlueprintModes.rotating;
						} else {
								float newScale = expandTimer / expandDuration;
								blueprintStructure.transform.localScale = new Vector3 (newScale, newScale, newScale);
						}
				}

				if (blueprintMode == BlueprintModes.rotating || blueprintMode == BlueprintModes.expanding) {
						MouseHoverInfo mhi = clickController.GetMouseHoverInfo (blueprintRange);

						if (mhi.IsHit && mhi.hoverObject.layer == LayerMask.NameToLayer ("Terrain")) {
								blueprintStructure.transform.position = mhi.point;
						}
				}
		}

}
