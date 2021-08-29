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
    protected InputData _inputType;
    protected NodeBase _parentNode;
    protected int _nodeIndex;

    protected RectTransform rectImage;
    protected CanvasGroup group;

    protected bool _isOutput;
    #endregion

    #region Properties
    public NodeBase ParentNode
    {
        get => _parentNode;
    }
    public int NodeIndex
    {
        set => _nodeIndex = value;
        get => _nodeIndex;
    }

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
    public InputData InputType
    {
        get => _inputType; 
    }
    public UILineRenderer LineRenderer
    {
        get => lineRenderer;
    }  
    public string Name
    {
        set
        {
            text_Type.text = value;
        }
    }
    #endregion

    #endregion

    protected virtual void Awake()
    {
        group = GetComponent<CanvasGroup>();
        rectImage = image.GetComponent<RectTransform>();

        _parentNode = GetComponentInParent<NodeBase>();
    }

    public virtual void ChangeInputType(InputData type)
    {
        _inputType = type;

        lineRenderer.color = _inputType.Color;
        image.color = _inputType.Color;
    }

    public virtual void Clear()
    {
        _parentNode.ClearOutgoingConnection(_nodeIndex);
        lineRenderer.Clear();    
    }

    public virtual void ConnectNode(NodeInputBase inputNode)
    {
        if (_inputType.Type != inputNode.InputType.Type)
        {
            lineRenderer.End = Vector2.zero;
            LevelManager.PlaySound(connectFailedClip);
            return;
        }

        if (inputNode.ParentNode.HasReferenceTo(_parentNode))
        {
            lineRenderer.End = Vector2.zero;
            LevelManager.PlaySound(connectFailedClip);
            return;
        }

        lineRenderer.Target = inputNode.LineRenderer.rectTransform;


        _parentNode.AddOutputConnection(inputNode._parentNode, inputNode._nodeIndex, _nodeIndex);
        inputNode._parentNode.AddInputConnection(_parentNode, inputNode._nodeIndex,_nodeIndex);


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
        if (original.ParentNode == _parentNode) return;
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
