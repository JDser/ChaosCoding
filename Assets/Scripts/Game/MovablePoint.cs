using UnityEngine;

public class MovablePoint : MonoBehaviour
{
    RectTransform rect;

    public Vector3 Position
    {
        set => rect.anchoredPosition = value;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
}