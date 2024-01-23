using System.Collections;
using System.Collections.Generic;
using UI.Mission;
using UnityEngine;
using UnityEngine.Events;

public class SimpleButtonEventModule : MonoBehaviour
{
    [SerializeField]
    private SimpleButtonView _view;
    private SimpleButtonController _controller;

    public UnityEvent OnClicked;
    // Start is called before the first frame update
    void Start()
    {
        if(_view == null)
        {
            TryGetComponent(out _view);
        }

        _controller = new SimpleButtonController(_view, OnClicked.Invoke);
    }
}
