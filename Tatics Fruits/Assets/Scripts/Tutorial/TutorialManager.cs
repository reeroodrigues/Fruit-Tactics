using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [System.Serializable]
        public enum StepType{ TextOnly, PlayerAction}
        
        [System.Serializable]
        public class TutorialStep
        {
            [TextArea]
            public string tutorialText; // O texto que vai aparecer no balão
            public RectTransform targetUIPosition; // A posição da UI para onde a mão vai apontar
            public Vector2 handOffset = Vector2.zero; // Ajuste fino da posição da mão
            public float handRotation = 0f; // Rotação da mão (em graus)
            public StepType stepType;
        }

        [Header("UI Elements")]
        public GameObject tutorialOverlayPanel; // O painel de fundo escuro
        public Image pointingHandImage; // A imagem da mão apontando (AGORA É UM IMAGE, NÃO RECTTRANSFORM)
        public GameObject speechBubblePanel; // O painel do balão de texto
        public TextMeshProUGUI speechBubbleText; // O componente de texto do balão
        public Button nextButton; // O botão "Próximo"
        public Button skipTutorialButton;
        
        [Header("Game Settings")]
        public string gameplaySceneName = "GameplayScene";
        
        [Header("Tutorial Steps")]
        public List<TutorialStep> tutorialSteps;
        
        private int currentStepIndex = 0;

        public static TutorialManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void Start()
        {
            // Certifique-se de que a mão esteja desativada no início
            pointingHandImage.gameObject.SetActive(false); 
            StartTutorial();
        }

        public void StartTutorial()
        {
            if (tutorialSteps.Count > 0)
            {
                tutorialOverlayPanel.SetActive(true);
                speechBubblePanel.SetActive(true);
                pointingHandImage.gameObject.SetActive(true); // Ativa a mão
                nextButton.onClick.RemoveAllListeners(); // Limpa listeners anteriores para evitar duplicidade
                nextButton.onClick.AddListener(NextStep);
                skipTutorialButton.onClick.AddListener(SkipTutorial);
                DisplayStep(currentStepIndex);
            }
            else
            {
                Debug.LogWarning("Nenhum passo de tutorial configurado!");
            }
        }

        public void NextStep()
        {
            currentStepIndex++;
            if (currentStepIndex < tutorialSteps.Count)
            {
                DisplayStep(currentStepIndex);
            }
            else
            {
                EndTutorial();
            }
        }

        private void DisplayStep(int stepIndex)
        {
            TutorialStep currentStep = tutorialSteps[stepIndex];
            speechBubbleText.text = currentStep.tutorialText;
            
            // Ele garante que a mão sempre aponta para o alvo, se houver um.
            if (currentStep.targetUIPosition != null)
            {
                pointingHandImage.rectTransform.position = currentStep.targetUIPosition.position + new Vector3(currentStep.handOffset.x, currentStep.handOffset.y, 0);
                pointingHandImage.rectTransform.rotation = Quaternion.Euler(0, 0, currentStep.handRotation);
                pointingHandImage.gameObject.SetActive(true);
            }
            else
            {
                // Se não houver alvo, esconde a mão
                pointingHandImage.gameObject.SetActive(false);
            }

            // Este bloco de código controla APENAS o botão "Próximo"
            // Ele garante que o tutorial só avança com o clique do jogador para os passos de texto.
            if (currentStep.stepType == StepType.PlayerAction)
            {
                nextButton.gameObject.SetActive(false); // Esconde o botão Próximo
            }
            else
            {
                nextButton.gameObject.SetActive(true); // Mostra o botão Próximo
            }
        }

        public void ResumeTutorial()
        {
            NextStep();
        }

        public void SkipTutorial()
        {
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void EndTutorial()
        {
            tutorialOverlayPanel.SetActive(false);
            speechBubblePanel.SetActive(false);
            pointingHandImage.gameObject.SetActive(false); // Esconde a mão
            nextButton.onClick.RemoveAllListeners(); // Limpa o listener

            Debug.Log("Tutorial concluído!");
            
            SceneManager.LoadScene(gameplaySceneName);
            // PlayerPrefs.SetInt("HasCompletedTutorial", 1);
        }
    }
}