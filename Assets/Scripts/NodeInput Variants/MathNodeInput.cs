using UnityEngine;
using UnityEngine.EventSystems;

public class MathNodeInput : NodeInputBase
{
    //MathNode parent;
    //
    //UILineAnimation lineAnimation;
    //
    //public string ValueName
    //{
    //    set
    //    {
    //        text_Type.text = value;
    //    }
    //}
    //
    //protected override void Awake()
    //{
    //    base.Awake();
    //    parent = (MathNode)_parentNode;
    //    lineAnimation = (UILineAnimation)(lineRenderer);
    //}
    //
    ////public override void ChangeInputType(InputData type)
    ////{
    ////    _inputType = type;
    ////
    ////    lineAnimation.color = _inputType.Color;
    ////    lineAnimation.SecondColor = _inputType.SecondColor;
    ////
    ////    image.color = _inputType.Color;
    ////}
    //
    //public override void ConnectNode(NodeInputBase inputNode)
    //{
    //    if (_inputType.Type == DataType.Var || _inputType.Type == inputNode.InputType.Type)
    //    {
    //        //parent.ChangeDataType(inputNode.InputType);
    //
    //        lineRenderer.Target = inputNode.LineRenderer.rectTransform;
    //        _parentNode.AddOutputConnection(inputNode.ParentNode, inputNode.NodeIndex, _nodeIndex);
    //        LevelManager.PlaySound(connectSuccessClip);
    //        return;
    //    }
    //    else
    //    {
    //        lineRenderer.End = Vector2.zero;
    //        LevelManager.PlaySound(connectFailedClip);
    //        return;
    //    }
    //}
    //
    //public override void OnDrop(PointerEventData eventData)
    //{
    //    if (_isOutput) return;
    //
    //    if (eventData.pointerDrag == null)
    //        return;
    //
    //    NodeInputBase original = eventData.pointerDrag.GetComponent<NodeInputBase>(); // first selected nodeInput
    //
    //    if (original == null) return;
    //
    //    if (original.ParentNode == _parentNode) return;
    //
    //    if (_inputType.Type == DataType.Var || _inputType.Type == original.InputType.Type)
    //    {
    //        //parent.ChangeDataType(original.InputType);
    //        original.ConnectNode(this);
    //    }
    //}
}