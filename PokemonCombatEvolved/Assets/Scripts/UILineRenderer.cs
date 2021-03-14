using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO:
// Editar el script para que se dibujen más que los 2 primeros puntos de cada línea (todos los puntos)

public class UILineRenderer : Graphic
{
    public List<List<RectTransform>> lines = new List<List<RectTransform>>();
    public float thickness = 20f;


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        for (int i = 0; i < lines.Count; i++)
        {
            Vector3 point1Position = rectTransform.InverseTransformPoint(lines[i][0].TransformPoint(new Vector3()));
            Vector3 point2Position = rectTransform.InverseTransformPoint(lines[i][1].TransformPoint(new Vector3()));
            float angle = GetAngle(point1Position, point2Position);

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(0, -thickness / 2) + point1Position;
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(0, +thickness / 2) + point1Position;
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(0, +thickness / 2) + point2Position;
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(0, -thickness / 2) + point2Position;
            vh.AddVert(vertex);

            int offset = 4 * i;

            vh.AddTriangle(0 + offset, 1 + offset, 2 + offset);
            vh.AddTriangle(0 + offset, 2 + offset, 3 + offset);
        }
    }

    public float GetAngle(Vector2 start, Vector2 end)
    {
        return (float)(Mathf.Atan2(end.y - start.y, end.x - start.x) * (180f / Mathf.PI));
    }
}
