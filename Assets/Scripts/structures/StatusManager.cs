using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Statuses
{
    active, //green
    full, empty,nostorage, notsetup, inflating, //yellow
    lowpower, //orange
    broken, //purple
    off //red
}

public static class StatusManager
{
    private static Dictionary<Statuses, Texture> statusTextures;
    private static Dictionary<Statuses, string> statusLabels;

    public static Texture GetStatusTexture(Statuses status)
    {
        if (statusTextures == null)
            statusTextures = new Dictionary<Statuses, Texture>();

        if (!statusTextures.ContainsKey(status))
        {
            string path = GetPathFromStatus(status);
            Texture texture = Resources.Load(path) as Texture;
            if (texture == null)
                Debug.LogError("GetStatusTexture got null texture. " + path);
            statusTextures.Add(status, texture);
        }
        return statusTextures[status];
    }

    public static string GetStatusLabel(Statuses status)
    {
        if (statusLabels == null)
            SetStatusLabels();

        if (statusLabels.ContainsKey(status))
            return statusLabels[status];

        Debug.LogError("GetStatusLabel failed to find a label. " + status);
        return "Unknown";
    }

    public static void SetStatusLabels()
    {
        statusLabels = new Dictionary<Statuses, string>();
        statusLabels.Add(Statuses.active, "Active");
        statusLabels.Add(Statuses.broken, "Broken");
        statusLabels.Add(Statuses.full, "Full");
        statusLabels.Add(Statuses.empty, "Empty");
        statusLabels.Add(Statuses.nostorage, "No Storage");
        statusLabels.Add(Statuses.notsetup, "Not Set Up");
        statusLabels.Add(Statuses.inflating, "Inflating");
        statusLabels.Add(Statuses.lowpower, "Low power");
        statusLabels.Add(Statuses.off, "Off");
    }

    public static string GetPathFromStatus(Statuses status)
    {
        string folder = "gui/status/";

        if (status == Statuses.active)
            return folder + "green";
        if (status == Statuses.broken)
            return folder + "purple";
        if (status == Statuses.off)
            return folder + "red";
        if (status == Statuses.lowpower)
            return folder + "orange";
        
        return folder + "yellow";
    }
}
