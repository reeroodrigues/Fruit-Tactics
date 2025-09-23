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
    [SerializeField] private RectTransform settingsIcon;
    
    [Header("Ranking")]
    [SerializeField] private GameObject rankingPanel;
    
    [Header("Store")]
    [SerializeField] private GameObject storePanel;
    [SerializeField] private RectTransform storeRoot;
    [SerializeField] private CanvasGroup storeCanvasGroup;
    
    [Header("DailyMissions")]
    [SerializeField] private GameObject dailyMissionsPanel;
    [SerializeField] private RectTransform dailyMissionsRoot;
    [SerializeField] private CanvasGroup dailyMissionsCanvasGroup;

    [Header("Animação de tilt")]
    [SerializeField] private float initialDelay = 1.5f;
    [SerializeField] private float repeatInterval = 3f;
    [SerializeField] private float tiltAngle = 12f;
    [SerializeField] private float tiltDuration = 0.18f;
    [SerializeField] private float returnDuration = 0.22f;
    [SerializeField] private float iconsStagger = 0.07f;

    private Sequence _titleSeq;

    private void Start()
    {
        titleTransform.localScale = Vector3.zero;
        _titleSeq = DOTween.Sequence()
            .Append(titleTransform.DOScale(1f, 0.8f).SetEase(Ease.OutBounce));
        
        playButton.onClick.AddListener(() => SceneManager.LoadScene("Gameplay Scene"));
        rankingButton.onClick.AddListener(() =>
        {
            if (!rankingPanel) return;
            rankingPanel.SetActive(true);
        });
        
        storeButton.onClick.AddListener(OpenStorePanel);

        dailyMissionsButton.onClick.AddListener(() =>
            {
                dailyMissionsPanel.SetActive(true);
            });
        
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
        CancelInvoke(nameof(NudgeAllOnce));
        InvokeRepeating(nameof(NudgeAllOnce), 0.25f, repeatInterval);
    }

    private void NudgeAllOnce()
    {
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

    private void OpenStorePanel()
    {
        if (!storePanel) return;
        
        storePanel.SetActive(true);
        if (storeRoot) storeRoot.localScale = Vector3.one * 0.85f;

        if (!storeCanvasGroup && storeRoot)
            storeCanvasGroup = storeRoot.GetComponent<CanvasGroup>();

        if (storeCanvasGroup)
        {
            storeCanvasGroup.alpha = 0f;
            storeCanvasGroup.interactable = false;
            storeCanvasGroup.blocksRaycasts = true;
            DOTween.Kill(storeCanvasGroup);
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
