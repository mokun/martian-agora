using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum IconBackgroundTypes
{
    circle,
    square,
    bar
}

//classes that end in "Reusable" are generic GUI functions.
public class IconGroupReusable
{
    //this draws any number of icons in a rect. Icons are optionally surrounded by squares or circles.
    //If there are too many for a pictograph, text numbers will appear.
    private int iconCount = 0;
    private int iconTypeCount = 0;
    private bool alwaysUseTextNumbers;

    private static GUIStyle textStyle,moreStyle;
    private static Dictionary<IconBackgroundTypes,Texture> iconBackgroundTextures;

    private const string iconFolder = "gui/icons/";

    public class IconGroup
    {
        public Texture iconTexture, backgroundTexture;
        public int count;
        public IconGroup(Texture iconTexture, Texture backgroundTexture, int count)
        {
            this.iconTexture = iconTexture;
            this.backgroundTexture = backgroundTexture;
            this.count = count;
        }
    }
    private List<IconGroup> iconGroups;

    public static void DrawLabelledIGRList(List<string> labels, List<IconGroupReusable> igrList, Rect drawRect, GUIStyle guiStyle)
    {
        //this draws a list of labelled IGRs. Each row is a label on the left and its igr on the right.
        float rowHeight = drawRect.height / igrList.Count;

        Rect labelRect = new Rect(drawRect.x+GUIFunctions.margin, drawRect.y, drawRect.width, rowHeight);
        Rect igrRect = new Rect(0, drawRect.y, drawRect.width / 2, rowHeight);
        float igrOffset=drawRect.x+GUIFunctions.margin*2;

        for (int i = 0; i < labels.Count; i++)
        {
            if (igrList[i].iconCount > 0)
            {
                float textWidth = guiStyle.CalcSize(new GUIContent(labels[i])).x;
                igrRect.x = igrOffset + textWidth;
                GUI.Label(labelRect, labels[i], guiStyle);
                igrList[i].Draw(igrRect);
                labelRect.y += rowHeight;
                igrRect.y += rowHeight;
            }
        }
    }

    public IconGroupReusable(bool alwaysUseTextNumbers)
    {
        this.alwaysUseTextNumbers = alwaysUseTextNumbers;
        iconGroups = new List<IconGroup>();

        textStyle = GUIFunctions.GetStandardGUIStyle(10);
        textStyle.alignment = TextAnchor.UpperRight;

        moreStyle = GUIFunctions.GetStandardGUIStyle(10);
        moreStyle.alignment = TextAnchor.MiddleLeft;
    }

    public void AddIcons(Texture iconTexture, Texture backgroundTexture, int count)
    {
        //can be called many times. "count" number of "iconTexture" icons will be part of the group.
        if (count > 0)
        {
            iconCount += count;
            iconGroups.Add(new IconGroup(iconTexture, backgroundTexture, count));
            iconTypeCount++;
        }
    }

    public void Draw(Rect drawRect)
    {
        //alwaysUseTextNumbers changes five identical icons to just one with "x5", even if all fit.
        float height = drawRect.height;
        Rect iconRect = new Rect(drawRect.x, drawRect.y, height, height);

        float expectedWidth = height * (iconCount + iconTypeCount);
        if (alwaysUseTextNumbers||expectedWidth > drawRect.width)
        {
            //case: icons are too numerous for pictograph, text will say how many of each icon
            foreach (IconGroup iconGroup in iconGroups)
            {
                if (iconRect.x + iconRect.width > drawRect.width+drawRect.x)
                {
                    //not wide enough to draw all icons. draw elipses.
                    GUI.Label(iconRect, "more...", moreStyle);
                    break;
                }
                else
                {
                    //there is enough space for another icon. draw one.
                    if (iconGroup.backgroundTexture != null)
                        GUI.DrawTexture(iconRect, iconGroup.backgroundTexture);
                    GUI.DrawTexture(iconRect, iconGroup.iconTexture);

                    if (iconGroup.count > 1)
                        GUI.Label(iconRect, "x" + iconGroup.count, textStyle);
                    iconRect.x += iconRect.width;
                }
            }
        }
        else
        {
            //case: few enough icons to show all pictograph
            foreach (IconGroup iconGroup in iconGroups)
            {
                for (int i = 0; i < iconGroup.count; i++)
                {
                    if (iconGroup.backgroundTexture != null)
                        GUI.DrawTexture(iconRect, iconGroup.backgroundTexture);
                    GUI.DrawTexture(iconRect, iconGroup.iconTexture);
                    iconRect.x += iconRect.width;
                }
            }
        }
    }

    public static Texture GetIconBackgroundTexture(IconBackgroundTypes ibt)
    {
        if (iconBackgroundTextures == null)
            iconBackgroundTextures = new Dictionary<IconBackgroundTypes, Texture>();
        if (!iconBackgroundTextures.ContainsKey(ibt))
        {
            string filename="unknown";
            if (ibt == IconBackgroundTypes.circle)
                filename = "circle";
            else if (ibt == IconBackgroundTypes.square)
                filename = "square";
            else if (ibt == IconBackgroundTypes.bar)
                filename = "bar";

            Texture texture = Resources.Load(iconFolder + filename) as Texture;
            if (texture == null)
                Debug.LogError("GetIconBackgroundTexture got null texture. ibt=" + ibt+ " filename="+filename);

            iconBackgroundTextures.Add(ibt, texture);
        }
        return iconBackgroundTextures[ibt];
    }

    public static IconGroupReusable GetResourcesIconGroupReusable(BlueprintDesign blueprintDesign, bool alwaysUseTextNumbers)
    {
        StructureInfo structureInfo = new StructureInfo(blueprintDesign.thingType);
        IconGroupReusable igr = new IconGroupReusable(alwaysUseTextNumbers);

        foreach (ResourceTypes resourceType in System.Enum.GetValues(typeof(ResourceTypes)))
        {
            //consume or produce resources
            int iconCount = structureInfo.GetInfoIconCount(resourceType, StructureInfo.InfoTypes.optimum);
            IconBackgroundTypes ibt = iconCount < 0 ? IconBackgroundTypes.bar : IconBackgroundTypes.circle;
            if (iconCount != 0)
            {
                Texture backgroundTexture = IconGroupReusable.GetIconBackgroundTexture(ibt);
                Texture texture = ResourceReusable.GetResourceTextureFromResourceType(resourceType);
                igr.AddIcons(texture, backgroundTexture, Mathf.Abs(iconCount));
            }

            //capacity resources
            iconCount = structureInfo.GetInfoIconCount(resourceType, StructureInfo.InfoTypes.capacity);
            if (iconCount > 0)
            {
                Texture backgroundTexture = IconGroupReusable.GetIconBackgroundTexture(IconBackgroundTypes.square);
                Texture texture = ResourceReusable.GetResourceTextureFromResourceType(resourceType);
                igr.AddIcons(texture, backgroundTexture, iconCount);
            }
        }
        return igr;
    }
}