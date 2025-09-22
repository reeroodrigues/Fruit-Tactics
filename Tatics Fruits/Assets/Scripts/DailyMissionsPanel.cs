using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyMissionsPanelTabs : MonoBehaviour
{
    [Header("Logic")]
    [SerializeField] private DailyMissionsController controller;
    [SerializeField] private PlayerProfileController profile;

    [Header("Root / Anim")]
    [SerializeField] private CanvasGroup panelCanvasGroup;   // CanvasGroup no root
    [SerializeField] private RectTransform window;           // janela (para scale)
    [SerializeField] private Button closeButton;             // botão X (fecha)
    [SerializeField] private Button dimButton;               // opcional: botão no Dim para fechar ao tocar fora

    [Header("Tabs (buttons)")]
    [SerializeField] private Button missionsTabButton;       // "Missões diárias"
    [SerializeField] private Button bonusTabButton;          // "Bônus diário"
    [SerializeField] private Image missionsTabBg;            // opcional: bg do botão (pra destacar seleção)
    [SerializeField] private Image bonusTabBg;               // opcional: bg do botão
    [SerializeField] private Color tabSelected = Color.white;
    [SerializeField] private Color tabUnselected = new Color(1,1,1,0.5f);

    [Header("Panels (contents)")]
    [SerializeField] private GameObject missionsPanelRoot;   // seu "Content - DailyMission" (ou equivalente)
    [SerializeField] private CanvasGroup missionsPanelCg;    // CanvasGroup do painel de missões (opcional)
    [SerializeField] private GameObject bonusPanelRoot;      // seu "Content - DailyBonus" (ou equivalente)
    [SerializeField] private CanvasGroup bonusPanelCg;       // CanvasGroup do painel de login (opcional)

    [Header("Missions List")]
    [SerializeField] private Transform missionsParent;       // onde instanciar os itens
    [SerializeField] private DailyMissionItemView missionItemPrefab;

    [Header("Daily Login UI")]
    [SerializeField] private Button dailyLoginButton;        // botão "Coletar"
    [SerializeField] private TextMeshProUGUI dailyLoginText; // "Coletar +50 Gold"
    [SerializeField] private GameObject dailyLoginBadge;     // etiqueta "Disponível"

    [Header("Defaults")]
    [SerializeField] private bool startOnMissions = true;    // abre com Missões por padrão

    private bool _missionsBuilt;
    private enum Tab { Missions, Bonus }
    private Tab _current;

    // ---------- Lifecycle ----------

    private void Awake()
    {
        if (closeButton) closeButton.onClick.AddListener(Hide);
        if (dimButton)   dimButton.onClick.AddListener(Hide);

        missionsTabButton.onClick.AddListener(() => SwitchTo(Tab.Missions));
        bonusTabButton.onClick.AddListener(() => SwitchTo(Tab.Bonus));

        if (dailyLoginButton) dailyLoginButton.onClick.AddListener(OnClickClaimLogin);
    }

    private void OnEnable()
    {
        // garante set do dia e estado inicial
        controller.EnsureDayGenerated();

        if (startOnMissions) SwitchTo(Tab.Missions, instant:true);
        else                 SwitchTo(Tab.Bonus,    instant:true);

        // animação de abertura
        Show();
    }

    private void OnDisable()
    {
        DOTween.Kill(panelCanvasGroup);
        DOTween.Kill(window);
        if (missionsPanelCg) DOTween.Kill(missionsPanelCg);
        if (bonusPanelCg)    DOTween.Kill(bonusPanelCg);
    }

    // ---------- Public Show/Hide ----------

    public void Show()
    {
        gameObject.SetActive(true);

        if (panelCanvasGroup)
        {
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = true;
            panelCanvasGroup.alpha = 0f;
        }
        if (window) window.localScale = Vector3.one * 0.9f;

        DOTween.Sequence()
            .Append(panelCanvasGroup ? panelCanvasGroup.DOFade(1f, 0.18f) : null)
            .Join(window ? window.DOScale(1f, 0.22f).SetEase(Ease.OutBack) : null)
            .OnComplete(() => { if (panelCanvasGroup) panelCanvasGroup.interactable = true; });
    }

    public void Hide()
    {
        if (panelCanvasGroup) panelCanvasGroup.interactable = false;
        DOTween.Sequence()
            .Append(panelCanvasGroup ? panelCanvasGroup.DOFade(0f, 0.15f) : null)
            .Join(window ? window.DOScale(0.94f, 0.15f).SetEase(Ease.InSine) : null)
            .OnComplete(() => gameObject.SetActive(false));
    }

    // ---------- Tabs ----------

    private void SwitchTo(Tab tab, bool instant = false)
    {
        _current = tab;

        // Ativa/Desativa roots
        missionsPanelRoot.SetActive(tab == Tab.Missions);
        bonusPanelRoot.SetActive(tab == Tab.Bonus);

        // Fade suave entre painéis (se CanvasGroup estiver atribuído)
        if (!instant)
        {
            if (missionsPanelCg) missionsPanelCg.alpha = (tab == Tab.Missions) ? 0f : 1f;
            if (bonusPanelCg)    bonusPanelCg.alpha    = (tab == Tab.Bonus)    ? 0f : 1f;

            if (tab == Tab.Missions && missionsPanelCg)
                missionsPanelCg.DOFade(1f, 0.18f);
            else if (tab == Tab.Bonus && bonusPanelCg)
                bonusPanelCg.DOFade(1f, 0.18f);
        }
        else
        {
            if (missionsPanelCg) missionsPanelCg.alpha = (tab == Tab.Missions) ? 1f : 0f;
            if (bonusPanelCg)    bonusPanelCg.alpha    = (tab == Tab.Bonus)    ? 1f : 0f;
        }

        // Estilo visual dos botões
        if (missionsTabBg) missionsTabBg.color = (tab == Tab.Missions) ? tabSelected : tabUnselected;
        if (bonusTabBg)    bonusTabBg.color    = (tab == Tab.Bonus)    ? tabSelected : tabUnselected;

        // Conteúdo de cada aba
        if (tab == Tab.Missions)
        {
            BuildMissionsIfNeeded();
            RefreshAllMissionItems(); // caso tenha progredido algo enquanto painel estava fechado
        }
        else
        {
            RefreshLogin();
        }
    }

    // ---------- Missões ----------

    private void BuildMissionsIfNeeded()
    {
        if (_missionsBuilt || missionItemPrefab == null || missionsParent == null) return;

        foreach (Transform t in missionsParent) Destroy(t.gameObject);

        var list = controller.GetMissions();
        foreach (var st in list)
        {
            var item = Instantiate(missionItemPrefab, missionsParent);
            item.Setup(controller, st);
        }
        _missionsBuilt = true;
    }

    private void RefreshAllMissionItems()
    {
        // percorre os itens e pede para atualizarem UI
        foreach (Transform t in missionsParent)
        {
            var item = t.GetComponent<DailyMissionItemView>();
            if (item) item.Refresh();
        }
    }

    // ---------- Login diário ----------

    private void RefreshLogin()
    {
        if (dailyLoginText)  dailyLoginText.text = $"Coletar +{profile.Data.Daily.loginRewardGold} Gold";

        bool available = controller.IsDailyLoginAvailable();

        if (dailyLoginButton) dailyLoginButton.interactable = available;
        if (dailyLoginBadge)  dailyLoginBadge.SetActive(available);
    }

    private void OnClickClaimLogin()
    {
        if (controller.TryClaimDailyLogin())
            RefreshLogin();
    }
}
