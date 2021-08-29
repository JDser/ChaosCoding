using UnityEngine;
using System.Collections;

public class EndNode : NodeBase
{
    public delegate void CheckHandler(bool result);
    public event CheckHandler OnCheckEnd;

    public override void Enact()
    {
        for (int i = 0; i < incomingConnections.Length; i++)
        {
            if (incomingConnections[i] == null)
            {
                StartCoroutine(FailRoutine());
                OnCheckEnd?.Invoke(false);
                return;
            }

            NodeConnection currentConnection = incomingConnections[i];
            if (currentConnection.OutputNode.Outputs[currentConnection.OutputIndex].Value != inputs[i].Value)
            {
                StartCoroutine(FailRoutine());
                OnCheckEnd?.Invoke(false);
                return;
            }
        }

        LevelManager.PlaySound(clip);
        StopAllCoroutines();
        StartCoroutine(ShakeAnimationRoutine());

        OnCheckEnd?.Invoke(true);
    }

    public override void CheckNewOutput() { }

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


    private IEnumerator FailRoutine()
    {
        float _progress = 0;
        float zAmount;

        while (_progress < Mathf.PI)
        {
            _progress += Time.deltaTime * 20;
            zAmount = -(Mathf.Sin(_progress) * 0.2f - 1);
            dragRectTransform.localScale = new Vector3(zAmount, zAmount, zAmount);

            yield return null;
        }

        dragRectTransform.localScale = Vector3.one;
    }

}