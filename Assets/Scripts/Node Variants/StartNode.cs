using UnityEngine;
using System.Collections;

public class StartNode : NodeBase
{
    UILineAnimation[] lineAnims;

    protected override void Awake()
    {
        base.Awake();

        SetupLineAnimators();
    }

    private void SetupLineAnimators()
    {
        lineAnims = new UILineAnimation[_outgoingConnections.Length];

        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            NodeConnection current = _outgoingConnections[i];

            lineAnims[i] = (UILineAnimation)current.NodeOutputBase.LineRenderer;
            lineAnims[i].SecondColor = current.NodeOutputBase.InputType.SecondColor;

            lineAnims[i].OnAnimationEnd += OnAnimationEnd;
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

            lineAnims[i].StartAnimation();
        }

        LevelManager.PlaySound(confirmClip);
        StartCoroutine(ShakeAnimationRoutine());
    }

    public override void CheckNewOutput() { }

    private void OnAnimationEnd()
    {
        for (int i = 0; i < _outgoingConnections.Length; i++)
        {
            if (_outgoingConnections[i].IsValid)
            {
                _outgoingConnections[i].InputNode.Enact();
            }
        }
    }
}