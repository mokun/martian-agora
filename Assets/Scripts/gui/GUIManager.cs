using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour
{
		//this class manages user input and controls when to switch between the major gui states.
		private enum GUIStates
		{
				pauseMenu,
				walking,
				colony,
				vehicle,
				personal
		}

		private GUIStates guiState;

		private GameplayGUI gameplayGUI;
		private PersonalToolbar personalToolbar;
		private ColonyToolbar colonyToolbar;

		private GUIStates previousState;
		private static GUIManager guiManager;
		private GameManager gameManager;

		void Start ()
		{
				gameManager = FindObjectOfType<GameManager> ();
				gameplayGUI = gameObject.AddComponent<GameplayGUI> ();
				personalToolbar = gameObject.AddComponent<PersonalToolbar> ();
				colonyToolbar = gameObject.AddComponent<ColonyToolbar> ();

				gameObject.AddComponent<FramesPerSecond> ();

				SetGuiState (GUIStates.walking);
		}

		private static GUIManager GetGuiManager ()
		{
				if (guiManager == null)
						guiManager = FindObjectOfType<GUIManager> ();
				return guiManager;
		}


		public static void SetDrivingStatus (bool isDriving)
		{
				GetGuiManager ().SetGuiState (isDriving ? GUIStates.vehicle : GUIStates.walking);
		}

		private void DisableGUIs ()
		{
				gameplayGUI.enabled = false;
				personalToolbar.enabled = false;
				colonyToolbar.enabled = false;
		}

		private void SetGuiState (GUIStates newGuiState)
		{
				//Debug.Log("newGuiState= " + newGuiState);
				guiState = newGuiState;
				DisableGUIs ();
				TooltipManager.ClearAllTooltips ();

				if (newGuiState == GUIStates.walking) {
						SetCursorLock (true);
						gameplayGUI.enabled = true;
						gameplayGUI.SelectSlotIndex (gameplayGUI.selectedSlotIndex);
				} else if (newGuiState == GUIStates.personal) {
						SetCursorLock (false);
						personalToolbar.enabled = true;
				} else if (newGuiState == GUIStates.colony) {
						SetCursorLock (false);
						colonyToolbar.enabled = true;
				}
		}

		private void SetCursorLock (bool isLocked)
		{
				Screen.lockCursor = isLocked;
				Cursor.visible = !isLocked;
				MouseLook mouselook = gameManager.GetPlayer ().GetComponent<MouseLook> ();
				mouselook.enabled = isLocked;
		}

		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.Escape)) {
						if (guiState != GUIStates.pauseMenu) {
								previousState = guiState;
								SetGuiState (GUIStates.pauseMenu);
						} else {
								SetGuiState (previousState);
						}
				}

				if (Input.GetKeyDown (KeyCode.I)) {
						if (guiState == GUIStates.personal) {
								SetGuiState (GUIStates.walking);
						} else {
								SetGuiState (GUIStates.personal);
						}
				}

				if (Input.GetKeyDown (KeyCode.C)) {
						if (guiState == GUIStates.colony) {
								SetGuiState (GUIStates.walking);
						} else {
								SetGuiState (GUIStates.colony);
						}
				}

		}
}
