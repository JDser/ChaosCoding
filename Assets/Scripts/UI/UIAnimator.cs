using System.Collections;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ChangeSize()
    {
        Debug.LogError("Add Parameters");
        StartCoroutine(ChangeSizeRoutine());
    }

    private IEnumerator ChangeSizeRoutine()
    {
        float _progress = 0;
        float zAmount;

        Vector3 prevScale = rectTransform.localScale;

        while (_progress < Mathf.PI)
        {
            _progress += Time.deltaTime * 20;
            zAmount = -(Mathf.Sin(_progress) * 0.2f - 1);
            rectTransform.localScale = new Vector3(zAmount, zAmount, zAmount);

            yield return null;
        }

        rectTransform.localScale = prevScale;
    }

    public void Shake()
    {
        Debug.LogError("Add Parameters");
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
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
}