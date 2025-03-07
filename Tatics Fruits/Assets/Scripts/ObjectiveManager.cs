using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ObjectiveManager : MonoBehaviour
    {
        public Slider _progressBar;
        public ScoreManager _scoreManager;
        public GameObject[] _stars;

        private readonly int[] _starThresholds = { 25, 50, 100 };

        private void Start()
        {
            UpdateProgress();
        }

        private void Update()
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (_scoreManager == null || _progressBar == null)
                return;

            var score = _scoreManager.GetScore();
            var fillAmount = 0f;

            if (score < 25)
                fillAmount = (float)score / 25f * 0.2f;
            else if (score < 50)
                fillAmount = 0.2f + ((score - 25f) / 25f * 0.3f);
            else if (score < 100)
                fillAmount = 0.5f + ((score - 50f) / 50f * 0.5f);
            else
                fillAmount = 1f;

            _progressBar.DOValue(fillAmount, 0.5f);
            ActivateStars(score);
        }

        private void ActivateStars(int score)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                if (i < _starThresholds.Length && score >= _starThresholds[i])
                {
                    if (!_stars[i].activeSelf)
                    {
                        _stars[i].SetActive(true);
                        AnimateStar(_stars[i]); // Chama a animação
                    }
                }
                else
                {
                    _stars[i].SetActive(false);
                }
            }
        }

        private void AnimateStar(GameObject star)
        {
            var image = star.GetComponent<Image>();

            if (image == null) return;

            // Animação de rotação suave
            star.transform.DORotate(new Vector3(0, 0, 15), 0.3f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            // Efeito de brilho (fade in/out)
            image.DOFade(1f, 0.2f).From(0.5f).SetLoops(2, LoopType.Yoyo);
        }
    }
}
