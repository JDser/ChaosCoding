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
        lineAnims = new UILineAnimation[_createdOutputs.Length];

        for (int i = 0; i < _createdOutputs.Length; i++)
        {
            lineAnims[i] = (UILineAnimation)_createdOutputs[i].LineRenderer;
            lineAnims[i].SecondColor = _createdOutputs[i].InputType.SecondColor;

            lineAnims[i].OnAnimationEnd += Anim_OnAnimationEnd;
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

    public override void CheckNewOutput() { }

    private void Anim_OnAnimationEnd()
    {
        for (int i = 0; i < outgoingConnections.Length; i++)
        {
            if (outgoingConnections[i] != null)
            {
                outgoingConnections[i].InputNode.Enact();
            }
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

}