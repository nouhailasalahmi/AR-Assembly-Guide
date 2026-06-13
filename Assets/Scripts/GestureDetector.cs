using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Détecte les gestes tactiles (tap, swipe) pour déclencher la validation
/// en mode ValidationMode.Gesture.
/// </summary>
public class GestureDetector : MonoBehaviour
{
    [Header("Seuils")]
    [Tooltip("Distance minimale en pixels pour qu'un swipe soit détecté")]
    public float swipeThreshold = 50f;

    [Tooltip("Durée maximale en secondes pour un tap")]
    public float tapMaxDuration = 0.3f;

    // ─── Events Unity ───────────────────────────────────────────────────────────
    public UnityEvent OnTapDetected;
    public UnityEvent<SwipeDirection> OnSwipeDetected;

    // ─── État interne ───────────────────────────────────────────────────────────
    private Vector2 _touchStartPos;
    private float _touchStartTime;
    private bool _gestureActive = false;

    public bool IsListening { get; set; } = false;

    void Update()
    {
        if (!IsListening) return;

#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    // ─── Gestion touch ──────────────────────────────────────────────────────────

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _touchStartPos  = touch.position;
                _touchStartTime = Time.time;
                _gestureActive  = true;
                break;

            case TouchPhase.Ended:
                if (!_gestureActive) break;
                _gestureActive = false;
                ClassifyGesture(touch.position, Time.time - _touchStartTime);
                break;

            case TouchPhase.Canceled:
                _gestureActive = false;
                break;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchStartPos  = Input.mousePosition;
            _touchStartTime = Time.time;
            _gestureActive  = true;
        }
        if (Input.GetMouseButtonUp(0) && _gestureActive)
        {
            _gestureActive = false;
            ClassifyGesture(Input.mousePosition, Time.time - _touchStartTime);
        }
    }

    // ─── Classification ─────────────────────────────────────────────────────────

    private void ClassifyGesture(Vector2 endPos, float duration)
    {
        Vector2 delta = endPos - _touchStartPos;
        float distance = delta.magnitude;

        if (distance < swipeThreshold && duration <= tapMaxDuration)
        {
            Debug.Log("[GestureDetector] Tap détecté");
            OnTapDetected?.Invoke();
        }
        else if (distance >= swipeThreshold)
        {
            SwipeDirection dir = GetSwipeDirection(delta);
            Debug.Log($"[GestureDetector] Swipe détecté : {dir}");
            OnSwipeDetected?.Invoke(dir);
        }
    }

    private SwipeDirection GetSwipeDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        else
            return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
    }
}

public enum SwipeDirection { Up, Down, Left, Right }