using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public struct NodeConnection
{
    [SerializeField] NodeBase _outputNode;
    [SerializeField] InputData _outputStruct;
    [SerializeField] NodeInputBase _nodeOutputBase;
    [SerializeField] int _outputIndex;

    [Space]

    [SerializeField] NodeBase _inputNode;
    [SerializeField] InputData _inputStruct;
    [SerializeField] NodeInputBase _nodeInputBase;
    [SerializeField] int _inputIndex;

    #region Properties
    public NodeBase OutputNode
    {
        get => _outputNode;
    }
    public InputData OutputStruct
    {
        get => _outputStruct;
    }
    public NodeInputBase NodeOutputBase
    {
        get => _nodeOutputBase;
    }
    public int OutputIndex
    {
        get => _outputIndex;
    }

    public NodeBase InputNode
    {
        get => _inputNode;
    }  
    public InputData InputStruct
    {
        get => _inputStruct;
    }
    public NodeInputBase NodeInputBase 
    {
        get => _nodeInputBase;
    }
    public int InputIndex
    {
        get => _inputIndex;
    }

    public bool IsValid
    {
        get => _inputNode != null && _outputNode != null;
    }
    #endregion

    #region Methods
    public void SetInput(NodeBase node,InputData inputStruct, NodeInputBase nodeInput,int index)
    {
        _inputNode = node;
        _inputStruct = inputStruct;
        _nodeInputBase = nodeInput;
        _inputIndex = index;
    }
    public void ClearInput()
    {
        _inputNode = null;
        _inputStruct = null;
        _nodeInputBase = null;
        _inputIndex = -1;
    }

    public void SetOutput(NodeBase node, InputData inputStruct, NodeInputBase nodeInput, int index)
    {
        _outputNode = node;
        _outputStruct = inputStruct;
        _nodeOutputBase = nodeInput;
        _outputIndex = index;
    }
    public void ClearOutput()
    {
        _outputNode = null;
        _outputStruct = null;
        _nodeOutputBase = null;
        _outputIndex = -1;
    }
    #endregion
}

