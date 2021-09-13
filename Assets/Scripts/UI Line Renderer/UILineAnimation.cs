using System.Collections.Generic;
using UnityEngine;

struct PointProgress
{
    public MovablePoint Point
    {
        get;
        private set;
    }
    public float Progress
    {
        get;
        set;
    }
    public bool IsDone
    {
        get => Progress >= 1f;
    }

    public PointProgress(MovablePoint m)
    {
        Point = m;
        Progress = 0;
    }

    public PointProgress(MovablePoint m, float currentProgress)
    {
        Point = m;
        Progress = currentProgress;
    }
}

public class UILineAnimation : UILineRenderer
{
    #region Variables
    [Header("Animation")]
    [SerializeField] float frequency;
    [SerializeField] float moveSpeed;

    Queue<PointProgress> movablePoints;
    List<PointProgress> ActiveMovablePoints;

    #region NonSerialized
    float _nextTime = 0f;

    bool _isAnimating = false;

    bool _startEventTriggered = false;
    bool _endEventTriggered = false;

    bool _onFirstPointArrive = false;
    bool _onLastPointArrive = false;

    float xDiff, yDiff;
    float currentProgress;
    Vector3 pos;

    #endregion

    #region Events
    public delegate void AnimationHandler();
    public event AnimationHandler OnAnimationStart; 
    public event AnimationHandler OnAnimationEnd;

    public event AnimationHandler OnFirstPoint;
    public event AnimationHandler OnLastPoint;
    #endregion

    #region Properties
    /// <summary>
    ///  NO USE!
    /// </summary>
    public Color SecondColor 
    { 
        set 
        {
            //_image.color = value;
        }
    }
    public bool Animate
    {
        get => _isAnimating;
        set => _isAnimating = value;
    }
    #endregion

    #endregion

    #region BuiltIn Methods
    protected override void Update()
    {
        base.Update();
        Animation();
    }

    protected override void OnDestroy()
    {
        if (movablePoints != null && movablePoints.Count !=0)
        {
            for (int i = 0; i < movablePoints.Count; i++)
            {
                GameManager.ReturnPoint(movablePoints.Dequeue().Point);
            }
        }
        if (ActiveMovablePoints != null && ActiveMovablePoints.Count != 0)
        {
            for (int i = 0; i < ActiveMovablePoints.Count; i++)
            {
                GameManager.ReturnPoint(ActiveMovablePoints[i].Point);
            }
        }

        base.OnDestroy();
    }
    #endregion

    private void Animation()
    {
        if (_isAnimating)
        {
            if (!_startEventTriggered)
            {
                _startEventTriggered = true;
                OnAnimationStart?.Invoke();
            }

            if (ActiveMovablePoints.Count < frequency)
            {
                if (Time.time > _nextTime)
                {
                    _nextTime = Time.time + (1 / frequency);

                    if (movablePoints.Count == 0)
                    {
                        PointProgress _point = new PointProgress(GameManager.GetPoint(), 0);

                        _point.Point.gameObject.SetActive(true);

                        _point.Point.transform.SetParent(transform);
                        _point.Point.transform.localPosition = Vector3.zero;
                        _point.Point.transform.localScale = Vector3.one;

                        ActiveMovablePoints.Add(_point);
                    }
                    else
                    {
                        PointProgress _point = movablePoints.Dequeue();

                        _point.Point.gameObject.SetActive(true);
                        _point.Point.transform.localPosition = Vector3.zero;
                        _point.Point.transform.localScale = Vector3.one;

                        ActiveMovablePoints.Add(_point);
                    }
                }
            }

            xDiff = end.x;
            yDiff = end.y;
            pos = Vector3.zero;

            for (int i = 0; i < ActiveMovablePoints.Count; i++)
            {
                if (ActiveMovablePoints[i].IsDone)
                {
                    ActiveMovablePoints[i] = new PointProgress(ActiveMovablePoints[i].Point);

                    if (_onFirstPointArrive == false)
                    {
                        _onFirstPointArrive = true;
                        OnFirstPoint?.Invoke();
                    }
                }

                currentProgress = Normalize(ActiveMovablePoints[i].Progress + Time.deltaTime * moveSpeed, 0, 1);

                if (xDiff < rectTransform.anchoredPosition.x)
                {
                    pos.x = Mathf.Lerp(0, end.x, currentProgress) * horizontalCurve.Evaluate(currentProgress);
                    pos.y = yDiff * verticalCurve.Evaluate(currentProgress);
                }
                else
                {
                    pos.x = Mathf.Lerp(0, end.x, currentProgress);
                    pos.y = yDiff * verticalCurve.Evaluate(currentProgress);
                }

                ActiveMovablePoints[i].Point.Position = pos;
                ActiveMovablePoints[i] = new PointProgress(ActiveMovablePoints[i].Point, currentProgress);
            }
        }
        else if (ActiveMovablePoints!=null && ActiveMovablePoints.Count > 0)
        {
            xDiff = end.x;
            yDiff = end.y;
            pos = Vector3.zero;

            for (int i = 0; i < ActiveMovablePoints.Count; i++)
            {
                if (ActiveMovablePoints[i].IsDone)
                {
                    ActiveMovablePoints[i].Point.gameObject.SetActive(false);

                    movablePoints.Enqueue(new PointProgress(ActiveMovablePoints[i].Point));

                    ActiveMovablePoints.RemoveAt(i);

                    if (!_endEventTriggered)
                    {
                        _endEventTriggered = true;
                        OnAnimationEnd?.Invoke();
                    }

                    if (_onLastPointArrive == false && ActiveMovablePoints.Count == 0)
                    {
                        _onLastPointArrive = true;
                        OnLastPoint?.Invoke();

                        ResetAnim();
                        return;
                    }

                    continue;
                }

                currentProgress = Normalize(ActiveMovablePoints[i].Progress + Time.deltaTime * moveSpeed, 0, 1);

                if (xDiff < rectTransform.anchoredPosition.x)
                {
                    pos.x = Mathf.Lerp(0, end.x, currentProgress) * horizontalCurve.Evaluate(currentProgress);
                    pos.y = yDiff * verticalCurve.Evaluate(currentProgress);
                }
                else
                {
                    pos.x = Mathf.Lerp(0, end.x, currentProgress);
                    pos.y = yDiff * verticalCurve.Evaluate(currentProgress);
                }

                ActiveMovablePoints[i].Point.Position = pos;
                ActiveMovablePoints[i] = new PointProgress(ActiveMovablePoints[i].Point, currentProgress);
            }
        }
    }

    public void ResetAnim()
    {
        ActiveMovablePoints.Clear();

        _isAnimating = false;

        _startEventTriggered = false;
        _endEventTriggered = false;

        _onFirstPointArrive = false;
        _onLastPointArrive = false;

        currentProgress = 0;
    }

    public void PoolPoints()
    {
        movablePoints = new Queue<PointProgress>();
        ActiveMovablePoints = new List<PointProgress>();

        for (int i = 0; i < frequency; i++)
        {
            MovablePoint _m = GameManager.GetPoint();

            movablePoints.Enqueue(new PointProgress(_m));
            _m.transform.SetParent(transform);

            _m.transform.localPosition = Vector3.zero;
            _m.transform.localScale = Vector3.one;
        }
    }
}
