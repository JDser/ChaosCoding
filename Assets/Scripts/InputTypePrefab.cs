using UnityEngine;

public enum DataType
{
    None = -1,
    Int,
    Float,
    Bool,
    String,
    Var,
}

public enum MathOperation
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Cast,
}

[System.Serializable]
public class InputStruct
{
    [SerializeField] InputTypePrefab _prefab;
    [SerializeField] string _value;


    public string ValueName
    {
        get => _prefab.ValueName;
    }
    public string Value
    {
        get => _value;
        set => _value = value;
    }
    public DataType Type 
    {
        get => _prefab.InputType;
    }
    public Color Color
    {
        get => _prefab.Color;
    }
    public Color SecondColor
    {
        get => _prefab.SecondColor;
    }

    public bool IsValid
    {
        get => _prefab != null;
    }

}

[CreateAssetMenu(fileName = "New Input Type Prefab")]
public class InputTypePrefab : ScriptableObject
{
    [SerializeField] DataType _type;
    [SerializeField] string _valueName;
    [SerializeField] Color _color;
    [SerializeField] Color _secondColor;

    public DataType InputType { get => _type; }
    public string ValueName { get => _valueName; }
    public Color Color { get => _color; }
    public Color SecondColor { get => _secondColor; }
}