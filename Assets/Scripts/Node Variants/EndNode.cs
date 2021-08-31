using UnityEngine;
using System.Collections;

public class EndNode : NodeBase
{
    public delegate void CheckHandler(bool result);
    public event CheckHandler OnCheckEnd;

    public override void Enact()
    {
        StopAllCoroutines();

        for (int i = 0; i < _incomingConnections.Length; i++)
        {
            if (_incomingConnections[i].IsValid == false)
            {
                LevelManager.PlaySound(deniedClip);
                StartCoroutine(FailRoutine());

                OnCheckEnd?.Invoke(false);
                return;
            }
            else if (_incomingConnections[i].OutputStruct.DefaultValue != inputs[i].DefaultValue)
            {
                LevelManager.PlaySound(deniedClip);
                StartCoroutine(FailRoutine());

                OnCheckEnd?.Invoke(false);
                return;
            }
        }

        LevelManager.PlaySound(confirmClip);
        StartCoroutine(ShakeAnimationRoutine());

        OnCheckEnd?.Invoke(true);
    }

    public override void CheckNewOutput() { }
}