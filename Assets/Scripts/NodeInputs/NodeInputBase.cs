using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public abstract class NodeInputBase : MonoBehaviour, IDropHandler, IBeginDragHandler, IEndDragHandler, IDragHandler , IPointerEnterHandler,IPointerExitHandler
{
    #region Static Wrappers
    static NodeInputBase CurrentNode;
    static UILineRenderer CurrentLine;
    #endregion

    #region Variables
    [SerializeField] protected Image image;
    [SerializeField] protected TextMeshProUGUI text_Type;
    [SerializeField] protected UILineRenderer _lineRenderer;

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
            _lineRenderer.enabled = value;

            _lineRenderer.rectTransform.anchoredPosition = new Vector2(value ? 15 : -15, 0);

            text_Type.rectTransform.anchoredPosition = new Vector2(value ? -55 : 55, 0);

            rectImage.localScale = new Vector3(value ? 1 : -1, 1, 1);

            text_Type.alignment = value ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;

            if (value) ((UILineAnimation)_lineRenderer).PoolPoints();
        }
    }
    public InputData InputType
    {
        get => _inputType; 
    }
    public UILineRenderer LineRenderer
    {
        get => _lineRenderer;
    }  
    public string Name
    {
        set => text_Type.text = value;   
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

        _lineRenderer.color = _inputType.Color;
        image.color = _inputType.Color;
    }

    protected virtual void Clear()
    {
        _parentNode.ClearOutgoingConnection(_nodeIndex);
        _lineRenderer.Clear();
    }

    public virtual void ConnectNode(NodeInputBase inputNode)
    {
        if (_inputType.Type != inputNode.InputType.Type)
        {
            _lineRenderer.End = Vector2.zero;
            LevelManager.PlaySound(connectFailedClip);
            return;
        }

        if (inputNode.ParentNode.HasReferenceTo(_parentNode))
        {
            _lineRenderer.End = Vector2.zero;
            LevelManager.PlaySound(connectFailedClip);
            return;
        }

        _parentNode.AddOutputConnection(inputNode._parentNode, inputNode._nodeIndex, _nodeIndex);

        LevelManager.PlaySound(connectSuccessClip);
    }
    
    #region Interfaces
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isOutput) return;

        Clear();

        CurrentNode = this;
        CurrentLine = _lineRenderer;

        group.blocksRaycasts = false;
        LevelManager.PlaySound(beginDragClip);
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!_isOutput) return;

        Vector2 screenPoint = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_lineRenderer.rectTransform, screenPoint, _lineRenderer.Camera, out Vector2 local);

        _lineRenderer.End = local;
    }
    public virtual void OnDrop(PointerEventData eventData)
    {
        if (_isOutput) return;

        if (CurrentNode && CurrentNode.ParentNode != _parentNode)
            CurrentNode.ConnectNode(this);
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!_isOutput) return;

        group.blocksRaycasts = true;

        if (_lineRenderer.Target == null)
        {
            _lineRenderer.End = Vector2.zero;
            LevelManager.PlaySound(connectFailedClip);
        }

        CurrentNode = null;
        CurrentLine = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isOutput) return;

        if (CurrentLine)
            CurrentLine.Target = this._lineRenderer.rectTransform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isOutput) return;

        if (CurrentLine)
            CurrentLine.Target = null;
    }
    #endregion
}
