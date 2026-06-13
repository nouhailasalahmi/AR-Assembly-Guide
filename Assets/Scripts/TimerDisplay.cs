using UnityEngine;
using TMPro;

/// <summary>
/// Affiche en temps réel le chronomètre global et le chronomètre de l'étape courante.
/// </summary>
public class TimerDisplay : MonoBehaviour
{
    [Header("Références UI")]
    public TextMeshProUGUI sessionTimerText;
    public TextMeshProUGUI stepTimerText;

    [Header("Références")]
    public ScoringManager scoringManager;

    [Header("Couleurs d'alerte")]
    public Color normalColor  = Color.white;
    public Color warningColor = Color.yellow;
    public Color dangerColor  = Color.red;

    [Tooltip("Durée (secondes) à partir de laquelle le chrono de l'étape vire au jaune")]
    public float warningThreshold = 45f;

    [Tooltip("Durée (secondes) à partir de laquelle le chrono de l'étape vire au rouge")]
    public float dangerThreshold = 90f;

    private bool _running = false;

    public void StartTimer() => _running = true;
    public void StopTimer()  => _running = false;

    void Update()
    {
        if (!_running || scoringManager == null) return;

        UpdateSessionTimer();
        UpdateStepTimer();
    }

    private void UpdateSessionTimer()
    {
        if (sessionTimerText == null) return;
        float t = scoringManager.SessionElapsed;
        sessionTimerText.text = $"⏱ {FormatTime(t)}";
    }

    private void UpdateStepTimer()
    {
        if (stepTimerText == null) return;
        float t = scoringManager.CurrentStepElapsed;
        stepTimerText.text = $"Étape : {FormatTime(t)}";

        // Couleur selon seuil
        if (t >= dangerThreshold)
            stepTimerText.color = dangerColor;
        else if (t >= warningThreshold)
            stepTimerText.color = warningColor;
        else
            stepTimerText.color = normalColor;
    }

    private string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }
}