using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    private Coroutine _inputing;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
        if (_inputing != null)
        {
            StopCoroutine(_inputing);
        }
        _inputing = StartCoroutine(Inputing());
    }

    private IEnumerator Inputing()
    {
        while (true)
        {
            InputManager.Instance.OnInput?.Invoke(Direction);
            yield return null;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
        StopCoroutine(_inputing);
        InputManager.Instance.OnInput?.Invoke(Vector2.zero);
    }
}