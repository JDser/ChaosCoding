using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager :MonoBehaviour
{
    //#region Static Wrappers
    //public static ProgressManager Instance { get; private set; }

    //public static int GetStartNumber()
    //{
    //    return Instance._currentData.CurrentNumber;
    //}
    //public static int GetEndNumber()
    //{
    //    return Instance._currentData.NextNumber;
    //}
    //public static void NextSequence()
    //{
    //    Instance._currentData.IncreaseNumber();
    //}
    //#endregion

    //[SerializeField] int numberOfSequences;
    //[SerializeField] Vector2Int minMaxRange;
    //
    //[SerializeField] Progress _currentData;
    //
    //
    //
    //[SerializeField] bool ignoreLoad;
    //private void Awake()
    //{
    //    Instance = this;
    //
    //    //Random.InitState(1);
    //
    //    if (ignoreLoad)
    //    {
    //        _currentData = new Progress(numberOfSequences, minMaxRange.x, minMaxRange.y);
    //        return;
    //    }
    //
    //
    //
    //    Progress data = SaveLoadHelper.LoadData();
    //    if (data != null)
    //        _currentData = data;
    //    else
    //        _currentData = new Progress(numberOfSequences, minMaxRange.x, minMaxRange.y);
    //}
}