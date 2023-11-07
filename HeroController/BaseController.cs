using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : IDisposable
{
    private readonly List<GameObject> _gameObjects = new List<GameObject>();
    private readonly List<BaseController> _childControllers = new List<BaseController>();
    private bool _isDisposed;
    
    public void Dispose()
    {
        OnDispose();
        
        foreach (var childController in _childControllers)
            childController.Dispose();
        _childControllers.Clear();

        foreach (var gameObject in _gameObjects)
        {
            if(gameObject == null)
                continue;
            GameObject.Destroy(gameObject);
        }
        
        _gameObjects.Clear();
        _isDisposed = true;
    }

    protected void AddGameObject(GameObject gameObject) => _gameObjects.Add(gameObject);
    
    protected void AddChildController(BaseController childController) => _childControllers.Add(childController);

    protected virtual void OnDispose() { }
}