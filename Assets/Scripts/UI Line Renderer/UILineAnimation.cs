using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UILineAnimation : UILineRenderer
{
    #region Variables
    [Header("Animation")]
    [SerializeField] float frequency;
    [SerializeField] float moveSpeed;

    Vector3 desiredPosition;

    List<MovablePoint> movablePoints;

    float _nextTime = 0f;

    public delegate void AnimationHandler();
    public event AnimationHandler OnAnimationEnd;

    public Color SecondColor 
    { 
        set 
        {
            //_image.color = value;
        }
    }
    public bool Animate
    {
        get;
        set;
    }
    #endregion

    #region BuiltIn Methods
    protected override void Awake()
    {
        base.Awake();

        movablePoints = new List<MovablePoint>();

        for (int i = 0; i < frequency; i++)
        {
            movablePoints.Add(GameManager.GetPoint());
        }

    }

    protected override void Update()
    {
        base.Update();

        if (Animate)
        {
            if(Time.time > _nextTime)
            {
                _nextTime = Time.time + (1/frequency);
                for (int i = 0; i < movablePoints.Count; i++)
                {
                   // movablePoints[i].Position = points[0];
                }
            }


        }

    }

    protected override void OnDestroy()
    {
        foreach (MovablePoint _m in movablePoints)
        {
            GameManager.ReturnPoint(_m);
        }

        base.OnDestroy();
        StopAllCoroutines();
    }
    #endregion

    //public void StartAnimation()
    //{
    //    rect_Image.gameObject.SetActive(true);
    //
    //    StartCoroutine(MoveRectRoutine());
    //}
    //
    //IEnumerator MoveRectRoutine()
    //{
    //    float _progress = 0;
    //
    //    while (_progress<1f)
    //    {        
    //        float stepNormalize = Normalize(_progress, 0, 1);
    //        desiredPosition.x = Mathf.Lerp(0, end.x, stepNormalize);
    //
    //        //desiredPosition.y = end.y * curve.Evaluate(stepNormalize);
    //
    //        rect_Image.anchoredPosition = desiredPosition;
    //
    //        _progress += Time.deltaTime * moveSpeed;
    //
    //        yield return null;
    //    }
    //
    //    rect_Image.gameObject.SetActive(false);
    //    OnAnimationEnd?.Invoke();
    //}
}
