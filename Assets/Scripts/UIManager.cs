using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UIManager centralise toutes les opérations d'affichage :
/// écran d'accueil, HUD d'étape, feedback succès/erreur, écran résultat.
/// </summary>
public class UIManager : MonoBehaviour
{
    // ─── Panels principaux ──────────────────────────────────────────────────────
    [Header("Panels")]
    public GameObject startScreenPanel;
    public GameObject stepHUDPanel;
    public GameObject resultScreenPanel;
    public GameObject successFeedbackPanel;
    public GameObject errorFeedbackPanel;

    // ─── Écran d'accueil ────────────────────────────────────────────────────────
    [Header("Start Screen")]
    public TextMeshProUGUI startTitleText;
    public TextMeshProUGUI startSubtitleText;
    public Button startButton;

    // ─── HUD étape ─────────────────────────────────────────────────────────────
    [Header("Step HUD")]
    public TextMeshProUGUI stepNumberText;       // "Étape 2 / 5"
    public TextMeshProUGUI stepInstructionText;  // Instruction de l'étape
    public Slider progressBar;                   // Barre de progression globale
    public Button validateButton;               // Bouton "Valider"

    // ─── Feedback succès ────────────────────────────────────────────────────────
    [Header("Success Feedback")]
    public TextMeshProUGUI successText;

    // ─── Feedback erreur ────────────────────────────────────────────────────────
    [Header("Error Feedback")]
    public TextMeshProUGUI errorFeedbackText;

    // ─── Écran résultat ─────────────────────────────────────────────────────────
    [Header("Result Screen")]
    public TextMeshProUGUI resultScoreText;
    public TextMeshProUGUI resultGradeText;
    public TextMeshProUGUI resultTimeText;
    public TextMeshProUGUI resultErrorsText;
    public Button retryButton;

    // ─── Référence StateMachine ─────────────────────────────────────────────────
    [Header("Références")]
    public AssemblyStateMachine stateMachine;
    public TimerDisplay timerDisplay;

    // ─── Lifecycle ─────────────────────────────────────────────────────────────
    void Awake()
    {
        // Boutons câblés en code pour éviter les oublis Inspector
        startButton?.onClick.AddListener(() => stateMachine.StartSession());
        validateButton?.onClick.AddListener(() => stateMachine.RequestValidation());
        retryButton?.onClick.AddListener(() => stateMachine.ResetSession());
    }

    // ─── API publique ──────────────────────────────────────────────────────────

    /// <summary>Affiche l'écran d'accueil.</summary>
    public void ShowStartScreen(string procedureName, int totalSteps)
    {
        HideAll();
        startScreenPanel?.SetActive(true);

        if (startTitleText != null)   startTitleText.text   = procedureName;
        if (startSubtitleText != null) startSubtitleText.text = $"{totalSteps} étapes de montage";
    }

    /// <summary>Affiche le HUD pendant une étape.</summary>
    public void ShowStepUI(AssemblyStep step, int index, int total)
    {
        HideAll();
        stepHUDPanel?.SetActive(true);

        if (stepNumberText != null)
            stepNumberText.text = $"Étape {index + 1} / {total}";

        if (stepInstructionText != null)
            stepInstructionText.text = step.instruction;

        if (progressBar != null)
        {
            progressBar.minValue = 0;
            progressBar.maxValue = total;
            progressBar.value    = index;
        }

        // Activer le bouton Valider uniquement en mode Button
        if (validateButton != null)
            validateButton.gameObject.SetActive(step.validationMode == ValidationMode.Button);

        timerDisplay?.StartTimer();
    }

    /// <summary>Affiche le feedback de succès (court).</summary>
    public void ShowStepSuccess()
    {
        timerDisplay?.StopTimer();
        if (successFeedbackPanel != null)
        {
            successFeedbackPanel.SetActive(true);
            if (successText != null) successText.text = "✅ Étape validée !";
        }
    }

    /// <summary>Affiche le feedback d'erreur.</summary>
    public void ShowErrorFeedback(string reason)
    {
        if (errorFeedbackPanel != null)
        {
            errorFeedbackPanel.SetActive(true);
            if (errorFeedbackText != null) errorFeedbackText.text = reason;
        }
    }

    /// <summary>Affiche l'écran de résultat final.</summary>
    public void ShowResultScreen(SessionResult result)
    {
        HideAll();
        timerDisplay?.StopTimer();
        resultScreenPanel?.SetActive(true);

        if (resultScoreText  != null) resultScoreText.text  = $"{result.score} / 100";
        if (resultGradeText  != null) resultGradeText.text  = result.grade;
        if (resultTimeText   != null) resultTimeText.text   = $"Temps total : {result.FormattedTime}";
        if (resultErrorsText != null) resultErrorsText.text = $"Erreurs : {result.totalErrors}";
    }

    // ─── Privé ─────────────────────────────────────────────────────────────────

    private void HideAll()
    {
        startScreenPanel?.SetActive(false);
        stepHUDPanel?.SetActive(false);
        resultScreenPanel?.SetActive(false);
        successFeedbackPanel?.SetActive(false);
        errorFeedbackPanel?.SetActive(false);
    }
}