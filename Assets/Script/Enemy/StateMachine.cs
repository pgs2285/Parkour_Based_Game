using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    #region variables
    private T context;
    private State<T> currentState;
    private State<T> previousState;
    private float elapsedTimeInState = 0.0f;
    private Dictionary<System.Type, State<T>> states = new Dictionary<System.Type, State<T>>(); // States들을 모두 저장하는 Dictionary

    #endregion

    #region getter
    public State<T> CurrentState => currentState;
    public State<T> PreviousState => previousState;
    public float ElapsedTimeInState => elapsedTimeInState;

    #endregion


    public StateMachine(T context, State<T> initialState)
    {
        this.context = context;
        AddState(initialState);
        currentState = initialState;
        currentState.OnEnter();

    }

    public void AddState(State<T> state)
    {
        state.SetStateMachineAndContext(this, context);
        states[state.GetType()] = state;
    }

    public void Update(float deltaTime)
    {
        elapsedTimeInState += deltaTime;
        currentState.Update(deltaTime);
    }
    public R ChangeState<R>() where R : State<T>
    {
        var newType = typeof(R);
        if(currentState.GetType() == newType)
        {
            return currentState as R;
        }

        if(currentState!=null)
        {
            currentState.OnExit();
        }
        previousState = currentState;
        currentState = states[newType];
        currentState.OnEnter();
        elapsedTimeInState = 0.0f;

        return currentState as R;
    }
 }

public abstract class State<T>
{
    protected StateMachine<T> stateMachine;
    protected T context;
    public State()
    {

    }

    internal void SetStateMachineAndContext(StateMachine<T> stateMachine, T context)
    {
        this.stateMachine = stateMachine;
        this.context = context;

       
    }
    public virtual void OnInitialiezed() {}
    public virtual void OnEnter() { }
    public abstract void Update(float deltaTime);
    public virtual void OnExit() { }
}