using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Mode erreur : affiche une annotation corrective quand une mauvaise action est détectée.
/// Compte les erreurs par étape et gère le cooldown d'affichage.
/// </summary>
public class ErrorModule : MonoBehaviour
{
    // ─── Références ────────────────────────────────────────────────────────────
    [Header("UI Erreur")]
    [Tooltip("Panel Unity UI affiché lors d'une erreur")]
    public GameObject errorPanel;

    [Tooltip("TextMeshPro pour afficher le message d'erreur")]
    public TextMeshProUGUI errorText;

    [Tooltip("TextMeshPro pour afficher l'instruction de correction")]
    public TextMeshProUGUI correctionText;

    [Header("Paramètres")]
    [Tooltip("Durée d'affichage du panneau d'erreur en secondes")]
    public float displayDuration = 3f;

    [Tooltip("Délai minimum entre deux affichages d'erreur (cooldown)")]
    public float errorCooldown = 2f;

    // ─── État interne ───────────────────────────────────────────────────────────
    private int _totalErrors = 0;
    private int _errorsCurrentStep = 0;
    private float _lastErrorTime = -999f;
    private Coroutine _hideCoroutine;

    public int TotalErrors => _totalErrors;
    public int ErrorsCurrentStep => _errorsCurrentStep;

    // ─── API ───────────────────────────────────────────────────────────────────

    /// <summary>Affiche le panneau d'erreur avec message et correction.</summary>
    public void ShowError(AssemblyStep step, string reason)
    {
        // Cooldown : évite le spam si l'opérateur reste bloqué
        if (Time.time - _lastErrorTime < errorCooldown) return;

        _totalErrors++;
        _errorsCurrentStep++;
        _lastErrorTime = Time.time;

        // Texte d'erreur
        if (errorText != null)
            errorText.text = $"⚠ Action incorrecte\n{reason}";

        // Message de correction contextuel
        if (correctionText != null)
            correctionText.text = BuildCorrectionMessage(step, reason);

        // Afficher le panel
        if (errorPanel != null)
        {
            errorPanel.SetActive(true);

            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            _hideCoroutine = StartCoroutine(HideAfterDelay(displayDuration));
        }

        Debug.LogWarning($"[ErrorModule] Erreur #{_totalErrors} — Étape {step.id} : {reason}");
    }

    /// <summary>Cache immédiatement le panneau d'erreur.</summary>
    public void HideError()
    {
        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }
        if (errorPanel != null)
            errorPanel.SetActive(false);
    }

    /// <summary>Réinitialise les compteurs (début de session).</summary>
    public void Reset()
    {
        _totalErrors = 0;
        _errorsCurrentStep = 0;
        _lastErrorTime = -999f;
        HideError();
    }

    /// <summary>Réinitialise le compteur d'erreurs de l'étape courante.</summary>
    public void ResetStepErrors()
    {
        _errorsCurrentStep = 0;
    }

    // ─── Privé ─────────────────────────────────────────────────────────────────

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (errorPanel != null) errorPanel.SetActive(false);
        _hideCoroutine = null;
    }

    /// <summary>Génère un message de correction contextuel selon le type d'erreur.</summary>
    private string BuildCorrectionMessage(AssemblyStep step, string reason)
    {
        if (reason.Contains("Position"))
            return $"Repositionnez la pièce à la position indiquée par la flèche.";
        if (reason.Contains("Orientation") || reason.Contains("rotation"))
            return $"Faites pivoter la pièce dans la direction indiquée (±{step.rotationTolerance:F0}°).";
        if (reason.Contains("non détecté"))
            return "Pointez la caméra vers le marker et assurez-vous qu'il est bien éclairé.";

        return $"Reprenez l'étape depuis le début : {step.instruction}";
    }
}