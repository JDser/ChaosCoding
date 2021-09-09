using UnityEngine;
using System.Collections;

public class MathNode : NodeBase
{
    #region Variables
    [Header("Custom")]
    [SerializeField] InputData originData;
    [SerializeField] MathOperation mathOperation;

    UILineAnimation[] lineAnims;

    DataType currentType;

    public InputData OriginData
    {
        get => originData;
        set => originData = value;
    }
    #endregion

    public override void SetupNode()
    {
        base.SetupNode();
        SetupLineAnimators();
    }

    private void SetupLineAnimators()
    {
        lineAnims = new UILineAnimation[_outgoingConnections.Length];

        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            lineAnims[i] = (UILineAnimation)_outgoingConnections[i].NodeOutputBase.LineRenderer;
            lineAnims[i].SecondColor = _outgoingConnections[i].NodeOutputBase.InputType.SecondColor;

            lineAnims[i].OnAnimationEnd += OnAnimationEnd;
        }
    }

    private void OnAnimationEnd()
    {
        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            if (_outgoingConnections[i].IsValid)
                _outgoingConnections[i].InputNode.Enact();
        }
    }

    public override void Enact()
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

            //lineAnims[i].StartAnimation();
            lineAnims[i].Animate = true;
        }

        LevelManager.PlaySound(confirmClip);
        StartCoroutine(ShakeAnimationRoutine());
    }


    #region Connection Managment
    protected override void AddInputConnection(NodeBase inputNode, int thisInputIndex, int otherOutputIndex)
    {
        base.AddInputConnection(inputNode, thisInputIndex, otherOutputIndex);

        NodeConnection current = _incomingConnections[thisInputIndex];


        if (string.IsNullOrEmpty(current.OutputStruct.DefaultValue))
        {
            current.NodeInputBase.Name = current.OutputStruct.DataName;
            inputs[thisInputIndex].DefaultValue = null;
        }
        else
        {
            current.NodeInputBase.Name = current.OutputStruct.DefaultValue;
            inputs[thisInputIndex].DefaultValue = current.OutputStruct.DefaultValue;
        }

        currentType = current.OutputStruct.Type;
        CheckNewOutput();
    }

    protected override void ClearIncomingConnection(int connectionIndex)
    {
        base.ClearIncomingConnection(connectionIndex);

        if (string.IsNullOrEmpty(originData.DefaultValue))
        {
            _incomingConnections[connectionIndex].NodeInputBase.Name = originData.DataName;
            inputs[connectionIndex].DefaultValue = null;
        }
        else
        {
            _incomingConnections[connectionIndex].NodeInputBase.Name = originData.DefaultValue;
            inputs[connectionIndex].DefaultValue = originData.DefaultValue;
        }

        currentType = originData.Type;
        CheckNewOutput();
    }
    #endregion

    public override void CheckNewOutput()
    {
        if (_outgoingConnections.Length > 1)
            Debug.LogError("More than 1 output!", this);


        string current = "";

        if (_incomingConnections[0].IsValid)
            current = _incomingConnections[0].OutputStruct.DefaultValue;

        for (int i = 1; i < _incomingConnections.Length; i++)
        {
            if (_incomingConnections[i].IsValid == false) continue;

            string next = _incomingConnections[i].OutputStruct.DefaultValue;

            switch (mathOperation)
            {
                case MathOperation.Add:
                    current = HandleAdd(current, next);
                    break;

                case MathOperation.Subtract:
                    current = HandleSubtract(current, next);
                    break;

                case MathOperation.Multiply:
                    current = HandleMultiply(current, next);
                    break;

                case MathOperation.Divide:
                    current = HandleDivide(current, next);
                    break;

                case MathOperation.Cast:
                    current = HandleCast(current);
                    break;

                default:
                    break;
            }

        }

        SetNewOutput(current);
    }

    private void SetNewOutput(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            _outgoingConnections[0].NodeOutputBase.Name = originData.DataName;
            outputs[0].DefaultValue = null;
        }
        else
        {
            for (int i = 0; i < _outgoingConnections.Length; i++)
            {
                outputs[i].DefaultValue = newValue;
                _outgoingConnections[i].NodeOutputBase.Name = newValue;
            }
        }

        PropagateNewOutput();
    }

    private void PropagateNewOutput()
    {
        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            if (_outgoingConnections[i].IsValid == false) continue;

            _outgoingConnections[i].InputNode.CheckNewOutput();
        }
    }
  

    #region Operations Logic
    private string HandleAdd(string a,string b)
    {
        switch (currentType)
        {
            default:
                return null;
            case DataType.None:
                return null;
            case DataType.Var:
                return null;         

            case DataType.Int:
                return IntAdd(a, b);

            case DataType.Float:
                return FloatAdd(a,b);

            case DataType.Bool:
                return null;

            case DataType.String:
                return StringConcat(a,b);
        }
    }
    private string HandleSubtract(string a,string b)
    {
        switch (currentType)
        {
            default:
                return null;
            case DataType.None:
                return null;
            case DataType.Var:
                return null;

            case DataType.Int:
                return IntSubtract(a, b);

            case DataType.Float:
                return FloatSubtract(a, b);

            case DataType.Bool:
                return null;

            case DataType.String:
                return null;
        }
    }
    private string HandleMultiply(string a,string b)
    {
        switch (currentType)
        {
            default:
                return null;
            case DataType.None:
                return null;
            case DataType.Var:
                return null;

            case DataType.Int:
                return IntMultiply(a, b);

            case DataType.Float:
                return FloatMultiply(a, b);

            case DataType.Bool:
                return null;

            case DataType.String:
                return null;
        }
    }
    private string HandleDivide(string a,string b)
    {
        switch (currentType)
        {
            default:
                return null;
            case DataType.None:
                return null;
            case DataType.Var:
                return null;

            case DataType.Int:
                return IntDivide(a, b);

            case DataType.Float:
                return FloatDivide(a, b);

            case DataType.Bool:
                return null;

            case DataType.String:
                return null;
        }
    }
    private string HandleCast(string a)
    {
        return null;
    }

    private string IntAdd(string a, string b)
    {
        string result;

        int.TryParse(a, out int a_int);
        int.TryParse(b, out int b_int);

        result = (a_int + b_int).ToString();

        return result;
    }
    private string IntSubtract(string a, string b)
    {
        string result;

        int.TryParse(a, out int a_int);
        int.TryParse(b, out int b_int);

        result = (a_int - b_int).ToString();

        return result;
    }
    private string IntMultiply(string a, string b)
    {
        string result;

        int.TryParse(a, out int a_int);
        int.TryParse(b, out int b_int);

        result = (a_int * b_int).ToString();

        return result;
    }
    private string IntDivide(string a, string b)
    {
        string result = null;

        int.TryParse(a, out int a_int);
        int.TryParse(b, out int b_int);


        if (a_int > 0 && b_int > 0)
            result = (a_int / b_int).ToString();

        return result;
    }

    private string FloatAdd(string a, string b)
    {
        string result;

        float.TryParse(a, out float a_float);
        float.TryParse(b, out float b_float);

        result = (a_float + b_float).ToString();

        return result;
    }
    private string FloatSubtract(string a, string b)
    {
        string result;

        float.TryParse(a, out float a_float);
        float.TryParse(b, out float b_float);

        result = (a_float - b_float).ToString();

        return result;
    }
    private string FloatMultiply(string a, string b)
    {
        string result;

        float.TryParse(a, out float a_float);
        float.TryParse(b, out float b_float);

        result = (a_float * b_float).ToString();

        return result;
    }
    private string FloatDivide(string a, string b)
    {
        string result;

        float.TryParse(a, out float a_float);
        float.TryParse(b, out float b_float);

        result = (a_float / b_float).ToString();

        return result;
    }


    private string StringConcat(string a, string b)
    {
        return a + b;
    }
    #endregion
}