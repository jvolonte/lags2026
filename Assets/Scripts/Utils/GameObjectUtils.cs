using UnityEngine;

public static class GameObjectUtils 
{
    public static void SetLayerRecursively(this GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' does not exist.");
            return;
        }

        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = layer;
    }
    public static void ReplaceLayerRecursively(this GameObject obj, string fromLayer, string toLayer)
    {
        int from = LayerMask.NameToLayer(fromLayer);
        int to = LayerMask.NameToLayer(toLayer);

        if (from == -1) { Debug.LogWarning($"Layer '{fromLayer}' does not exist."); return; }
        if (to == -1) { Debug.LogWarning($"Layer '{toLayer}' does not exist."); return; }

        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.layer == from)
                t.gameObject.layer = to;
        }
    }
}
