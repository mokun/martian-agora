using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColonyToolbar : MonoBehaviour
{
    private Toolbar toolbar;
    
    void Start()
    {
        SetToolbar();
    }
    
    private void SetToolbar()
    {
        toolbar = new Toolbar();

        toolbar.AddButton(WindowTypes.resources, "resources",
            "Shows how much water, oxygen, food, and electricity the colony has.");
        toolbar.AddButton(WindowTypes.crew, "crew",
            "Shows how all the crew members are doing, and what they're up to.");
        toolbar.AddButton(WindowTypes.log, "log",
            "Shows important messages and events.");
        toolbar.AddButton(WindowTypes.operations, "operations",
            "Control how much resources are consumed by different activities.");
        toolbar.AddButton(WindowTypes.structures, "structures",
            "Lists all structures and their statuses.");
        toolbar.AddButton(WindowTypes.robots, "robots",
            "Shows how all the robots are doing, and what they're up to.");
        toolbar.AddButton(WindowTypes.researchprojects, "research-projects",
            "Lists all research projects and their statuses.");
        toolbar.AddButton(WindowTypes.research, "research",
            "Shows the progress and outcomes of present and future research.");
        toolbar.AddButton(WindowTypes.widgetdesign, "widget-design",
            "Design new widgets here.");
        toolbar.AddButton(WindowTypes.basedesign, "base-design",
    "Design the base layout here.");
    }
    
    private void OnGUI()
    {
        toolbar.Draw();
    }
}
