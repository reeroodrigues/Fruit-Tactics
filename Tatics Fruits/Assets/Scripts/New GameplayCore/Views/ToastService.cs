using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace New_GameplayCore.Views
{
    public class ToastService : MonoBehaviour
    {
        public static ToastService Instance;
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private GameObject toastPrefab;
        [SerializeField] private float showTime = 1.6f;
        
        private readonly Queue<string> _queue = new();
        [SerializeField] private bool showing = false;

        private void Awake()
        {
            Instance = this;
        }

        public static void Show(string msg)
        {
            if(Instance == null)
                return;
            
            Instance._queue.Enqueue(msg);

            if (!Instance.showing)
                Instance.StartCoroutine(Instance.Run());
        }

        private IEnumerator Run()
        {
            showing = true;
            while (_queue.Count > 0)
            {
                var go = Instantiate(toastPrefab, rootCanvas.transform);
                var cg = go.GetComponent<CanvasGroup>();
                var txt = go.GetComponent<TextMeshProUGUI>();
                if(txt)
                    txt.text = _queue.Dequeue();
                
                cg.alpha = 0;
                float t = 0;
                while (t < 0.15f)
                {
                    t += Time.deltaTime;
                    cg.alpha = t / 0.15f;
                    yield return null;
                }
                cg.alpha = 1;

                yield return new WaitForSeconds(showTime);

                t = 0;
                while (t < 0.2f)
                {
                    t += Time.deltaTime;
                    cg.alpha = 1 -(t/ 0.2f);
                    yield return null;
                }
                Destroy(go);
            }
            showing = false;
        }
    }
}