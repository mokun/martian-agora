using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//classes that end in "Reusable" are generic GUI functions.
public class StructureReusable
{
    Rect iconRect, titleRect, listRect;
    public List<IconGroupReusable> igrList;
    public List<string> labels;
    private static GUIStyle titleStyle, labelStyle;
    private float titleHeight;

    private Texture iconTexture, background;
    private string title;
    private BlueprintDesign blueprintDesign;

    public StructureReusable(Rect drawRect, Texture iconTexture, string title, BlueprintDesign blueprintDesign)
    {
        igrList = new List<IconGroupReusable>();
        labels = new List<string>();

        this.iconTexture = iconTexture;
        this.title = title;
        this.blueprintDesign = blueprintDesign;

        titleStyle = GUIFunctions.GetStandardGUIStyle(12);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle = GUIFunctions.GetStandardGUIStyle(8);
        labelStyle.alignment = TextAnchor.MiddleLeft;

        titleHeight = titleStyle.CalcSize(new GUIContent("HEIGHT LAWL")).y;

        float iconSize = drawRect.height - titleHeight;
        iconRect = new Rect(0, 0, iconSize, iconSize);
        titleRect = new Rect(0, 0, drawRect.width, titleHeight);
        listRect = new Rect(iconRect.width, titleHeight, drawRect.width, iconRect.height);

        background = GUIFunctions.Get1x1Texture(new Color(0f, 1f, 1f, 0.5f));

        BuildIGRList();
    }

    private void BuildIGRList()
    {
        //create resource exchange icon group
        labels.Add("Resources:");
        IconGroupReusable igr = IconGroupReusable.GetResourcesIconGroupReusable(blueprintDesign, false);
        igrList.Add(igr);

        //create materials needed icon group
        igr = new IconGroupReusable(true);
        igrList.Add(igr);
        labels.Add("Materials needed:");
        foreach (ThingTypes tt in blueprintDesign.GetThingTotal().Keys)
        {
            Texture texture = ThingFactory.MakeThing(tt).iconTexture;
            int count = blueprintDesign.GetThingTotal()[tt];
            igr.AddIcons(texture, null, count);
        }

        //create work needed icon group
        igr = new IconGroupReusable(true);
        igrList.Add(igr);
        labels.Add("Tools needed:");
        foreach (ThingTypes tt in blueprintDesign.GetToolTotal().Keys)
        {
            Texture texture = ThingFactory.MakeThing(tt).iconTexture;
            float amount = blueprintDesign.GetToolTotal()[tt];
            igr.AddIcons(texture, null, Mathf.CeilToInt(amount));
        }

    }

    public void Draw(Rect drawRect, bool drawBackgroundColor)
    {
        iconRect.x = drawRect.x + GUIFunctions.margin;
        titleRect.x = drawRect.x + GUIFunctions.margin;
        listRect.x = drawRect.x + iconRect.width + GUIFunctions.margin * 2;

        iconRect.y = drawRect.y + titleHeight;
        titleRect.y = drawRect.y;
        listRect.y = drawRect.y + titleHeight;

        if (drawBackgroundColor)
            GUI.DrawTexture(drawRect, background);

        GUI.DrawTexture(iconRect, iconTexture);
        GUI.Label(titleRect, title, titleStyle);
        IconGroupReusable.DrawLabelledIGRList(labels, igrList, listRect, labelStyle);
    }

}
