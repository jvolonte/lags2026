using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers
{
    static Camera camera;

    public static Camera Camera
    {
        get
        {
            if (camera == null) camera = Camera.main;
            return camera;
        }
    }


    static PointerEventData eventDataCurrentPosition;
    static List<RaycastResult> results;

    public static bool IsOverUI()
    {
        eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Any();
    }

    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }

    public static void DeleteChildren(this Transform t)
    {
        if (t != null)
        {
            foreach (Transform child in t)
            {
                if (child != null)
                    Object.Destroy(child.gameObject);
            }
        }
    }

    public static Vector3 CalculateOffset(Vector3 vector)
    {
        var width = Screen.width;
        var height = Screen.height;
        var x = width * vector.x / 1920;
        var y = height * vector.y / 1080;

        return new Vector3(x, y);
    }
}