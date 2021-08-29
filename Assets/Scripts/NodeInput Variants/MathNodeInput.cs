using UnityEngine;
using UnityEngine.EventSystems;

public class MathNodeInput : NodeInputBase
{
    MathNode parent;

    UILineAnimation lineAnimation;

    public string ValueName
    {
        set
        {
            text_Type.text = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        parent = (MathNode)parentNode;
        lineAnimation = (UILineAnimation)(lineRenderer);
    }

    public override void ChangeInput(InputStruct type)
    {
        inputType = type;

        lineAnimation.color = inputType.Color;
        lineAnimation.SecondColor = inputType.SecondColor;

        image.color = inputType.Color;
    }

    public override void ConnectNode(NodeInputBase inputNode)
    {
        if (inputType.Type == DataType.Var || inputType.Type == inputNode.InputType.Type)
        {
            parent.ChangeDataType(inputNode.InputType);

            lineRenderer.Target = inputNode.LineRenderer.rectTransform;
            parentNode.AddOutputConnection(inputNode.ParentNode, inputNode.NodeIndex, nodeIndex);
            LevelManager.PlaySound(connectSuccessClip);
            return;
        }
        else
        {
            lineRenderer.End = Vector2.zero;
            LevelManager.PlaySound(connectFailedClip);
            return;
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (_isOutput) return;

        if (eventData.pointerDrag == null)
            return;

        NodeInputBase original = eventData.pointerDrag.GetComponent<NodeInputBase>(); // first selected nodeInput

        if (original == null) return;

        if (original.ParentNode == parentNode) return;

        if (inputType.Type == DataType.Var || inputType.Type == original.InputType.Type)
        {
            parent.ChangeDataType(original.InputType);
            original.ConnectNode(this);
        }
    }

}