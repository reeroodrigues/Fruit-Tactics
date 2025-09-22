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

    [Header("Badges")]
    [SerializeField] private DailyMissionsController dailyController;
    [SerializeField] private GameObject dailyBadge;

    [Header("Settings (janela/painel)")]
    [SerializeField] private SettingsMenu settingsPanel;
    [SerializeField] private RectTransform settingsIcon; // opcional
    
    [Header("Ranking")]
    [SerializeField] private GameObject rankingPanel;

    // === NOVO: Loja ===
    [Header("Store")]
    [SerializeField] private GameObject storePanel;          // painel da loja (GameObject) — desativado por padrão
    [SerializeField] private RectTransform storeRoot;        // raiz visual da loja (para escalar)
    [SerializeField] private CanvasGroup storeCanvasGroup;   // CanvasGroup para fade
    
    // === NOVO: DailyMissions ===
    [Header("DailyMissions")]
    [SerializeField] private GameObject dailyMissionsPanel;          // painel de dailymissions (GameObject) — desativado por padrão
    [SerializeField] private RectTransform dailyMissionsRoot;        // raiz visual do dailymissions (para escalar)
    [SerializeField] private CanvasGroup dailyMissionsCanvasGroup;   // CanvasGroup para fade

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
        rankingButton.onClick.AddListener(() =>
        {
            if (!rankingPanel) return;
            rankingPanel.SetActive(true); // LeaderboardController abre com animação no OnEnable
        });

        // === NOVO: abrir loja ===
        storeButton.onClick.AddListener(OpenStorePanel);

        dailyMissionsButton.onClick.AddListener(() =>
            {
                dailyMissionsPanel.SetActive(true);
            });
        
        //badge
        if (dailyController)
        {
            dailyController.OnAttentionChanged += (has) =>
            {
                if (dailyBadge)
                    dailyBadge.SetActive(has);
            };
            if (dailyBadge)
            {
                dailyBadge.SetActive(dailyController.HasAnyClaimAvailable());
            }
        }
        
        
        settingsButton.onClick.AddListener(() =>
        {
            if (settingsPanel == null) return;
            settingsPanel.Toggle();
        });

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

    // ===== Loja =====

    private void OpenStorePanel()
    {
        if (!storePanel) return;

        // ativa painel e reseta estado visual
        storePanel.SetActive(true);
        if (storeRoot) storeRoot.localScale = Vector3.one * 0.85f;

        if (!storeCanvasGroup && storeRoot) // fallback: tenta pegar no StoreRoot
            storeCanvasGroup = storeRoot.GetComponent<CanvasGroup>();

        if (storeCanvasGroup)
        {
            storeCanvasGroup.alpha = 0f;
            storeCanvasGroup.interactable = false;
            storeCanvasGroup.blocksRaycasts = true;
            // anima
            DOTween.Kill(storeCanvasGroup); // garante que não empilhe animações
            DOTween.Sequence()
                .Append(storeCanvasGroup.DOFade(1f, 0.18f))
                .Join(storeRoot.DOScale(1f, 0.22f).SetEase(Ease.OutBack))
                .OnComplete(() => storeCanvasGroup.interactable = true);
        }
        else if (storeRoot)
        {
            storeRoot.DOScale(1f, 0.22f).SetEase(Ease.OutBack);
        }
    }

    // Ligue este método no botão "X" da loja (no Inspector)
    public void CloseStorePanel()
    {
        if (!storePanel) return;

        if (storeCanvasGroup && storeRoot)
        {
            storeCanvasGroup.interactable = false;
            DOTween.Sequence()
                .Append(storeCanvasGroup.DOFade(0f, 0.15f))
                .Join(storeRoot.DOScale(0.9f, 0.15f).SetEase(Ease.InSine))
                .OnComplete(() => storePanel.SetActive(false));
        }
        else
        {
            storePanel.SetActive(false);
        }
    }
}
