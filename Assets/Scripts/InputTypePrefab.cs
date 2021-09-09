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
public class InputData
{
    [SerializeField] InputTypePrefab _prefab;
    [SerializeField] string _defaultValue;

    #region Properties
    public string DataName
    {
        get => _prefab.DataName;
    }
    public string DefaultValue
    {
        get => _defaultValue;
        set => _defaultValue = value;
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
    #endregion

    public InputData(string __value,InputTypePrefab type)
    {
        _defaultValue = __value;
        _prefab = type;
    }

}

[CreateAssetMenu(fileName = "New Input Type Prefab")]
public class InputTypePrefab : ScriptableObject
{
    [SerializeField] DataType _type;
    [SerializeField] string _dataName;
    [SerializeField] Color _color;
    [SerializeField] Color _secondColor;

    public DataType InputType { get => _type; }
    public string DataName { get => _dataName; }
    public Color Color { get => _color; }
    public Color SecondColor { get => _secondColor; }
}