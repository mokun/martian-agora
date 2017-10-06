using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RoverModes
{
		parked,
		driving,
		braking
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Rover : MonoBehaviour
{
		public bool showGizmos = false;
		public RoverModes roverMode;
		public List<Camera> cameras;
		private Rigidbody rb;
		private int cameraIndex;

		private float deadZone;
		private float maxSuspensionForce, forwardAcceleration, backwardAcceleration, sidewaysMultiplier;
		private float currentThrust, currentTurn, turnStrength;
		//if the car is going slower than this and is breaking, it will immediately stop.
		private float minimumBreakSpeed;
		private float minimumBreakSpeedSquared, wheelBreakForce;

		private float wheelRotateMinSpeedSquared = 0.001f;

		[SerializeField]
		private GameObject[] wheelGameObjects;

		[SerializeField]
		private GameObject[] rotatingWheelGameObjects;

		[SerializeField]
		private GameObject driverPosition;

		private RoverWheel[] wheels;
		private GameManager gameManager;
		private float wheelCircumference;

		private bool isSetup=false;
		private bool hasBeenDriven=false;

		//optimization: test_rover eats frames, up until it is moved forward 1cm..?

		void Start ()
		{
				Setup ();

		}

		public GameObject GetDriverPosition(){
				return driverPosition;
		}

		private void Setup(){
				if (isSetup)
						return;

				gameManager = FindObjectOfType<GameManager> ();

				if (driverPosition == null) {
						Debug.LogError (this + " not set up. Null values.");
						return;
				}

				if (FindObjectOfType<GreebleCameraRotate> () == null) {
						Debug.LogWarning (this + " does not have GreebleCameraRotate.");
				}

				//find and set wheels
				if (wheelGameObjects.Length == 0) {
						Debug.LogError (this + " failed. No wheels.");
						return;
				}

				//estimate the diameter of the wheel based on the mesh, for wheel spin animation.
				MeshRenderer mr = wheelGameObjects [0].GetComponent<MeshRenderer> ();
				float wheelDiameter = Mathf.Min (mr.bounds.size.x, mr.bounds.size.y, mr.bounds.size.z);
				Debug.Log (this + " estimated wheel diameter to be " + wheelDiameter);
				wheelCircumference = wheelDiameter * Mathf.PI;

				wheels=new RoverWheel[wheelGameObjects.Length];
				for(int i =0;i<wheelGameObjects.Length;i++){						
						RoverWheel wheel = wheelGameObjects[i].AddComponent<RoverWheel> ();
						wheel.showGizmos = showGizmos;
						wheel.Initialize (transform);
						wheels[i]=wheel;
				}

				//set constants, set here so they aren't cached/serialized, easier for debugging
				maxSuspensionForce = 7000;
				forwardAcceleration = 500f;
				backwardAcceleration = 200f;
				turnStrength = 100f;
				sidewaysMultiplier = 100f;
				minimumBreakSpeed = 0.5f;
				wheelBreakForce = 2000f;

				rb = GetComponent<Rigidbody> ();
				rb.mass = 1200;
				rb.drag = 0;
				rb.angularDrag = 0.05f;
				rb.centerOfMass -= new Vector3 (0, 2, 0);

				minimumBreakSpeedSquared = Mathf.Pow (minimumBreakSpeed, 2);


				gameObject.layer = LayerMask.NameToLayer ("VehicleBox");

				SetMode (roverMode);

				isSetup = true;
		}

		public void SetMode (RoverModes newVehicleMode)
		{
				roverMode = newVehicleMode;
				if (roverMode == RoverModes.driving) {
						hasBeenDriven = true;
						GetComponent<Rigidbody> ().isKinematic = false;
				}
				DisableCameras ();
		}

		void OnDrawGizmos ()
		{
				if (wheels == null || !showGizmos)
						return;

				foreach (RoverWheel wheel in wheels) {
						RoverWheel.Modes wheelMode = wheel.GetMode ();

						if (wheelMode == RoverWheel.Modes.below_terrain)
								Gizmos.color = Color.blue;
						else if (wheelMode == RoverWheel.Modes.contact)
								Gizmos.color = Color.green;
						else if (wheelMode == RoverWheel.Modes.no_contact)
								Gizmos.color = Color.red;
						else
								Gizmos.color = Color.white;
						Gizmos.DrawSphere (wheel.transform.position, 0.5f);
				}
		}

		private void NextCamera ()
		{
				//cameraIndex=cameras.Count when we should use the crewCamera.
				if (cameras == null)
						return;

				for (int i = 0; i < cameras.Count; i++)
						cameras [i].enabled = cameraIndex == i;

				gameManager.GetPlayerCamera().enabled = cameraIndex == cameras.Count;

				cameraIndex++;
				if (cameraIndex > cameras.Count)
						cameraIndex = 0;
		}

		private void DisableCameras ()
		{
				for (int i = 0; i < cameras.Count; i++)
						cameras [i].enabled = false;
		}

		private void OccupiedUpdate ()
		{
				//forward and reverse
				currentThrust = 0.0f;
				float accelerationAxis = Input.GetAxis ("Vertical");
				if (accelerationAxis > deadZone)
						currentThrust = accelerationAxis * forwardAcceleration;
				else if (accelerationAxis < -deadZone)
						currentThrust = accelerationAxis * backwardAcceleration;

				//turning
				currentTurn = 0.0f;
				float turnAxis = Input.GetAxis ("Horizontal");
				if (Mathf.Abs (turnAxis) > deadZone)
						currentTurn = turnAxis * turnStrength;

				//other
				if (Input.GetKeyDown (KeyCode.M))
						NextCamera ();

				if (Input.GetKey (KeyCode.Space))
						roverMode = RoverModes.braking;
				else
						roverMode = RoverModes.driving;
		}

		public static Rover GetVehicleControllerFromChild (GameObject child)
		{
				foreach (GameObject parent in ParentChildFunctions.GetAllParents(child)) {
						Rover vc = parent.GetComponent<Rover> ();
						if (vc != null)
								return vc;
				}
				return null;
		}

		void Update ()
		{
				if (roverMode != RoverModes.parked)
						OccupiedUpdate ();
		}

		private void DampenMovementWithSuspension (int wheelTouchCount)
		{
				const float squareMagnitudeDeadzone = 0.0001f;

				//dampen vertical bobbing
				float dampCoefficient = 0.002f * wheelTouchCount;
				Vector3 verticalVelocity = Vector3.Project (rb.velocity, transform.up);
				Vector3 dampenVelocity = verticalVelocity * dampCoefficient;
				//Debug.Log("rigidbody.angularVelocity=" + rigidbody.angularVelocity + " verticalVelocity=" + verticalVelocity + " dampenVelocity=" + dampenVelocity);
				Vector3 newVelocity = rb.velocity - dampenVelocity;
				if (newVelocity.sqrMagnitude > squareMagnitudeDeadzone)
						rb.velocity = newVelocity;
				else
						rb.velocity = Vector3.zero;

				//dampen rotational bobbing
				Vector3 newAngularVelocity = rb.angularVelocity * (1 - dampCoefficient);

				if (newAngularVelocity.sqrMagnitude > squareMagnitudeDeadzone)
						rb.angularVelocity = newAngularVelocity;
				else
						rb.angularVelocity = Vector3.zero;

		}

		private void ApplyBrakes (int wheelTouchCount)
		{
				if (wheelTouchCount == 0)
						return;

				if (rb.velocity.sqrMagnitude < minimumBreakSpeedSquared) {
						rb.velocity = Vector3.zero;
				} else {
						rb.AddForce (rb.velocity.normalized * -1 * wheelTouchCount * wheelBreakForce);
				}
		}

		void FixedUpdate ()
		{
				int wheelTouchCounter = 0;
				foreach (RoverWheel wheel in wheels) {
						RoverWheel.Modes wheelMode = wheel.GetMode ();

						if (wheelMode == RoverWheel.Modes.contact || wheelMode == RoverWheel.Modes.below_terrain) {
								wheelTouchCounter++;

								//apply upward force to vehicle from wheel
								rb.AddForceAtPosition (transform.up * maxSuspensionForce * wheel.GetForceRatio (),
										wheel.transform.position);

								//apply force for sideways skidding
								Vector3 sidewaysProjection = Vector3.Project (rb.velocity, transform.right);
								rb.AddForceAtPosition (sidewaysProjection * sidewaysMultiplier * -1,
										wheel.transform.position);

								//apply driving forward force
								if (roverMode == RoverModes.driving)
										rb.AddForceAtPosition (transform.forward * currentThrust, wheel.gameObject.transform.position);

						}
				}
				if (!hasBeenDriven && wheelTouchCounter == wheelGameObjects.Length)
						rb.isKinematic = true;

				DampenMovementWithSuspension (wheelTouchCounter);

				//apply turning
				if (roverMode == RoverModes.driving)
						rb.AddRelativeTorque (transform.up * currentTurn * turnStrength);

				if (roverMode == RoverModes.braking || roverMode == RoverModes.parked) {
						ApplyBrakes (wheelTouchCounter);
				}

				//turn wheels
				if (roverMode == RoverModes.driving) {
						float metersThisFrame = rb.velocity.magnitude * Time.fixedDeltaTime;
						float angle = (metersThisFrame / wheelCircumference)*360;
						foreach (GameObject wheelGO in rotatingWheelGameObjects) {
								wheelGO.transform.RotateAround (wheelGO.transform.position, wheelGO.transform.right, angle);
						}
				}
		}
}
