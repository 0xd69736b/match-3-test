using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class CoroutineRunner : MonoBehaviour
    {
        private HashSet<IEnumerator> runningRoutines = new HashSet<IEnumerator>(10);


        public Coroutine RunCoroutine(IEnumerator coroutine)
        {
            Coroutine routine = StartCoroutine(Run(coroutine));
            runningRoutines.Add(coroutine);
            return routine;
        }

        private void OnDestroy()
        {
            foreach(var r in runningRoutines)
            {
                StopCoroutine(r);
            }

            runningRoutines.Clear();
        }


        private IEnumerator Run(IEnumerator coroutine)
        {
            yield return coroutine;
            runningRoutines.Remove(coroutine);
        }
    }
}