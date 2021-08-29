using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public abstract class NodeInputBase : MonoBehaviour, IDropHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region Variables
    [SerializeField] protected Image image;
    [SerializeField] protected TextMeshProUGUI text_Type;
    [SerializeField] protected UILineRenderer lineRenderer;

    [Header("Sounds")]
    [SerializeField] protected AudioClip beginDragClip;
    [SerializeField] protected AudioClip connectSuccessClip;
    [SerializeField] protected AudioClip connectFailedClip;

    #region NonSerialized
    protected InputStruct inputType;
    protected NodeBase parentNode;
    protected int nodeIndex;

    protected RectTransform rectImage;
    protected CanvasGroup group;

    protected bool _isOutput;
    #endregion

    #region Properties
    public bool IsOutput
    {
        get
        {
            return _isOutput;
        }
        set
        {
            _isOutput = value;
            lineRenderer.enabled = value;

            lineRenderer.rectTransform.anchoredPosition = new Vector2(value ? 15 : -15, 0);

            Vector2 position = new Vector2(value ? -55 : 55, 0);
            text_Type.rectTransform.anchoredPosition = position;

            rectImage.localScale = new Vector3(value ? 1 : -1, 1, 1);

            text_Type.alignment = value ? TextAlignmentOptions.Right : TextAlignmentOptions.Left ;
        }
    }
    public InputStruct InputType { get => inputType; }
    public UILineRenderer LineRenderer
    {
        get => lineRenderer;
    }  

    public NodeBase ParentNode
    {
        get => parentNode;
    }
    public int NodeIndex
    {
        get => nodeIndex;
    }
    #endregion

    #endregion

    protected virtual void Awake()
    {
        group = GetComponent<CanvasGroup>();
        rectImage = image.GetComponent<RectTransform>();

        parentNode = GetComponentInParent<NodeBase>();
    }

    public virtual void SetInput(InputStruct type,int index)
    {
        inputType = type;
        nodeIndex = index;

        lineRenderer.color = inputType.Color;
        image.color = inputType.Color;

        if (string.IsNullOrEmpty(type.Value) == false)
            text_Type.text = type.Value;
        else
            text_Type.text = type.ValueName;
    }
    public virtual void ChangeInput(InputStruct type)
    {
        inputType = type;

        lineRenderer.color = inputType.Color;
        image.color = inputType.Color;

        if (string.IsNullOrEmpty(type.Value) == false)
            text_Type.text = type.Value;
        else
            text_Type.text = type.ValueName;
    }

    public virtual void Clear()
    {
        parentNode.RemoveOutputConnection(nodeIndex);
        lineRenderer.Clear();    
    }

    public virtual void ConnectNode(NodeInputBase inputNode)
    {
        if (inputType.Type != inputNode.InputType.Type)
        {
            lineRenderer.End = Vector2.zero;
            LevelManager.PlaySound(connectFailedClip);
            return;
        }

        lineRenderer.Target = inputNode.LineRenderer.rectTransform;
        parentNode.AddOutputConnection(inputNode.parentNode, inputNode.nodeIndex, nodeIndex);
        LevelManager.PlaySound(connectSuccessClip);
    }
    
    #region Interfaces
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isOutput) return;

        Clear();

        group.blocksRaycasts = false;
        LevelManager.PlaySound(beginDragClip);
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!_isOutput) return;

        Vector2 screenPoint = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(lineRenderer.rectTransform, screenPoint, lineRenderer.Camera, out Vector2 local);

        lineRenderer.End = local;
    }
    public virtual void OnDrop(PointerEventData eventData)
    {
        if (_isOutput) return;

        if (eventData.pointerDrag == null)
            return;

        NodeInputBase original = eventData.pointerDrag.GetComponent<NodeInputBase>(); // first selected nodeInput

        if (original.ParentNode == parentNode) return;

        original.ConnectNode(this);
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!_isOutput) return;

        group.blocksRaycasts = true;

        if (lineRenderer.Target == null)
        {
            lineRenderer.End = Vector2.zero;
            LevelManager.PlaySound(connectFailedClip);
        }
    }
    #endregion
}
