using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Video;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private VideoPlayer tutorialVideoPlayer;
    [SerializeField] private VideoClip[] _tutorialvideoPlayer;
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private string[] _tutorialTexts;
    [SerializeField] private Sprite[] tutorialSprites;
    [SerializeField] private GameObject _videoContainer;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform dotsContainer;
    [SerializeField] private Sprite dotOnSprite;
    [SerializeField] private Sprite dotOffSprite;

    private int currentIndex = 0;
    private Image[] dots;

    private void Start()
    {
        nextButton.onClick.AddListener(NextSlide);
        prevButton.onClick.AddListener(PreviousSlide);
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));

        CreateDots();
        UpdateTutorialUI();
    }

    private void CreateDots()
    {
        dots = new Image[tutorialSprites.Length];

        for (int i = 0; i < tutorialSprites.Length; i++)
        {
            var dotGO = Instantiate(dotsContainer.GetChild(0).gameObject, dotsContainer);
            dotGO.SetActive(true);
            dots[i] = dotGO.GetComponent<Image>();
        }

        dotsContainer.GetChild(0).gameObject.SetActive(false); // desativa o dot de template
    }

    private void NextSlide()
    {
        if (currentIndex < tutorialSprites.Length - 1)
        {
            currentIndex++;
            UpdateTutorialUI();
        }
    }

    private void PreviousSlide()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateTutorialUI();
        }
    }

    private void UpdateTutorialUI()
    {
        var hasVideo = currentIndex < _tutorialvideoPlayer.Length && _tutorialvideoPlayer[currentIndex] != null;

        if (hasVideo)
        {
            tutorialImage.gameObject.SetActive(false);
            _videoContainer.SetActive(true);

            tutorialVideoPlayer.Stop();
            tutorialVideoPlayer.clip = _tutorialvideoPlayer[currentIndex];
            tutorialVideoPlayer.Play();
        }
        else
        {
            tutorialVideoPlayer.Stop();
            _videoContainer.SetActive(false);
            tutorialImage.gameObject.SetActive(true);

            tutorialImage.DOFade(0f, 0.25f).OnComplete(() =>
            {
                tutorialImage.sprite = tutorialSprites[currentIndex];
                tutorialImage.DOFade(1f, 0.25f);
            });
        }

        if (currentIndex < _tutorialTexts.Length)
        {
            _tutorialText.text = _tutorialTexts[currentIndex];
        }
        
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].sprite = i == currentIndex ? dotOnSprite : dotOffSprite;
        }
        
        prevButton.gameObject.SetActive(currentIndex > 0);
        nextButton.gameObject.SetActive(currentIndex < tutorialSprites.Length - 1);
    }
}
