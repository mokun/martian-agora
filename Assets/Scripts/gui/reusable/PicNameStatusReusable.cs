using UnityEngine;
using System.Collections;

//classes that end in "Reusable" are generic GUI functions.
public class PicNameStatusReusable
{
    //this draws an icon with text next to it. optionally, a small status orb appears inside the thing icon.
    private Texture iconTexture;
    private string label;
    private static GUIStyle textStyle;
    

    public PicNameStatusReusable(string label, Texture iconTexture)
    {
        this.iconTexture = iconTexture;
        this.label = label;
        textStyle = GUIFunctions.GetStandardGUIStyle(10);
        textStyle.alignment = TextAnchor.MiddleLeft;
    }

    public void Draw(Rect drawRect)
    {
        Draw(drawRect, null);
    }

    public void Draw(Rect drawRect, Texture statusTexture)
    {
        Rect iconRect = new Rect(drawRect.x, drawRect.y, drawRect.height, drawRect.height);
        Rect textRect = new Rect(drawRect.x + drawRect.height * 1.2f, drawRect.y, drawRect.width - drawRect.height, drawRect.height);

        GUI.DrawTexture(iconRect, iconTexture);
        GUI.Label(textRect, label, textStyle);

        if (statusTexture != null)
        {
            float statusSize=iconRect.width/3;
            Rect statusRect = new Rect(iconRect.x + iconRect.width - statusSize, iconRect.y,
                statusSize, statusSize);
            GUI.DrawTexture(statusRect, statusTexture);
        }
    }
}