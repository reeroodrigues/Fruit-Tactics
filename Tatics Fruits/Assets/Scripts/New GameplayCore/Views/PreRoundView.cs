using System;
using System.Collections;
using New_GameplayCore.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace New_GameplayCore.Views
{
    public class PreRoundView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image fadeBlocker;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI objectiveText;
        
        [Header("Stars")]
        [SerializeField] private Image star1;

        [Header("Buttons")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button nextButton;
        
        private IPreRoundPresenter _presenter;
        private PreRoundModel _model;

        public void Bind(IPreRoundPresenter presenter, PreRoundModel model)
        {
            _presenter = presenter;
            _model = model;

            if (titleText)
                titleText.text = string.IsNullOrEmpty(model.displayName) ? $"Fase {model.levelId}" : model.displayName;
            
            if (objectiveText)
                objectiveText.text = $"Faça <b>{model.targetScore}</b> pontos em <b>{model.initialTimeSec}</b> segundos para ganhar as <b>3</b> estrelas!";

            SetStar(star1, false);

            if (mainMenuButton) { mainMenuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenu");
            });}
            if (nextButton)     { nextButton.onClick.RemoveAllListeners();     nextButton.onClick.AddListener(OnStart); }
            
            gameObject.SetActive(true);
            StartCoroutine(FadeCanvas(0f, 1f, 0.2f));
        }

        private void OnStart()
        {
            StartCoroutine(CloseThen(_presenter.OnStartClicked));
        }

        private IEnumerator CloseThen(Action callback)
        {
            yield return FadeCanvas(1f, 0f, 0.15f);
            callback?.Invoke();
            Destroy(gameObject);
        }

        private IEnumerator FadeCanvas(float from, float to, float dur)
        {
            if(!canvasGroup)
                yield break;
            
            canvasGroup.alpha = from;

            if (fadeBlocker)
                fadeBlocker.raycastTarget = true;

            var t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, t / dur);
                yield return null;
            }
            canvasGroup.alpha = to;
            if (fadeBlocker)
                fadeBlocker.raycastTarget = to > 0.99f;
        }

        private void SetStar(Image img, bool on)
        {
            if(!img)
                return;

            img.enabled = true;
        }
    }
}