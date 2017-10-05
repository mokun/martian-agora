using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ThingFactory
{
		public static Dictionary<ThingTypes, Thing> thingTemplates;
		public static List<ThingTypes> blueprintThingTypes;
		private static List<ThingTypes> toolThingTypes;
		private static string[,] thingDataGrid;
		private static Dictionary<string, Texture> iconTextures;

		private static bool isSetup = false;

		public static Thing MakeThing (ThingTypes thingType)
		{
				//makes a deep copy off the correct thingTemplate. from python import deepcopy plz
				Thing template = thingTemplates [thingType];

				Thing newThing = new Thing ();
				newThing.iconTexture = template.iconTexture;

				newThing.thingType = template.thingType;
				newThing.key = template.key;
				newThing.name = template.name;
				newThing.longName = template.longName;
				newThing.isBlueprint = template.isBlueprint;
				newThing.isTool = template.isTool;
				newThing.workVerb = template.workVerb;
				newThing.maxDurability = template.maxDurability;

				//dependent data
				newThing.durability = template.durability;
				newThing.quantity = template.quantity;
				if (newThing.quantity == 0)
						newThing.quantity = 1;

				return newThing;
		}

		public static string GetKeyFromThingType (ThingTypes thingType)
		{
				if (!isSetup)
						Setup ();
				return thingTemplates [thingType].key;
		}

		public static bool IsBlueprintVisor (ThingTypes thingType)
		{
				//returns true if this thing is a VR visor that projects blueprints
				if (!isSetup)
						Setup ();
				return thingType == ThingTypes.vrvisor;
		}

		public static bool IsBlueprint (ThingTypes thingType)
		{
				//returns true if this thing can be laid down as a rotating blueprint.
				if (!isSetup)
						Setup ();
				return blueprintThingTypes.Contains (thingType);
		}

		public static bool IsTool (ThingTypes thingType)
		{
				//returns true if this thing is a handheld tool and shown in front of the camera when selected.
				if (!isSetup)
						Setup ();
				return toolThingTypes.Contains (thingType);
		}

		private static string[,] GetThingDataGrid ()
		{
				if (thingDataGrid == null)
						thingDataGrid = CSVReader.ReadCSV ("data/ThingData", false);
				return thingDataGrid;
		}

		private static int GetThingCountFromCSV ()
		{
				return GetThingDataGrid ().GetUpperBound (1);
		}

		private static string GetFromCSV (int x, int y)
		{
				GetThingDataGrid ();

				int maxX = thingDataGrid.GetUpperBound (0);
				int maxY = thingDataGrid.GetUpperBound (1);

				if (x < 0 || x > maxX || y < 0 || y > maxY) {
						Debug.LogError ("GetFromGrid got invalid x or y. (" + x + "," + y + ")");
						return "";
				}

				if (thingDataGrid [x, y] == null)
						return "";

				return thingDataGrid [x, y];
		}

		public static void Setup ()
		{
				if (isSetup)
						return;
				
				thingTemplates = new Dictionary<ThingTypes, Thing> ();
				blueprintThingTypes = new List<ThingTypes> ();
				toolThingTypes = new List<ThingTypes> ();
				iconTextures = new Dictionary<string, Texture> ();

				int thingCount = GetThingCountFromCSV ();

				for (int i = 1; i < thingCount; i++) {
						if (GetFromCSV (0, i).Equals (""))
								continue;
						int id = System.Int32.Parse (GetFromCSV (0, i));
						if (id == -1)
								continue;
            
						Thing newThing = new Thing ();

						//process each column
						newThing.thingType = (ThingTypes)id;
						newThing.key = GetFromCSV (1, i);
						newThing.name = GetStringFromCell (GetFromCSV (2, i), GetCapitalized (newThing.key));
						newThing.longName = GetStringFromCell (GetFromCSV (3, i), newThing.name);
						newThing.isBlueprint = !GetFromCSV (4, i).Equals ("");
						newThing.isTool = !GetFromCSV (5, i).Equals ("");
						newThing.workVerb = GetFromCSV (6, i);
						newThing.maxDurability = GetFloatFromCell (GetFromCSV (7, i));

						//use data to build data that depends on it
						if (newThing.isBlueprint)
								blueprintThingTypes.Add (newThing.thingType);
						if (newThing.isTool)
								toolThingTypes.Add (newThing.thingType);
						newThing.quantity = 1;
						newThing.durability = newThing.maxDurability;
						newThing.iconTexture = GetObjectIcon (newThing.key);
						thingTemplates [newThing.thingType] = newThing;
				}
		}

		private static Texture GetObjectIcon (string path)
		{
				if (!path.Contains ("gui/things/"))
						path = "gui/things/" + path;
				if (iconTextures.ContainsKey (path))
						return iconTextures [path];

				Texture texture = Resources.Load (path) as Texture;
				if (texture == null) {
						Debug.Log ("Warning: failed to load texture. path='" + path + "'");
						return iconTextures ["default"];
				}
				iconTextures [path] = texture;
				return texture;
		}

		private static string GetStringFromCell (string cell, string valueIfEmpty)
		{
				if (cell.Equals (""))
						return valueIfEmpty;
				return cell;
		}

		private static float GetFloatFromCell (string text)
		{
				if (text.Equals (""))
						return -1;
				return System.Int32.Parse (text);
		}

		private static string GetCapitalized (string text)
		{
				//text = "bob eats cake"
				//returns "Bob Eats Cake"
				char[] charArray = text.ToCharArray ();
				bool spaceFlag = true;
				for (int i = 0; i < text.Length; i++) {
						if (spaceFlag) {
								spaceFlag = false;
								charArray [i] = char.ToUpper (charArray [i]);
						}

						if (charArray [i].Equals (' '))
								spaceFlag = true;
				}

				return new string (charArray);
		}
}
