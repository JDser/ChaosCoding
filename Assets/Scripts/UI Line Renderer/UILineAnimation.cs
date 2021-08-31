using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILineAnimation : UILineRenderer
{
    #region Variables
    [Header("Animation")] 
    [SerializeField] RectTransform rect_Image;
    [SerializeField] float moveSpeed;

    Vector3 desiredPosition;

    GameObject rect_imageObject;
    Image _image;

    public delegate void AnimationHandler();
    public event AnimationHandler OnAnimationEnd;

    public Color SecondColor 
    { 
        set 
        {
            _image.color = value;
        }
    }

    #endregion

    #region BuiltIn Methods
    protected override void Awake()
    {
        base.Awake();

         rect_imageObject = rect_Image.gameObject;
        _image = rect_Image.GetComponent<Image>();

        rect_imageObject.SetActive(false);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        
        rect_imageObject = rect_Image.gameObject;
        _image = rect_Image.GetComponent<Image>();
    
        rect_imageObject.SetActive(false);
    }
#endif


    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
    }
    #endregion

    public void StartAnimation()
    {
        rect_Image.gameObject.SetActive(true);

        StartCoroutine(MoveRectRoutine());
    }

    IEnumerator MoveRectRoutine()
    {
        float _progress = 0;

        while (_progress<1f)
        {        
            float stepNormalize = Normalize(_progress, 0, 1);
            desiredPosition.x = Mathf.Lerp(0, end.x, stepNormalize);

            desiredPosition.y = end.y * curve.Evaluate(stepNormalize);

            rect_Image.anchoredPosition = desiredPosition;

            _progress += Time.deltaTime * moveSpeed;

            yield return null;
        }

        rect_Image.gameObject.SetActive(false);
        OnAnimationEnd?.Invoke();
    }
}
