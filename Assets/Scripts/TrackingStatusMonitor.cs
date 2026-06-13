using UnityEngine;
using UnityEngine.Events;

#if VUFORIA_PRESENT
using Vuforia;
#endif

/// <summary>
/// Surveille l'état du tracking Vuforia et notifie les autres systèmes.
/// Désactive la validation si le tracking est perdu.
/// </summary>
public class TrackingStatusMonitor : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnTrackingFound;
    public UnityEvent OnTrackingLost;

    [Header("Références")]
    public ValidationModule validationModule;
    public UIManager uiManager;

    [Header("UI Feedback")]
    [Tooltip("Message affiché à l'opérateur quand le tracking est perdu")]
    public string lostMessage = "⚠ Marker non détecté — pointez la caméra vers le marker.";

    private bool _isTracking = false;
    public bool IsTracking => _isTracking;

#if VUFORIA_PRESENT
    private ObserverBehaviour _observer;

    void Start()
    {
        _observer = GetComponent<ObserverBehaviour>();
        if (_observer != null)
        {
            _observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    void OnDestroy()
    {
        if (_observer != null)
            _observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour obs, TargetStatus status)
    {
        bool tracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;

        if (tracked && !_isTracking)
        {
            _isTracking = true;
            Debug.Log("[TrackingMonitor] Tracking retrouvé.");
            OnTrackingFound?.Invoke();
        }
        else if (!tracked && _isTracking)
        {
            _isTracking = false;
            Debug.LogWarning("[TrackingMonitor] Tracking perdu.");
            uiManager?.ShowErrorFeedback(lostMessage);
            OnTrackingLost?.Invoke();
        }
    }
#else
    // Stub pour compilation sans Vuforia (tests Editor)
    void Start()
    {
        Debug.Log("[TrackingMonitor] Mode sans Vuforia — tracking simulé comme actif.");
        _isTracking = true;
        OnTrackingFound?.Invoke();
    }
#endif
}