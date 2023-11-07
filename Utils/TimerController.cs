using System;
using JoostenProductions;
using UnityEngine;

public class TimerController : BaseController
{
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }
    
    private readonly Action _action;
    private readonly bool _isRepetitive;
    private float _delay;
    private float _currentTime;

    public TimerController(Action action, bool isRepetitive, float delay = 0f) 
    {
        _action = action;
        _isRepetitive = isRepetitive;
        _delay = delay;
    }

    public void SetDelayAndResetCurrentTime(float delay)
    {
        _delay = delay;
        _currentTime = 0f;
    }
    
    public void StartWithAction()
    {
        if (_delay == 0 || IsRunning)
            DebugTimer();         
        
        _action?.Invoke();
        IsRunning = true;
        UpdateManager.SubscribeToUpdate(RunTimerOnUpdate);
    }
    
    public void StartWithSetDelay()
    {
        if (_delay == 0 || IsRunning)
            DebugTimer();      
        
        IsRunning = true;
        UpdateManager.SubscribeToUpdate(RunTimerOnUpdate);
    }
    
    public void StartWithCustomDelay(float delay)
    {
        if (_delay == 0 || IsRunning)
            DebugTimer();      
        
        var timer = new TimerController(StartWithAction, false, delay);
        AddChildController(timer);
        timer.StartWithSetDelay();
        IsRunning = true;
    }

    public void Pause() => IsPaused = true;
    public void Continue() => IsPaused = false;
    public void ResetCurrentTime() => _currentTime = 0;

    public void StopAndReset()
    { 
        IsRunning = false;
        IsPaused = false;
        _currentTime = 0;
        UpdateManager.UnsubscribeFromUpdate(RunTimerOnUpdate);
    }

    private void RunTimerOnUpdate()
    {
        if(!IsRunning || IsPaused) return;
        _currentTime += Time.deltaTime;
        
        if (_currentTime < _delay) return;
        _action?.Invoke();
        if (_isRepetitive)
            ResetCurrentTime();
        else
            StopAndReset();
    }

    private void DebugTimer()
    {
        var stackTrace = new System.Diagnostics.StackTrace(); 
        Debug.LogError($"Timer Issue: _delay - {_delay}, IsActive - {IsRunning}. " +
                       $"{System.Reflection.MethodBase.GetCurrentMethod().Name} was called by {stackTrace.GetFrame(1).GetMethod().Name} in {stackTrace.GetFrame(1).GetMethod().DeclaringType}");
    }

    protected override void OnDispose()
    {
        UpdateManager.UnsubscribeFromUpdate(RunTimerOnUpdate);
        base.OnDispose();
    }
}