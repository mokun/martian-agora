using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GUIFunctions
{
    //collection of generic functions that are useful to many gui classes.

    public static GUIStyle hotkeyStyle, quantityStyle, energyStyle;
    private static Font standardFont;
    public static float margin;

    //stolen from GUI.skin so they can be used outside of OnGUI. dirty hack, waiting on unity 4.6 gui system or gui revamp.
    public const float heightOfWindowBar = 18;
    public const float verticalBarWidth = 15;

    public static void Initialize()
    {
        standardFont = Resources.Load("fonts/nulshock bd") as Font;
        SetGUIStyles();
        margin = Screen.width / 100;
    }

    public static Vector2 GetGUIMousePositionFromInputMousePosition()
    {
        //Input.mousePosition origins are bottom left
        //gui coordinates are bottom left. This uses Input.mousePosition to return GUI like coordinates.   

        return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    }

    public static bool IsMouseOver(Rect rect)
    {
        return rect.Contains(Event.current.mousePosition);
    }

    public static Rect AddRectAndRectCoords(Rect rect, Rect rectCoords)
    {
        //used when the coorindates of "rect" are relative to coordinates of "rectCoords".
        //returns a Rect of same size as "rect", but whose coordinates have been added with rectCoords.
        return new Rect(rect.x + rectCoords.x, rect.y + rectCoords.y, rect.width, rect.height);
    }

    public static string MakeTextMultilined(string text, int maxCharsPerLine)
    {
        //given some text, this will insert newline characters within spaces so that a single line will not
        //be longer than maxCharsPerLine
        List<string> words = new List<string>();

        int lastSpaceIndex = -1;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                string word = text.Substring(lastSpaceIndex + 1, i - lastSpaceIndex - 1);
                words.Add(word);
                lastSpaceIndex = i;
            }
        }
        string lastWord = text.Substring(lastSpaceIndex + 1, text.Length - lastSpaceIndex - 1);
        words.Add(lastWord);


        string result = "";
        int currentCharCount = 0;
        foreach (string word in words)
        {
            if (currentCharCount + word.Length > maxCharsPerLine)
            {
                result += "\n";
                currentCharCount = 0;
            }
            currentCharCount += word.Length;
            result += " " + word;
        }
        return result.Substring(1, result.Length - 1);
    }

    public static GUIStyle GetStandardGUIStyle(int fontSize)
    {
        GUIStyle newStyle = new GUIStyle();
        newStyle.normal.textColor = Color.white;
        newStyle.alignment = TextAnchor.MiddleCenter;
        newStyle.fontSize = Screen.height * fontSize / 500;
        newStyle.font = standardFont;

        return newStyle;
    }

    public static Vector2 GetMousePositionFromInputMousePosition()
    {
        //Input.mousePosition origin is bottom left.
        //Event.current.mousePosition origin is top left.
        //this uses the non-gui function Input.mousePosition to return a gui-like coordinate.

        return new Vector2(Input.mousePosition.x, Mathf.Abs(Input.mousePosition.y - Screen.height));
    }

    private static void SetGUIStyles()
    {
        hotkeyStyle = GetStandardGUIStyle(30);
        hotkeyStyle.alignment = TextAnchor.MiddleCenter;

        quantityStyle = GetStandardGUIStyle(12);
        quantityStyle.fontStyle = FontStyle.Bold;
        quantityStyle.alignment = TextAnchor.UpperLeft;

        energyStyle = GetStandardGUIStyle(10);
        energyStyle.fontStyle = FontStyle.Bold;
        energyStyle.alignment = TextAnchor.UpperLeft;
    }

    public static Texture2D Get1x1Texture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();
        return texture;
    }

    public static void DrawThing(Rect rect, Thing thing, bool drawWithFrame)
    {
        if (thing == null)
        {
            Debug.LogError("DrawThing given null thing. " + thing);
        }
        else
        {
            GUI.DrawTexture(rect, thing.iconTexture);

            if (drawWithFrame)
                GUI.DrawTexture(rect, thing.GetIconFrameTexture());

            if (thing.quantity > 1)
            {
                GUI.Label(rect, thing.quantity.ToString(), quantityStyle);
            }
            else if (thing.durability > 0)
            {
                GUI.Label(rect, thing.durability.ToString("F1"), energyStyle);
            }
        }
    }
}
