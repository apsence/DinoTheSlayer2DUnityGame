using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    public Queue<IEnumerator> speakerQueue = new Queue<IEnumerator>();
    private bool isProcessing = false;

    public void AddCoroutine(IEnumerator coroutine)
    {
        speakerQueue.Enqueue(coroutine);
        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;
        while (speakerQueue.Count > 0)
        {
            IEnumerator coroutine = speakerQueue.Dequeue();
            Debug.Log("Запуск корутины: " + coroutine);
            yield return StartCoroutine(coroutine);
        }
        isProcessing = false;
    }
}
