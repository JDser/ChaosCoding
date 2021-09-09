using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    #region Variables
    [Header("Settings")] 

    [SerializeField, Range(0, 100)] int steps;
    [SerializeField] float thickness;

    [SerializeField] protected AnimationCurve verticalCurve;
    [SerializeField] protected AnimationCurve horizontalCurve;

    protected RectTransform _target;

    protected Camera cam;

    protected Vector2 end;
    protected Vector2[] points;

    float width;
    float height;

    public RectTransform Target
    {
        get => _target;
        set => _target = value;
    }
    public Vector2 End
    {
        set => end = value;
    }
    public Camera Camera
    { 
        get => cam;
    }
    #endregion



    protected override void Awake()
    {
        base.Awake();
        cam = Camera.main;
    }

    protected virtual void Update()
    {
       SetAllDirty();
    }

    public virtual void Clear()
    {
        _target = null;
        end = Vector2.zero;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 local;
        if (_target != null)
        {
            Vector2 screenPoint = cam.WorldToScreenPoint(_target.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, cam, out local);
            end = local;
        }    

        points = new Vector2[steps];

        float xDiff = end.x;
        float yDiff = end.y;

        for (int i = 1; i < points.Length; i++)
        {
            if (xDiff < rectTransform.anchoredPosition.x)
            {
                float stepNormalize = Normalize(i + 1, 0, steps);

                points[i].x = Mathf.Lerp(0, end.x, stepNormalize) * horizontalCurve.Evaluate(stepNormalize);
                points[i].y = yDiff * verticalCurve.Evaluate(stepNormalize);
            }
            else
            {
                float stepNormalize = Normalize(i + 1, 0, steps);

                points[i].x = Mathf.Lerp(0, end.x, stepNormalize);
                points[i].y = yDiff * verticalCurve.Evaluate(stepNormalize);
            }

        }

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        if (points.Length < 2)
        {
            return;
        }

        float angle = 0;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 point = points[i];

            if (i < points.Length - 1)
            {
                angle = GetAngle(points[i], points[i + 1]) + 90f;
            }

            DrawVerticesForPoint(point, vh, angle);
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index);
        }
    }

    private void DrawVerticesForPoint(Vector2 point, VertexHelper vh, float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(width * point.x, height * point.y);
        vh.AddVert(vertex);


        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(width * point.x, height * point.y);
        vh.AddVert(vertex);
    }

    protected float GetAngle(Vector2 me,Vector2 target)
    {
        return (float)(Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI));
    }
    protected float Normalize(float x, float min, float max)
    {
        return (x - min) / (max - min);
    }
}