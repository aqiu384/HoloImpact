using UnityEngine;

public static class Utilities
{
    /// <summary>
    /// Get the full path from the root hiearchy to this transform.
    /// <param name="startTransform">The transform to get the path of</param>
    /// </summary>
    public static string GetTransformPath(Transform startTransform)
    {
        var path = "/";

        if (startTransform != null)
        {
            path += startTransform.name;

            while (startTransform.parent != null)
            {
                startTransform = startTransform.parent;
                path = "/" + startTransform.name + path;
            }
        }

        return path;
    }
}