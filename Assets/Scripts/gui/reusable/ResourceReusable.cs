using UnityEngine;
using System.Collections;

//classes that end in "Reusable" are generic GUI functions.
public class ResourceReusable
{
    //this is the standard way to draw a resource icon with a number next to it.
    public string tooltipText;
    public ResourceTypes resourceType;
    private Texture iconTexture;

    private static Texture[] iconTextures;
    private static GUIStyle valueStyle;

    public ResourceReusable(ResourceTypes resourceType, string tooltipText)
    {
        
        this.resourceType = resourceType;
        this.tooltipText = tooltipText;
        valueStyle = GUIFunctions.GetStandardGUIStyle(16);
        valueStyle.alignment = TextAnchor.MiddleLeft;

        iconTexture = GetResourceTextureFromResourceType(resourceType);
    }

    public static Texture GetResourceTextureFromResourceType(ResourceTypes resourceType)
    {
        string iconName = GetIconNameFromResourceType(resourceType);
        string path = "gui/icons/" + iconName;
        Texture iconTexture = Resources.Load(path) as Texture;
        if (iconTexture == null)
        {
            iconTexture = GUIFunctions.Get1x1Texture(Color.black);
            Debug.LogError("ResourceReusable failed to load texture: " + path);
        }
        return iconTexture;
    }

    public void Draw(Rect drawRect, float resourceQuantity)
    {
        float iconSize = drawRect.height * 2 / 3;
        Rect iconRect = new Rect(drawRect.x, drawRect.y + drawRect.height / 2 - iconSize / 2, iconSize, iconSize);
        Rect textRect = new Rect(drawRect.x + iconSize, drawRect.y, drawRect.width - iconSize, drawRect.height);
        GUI.DrawTexture(iconRect, iconTexture);
        GUI.Label(textRect, string.Format("{0:0.0}", resourceQuantity), valueStyle);
    }

    private static string GetIconNameFromResourceType(ResourceTypes resourceType)
    {
        if (resourceType == ResourceTypes.water)
            return "water";
        if (resourceType == ResourceTypes.oxygen)
            return "oxygen";
        if (resourceType == ResourceTypes.food)
            return "food";
        if (resourceType == ResourceTypes.electricity)
            return "electricity";
        if (resourceType == ResourceTypes.inspiration)
            return "inspiration";

        Debug.LogError("GetIconNameFromResourceType found no iconName");
        return "";
    }
}