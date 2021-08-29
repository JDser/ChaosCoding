using UnityEngine;
using System.Collections;

public class MathNode : NodeBase
{
    [Header("Custom")]
    [SerializeField] InputStruct originVar;
    [SerializeField] MathOperation mathOperation;

    UILineAnimation[] lineAnims;

    DataType currentType;

    public int ValidIncomes
    {
        get
        {
            int validIncomes = 0;
            for (int i = 0; i < incomingConnections.Length; i++)
            {
                if (incomingConnections[i] != null)
                    validIncomes++;
            }
            return validIncomes;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        SetupLineAnimators();
    }
    private void SetupLineAnimators()
    {
        lineAnims = new UILineAnimation[_createdOutputs.Length];

        for (int i = 0; i < _createdOutputs.Length; i++)
        {
            lineAnims[i] = (UILineAnimation)_createdOutputs[i].LineRenderer;
            lineAnims[i].SecondColor = _createdOutputs[i].InputType.SecondColor;

            lineAnims[i].OnAnimationEnd += Anim_OnAnimationEnd;
        }
    }


    private void Anim_OnAnimationEnd()
    {
        for (int i = 0; i < outgoingConnections.Length; i++)
        {
            if (outgoingConnections[i] != null)
                outgoingConnections[i].InputNode.Enact();
        }
    }

    public override void Enact()
    {
        LevelManager.PlaySound(clip);
        StopAllCoroutines();
        StartCoroutine(ShakeAnimationRoutine());

        for (int i = 0; i < outgoingConnections.Length; i++)
        {
            if (outgoingConnections[i] == null)
                return;

            lineAnims[i].StartAnimation();
        }
    }
    private IEnumerator ShakeAnimationRoutine()
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


    protected override void AddInputConnection(NodeConnection connection)
    {
        base.AddInputConnection(connection);


        if (string.IsNullOrEmpty(connection.OutputStruct.Value) == false)
            (_createdInputs[connection.InputIndex] as MathNodeInput).ValueName = connection.OutputStruct.Value;
        else
            (_createdInputs[connection.InputIndex] as MathNodeInput).ValueName = connection.OutputStruct.ValueName;


        CheckNewOutput();
    }

    protected override void RemoveInputConnection(int index)
    {
        base.RemoveInputConnection(index);

        if (originVar.IsValid)
            (_createdInputs[index] as MathNodeInput).ValueName = originVar.ValueName;

        ResetConnection();
        CheckNewOutput();
    }
    public override void RemoveOutputConnection(int outputIndex)
    {
        base.RemoveOutputConnection(outputIndex);

        ResetConnection();
    }


    private void ResetConnection()
    {
        for (int i = 0; i < incomingConnections.Length; i++)
        {
            if (incomingConnections[i] != null)
                return;
        }
        for (int i = 0; i < outgoingConnections.Length; i++)
        {
            if (outgoingConnections[i] != null)
                return;
        }


        if (originVar.IsValid == false)
        {
            for (int i = 0; i < _createdInputs.Length; i++)
            {
                (_createdInputs[i] as MathNodeInput).ValueName = inputs[i].ValueName;
            }
            for (int i = 0; i < _createdOutputs.Length; i++)
            {
                (_createdOutputs[i] as MathNodeInput).ValueName = outputs[i].ValueName;
            }
            return;
        }
        else
        {
            currentType = originVar.Type;

            for (int i = 0; i < _createdInputs.Length; i++)
            {
                _createdInputs[i].ChangeInput(originVar);
                (_createdInputs[i] as MathNodeInput).ValueName = originVar.ValueName;
            }
            for (int i = 0; i < _createdOutputs.Length; i++)
            {
                _createdOutputs[i].ChangeInput(originVar);
                (_createdOutputs[i] as MathNodeInput).ValueName = originVar.ValueName;
            }
        }
    }

    public void ChangeDataType(InputStruct type)
    {
        if (mathOperation == MathOperation.Cast) return;


        currentType = type.Type;

        for (int i = 0; i < _createdInputs.Length; i++)
        {
            _createdInputs[i].ChangeInput(type);
        }
        for (int i = 0; i < _createdOutputs.Length; i++)
        {
            _createdOutputs[i].ChangeInput(type);
        }
    }

    public override void CheckNewOutput()
    {
        if (_createdOutputs.Length > 1)
            Debug.LogError("More than 1 output!", this);

        if (mathOperation == MathOperation.Cast)
        {
            if (incomingConnections[0] == null)
                return;

            string _out = incomingConnections[0].OutputStruct.Value;
            SetNewOutput(_out);
            return;
        }

        if (ValidIncomes < 2)
        {
            for (int i = 0; i < incomingConnections.Length; i++)
            {
                if (incomingConnections[i] == null) continue;
                SetNewOutput(incomingConnections[i].OutputStruct.Value);
                return;
            }

            SetNewOutput(null);
            SetNewOutputIgnorePropagation(originVar.ValueName);
        }
        else
        {
            string current = null;

            if (incomingConnections[0] != null)
                current = incomingConnections[0].OutputStruct.Value;

            for (int i = 1; i < incomingConnections.Length; i++)
            {
                if (incomingConnections[i] == null)
                    continue;

                string next = incomingConnections[i].OutputStruct.Value;


                switch (mathOperation)
                {
                    case MathOperation.Add:
                        current = HandleAdd(current,next);
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

                    default:
                        break;
                }

            }
            SetNewOutput(current);
        }
    }

    private void SetNewOutput(string newValue)
    {
        outputs[0].Value = (_createdOutputs[0] as MathNodeInput).ValueName = newValue;
        PropagateNewOutput();
    }
    
    private void SetNewOutputIgnorePropagation(string newValue)
    {
        outputs[0].Value = (_createdOutputs[0] as MathNodeInput).ValueName = newValue;
    }

    private void PropagateNewOutput()
    {
        for (int i = 0; i < incomingConnections.Length; i++)
        {
            if (incomingConnections[i] != null)
            {
                if (incomingConnections[i].InputNode.HasReferenceTo(this)) return;

                if (string.IsNullOrEmpty(incomingConnections[i].OutputStruct.Value) == false)
                    (_createdInputs[i] as MathNodeInput).ValueName = incomingConnections[i].OutputStruct.Value;
                else
                    (_createdInputs[i] as MathNodeInput).ValueName = incomingConnections[i].OutputStruct.ValueName;
            }
        }

        for (int i = 0; i < outgoingConnections.Length; i++)
        {
            if (outgoingConnections[i] != null)
                outgoingConnections[i].InputNode.CheckNewOutput();
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
        return a + ' ' + b;
    }
    #endregion
}