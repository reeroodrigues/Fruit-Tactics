using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Botões")]
    [SerializeField] private Button playButton;      // Iniciar
    [SerializeField] private Button rankingButton;   // Ranking
    [SerializeField] private Button shopButton;      // Loja
    [SerializeField] private Button dailyButton;     // Missões Diárias
    [SerializeField] private Button optionsButton;   // Engrenagem (Settings)

    [Header("Painéis / Navegação")]
    [SerializeField] private Transform titleTransform;               // "TACTICS OF FRUITS"
    [SerializeField] private RectTransform[] animatedButtonsOrder;   // ordem de entrada (play, ranking, loja, daily)
    [SerializeField] private string gameplaySceneName = "Gameplay Scene";

    [Header("Ranking (desliza de baixo)")]
    [SerializeField] private RankingMenuController rankingMenu;      // controlador do Ranking (pode estar desativado)
    [SerializeField] private GameObject rankingRoot;                  // fallback: painel simples do Ranking (sem animação)

    [Header("Opções (Settings)")]
    [SerializeField] private SettingsMenu settingsMenuCtrl; // controlador que "desce" o Settings
    [SerializeField] private GameObject settingsRoot;                 // fallback: painel simples do Settings (sem animação)

    [Header("Outros painéis (opcional)")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject dailyPanel;

    [Header("Rótulos")]
    [SerializeField] private TextMeshProUGUI versionLabel;

    [Header("Animação")]
    [SerializeField, Min(0.1f)] private float titlePopDuration = 0.8f;
    [SerializeField] private Ease titleEase = Ease.OutBounce;
    [SerializeField, Min(0f)] private float buttonsStagger = 0.06f;
    [SerializeField, Range(0.5f, 1.1f)] private float idleScale = 0.95f;
    [SerializeField, Range(0.5f, 1.3f)] private float hoverScale = 1.00f;
    [SerializeField, Range(0.5f, 1.3f)] private float pressScale = 0.92f;
    [SerializeField, Min(0.05f)] private float hoverDuration = 0.15f;
    [SerializeField, Min(0.05f)] private float pressDuration = 0.08f;

    void Start()
    {
        if (versionLabel) versionLabel.text = Application.version;

        if (titleTransform)
        {
            titleTransform.localScale = Vector3.zero;
            titleTransform.DOScale(1f, titlePopDuration).SetEase(titleEase);
        }

        if (playButton)    { SetupButtonAnimation(playButton);    playButton.onClick.AddListener(StartGame); }
        if (rankingButton) { SetupButtonAnimation(rankingButton); rankingButton.onClick.AddListener(OpenRanking); }
        if (shopButton)    { SetupButtonAnimation(shopButton);    shopButton.onClick.AddListener(OpenShop); }
        if (dailyButton)   { SetupButtonAnimation(dailyButton);   dailyButton.onClick.AddListener(OpenDaily); }
        if (optionsButton) { SetupButtonAnimation(optionsButton); optionsButton.onClick.AddListener(OpenOptions); }

        // entrada sequencial dos botões
        if (animatedButtonsOrder != null && animatedButtonsOrder.Length > 0)
        {
            for (int i = 0; i < animatedButtonsOrder.Length; i++)
            {
                var t = animatedButtonsOrder[i];
                if (!t) continue;
                t.localScale = Vector3.zero;
                t.DOScale(idleScale, 0.25f).SetEase(Ease.OutBack).SetDelay(i * buttonsStagger);
            }
        }
    }

    void OnDisable()
    {
        DOTween.Kill(titleTransform);
        DOTween.Kill(playButton?.transform);
        DOTween.Kill(rankingButton?.transform);
        DOTween.Kill(shopButton?.transform);
        DOTween.Kill(dailyButton?.transform);
        DOTween.Kill(optionsButton?.transform);
        if (animatedButtonsOrder != null)
            foreach (var t in animatedButtonsOrder) if (t) DOTween.Kill(t);
    }

    // ---------- animações hover/press ----------
    private void SetupButtonAnimation(Button button)
    {
        if (!button) return;

        button.transform.localScale = Vector3.one * idleScale;

        var trigger = button.GetComponent<EventTrigger>();
        if (!trigger) trigger = button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>(); // evita duplicar

        AddTrigger(trigger, EventTriggerType.PointerEnter, _ =>
        {
            button.transform.DOScale(hoverScale, hoverDuration);
        });

        AddTrigger(trigger, EventTriggerType.PointerExit, _ =>
        {
            button.transform.DOScale(idleScale, hoverDuration);
        });

        AddTrigger(trigger, EventTriggerType.PointerDown, _ =>
        {
            button.transform.DOScale(pressScale, pressDuration);
        });

        AddTrigger(trigger, EventTriggerType.PointerUp, _ =>
        {
            button.transform.DOScale(hoverScale, hoverDuration);
        });
    }

    private void AddTrigger(EventTrigger trigger, EventTriggerType type, System.Action<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(action));
        trigger.triggers.Add(entry);
    }

    // ---------- ações ----------
    private void StartGame()
    {
        if (!string.IsNullOrEmpty(gameplaySceneName))
            SceneManager.LoadScene(gameplaySceneName);
        else
            Debug.LogWarning("[MainMenu] gameplaySceneName vazio.");
    }

    private void OpenRanking()
    {
        // Preferir o controlador com animação
        if (rankingMenu != null)
        {
            // se estiver desativado no início, ative antes
            if (!rankingMenu.gameObject.activeSelf) rankingMenu.gameObject.SetActive(true);
            rankingMenu.Open();
            return;
        }

        // Fallback: painel simples
        if (rankingRoot != null)
        {
            rankingRoot.SetActive(true);
            // anima um pop-in básico
            var t = rankingRoot.transform;
            t.localScale = Vector3.one * 0.9f;
            t.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
            return;
        }

        Debug.LogWarning("[MainMenu] Nenhum Ranking ligado. Preencha 'rankingMenu' ou 'rankingRoot'.");
    }

    private void OpenShop()
    {
        if (shopPanel) shopPanel.SetActive(true);
        else Debug.Log("[MainMenu] Abrir Loja: arraste um painel em 'shopPanel'.");
    }

    private void OpenDaily()
    {
        if (dailyPanel) dailyPanel.SetActive(true);
        else Debug.Log("[MainMenu] Abrir Missões Diárias: arraste um painel em 'dailyPanel'.");
    }

    private void OpenOptions()
    {
        // Preferir o controlador que desce o Settings
        if (settingsMenuCtrl != null)
        {
            if (!settingsMenuCtrl.gameObject.activeSelf) settingsMenuCtrl.gameObject.SetActive(true);
            settingsMenuCtrl.Open();
            return;
        }

        // Fallback: painel simples
        if (settingsRoot != null)
        {
            settingsRoot.SetActive(true);
            var t = settingsRoot.transform;
            t.localScale = Vector3.one * 0.9f;
            t.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            return;
        }

        Debug.LogWarning("[MainMenu] Nenhum Settings ligado. Preencha 'settingsMenuCtrl' ou 'settingsRoot'.");
    }
}
