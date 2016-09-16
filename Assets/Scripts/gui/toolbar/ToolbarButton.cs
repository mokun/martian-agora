using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolbarButton
{
    public Texture buttonTexture;
    public string tooltipText, tooltipID;
    public bool activated;
    public float mouseoverTimer;
    public WindowTypes windowType;

    private Rect drawRect;
    private Toolbar toolbar;
    private int buttonIndex;

    public ToolbarButton(Toolbar toolbar, int buttonIndex, string pictureName,
        WindowTypes windowType, string tooltipText)
    {
        this.buttonIndex = buttonIndex;
        tooltipID = "toolbar-" + pictureName;
        this.windowType = windowType;
        this.toolbar = toolbar;

        string path = "gui/toolbar/" + pictureName;
        buttonTexture = Resources.Load(path) as Texture;
        if (buttonTexture == null)
            Debug.LogError("ToolbarButton failed to load texture: " + path);

        this.tooltipText = tooltipText;

        activated = true;
        mouseoverTimer = 0;
    }


    public Rect GetDrawRect()
    {
        if (drawRect.width != 0)
            return drawRect;

        float bWidth = toolbar.GetButtonWidth();
        float rectX = Screen.width / 2 - toolbar.GetToolbarWidth() / 2 + bWidth * buttonIndex;
        drawRect = new Rect(rectX, Screen.height - bWidth, bWidth, bWidth);
        return drawRect;
    }
}
