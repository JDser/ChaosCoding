using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

public abstract class NodeBase : MonoBehaviour
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

    [Header("Prefabs")]
    [SerializeField] NodeInputBase inputPrefab;

    [Header("Audio")]
    [SerializeField] protected AudioClip confirmClip;  
    [SerializeField] protected AudioClip deniedClip;  
    
    [Header("Inputs & Outputs")]
    [SerializeField] protected InputData[] inputs;
    [SerializeField] protected InputData[] outputs;

    #region NonSerialized
    protected RectTransform rectTransform;

    protected NodeConnection[] _incomingConnections;
    protected NodeConnection[] _outgoingConnections; 
    
    protected UILineAnimation[] lineAnims;

    protected UIAnimator animator;

    readonly AnimationCurve popupCurve = new AnimationCurve(
    new Keyframe(0, 0),
    new Keyframe(0.7f, 1.2f),
    new Keyframe(0.9f, 0.9f),
    new Keyframe(1, 1)
    );
    readonly AnimationCurve unspawnCurve = new AnimationCurve(
    new Keyframe(0, 1),
    new Keyframe(0.2f, 1.2f),
    new Keyframe(1, 0)
    );
    #endregion

    #endregion

    #region BuiltIn Methods
    protected virtual void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        animator = GetComponent<UIAnimator>();
    }

    #if UNITY_EDITOR
    protected void OnValidate()
    {
        text_nodeName.text = nodeName;
        nodeNameBackground.color = color;
    }
    #endif

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

        animator.SizeAnimation(popupCurve, 2);
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
        float _height = rectTransform.sizeDelta.y;
        if (inputs.Length > outputs.Length)
            _height += 45 * (inputs.Length - 1);
        else
            _height += 45 * (outputs.Length - 1);

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _height);
    }

    protected virtual void SetupLineAnimators()
    {
        lineAnims = new UILineAnimation[_outgoingConnections.Length];

        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            lineAnims[i] = (UILineAnimation)_outgoingConnections[i].NodeOutputBase.LineRenderer;
            lineAnims[i].SecondColor = _outgoingConnections[i].NodeOutputBase.InputType.SecondColor;

            //lineAnims[i].OnAnimationStart += OnAnimationStart;
            //lineAnims[i].OnAnimationEnd += OnAnimationEnd;

            lineAnims[i].OnFirstPoint += OnFirstPoint;
            lineAnims[i].OnLastPoint += OnLastPoint;
        }
    }

    public virtual void SetNewInput(int index, InputData newData)
    {
        if (index >= inputs.Length)
            System.Array.Resize(ref inputs, index + 1);

        inputs[index] = newData;
    }

    public virtual void SetNewOutput(int index, InputData newData)
    {
        if (index >= outputs.Length) 
            System.Array.Resize(ref outputs, index + 1);

        outputs[index] = newData;
    }
    #endregion

    #region 
    public virtual void Enact()
    { 
        StopAllCoroutines();

        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            if (_outgoingConnections[i].IsValid == false)
            {
                LevelManager.PlaySound(deniedClip);
                animator.SizeAnimation();
                return;
            }

            lineAnims[i].Animate = true;
        }

        LevelManager.PlaySound(confirmClip);
        animator.ShakeAnimation();
    }

    public virtual void Stop()
    {
        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            lineAnims[i].Animate = false;
        }
    }

    public virtual void UnSpawn()
    {
        animator.SizeAnimation(unspawnCurve, 2);
    }

    public abstract void CheckNewOutput();
    #endregion

    #region Animation Events
    //protected abstract void OnAnimationStart();
    //
    //protected abstract void OnAnimationEnd();   
    
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
    #endregion

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
    #endregion
}