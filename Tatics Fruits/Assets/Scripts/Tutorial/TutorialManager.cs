using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [System.Serializable]
        public class TutorialStep
        {
            [TextArea]
            public string tutorialText; // O texto que vai aparecer no balão
            public RectTransform targetUIPosition; // A posição da UI para onde a mão vai apontar
            public Vector2 handOffset = Vector2.zero; // Ajuste fino da posição da mão
            public float handRotation = 0f; // Rotação da mão (em graus)
        }

        [Header("UI Elements")]
        public GameObject tutorialOverlayPanel; // O painel de fundo escuro
        public Image pointingHandImage; // A imagem da mão apontando (AGORA É UM IMAGE, NÃO RECTTRANSFORM)
        public GameObject speechBubblePanel; // O painel do balão de texto
        public TextMeshProUGUI speechBubbleText; // O componente de texto do balão
        public Button nextButton; // O botão "Próximo"

        [Header("Tutorial Steps")]
        public List<TutorialStep> tutorialSteps;

        private int currentStepIndex = 0;

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

            // Atualiza o texto do balão
            speechBubbleText.text = currentStep.tutorialText;

            // Move a mão para a posição do alvo + offset
            if (currentStep.targetUIPosition != null)
            {
                // A posição da mão será a do alvo mais o offset
                pointingHandImage.rectTransform.position = currentStep.targetUIPosition.position + new Vector3(currentStep.handOffset.x, currentStep.handOffset.y, 0);
            
                // Define a rotação da mão
                pointingHandImage.rectTransform.rotation = Quaternion.Euler(0, 0, currentStep.handRotation);
            }
            else
            {
                Debug.LogWarning($"Passo {stepIndex} não tem um 'targetUIPosition' configurado para a mão. A mão não será movida.");
                pointingHandImage.gameObject.SetActive(false); // Esconde a mão se não houver alvo
            }
        }

        public void EndTutorial()
        {
            tutorialOverlayPanel.SetActive(false);
            speechBubblePanel.SetActive(false);
            pointingHandImage.gameObject.SetActive(false); // Esconde a mão
            nextButton.onClick.RemoveAllListeners(); // Limpa o listener

            Debug.Log("Tutorial concluído!");
            // PlayerPrefs.SetInt("HasCompletedTutorial", 1);
        }
    }
}