public abstract class NodeBase : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Variables
    [Header("Node Setup")]
    [SerializeField] string nodeName;
    [SerializeField] Color color;

    [Header("References")]
    [SerializeField] TextMeshProUGUI text_nodeName;
    [SerializeField] Image nodeNameBackground;

    [SerializeField] Transform container_inputs;
    [SerializeField] Transform container_outputs;

    [Header("Rotation")]
    [SerializeField] Vector2 minMaxRotation;
    [SerializeField] float rotationSpeed = 1.5f;

    [Header("Prefabs")]
    [SerializeField] NodeInputBase inputPrefab;

    [Header("Audio")]
    [SerializeField] protected AudioClip confirmClip;  
    [SerializeField] protected AudioClip deniedClip;  
    
    [Header("Inputs & Outputs")]
    [SerializeField] protected InputData[] inputs;
    [SerializeField] protected InputData[] outputs;

    #region NonSerialized
    protected Canvas canvas;
    protected RectTransform dragRectTransform;

    protected NodeConnection[] _incomingConnections;
    protected NodeConnection[] _outgoingConnections; 
    
    protected UILineAnimation[] lineAnims;

    bool _isDrag;

    Vector3 prevMousePosition;
    Vector3 currentMousePosition;

    Vector3 deltaPosition;

    Vector3 newRotation;
    #endregion

    #endregion

    #region BuiltIn Methods
    protected virtual void Awake()
    {
        dragRectTransform = transform.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    protected virtual void Start() { }

    #if UNITY_EDITOR
    protected void OnValidate()
    {
        text_nodeName.text = nodeName;
        nodeNameBackground.color = color;
    }
    #endif

    protected void Update()
    {
        if (!_isDrag)
        {
            newRotation.z -= newRotation.z * rotationSpeed;
            newRotation.z = Mathf.Clamp(newRotation.z, minMaxRotation.x, minMaxRotation.y);

            dragRectTransform.rotation = Quaternion.Euler(newRotation);
        }
    }
    #endregion

    #region Node Setup
    public virtual void SetupNode()
    {
        text_nodeName.text = nodeName;
        nodeNameBackground.color = color;

        SetupOutputs();
        SetupInputs();
        SetupWindow();

        SetupLineAnimators();
    }

    protected virtual void SetupOutputs()
    {
        _outgoingConnections = new NodeConnection[outputs.Length];

        for (int i = 0; i < outputs.Length; i++)
        {
            NodeInputBase _nodeOutput = Instantiate(inputPrefab, container_outputs);

            _nodeOutput.NodeIndex = i;
            _nodeOutput.Name = string.IsNullOrEmpty(outputs[i].DefaultValue) ? outputs[i].DataName : outputs[i].DefaultValue;

            _nodeOutput.ChangeInputType(outputs[i]);
            _nodeOutput.IsOutput = true;

            _outgoingConnections[i].SetOutput(this, outputs[i], _nodeOutput, i);
        }
    }

    protected virtual void SetupInputs()
    {
        _incomingConnections = new NodeConnection[inputs.Length];

        for (int i = 0; i < inputs.Length; i++)
        {
            NodeInputBase _nodeInput = Instantiate(inputPrefab, container_inputs);

            _nodeInput.NodeIndex = i;
            _nodeInput.Name = string.IsNullOrEmpty(inputs[i].DefaultValue) ? inputs[i].DataName : inputs[i].DefaultValue;

            _nodeInput.ChangeInputType(inputs[i]);
            _nodeInput.IsOutput = false;

            _incomingConnections[i].SetInput(this, inputs[i], _nodeInput, i);
        }
    }

    protected virtual void SetupWindow()
    {
        float _height = dragRectTransform.sizeDelta.y;
        if (inputs.Length > outputs.Length)
            _height += 45 * (inputs.Length - 1);
        else
            _height += 45 * (outputs.Length - 1);

        dragRectTransform.sizeDelta = new Vector2(dragRectTransform.sizeDelta.x, _height);
    }

    protected virtual void SetupLineAnimators()
    {
        lineAnims = new UILineAnimation[_outgoingConnections.Length];

        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            lineAnims[i] = (UILineAnimation)_outgoingConnections[i].NodeOutputBase.LineRenderer;
            lineAnims[i].SecondColor = _outgoingConnections[i].NodeOutputBase.InputType.SecondColor;

            lineAnims[i].OnAnimationStart += OnAnimationStart;
            lineAnims[i].OnAnimationEnd += OnAnimationEnd;

            lineAnims[i].OnFirstPoint += OnFirstPoint;
            lineAnims[i].OnLastPoint += OnLastPoint;
        }
    }

    public void SetNewOutput(int index, InputData newData)
    {
        if (index >= outputs.Length) 
            System.Array.Resize(ref outputs, index + 1);

        outputs[index] = newData;
    }

    public void SetNewInput(int index, InputData newData)
    {
        if (index >= inputs.Length)
            System.Array.Resize(ref inputs, index + 1);

        inputs[index] = newData;
    }
    #endregion

    public bool HasReferenceTo(NodeBase reference)
    {
        bool hasReference = false;

        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            if (_outgoingConnections[i].IsValid == false)
                continue;

            if (_outgoingConnections[i].InputNode == reference)
                return true;

            hasReference = _outgoingConnections[i].InputNode.HasReferenceTo(reference);
        }

        return hasReference;
    }

    public virtual void Enact()
    { 
        StopAllCoroutines();

        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            if (_outgoingConnections[i].IsValid == false)
            {
                LevelManager.PlaySound(deniedClip);
                StartCoroutine(FailRoutine());
                return;
            }

            lineAnims[i].Animate = true;
        }

        LevelManager.PlaySound(confirmClip);
        StartCoroutine(ShakeAnimationRoutine());
    }

    public virtual void Stop()
    {
        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            lineAnims[i].Animate = false;
        }
    }

    public abstract void CheckNewOutput();

    protected abstract void OnAnimationStart();

    protected abstract void OnAnimationEnd();   
    
    protected virtual void OnFirstPoint()
    {
        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            if (_outgoingConnections[i].IsValid)
                _outgoingConnections[i].InputNode.Enact();
        }
    }

    protected virtual void OnLastPoint()
    {
        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            if (_outgoingConnections[i].IsValid)
                _outgoingConnections[i].InputNode.Stop();
        }
    }


    #region Connection Managment
    public virtual void AddOutputConnection(NodeBase inputNode, int otherInputIndex, int thisOutputIndex)
    {
        if (_outgoingConnections[thisOutputIndex].IsValid)
        {
            _outgoingConnections[thisOutputIndex].ClearInput();
        }
        
        _outgoingConnections[thisOutputIndex].SetInput(
            inputNode, 
            inputNode.inputs[otherInputIndex],
            inputNode._incomingConnections[otherInputIndex].NodeInputBase, 
            otherInputIndex);

        inputNode.AddInputConnection(this, otherInputIndex, thisOutputIndex);
    }
    protected virtual void AddInputConnection(NodeBase inputNode, int thisInputIndex, int otherOutputIndex)
    {
        if (_incomingConnections[thisInputIndex].IsValid)
        {
            _incomingConnections[thisInputIndex].OutputNode.ClearOutgoingConnectionIgnore(_incomingConnections[thisInputIndex].OutputIndex);
            _incomingConnections[thisInputIndex].ClearOutput();
        }

       _incomingConnections[thisInputIndex].SetOutput(
            inputNode,
            inputNode.outputs[otherOutputIndex], 
            inputNode._outgoingConnections[otherOutputIndex].NodeOutputBase,
            otherOutputIndex);
    }
   
    public virtual void ClearOutgoingConnection(int connectionIndex)
    {
        if (_outgoingConnections[connectionIndex].IsValid == false) return;

        _outgoingConnections[connectionIndex].InputNode.ClearIncomingConnection(_outgoingConnections[connectionIndex].InputIndex);
        _outgoingConnections[connectionIndex].ClearInput();
    } 
    
    protected virtual void ClearOutgoingConnectionIgnore(int connectionIndex)
    {
        if (_outgoingConnections[connectionIndex].IsValid == false) return;

        _outgoingConnections[connectionIndex].ClearInput();
        _outgoingConnections[connectionIndex].NodeOutputBase.LineRenderer.Clear();
    }
    protected virtual void ClearIncomingConnection(int connectionIndex)
    {
        _incomingConnections[connectionIndex].ClearOutput();
    }
    #endregion

    #region Interfaces
    public void OnPointerDown(PointerEventData eventData)
    {
        dragRectTransform.SetAsLastSibling();
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

        dragRectTransform.rotation = Quaternion.Euler(newRotation);
        prevMousePosition = Input.mousePosition;
        #endregion

        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;
    }
    #endregion

    #region Animations
    protected IEnumerator ShakeAnimationRoutine()
    {
        float _progress = 0;
        float zAmount;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * 5;

            zAmount = Mathf.Sin(_progress * 50) * 2.5f;

            dragRectTransform.rotation = Quaternion.Euler(0, 0, zAmount);

            yield return null;
        }
    }
    protected IEnumerator FailRoutine()
    {
        float _progress = 0;
        float zAmount;

        Vector3 prevScale = dragRectTransform.localScale;

        while (_progress < Mathf.PI)
        {
            _progress += Time.deltaTime * 20;
            zAmount = -(Mathf.Sin(_progress) * 0.2f - 1);
            dragRectTransform.localScale = new Vector3(zAmount, zAmount, zAmount);

            yield return null;
        }

        dragRectTransform.localScale = prevScale;
    }
    #endregion
}