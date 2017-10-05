﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(ClickController))]
[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(FPSInputController))]
[RequireComponent (typeof(CharacterMotor))]
public class Crew : MonoBehaviour
{
		public enum Modes
		{
				walking,
				driving
		}

		private Modes mode;

		public static int inventorySize = 64;

		[SerializeField]
		public float reachRange = 5;

		[SerializeField]
		private float batteryEnergy = 100;

		private Camera crewCamera;
		private Rover rover;
		private ClickController clickController;

		public Thing[] inventory;

		void Start ()
		{
				clickController = FindObjectOfType<ClickController> ();
				SetupDebugInventory ();
		}

		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.E)) {
						MouseHoverInfo mhi = clickController.GetMouseHoverInfo (reachRange);
						if (mhi.IsHit && mhi.hoverObject.GetComponent<Rover> () != null) {
								Debug.Log (this + " detected 'e' press on Rover.");
								bool isDriving = mode == Modes.walking;
								SetDrivingMode (isDriving);
								Rover rover = mhi.hoverObject.GetComponent<Rover> ();
								rover.SetMode (RoverModes.driving);
						}
				}
		}

		public Modes GetMode ()
		{
				return mode;
		}

		private Thing[] GetInventory ()
		{
				if (inventory == null)
						inventory = new Thing[inventorySize];
				return inventory;
		}

		public Camera GetCrewCamera ()
		{
				if (crewCamera == null) {
						foreach (GameObject child in ParentChildFunctions.GetAllChildren(gameObject, true)) {
								Camera c = child.GetComponent<Camera> ();
								if (c != null) {
										crewCamera = c;
										break;
								}
						}
				}
				return crewCamera;
		}

		public void SetDrivingMode (bool isDriving)
		{
				SetDrivingMode (isDriving ? Modes.driving : Modes.walking);
		}

		public void SetDrivingMode (Modes newMode)
		{
				bool isDriving = newMode == Modes.driving;
				GetComponent<CharacterController> ().enabled = !isDriving;
				GetComponent<CharacterMotor> ().enabled = !isDriving;
				GetComponent<FPSInputController> ().enabled = !isDriving;
		}

		public bool IsSlotIndexValid (int index)
		{
				return index > 0 && index <= InventoryWindow.thingsPerRow;
		}

		public float GetBatteryEnergy ()
		{
				return batteryEnergy;
		}

		public float GetLadderClimbSpeed ()
		{
				return 3;
		}

		public Rover GetVehicleController ()
		{
				return Rover.GetVehicleControllerFromChild (gameObject);
		}

		public int GetInventoryIndexFromSlotIndex (int slotIndex)
		{
				if (!IsSlotIndexValid (slotIndex)) {
						Debug.Log ("Warning: GetInventoryIndexFromSlotIndex got bad index. index=" + slotIndex);
						return 0;
				}
				int minimum = inventorySize - InventoryWindow.thingsPerRow;
				return minimum + slotIndex - 1;
		}

		public Thing FindThingFromInventory (ThingTypes thingType)
		{
				foreach (Thing thing in inventory) {
						if (thing != null && thing.thingType == thingType)
								return thing;
				}
				return null;
		}

		public Thing GetThingFromSlotIndex (int slotIndex)
		{
				if (!IsSlotIndexValid (slotIndex)) {
						Debug.Log ("Warning: GetThingFromSlotIndex got bad index. index=" + slotIndex);
						return null;
				}
				return GetInventory () [GetInventoryIndexFromSlotIndex (slotIndex)];
		}


		public void UseThingInSlot (int slotIndex, int quantityUsed)
		{
				Thing usedThing = GetThingFromSlotIndex (slotIndex);
				UseThing (usedThing, quantityUsed);
		}

		public void UseThing (Thing usedThing, int quantityUsed)
		{
				int inventoryIndex = FindThingIndex (usedThing);
				if (inventoryIndex == -1) {
						Debug.LogError ("UseThing didn't find usedThing in inventory. usedThing=" + usedThing);
						return;
				}

				usedThing.quantity -= quantityUsed;
				if (usedThing.quantity <= 0)
						inventory [inventoryIndex] = null;
		}

		public void UseEnergyThing (Thing usedThing, float energyUsed)
		{
				int inventoryIndex = FindThingIndex (usedThing);
				if (inventoryIndex == -1) {
						Debug.LogError ("UseEnergyThing didn't find usedThing in inventory. usedThing=" + usedThing);
						return;
				}

				usedThing.durability -= energyUsed;
				UseEnergy (energyUsed);
		}

		private void UseEnergy (float energyUsed)
		{
				//this function is called when something consumes power from the crew member's suit/inventory


				//decrease batteryEnergy
				return;
		}

		private int FindThingIndex (Thing thing)
		{
				//finds the index of that thing in the inventory array.
				int inventoryIndex = -1;
				for (int i = 0; i < inventorySize; i++) {
						if (thing == inventory [i])
								inventoryIndex = i;
				}
				return inventoryIndex;
		}

		void SetupDebugInventory ()
		{
				int i = 0;
				foreach (ThingTypes thingType in System.Enum.GetValues(typeof(ThingTypes))) {
						if (!ThingFactory.IsBlueprint (thingType)) {
								GetInventory () [i] = ThingFactory.MakeThing (thingType);
								if (!ThingFactory.IsTool (thingType))
										inventory [i].quantity = 1000;
								i++;
						}
				}

				inventory [58] = ThingFactory.MakeThing (ThingTypes.vrvisor);
				inventory [59] = ThingFactory.MakeThing (ThingTypes.greasegun);
				inventory [60] = ThingFactory.MakeThing (ThingTypes.shovel);
				inventory [61] = ThingFactory.MakeThing (ThingTypes.wirecutters);
				inventory [62] = ThingFactory.MakeThing (ThingTypes.wrench);
				inventory [63] = ThingFactory.MakeThing (ThingTypes.solderinggun);

		}
}
