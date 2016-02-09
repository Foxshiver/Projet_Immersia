using System;
using System.Collections;
using System.Threading;
using UnityEngine;

/// <summary>
/// Executes actions in a different thread than the Unity thread.
/// We use a singleton to avoid having too many threads launched. Only one will be used here
/// <example>
/// <code>
/// SeparateThread(parameter => { /* Action instructions */ parameter(); }, () => Debug.Log("Done"));
/// </code>
/// </example>
/// </summary>
public class SeparateThread : Singleton<SeparateThread>
{
    class ThreadStatus
    {
        public bool isExecuted = false;
    }

    /*
    void Awake()
    {
        instance = this;
    }*/

    /// <summary>
    /// Executes an action in a separate thread and fires a callback once the action has been executed.
    /// </summary>
    /// <example>
    /// <code>
    /// SeparateThread(parameter => { /* Action instructions */ parameter(); }, () => Debug.Log("Done"));
    /// </code>
    /// </example>
    /// <param name="action">The action (the parameter action must be called at the end)</param>
    /// <param name="callback">The callback which is fired</param>
    public void ExecuteInThread(Action<Action> action, Action callback)
    {
        // threadStatus synchronizes/joins the separate thread and the Unity thread.
        ThreadStatus threadStatus = new ThreadStatus();

        // Starts the separate thread.
        Thread thread = new Thread(() => action(() => { threadStatus.isExecuted = true; }));
        thread.Start();

        // Waits for the separate thread to be executed from the Unity thread.
        StartCoroutine(WaitForThreadExecution(threadStatus, callback));
    }


    IEnumerator WaitForThreadExecution(ThreadStatus threadStatus, Action callback)
    {
        while (!threadStatus.isExecuted)
        {
            yield return null;
        }
        callback();
    }
}
