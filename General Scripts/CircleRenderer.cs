using UnityEngine;
using UnityEngine.TextCore.Text;

public static class CircleRenderer
{
    public static LineRenderer DrawCircle(this GameObject gameObject, float radius, int segments = 50, string materialName = "Default Line Material")
    {
        gameObject.TryGetComponent<LineRenderer>(out LineRenderer line);

        if (line == null)
            line = gameObject.AddComponent<LineRenderer>();
        else
            line.enabled = true;

        line.material = (Material) Resources.Load(materialName);
        line.sortingLayerName = "UI";

        if ((radius / MapGenerator.CellSize) - 0.5f <= 0)
        {
            line.enabled = false;
            return null;
        }

        float xradius = radius / gameObject.transform.localScale.x;
        float yradius = radius / gameObject.transform.localScale.y;
        
        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        line.startWidth = 0.3f;
        line.endWidth = 0.3f;
        line.sortingLayerID = SortingLayer.NameToID("UI");
        line.sortingOrder = -1;

        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }

        return line;
    }

    public static void DestroyCircle(LineRenderer lineRenderer)
    {
        lineRenderer.enabled = false;
    }
}