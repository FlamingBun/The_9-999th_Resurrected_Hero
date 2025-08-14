using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UIDottedLineURP : Graphic
{
    public Vector2 startPoint;
    public Vector2 endPoint;

    public float dotLength = 12f;
    public float lineGap = 6f;
    public float lineThickness = 2f;
    public float lineSpeed = 1.5f;
    private float dashOffset;

    private bool needRedraw = false;

    void Update()
    {
        dashOffset += Time.unscaledDeltaTime * lineSpeed;
        dashOffset %= (dotLength + lineGap);

        needRedraw = true;
    }

    void LateUpdate()
    {
        if (needRedraw)
        {
            needRedraw = false;
            SetVerticesDirty();
        }
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 dir = (endPoint - startPoint).normalized;
        float length = Vector2.Distance(startPoint, endPoint);
        Vector2 perp = Vector2.Perpendicular(dir) * (lineThickness * 0.5f);

        float drawn = -dashOffset;

        while (drawn < length)
        {
            float seg = Mathf.Min(dotLength, length - drawn);
            if (seg > 0)
            {
                Vector2 p1 = startPoint + dir * drawn;
                Vector2 p2 = startPoint + dir * (drawn + seg);

                AddQuad(vh, p1 - perp, p1 + perp, p2 + perp, p2 - perp);
            }
            drawn += dotLength + lineGap;
        }
    }

    private void AddQuad(VertexHelper vh, Vector2 bl, Vector2 tl, Vector2 tr, Vector2 br)
    {
        int idx = vh.currentVertCount;

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        vert.position = bl; vh.AddVert(vert);
        vert.position = tl; vh.AddVert(vert);
        vert.position = tr; vh.AddVert(vert);
        vert.position = br; vh.AddVert(vert);

        vh.AddTriangle(idx, idx + 1, idx + 2);
        vh.AddTriangle(idx + 2, idx + 3, idx);
    }
}
