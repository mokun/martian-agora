using UnityEngine;
using System.Collections;

public class PlasticDome : MonoBehaviour
{
    public StructureController structureController;

    void Start()
    {
        GetStructureInfo().status = Statuses.inflating;
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
    
    public IEnumerator Cycle()
    {
        while (true)
        {
            if (GetStructureInfo().status == Statuses.inflating && IsDoneInflating())
                GetStructureInfo().status = Statuses.active;
            yield return new WaitForSeconds(GameManager.RecalculateWaitSeconds);
        }
    }

    private bool IsDoneInflating()
    {
        foreach (GameObject child in ParentChildFunctions.GetAllChildren(gameObject))
        {
            Animation a = child.GetComponent<Animation>();
            if (a != null && !a.isPlaying)
                return true;
        }
        return false;
    }
}
