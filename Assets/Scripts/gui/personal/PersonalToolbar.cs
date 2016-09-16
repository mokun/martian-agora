using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonalToolbar : MonoBehaviour
{
    private Toolbar toolbar;

    void Start()
    {
        SetToolbar();
    }

    private void SetToolbar()
    {
        toolbar = new Toolbar();

        toolbar.AddButton(WindowTypes.inventory, "inventory",
            "Shows all the stuff you are carrying and allows you to ready them for use.");
        toolbar.AddButton(WindowTypes.equipment, "equipment",
            "Shows all the equipment you are wearing and allows you to equip more, or equip less.");
        toolbar.AddButton(WindowTypes.vehicleinventory, "vehicle-inventory",
            "Shows all the stuff inside all nearby vehicles.");
        toolbar.AddButton(WindowTypes.blueprints, "blueprints",
            "See a list of available blueprints that you can place on Mars.");

    }

    private void OnGUI()
    {
        toolbar.Draw();
    }
}

