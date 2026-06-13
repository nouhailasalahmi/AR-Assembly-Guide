using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State Machine principale gérant le déroulement des étapes de montage.
/// Pilote les transitions, notifie les autres systèmes via events.
/// </summary>
public class AssemblyStateMachine : MonoBehaviour
{
    // ─── Références ────────────────────────────────────────────────────────────
    [Header("Références")]
    public ProcedureLoader procedureLoader;
    public AnnotationManager annotationManager;
    public ValidationModule validationModule;
    public ErrorModule errorModule;
    public ScoringManager scoringManager;
    public UIManager uiManager;

    // ─── État interne ───────────────────────────────────────────────────────────
    private List<AssemblyStep> _steps;
    private int _currentIndex = -1;
    private MachineState _state = MachineState.Idle;

    public MachineState CurrentState => _state;
    public AssemblyStep CurrentStep => (_currentIndex >= 0 && _currentIndex < _steps.Count)
        ? _steps[_currentIndex] : null;
    public int CurrentStepIndex => _currentIndex;
    public int TotalSteps => _steps?.Count ?? 0;

    // ─── Events ────────────────────────────────────────────────────────────────
    public event Action<AssemblyStep, int> OnStepStarted;   // step, index
    public event Action<AssemblyStep> OnStepCompleted;
    public event Action<AssemblyStep> OnStepError;
    public event Action<SessionResult> OnSessionCompleted;
    public event Action OnSessionReset;

    // ─── Unity ─────────────────────────────────────────────────────────────────
    void Start()
    {
        _steps = procedureLoader.Procedure?.steps;
        if (_steps == null || _steps.Count == 0)
        {
            Debug.LogError("[StateMachine] Aucune étape chargée.");
            return;
        }

        // Abonnements
        validationModule.OnValidationSuccess += HandleValidationSuccess;
        validationModule.OnValidationFailed  += HandleValidationFailed;

        uiManager.ShowStartScreen(procedureLoader.Procedure.procedureName, _steps.Count);
    }

    void OnDestroy()
    {
        if (validationModule != null)
        {
            validationModule.OnValidationSuccess -= HandleValidationSuccess;
            validationModule.OnValidationFailed  -= HandleValidationFailed;
        }
    }

    // ─── API publique ──────────────────────────────────────────────────────────

    /// <summary>Démarre la session depuis l'écran d'accueil.</summary>
    public void StartSession()
    {
        if (_state != MachineState.Idle) return;
        _currentIndex = -1;
        scoringManager.ResetSession(_steps.Count);
        errorModule.Reset();
        TransitionToNext();
    }

    /// <summary>Appelé par le bouton "Valider" de l'UI ou par ValidationModule en mode Auto.</summary>
    public void RequestValidation()
    {
        if (_state != MachineState.Running) return;
        validationModule.TriggerValidation(CurrentStep);
    }

    /// <summary>Réinitialise toute la session.</summary>
    public void ResetSession()
    {
        _state = MachineState.Idle;
        _currentIndex = -1;
        annotationManager.HideAll();
        scoringManager.ResetSession(_steps.Count);
        errorModule.Reset();
        OnSessionReset?.Invoke();
        uiManager.ShowStartScreen(procedureLoader.Procedure.procedureName, _steps.Count);
    }

    // ─── Transitions ───────────────────────────────────────────────────────────

    private void TransitionToNext()
    {
        _currentIndex++;

        if (_currentIndex >= _steps.Count)
        {
            CompleteSession();
            return;
        }

        _state = MachineState.Running;
        AssemblyStep step = _steps[_currentIndex];

        scoringManager.StartStep(step);
        annotationManager.ShowAnnotation(step);
        validationModule.PrepareStep(step);
        uiManager.ShowStepUI(step, _currentIndex, _steps.Count);

        OnStepStarted?.Invoke(step, _currentIndex);
        Debug.Log($"[StateMachine] ▶ Étape {_currentIndex + 1}/{_steps.Count} : {step.instruction}");
    }

    private void HandleValidationSuccess(AssemblyStep step)
    {
        if (_state != MachineState.Running) return;

        _state = MachineState.Transitioning;
        scoringManager.CompleteStep(step, wasError: false);
        annotationManager.PlaySuccessFeedback(step);
        uiManager.ShowStepSuccess();

        OnStepCompleted?.Invoke(step);
        Debug.Log($"[StateMachine] ✅ Étape {_currentIndex + 1} validée");

        // Transition vers l'étape suivante après un court délai
        Invoke(nameof(TransitionToNext), 1.2f);
    }

    private void HandleValidationFailed(AssemblyStep step, string reason)
    {
        if (_state != MachineState.Running) return;

        scoringManager.RegisterError(step);
        errorModule.ShowError(step, reason);
        uiManager.ShowErrorFeedback(reason);

        OnStepError?.Invoke(step);
        Debug.LogWarning($"[StateMachine] ❌ Erreur étape {_currentIndex + 1} : {reason}");
    }

    private void CompleteSession()
    {
        _state = MachineState.Completed;
        SessionResult result = scoringManager.BuildResult();
        annotationManager.HideAll();
        uiManager.ShowResultScreen(result);
        OnSessionCompleted?.Invoke(result);
        Debug.Log($"[StateMachine] 🏁 Session terminée — Score : {result.score}/100");
    }
}

/// <summary>États possibles de la machine.</summary>
public enum MachineState
{
    Idle,           // Avant démarrage
    Running,        // Étape en cours
    Transitioning,  // Animation entre deux étapes
    Completed       // Toutes les étapes terminées
}