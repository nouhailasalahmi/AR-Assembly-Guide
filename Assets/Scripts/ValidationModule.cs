using System;
using UnityEngine;

/// <summary>
/// Module de validation : compare la position/orientation actuelle de la pièce
/// à la cible définie dans l'étape. Supporte trois modes : Auto, Button, Gesture.
/// </summary>
public class ValidationModule : MonoBehaviour
{
    // ─── Références ────────────────────────────────────────────────────────────
    [Header("Références")]
    [Tooltip("Transform de la pièce/objet suivi (enfant de l'Image Target Vuforia)")]
    public Transform trackedObject;

    // ─── Events ────────────────────────────────────────────────────────────────
    public event Action<AssemblyStep> OnValidationSuccess;
    public event Action<AssemblyStep, string> OnValidationFailed;  // step, raison

    // ─── État interne ───────────────────────────────────────────────────────────
    private AssemblyStep _currentStep;
    private bool _autoCheckActive = false;

    // ─── Cycle Unity ───────────────────────────────────────────────────────────
    void Update()
    {
        if (_autoCheckActive && _currentStep?.validationMode == ValidationMode.Auto)
        {
            if (CheckPositionAndRotation(_currentStep, out _))
            {
                _autoCheckActive = false;
                OnValidationSuccess?.Invoke(_currentStep);
            }
        }
    }

    // ─── API ───────────────────────────────────────────────────────────────────

    /// <summary>Prépare le module pour une nouvelle étape.</summary>
    public void PrepareStep(AssemblyStep step)
    {
        _currentStep = step;
        _autoCheckActive = (step.validationMode == ValidationMode.Auto);
    }

    /// <summary>Déclenché par le bouton UI ou par le geste détecté.</summary>
    public void TriggerValidation(AssemblyStep step)
    {
        if (step == null) return;

        if (CheckPositionAndRotation(step, out string failReason))
        {
            OnValidationSuccess?.Invoke(step);
        }
        else
        {
            OnValidationFailed?.Invoke(step, failReason);
        }
    }

    // ─── Logique de vérification ───────────────────────────────────────────────

    /// <summary>
    /// Vérifie si la pièce suivie est dans les tolérances définies par l'étape.
    /// </summary>
    /// <param name="step">L'étape courante.</param>
    /// <param name="failReason">Message d'erreur si hors tolérance.</param>
    /// <returns>True si position ET rotation sont dans les tolérances.</returns>
    private bool CheckPositionAndRotation(AssemblyStep step, out string failReason)
    {
        failReason = string.Empty;

        if (trackedObject == null)
        {
            failReason = "Objet non détecté par le tracking.";
            return false;
        }

        // ── Vérification position ──────────────────────────────────────────────
        float posError = Vector3.Distance(trackedObject.position, step.targetPosition);
        if (posError > step.positionTolerance)
        {
            failReason = $"Position incorrecte (écart : {posError * 100f:F1} cm, tolérance : {step.positionTolerance * 100f:F0} cm).";
            return false;
        }

        // ── Vérification rotation ──────────────────────────────────────────────
        Quaternion targetRot = Quaternion.Euler(step.targetRotation);
        float rotError = Quaternion.Angle(trackedObject.rotation, targetRot);
        if (rotError > step.rotationTolerance)
        {
            failReason = $"Orientation incorrecte (écart : {rotError:F1}°, tolérance : {step.rotationTolerance:F0}°).";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Retourne les erreurs courantes de position/rotation sans déclencher d'événement.
    /// Utile pour l'affichage en temps réel dans l'UI.
    /// </summary>
    public (float posError, float rotError) GetCurrentErrors(AssemblyStep step)
    {
        if (trackedObject == null || step == null) return (float.MaxValue, float.MaxValue);

        float pos = Vector3.Distance(trackedObject.position, step.targetPosition);
        float rot = Quaternion.Angle(trackedObject.rotation, Quaternion.Euler(step.targetRotation));
        return (pos, rot);
    }
}