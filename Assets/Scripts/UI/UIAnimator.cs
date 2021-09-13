using System.Collections;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    #region Variables

    [SerializeField] AnimationCurve sizeCurve;
    [SerializeField] float sizeChangeSpeed;  
    
    [SerializeField] AnimationCurve shakeCurve;
    [SerializeField] float shakeChangeSpeed;

    RectTransform rectTransform;
    #endregion

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    #region Size Animation
    public void SizeAnimation()
    {
        StartCoroutine(SizeAnimationRoutine());
    }
    private IEnumerator SizeAnimationRoutine()
    {
        float _progress = 0;
        float _amount;

        Vector3 prevScale = rectTransform.localScale;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * sizeChangeSpeed;
            _amount = sizeCurve.Evaluate(_progress);
            rectTransform.localScale = prevScale * _amount;

            yield return null;
        }
    } 
    
    public void SizeAnimation(AnimationCurve curve)
    {
        StartCoroutine(SizeAnimationRoutine(curve));
    }
    private IEnumerator SizeAnimationRoutine(AnimationCurve curve)
    {
        float _progress = 0;
        float _amount;

        Vector3 prevScale = rectTransform.localScale;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * sizeChangeSpeed;
            _amount = curve.Evaluate(_progress);
            rectTransform.localScale = prevScale * _amount;

            yield return null;
        }
    }  
    
    public void SizeAnimation(float speed)
    {
        StartCoroutine(SizeAnimationRoutine(speed));
    }
    private IEnumerator SizeAnimationRoutine(float speed)
    {
        float _progress = 0;
        float _amount;

        Vector3 prevScale = rectTransform.localScale;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * speed;
            _amount = sizeCurve.Evaluate(_progress);
            rectTransform.localScale = prevScale * _amount;

            yield return null;
        }
    }  
    
    public void SizeAnimation(AnimationCurve curve,float speed)
    {
        StartCoroutine(SizeAnimationRoutine(curve, speed));
    }
    private IEnumerator SizeAnimationRoutine(AnimationCurve curve,float speed)
    {
        float _progress = 0;
        float _amount;

        Vector3 prevScale = rectTransform.localScale;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * speed;
            _amount = curve.Evaluate(_progress);
            rectTransform.localScale = prevScale * _amount;

            yield return null;
        }
    }
    #endregion

    #region Shake Animation
    public void ShakeAnimation()
    {
        StartCoroutine(ShakeAnimationRoutine());
    }
    private IEnumerator ShakeAnimationRoutine()
    {
        float _progress = 0;
        float zAmount;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * 5;

            zAmount = Mathf.Sin(_progress * 50) * 2.5f;

            rectTransform.rotation = Quaternion.Euler(0, 0, zAmount);

            yield return null;
        }
    } 
    
    public void ShakeAnimation(AnimationCurve curve)
    {
        StartCoroutine(ShakeAnimationRoutine(curve));
    }
    private IEnumerator ShakeAnimationRoutine(AnimationCurve curve)
    {
        float _progress = 0;
        float _amount;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * shakeChangeSpeed;

            _amount = curve.Evaluate(_progress);

            rectTransform.rotation = Quaternion.Euler(0, 0, _amount);

            yield return null;
        }
    }

    public void ShakeAnimation(float speed)
    {
        StartCoroutine(ShakeAnimationRoutine(speed));
    }
    private IEnumerator ShakeAnimationRoutine(float speed)
    {
        float _progress = 0;
        float _amount;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * speed;

            _amount = shakeCurve.Evaluate(_progress);

            rectTransform.rotation = Quaternion.Euler(0, 0, _amount);

            yield return null;
        }
    }

    public void ShakeAnimation(AnimationCurve curve,float speed)
    {
        StartCoroutine(ShakeAnimationRoutine(curve, speed));
    }
    private IEnumerator ShakeAnimationRoutine(AnimationCurve curve, float speed)
    {
        float _progress = 0;
        float _amount;

        while (_progress < 1)
        {
            _progress += Time.deltaTime * speed;

            _amount = curve.Evaluate(_progress);

            rectTransform.rotation = Quaternion.Euler(0, 0, _amount);

            yield return null;
        }
    }  
    #endregion
}