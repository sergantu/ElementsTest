using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CodeBase.Utils
{
    /// Author: Pim de Witte (pimdewitte.com) and contributors
    /// <summary>
    /// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
    /// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
    /// </summary>
    /// <inheritdoc cref="MonoBehaviour"/>
    ///
    ///     ///
    [PublicAPI]
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<IEnumerator> _executionQueue = new Queue<IEnumerator>();
        private static UnityMainThreadDispatcher _instance;

        /// <summary>
        /// Locks the queue and adds the IEnumerator to the queue
        /// </summary>
        /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
        public static void Enqueue(IEnumerator action)
        {
            lock (_executionQueue) {
                _executionQueue.Enqueue(action);
            }
        }

        /// <summary>
        /// Locks the queue and adds the Action to the queue
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        public static void Enqueue(Action action)
        {
            Enqueue(_instance.ActionWrapper(action));
        }

        public static void EnqueueNextFrame(Action action)
        {
            Enqueue(_instance.NextFrameActionWrapper(action));
        }

        /// <summary>
        /// This ensures that there's exactly one UnityMainThreadDispatcher in every scene, so the singleton will exist no matter which scene you play from.
        /// </summary>
        public static void AddDispatcherToScene()
        {
            var dispatcherContainer = new GameObject("UnityMainThreadDispatcher");
            GameObject appObject = GameObject.Find("_app_");
            if (appObject != null) {
                dispatcherContainer.transform.SetParent(appObject.transform);
            } else {
                DontDestroyOnLoad(dispatcherContainer);
            }
            dispatcherContainer.AddComponent<UnityMainThreadDispatcher>();
        }

        private void Awake()
        {
            if (_instance != null) {
                Destroy(gameObject);
            } else {
                _instance = this;
            }
        }

        private void Update()
        {
            lock (_executionQueue) {
                while (_executionQueue.Count > 0) {
                    StartCoroutine(_executionQueue.Dequeue());
                }
            }
        }

        private IEnumerator ActionWrapper(Action a)
        {
            a();
            yield return null;
        }

        private IEnumerator NextFrameActionWrapper(Action a)
        {
            yield return null;
            a();
        }
    }
}