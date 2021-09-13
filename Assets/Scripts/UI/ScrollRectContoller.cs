using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectContoller : MonoBehaviour
{
    [SerializeField] Vector2 minMaxZoom = new Vector2(0.5f, 1.5f);

    Transform scrollRectContent;

    float _scroll;
    float _amount;
    float _size;

    private void Awake()
    {
        scrollRectContent = transform.GetChild(0);
        _amount = 0.5f;
    }

    void Update()
    {
        Debug.LogError("!");

        _scroll = Input.mouseScrollDelta.y;
        if (_scroll != 0)
        {
            _amount += Time.deltaTime * _scroll;
            _amount = Mathf.Clamp(_amount, 0, 1);

            _size = Mathf.Lerp(minMaxZoom.x, minMaxZoom.y, _amount);
            scrollRectContent.localScale = new Vector3(_size, _size, _size);
        }
    }
}