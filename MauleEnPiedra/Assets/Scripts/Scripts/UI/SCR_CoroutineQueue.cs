using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CoroutineQueue : MonoBehaviour
{
    [SerializeField] private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
    [SerializeField] private bool isRunning = false;

    public void Enqueue(IEnumerator coroutine)
    {
        coroutineQueue.Enqueue(coroutine);
        if (!isRunning)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isRunning = true;

        while (coroutineQueue.Count > 0)
        {
            yield return StartCoroutine(coroutineQueue.Dequeue());
        }

        isRunning = false;
    }
}