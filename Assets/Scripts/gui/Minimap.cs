using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(TerrainManager))]
public class Minimap : MonoBehaviour
{
		[SerializeField]
		private Texture2D playerIcon;

		private TerrainManager terrainManager;
		private Rect minimapRect;
		private float widthScreenRatio = 0.1f;
		private float playerIconWidthRatio=0.01f;
		private float playerIconWidth;
		private float minimapMargin;
		private float minimapWidth;
		private bool isSetup = false;

		// Use this for initialization
		void Start ()
		{
				Setup ();
		}

		private void Setup ()
		{
				if (isSetup)
						return;

				if (playerIcon == null) {
						Debug.LogError (this + " not set up. Null values in inspector.");
						return;
				}						

				terrainManager = GetComponent<TerrainManager> ();
				minimapMargin = Screen.width * 0.02f;
				minimapWidth = Screen.width * widthScreenRatio;
				playerIconWidth = Screen.width * playerIconWidthRatio;
				minimapRect = new Rect (minimapMargin, Screen.height- minimapMargin-minimapWidth, minimapWidth, minimapWidth);

				isSetup = true;
		}

		void OnGUI ()
		{
				if (!isSetup)
						return;
				
				GUI.DrawTexture (minimapRect, terrainManager.GetMinimapTexture ());

				Vector2 playerPosition = terrainManager.GetPlayerMinimapPosition ();
				Rect iconRect = new Rect (minimapMargin+playerPosition.x *minimapWidth-playerIconWidth/2,
						Screen.height-minimapMargin-playerPosition.y * minimapWidth-playerIconWidth/2,
						playerIconWidth,
						playerIconWidth);
				GUI.DrawTexture (iconRect, playerIcon);
		}
}
