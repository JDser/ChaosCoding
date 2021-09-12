using UnityEngine;
using UnityEngine.EventSystems;

public class UIMovable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Variables
    [Header("Rotation")]
    [SerializeField] Vector2 minMaxRotation;
    [SerializeField] float rotationSpeed = 1.5f;

    #region NonSerialized
    RectTransform rectTransform;
    Canvas canvas;

    bool _isDrag;

    Vector3 prevMousePosition;
    Vector3 currentMousePosition;

    Vector3 deltaPosition;  
    Vector3 newRotation;
    #endregion

    #endregion

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    protected void Update()
    {
        if (!_isDrag)
        {
            newRotation.z -= newRotation.z * rotationSpeed;
            newRotation.z = Mathf.Clamp(newRotation.z, minMaxRotation.x, minMaxRotation.y);

            rectTransform.rotation = Quaternion.Euler(newRotation);
        }
    }

    #region Interfaces
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDrag = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        #region Rotation
        currentMousePosition = Input.mousePosition;

        deltaPosition = (currentMousePosition - prevMousePosition).normalized;

        newRotation.z -= deltaPosition.x * rotationSpeed;
        newRotation.z = Mathf.Clamp(newRotation.z, minMaxRotation.x, minMaxRotation.y);

        rectTransform.rotation = Quaternion.Euler(newRotation);
        prevMousePosition = Input.mousePosition;
        #endregion

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;
    }
    #endregion
}