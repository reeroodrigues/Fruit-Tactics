using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button rankingButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button dailyMissionsButton;
    [SerializeField] private Button settingsButton;

    [Header("Title")]
    [SerializeField] private RectTransform titleTransform;

    [Header("Icons ao lado dos botões")]
    [SerializeField] private RectTransform[] sideIcons;

    [Header("Settings (janela/painel)")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private RectTransform settingsIcon; // opcional

    [Header("Animação de tilt")]
    [SerializeField] private float initialDelay = 1.5f;   // 1ª vez
    [SerializeField] private float repeatInterval = 3f;   // repete a cada 3s
    [SerializeField] private float tiltAngle = 12f;
    [SerializeField] private float tiltDuration = 0.18f;
    [SerializeField] private float returnDuration = 0.22f;
    [SerializeField] private float iconsStagger = 0.07f;

    private Sequence _titleSeq;

    private void Start()
    {
        // Pop do título
        titleTransform.localScale = Vector3.zero;
        _titleSeq = DOTween.Sequence()
            .Append(titleTransform.DOScale(1f, 0.8f).SetEase(Ease.OutBounce));

        // Clicks
        playButton.onClick.AddListener(() => SceneManager.LoadScene("Gameplay Scene"));
        rankingButton.onClick.AddListener(() => Debug.Log("Abrindo Ranking..."));
        storeButton.onClick.AddListener(() => Debug.Log("Abrindo Loja..."));
        dailyMissionsButton.onClick.AddListener(() => Debug.Log("Abrindo Daily Missions..."));
        settingsButton.onClick.AddListener(OpenSettings);

        // Dispara e agenda repetição
        InvokeRepeating(nameof(NudgeAllOnce), initialDelay, repeatInterval);
    }

    private void OnDisable()
    {
        _titleSeq?.Kill();
        CancelInvoke(nameof(NudgeAllOnce));
        if (sideIcons != null)
        {
            foreach (var icon in sideIcons)
            {
                if (!icon) continue;
                icon.DOKill();
                icon.localRotation = Quaternion.identity;
                icon.localScale = Vector3.one;
            }
        }
        if (settingsIcon)
        {
            settingsIcon.DOKill();
            settingsIcon.localRotation = Quaternion.identity;
            settingsIcon.localScale = Vector3.one;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) return;
        // reinicia o agendamento ao voltar para o menu
        CancelInvoke(nameof(NudgeAllOnce));
        InvokeRepeating(nameof(NudgeAllOnce), 0.25f, repeatInterval);
    }

    private void NudgeAllOnce()
    {
        // Ícones laterais
        if (sideIcons != null)
        {
            for (int i = 0; i < sideIcons.Length; i++)
            {
                var icon = sideIcons[i];
                if (!icon) continue;

                var seq = DOTween.Sequence().SetDelay(i * iconsStagger);
                seq.Append(icon.DOLocalRotate(new Vector3(0, 0, tiltAngle), tiltDuration)
                                .SetEase(Ease.OutQuad));
                seq.Append(icon.DOLocalRotate(Vector3.zero, returnDuration)
                                .SetEase(Ease.InOutQuad));
                seq.Join(icon.DOScale(1.03f, tiltDuration + returnDuration)
                             .SetEase(Ease.InOutSine))
                   .Append(icon.DOScale(1f, 0.08f));
            }
        }

        // Settings (ícone ou botão)
        if (settingsIcon)
        {
            DOTween.Sequence()
                .Append(settingsIcon.DOLocalRotate(new Vector3(0, 0, -tiltAngle), tiltDuration)
                                     .SetEase(Ease.OutQuad))
                .Append(settingsIcon.DOLocalRotate(Vector3.zero, returnDuration)
                                     .SetEase(Ease.InOutQuad));
        }
        else if (settingsButton)
        {
            var t = settingsButton.transform;
            DOTween.Sequence()
                .Append(t.DOLocalRotate(new Vector3(0, 0, -tiltAngle), tiltDuration)
                               .SetEase(Ease.OutQuad))
                .Append(t.DOLocalRotate(Vector3.zero, returnDuration)
                               .SetEase(Ease.InOutQuad));
        }
    }

    private void OpenSettings()
    {
        if (!settingsPanel) return;
        settingsPanel.SetActive(true);
        var rt = settingsPanel.transform as RectTransform;
        if (!rt) return;

        var cg = settingsPanel.GetComponent<CanvasGroup>();
        if (cg) cg.alpha = 0f;

        rt.localScale = Vector3.one * 0.92f;
        DOTween.Sequence()
            .Append(rt.DOScale(1f, 0.25f).SetEase(Ease.OutBack))
            .Join(cg ? cg.DOFade(1f, 0.2f) : null);
    }
}
