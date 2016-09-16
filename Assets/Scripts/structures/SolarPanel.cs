using UnityEngine;
using System.Collections;

public class SolarPanel : MonoBehaviour
{
    public StructureController structureController;

    void Start()
    {
        StartCoroutine(Cycle());
    }

    private StructureInfo GetStructureInfo()
    {
        if (structureController == null)
            structureController = gameObject.GetComponent<StructureController>();
        return structureController.structureInfo;
    }

    private StructureController GetStructureController()
    {
        if (structureController == null)
            structureController = gameObject.GetComponent<StructureController>();
        return structureController;
    }

    private void SetHourlyRates()
    {
        //this makes tweaks to the resource hourly production rates.
        //setting hourly rates to zero/off is controlled by the structure controller as a generic task.
        if (GetStructureController().isRealStructure)
            GetStructureController().SetAllHourlyRatesToOptimum();
    }

    public IEnumerator Cycle()
    {
        while (true)
        {
            SetHourlyRates();
            if (GetStructureInfo().status == Statuses.active)
                GetStructureController().ConsumeAndProduceResources();
            yield return new WaitForSeconds(GameManager.RecalculateWaitSeconds);
        }
    }
}
