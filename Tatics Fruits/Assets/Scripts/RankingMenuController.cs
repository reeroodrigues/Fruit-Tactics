using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RankingMenuController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform rankingPanel; // RectTransform do painel
    [SerializeField] private GameObject dimmer;          // opcional: painel escuro para bloquear cliques
    [SerializeField] private Button backButton;          // botão X

    [Header("Anim")]
    [SerializeField, Min(0.05f)] private float duration = 0.25f;
    [SerializeField] private Ease easeIn = Ease.OutCubic;
    [SerializeField] private Ease easeOut = Ease.InCubic;

    [Header("Posicionamento")]
    [Tooltip("Altura final a partir do rodapé (px). Ex.: 260 = painel para 260px acima da borda inferior.")]
    [SerializeField] private float openOffsetY = 260f;

    [Tooltip("Quanto passar da borda inferior ao esconder (px). Aumente se algo ainda ficar visível.")]
    [SerializeField] private float hideOvershootY = 150f;

    private float _panelHeight;
    private float _hiddenY;  // posição escondida (abaixo da tela, com overshoot)
    private bool _isOpen;
    private Tweener _tween;
    private bool _ready;

    void Awake()
    {
        Prepare();
        SetHiddenInstant();

        if (dimmer != null) dimmer.SetActive(false);
        if (backButton) backButton.onClick.AddListener(Close);

        // Dimmer fecha ao clicar
        if (dimmer != null)
        {
            var btn = dimmer.GetComponent<Button>();
            if (!btn) btn = dimmer.AddComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(Close);
        }
    }

    // Calcula medidas e posições
    private void Prepare()
    {
        if (_ready) return;
        if (!rankingPanel) rankingPanel = GetComponent<RectTransform>();

        // Anchors recomendados: Bottom-Stretch (minY=maxY=0) e Pivot Y = 0
        Canvas.ForceUpdateCanvases();
        _panelHeight = rankingPanel.rect.height;

        // Escondido: abaixo da tela + overshoot para garantir que nada apareça
        _hiddenY = -(_panelHeight + hideOvershootY);

        _ready = true;
    }

    private void SetHiddenInstant()
    {
        var pos = rankingPanel.anchoredPosition;
        pos.y = _hiddenY;
        rankingPanel.anchoredPosition = pos;
    }

    public void Toggle() { if (_isOpen) Close(); else Open(); }

    public void Open()
    {
        Prepare();
        if (_tween != null && _tween.IsActive()) _tween.Kill();

        if (dimmer != null)
        {
            dimmer.SetActive(true);
            dimmer.transform.SetAsFirstSibling();
        }
        rankingPanel.SetAsLastSibling();

        _isOpen = true;
        _tween = rankingPanel.DOAnchorPosY(openOffsetY, duration)
                             .SetEase(easeIn)
                             .SetUpdate(true);
    }

    public void Close()
    {
        Prepare();
        if (_tween != null && _tween.IsActive()) _tween.Kill();

        _isOpen = false;
        _tween = rankingPanel.DOAnchorPosY(_hiddenY, duration)
                             .SetEase(easeOut)
                             .SetUpdate(true)
                             .OnComplete(() =>
                             {
                                 if (dimmer != null) dimmer.SetActive(false);
                             });
    }

#if UNITY_ANDROID || UNITY_STANDALONE
    void Update()
    {
        if (_isOpen && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }
#endif

    [ContextMenu("Recalc Now")]
    public void RecalcNow()
    {
        _ready = false;
        Prepare();
        SetHiddenInstant();
    }
}
