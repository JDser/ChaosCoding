using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[System.Serializable]
public class NodeConnection
{
    [SerializeField] NodeBase _inputNode;
    [SerializeField] InputStruct _inputStruct;
    [SerializeField] int _inputIndex;

    [SerializeField] NodeBase _outputNode;
    [SerializeField] InputStruct _outputStruct;
    [SerializeField] int _outputIndex;

    public NodeBase InputNode
    {
        get => _inputNode;
    }  
    public InputStruct InputStruct
    {
        get => _inputStruct;
    }
    public int InputIndex
    {
        get => _inputIndex;
    }

    public NodeBase OutputNode
    {
        get => _outputNode;
    }
    public InputStruct OutputStruct
    {
        get => _outputStruct;
    }
    public int OutputIndex
    {
        get => _outputIndex;
    }


    public NodeConnection(NodeBase input, int inputInd, NodeBase output, int outputInd)
    {
        _inputNode = input;
        _inputStruct = input.Inputs[inputInd];
        _inputIndex = inputInd;

        _outputNode = output;
        _outputStruct = output.Outputs[outputInd];
        _outputIndex = outputInd;
    }
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
    [SerializeField] protected AudioClip clip;  
    
    [Header("Inputs & Outputs")]
    [SerializeField] protected InputStruct[] inputs;
    [SerializeField] protected InputStruct[] outputs;

    #region NonSerialized
    protected Canvas canvas;
    protected RectTransform dragRectTransform;

    protected NodeInputBase[] _createdInputs;
    protected NodeInputBase[] _createdOutputs;

    protected NodeConnection[] incomingConnections;
    protected NodeConnection[] outgoingConnections;

    bool _isDrag;

    Vector3 prevMousePosition;
    Vector3 currentMousePosition;

    Vector3 deltaPosition;

    Vector3 newRotation;
    #endregion

    public InputStruct[] Inputs
    {
        get => inputs;
    }  
    public InputStruct[] Outputs
    {
        get => outputs;
    }

    #endregion

    #region BuiltIn Methods
    protected virtual void Awake()
    {
        dragRectTransform = transform.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        SetupNode();
    }

    protected void OnValidate()
    {
        text_nodeName.text = nodeName;
        nodeNameBackground.color = color;
    }

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

    protected void SetupNode()
    {
        text_nodeName.text = nodeName;
        nodeNameBackground.color = color;

        #region Outputs
        outgoingConnections = new NodeConnection[outputs.Length];
        _createdOutputs = new NodeInputBase[outputs.Length];

        for (int i = 0; i < outputs.Length; i++)
        {
            NodeInputBase _nodeInput = Instantiate(inputPrefab, container_outputs);
            _nodeInput.SetInput(outputs[i], i);
            _nodeInput.IsOutput = true;

            _createdOutputs[i] = _nodeInput;
        }
        #endregion

        #region Inputs
        incomingConnections = new NodeConnection[inputs.Length];
        _createdInputs = new NodeInputBase[inputs.Length];

        for (int i = 0; i < inputs.Length; i++)
        {
            NodeInputBase _nodeInput = Instantiate(inputPrefab, container_inputs);
            _nodeInput.SetInput(inputs[i], i);
            _nodeInput.IsOutput = false;

            _createdInputs[i] = _nodeInput;
        }
        #endregion

        #region Resize Window
        float _height = dragRectTransform.sizeDelta.y;
        if (inputs.Length > outputs.Length)
            _height += 45 * (inputs.Length - 1);
        else
            _height += 45 * (outputs.Length - 1);

        dragRectTransform.sizeDelta = new Vector2(dragRectTransform.sizeDelta.x, _height);
        #endregion
    }

    public abstract void Enact();

    public abstract void CheckNewOutput();

    public bool HasReferenceTo(NodeBase reference)
    {
        for (int i = 0; i < incomingConnections.Length; i++)
        {
            if (incomingConnections[i] == null) continue;

            if (incomingConnections[i].InputNode == reference)
                return true;
            if (incomingConnections[i].OutputNode == reference)
                return true;
        }

        for (int i = 0; i < outgoingConnections.Length; i++)
        {
            if (outgoingConnections[i] == null) continue;

            if (outgoingConnections[i].InputNode == reference)
                return true;
            if (outgoingConnections[i].OutputNode == reference)
                return true;
        }

        return false;
    }

    #region Connection Managment
    public virtual void AddOutputConnection(NodeBase inputNode, int otherInputIndex, int thisOutputIndex)
    {
        outgoingConnections[thisOutputIndex] = new NodeConnection(inputNode, otherInputIndex, this, thisOutputIndex);
        outgoingConnections[thisOutputIndex].InputNode.AddInputConnection(outgoingConnections[thisOutputIndex]);
    }
    protected virtual void AddInputConnection(NodeConnection connection)
    {
        if (incomingConnections[connection.InputIndex] != null)
            incomingConnections[connection.InputIndex].OutputNode.ClearOutgoingConnection(incomingConnections[connection.InputIndex].OutputIndex);

        incomingConnections[connection.InputIndex] = connection;
    }


    public virtual void RemoveOutputConnection(int outputIndex)
    {
        if (outgoingConnections[outputIndex] != null)
        {
            outgoingConnections[outputIndex].InputNode.RemoveInputConnection(outgoingConnections[outputIndex].InputIndex);
            outgoingConnections[outputIndex] = null;
        }
    }
    protected virtual void RemoveInputConnection(int index)
    {
        incomingConnections[index] = null;
    }


    protected virtual void ClearIncomingConnection(int connectionIndex)
    {
        _createdInputs[connectionIndex].Clear();
        incomingConnections[connectionIndex] = null;
    }   
    protected virtual void ClearOutgoingConnection(int connectionIndex)
    {
        _createdOutputs[connectionIndex].Clear();
        outgoingConnections[connectionIndex] = null;
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
}