using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère le chronomètre de performance et le calcul du score d'efficacité.
/// Score calculé sur 100 points : temps + erreurs.
/// </summary>
public class ScoringManager : MonoBehaviour
{
    // ─── Paramètres de scoring ──────────────────────────────────────────────────
    [Header("Paramètres de score")]
    [Tooltip("Score de base sur 100")]
    public int baseScore = 100;

    [Tooltip("Points déduits par erreur commise")]
    public int penaltyPerError = 10;

    [Tooltip("Pénalité par tranche de 30 secondes dépassée (par étape)")]
    public int penaltyPerTimeThreshold = 5;

    [Tooltip("Durée de référence en secondes pour chaque étape (pour calcul bonus/pénalité)")]
    public float referenceStepDuration = 60f;

    // ─── État de session ────────────────────────────────────────────────────────
    private float _sessionStartTime;
    private float _currentStepStartTime;
    private int _totalSteps;
    private int _totalErrors;

    private List<StepRecord> _records = new List<StepRecord>();
    private AssemblyStep _currentStep;

    public float SessionElapsed => Time.time - _sessionStartTime;
    public float CurrentStepElapsed => Time.time - _currentStepStartTime;
    public int TotalErrors => _totalErrors;

    // ─── API ───────────────────────────────────────────────────────────────────

    /// <summary>Réinitialise toutes les données de session.</summary>
    public void ResetSession(int stepCount)
    {
        _totalSteps = stepCount;
        _totalErrors = 0;
        _records.Clear();
        _sessionStartTime = Time.time;
        Debug.Log("[ScoringManager] Session réinitialisée.");
    }

    /// <summary>Démarre le chronomètre pour une étape.</summary>
    public void StartStep(AssemblyStep step)
    {
        _currentStep = step;
        _currentStepStartTime = Time.time;
    }

    /// <summary>Enregistre une erreur sur l'étape courante.</summary>
    public void RegisterError(AssemblyStep step)
    {
        _totalErrors++;
        Debug.Log($"[ScoringManager] Erreur #{_totalErrors} sur étape {step.id}");
    }

    /// <summary>Clôture l'étape courante et enregistre ses métriques.</summary>
    public void CompleteStep(AssemblyStep step, bool wasError)
    {
        float duration = Time.time - _currentStepStartTime;
        _records.Add(new StepRecord
        {
            stepId   = step.id,
            duration = duration,
            hadError = wasError
        });
        Debug.Log($"[ScoringManager] Étape {step.id} terminée en {duration:F1}s");
    }

    /// <summary>Construit le résultat final de session avec score calculé.</summary>
    public SessionResult BuildResult()
    {
        float totalTime = Time.time - _sessionStartTime;
        int score = CalculateScore(totalTime);

        return new SessionResult
        {
            score           = score,
            totalTimeSeconds = totalTime,
            totalErrors     = _totalErrors,
            totalSteps      = _totalSteps,
            stepRecords     = new List<StepRecord>(_records),
            grade           = GetGrade(score)
        };
    }

    // ─── Calcul du score ────────────────────────────────────────────────────────

    private int CalculateScore(float totalTime)
    {
        int score = baseScore;

        // Pénalité erreurs
        score -= _totalErrors * penaltyPerError;

        // Pénalité temps (par étape en moyenne)
        if (_totalSteps > 0)
        {
            float avgStepTime = totalTime / _totalSteps;
            float overtime = Mathf.Max(0f, avgStepTime - referenceStepDuration);
            int timePenalty = Mathf.FloorToInt(overtime / 30f) * penaltyPerTimeThreshold;
            score -= timePenalty;
        }

        return Mathf.Clamp(score, 0, 100);
    }

    private string GetGrade(int score)
    {
        if (score >= 90) return "Excellent";
        if (score >= 75) return "Bien";
        if (score >= 60) return "Correct";
        if (score >= 40) return "À améliorer";
        return "Insuffisant";
    }
}

// ─── Structures de données ───────────────────────────────────────────────────

[Serializable]
public class StepRecord
{
    public int stepId;
    public float duration;
    public bool hadError;
}

[Serializable]
public class SessionResult
{
    public int score;
    public float totalTimeSeconds;
    public int totalErrors;
    public int totalSteps;
    public string grade;
    public List<StepRecord> stepRecords;

    /// <summary>Durée formatée mm:ss</summary>
    public string FormattedTime
    {
        get
        {
            int m = Mathf.FloorToInt(totalTimeSeconds / 60f);
            int s = Mathf.FloorToInt(totalTimeSeconds % 60f);
            return $"{m:00}:{s:00}";
        }
    }
}