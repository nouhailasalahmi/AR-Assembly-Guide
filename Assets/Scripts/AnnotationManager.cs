using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère l'instanciation et la destruction des prefabs d'annotation AR
/// (flèches directionnelles, étiquettes textuelles) fournis par Membre B.
/// Les prefabs doivent être placés dans Assets/Resources/Annotations/.
/// </summary>
public class AnnotationManager : MonoBehaviour
{
    [Header("Référence tracking")]
    [Tooltip("Parent transform (Image Target ou AR Camera child) où ancrer les annotations")]
    public Transform annotationAnchor;

    [Header("Feedback")]
    [Tooltip("Prefab d'effet de succès (particules, flash vert, etc.)")]
    public GameObject successEffectPrefab;

    // Pool d'instances actives
    private readonly List<GameObject> _activeAnnotations = new List<GameObject>();

    // ─── API ───────────────────────────────────────────────────────────────────

    /// <summary>Instancie le prefab d'annotation de l'étape et le positionne.</summary>
    public void ShowAnnotation(AssemblyStep step)
    {
        HideAll();

        if (string.IsNullOrEmpty(step.annotationPrefabName)) return;

        // Charge depuis Resources/Annotations/
        GameObject prefab = Resources.Load<GameObject>($"Annotations/{step.annotationPrefabName}");
        if (prefab == null)
        {
            Debug.LogWarning($"[AnnotationManager] Prefab introuvable : Annotations/{step.annotationPrefabName}");
            return;
        }

        Transform parent = annotationAnchor != null ? annotationAnchor : transform;
        GameObject instance = Instantiate(prefab, parent);

        // Positionnement selon métadonnées de l'étape
        instance.transform.localPosition = step.targetPosition;
        instance.transform.localRotation = Quaternion.Euler(step.targetRotation);

        _activeAnnotations.Add(instance);
        Debug.Log($"[AnnotationManager] Annotation affichée : {step.annotationPrefabName}");
    }

    /// <summary>Cache et détruit toutes les annotations actives.</summary>
    public void HideAll()
    {
        foreach (var go in _activeAnnotations)
        {
            if (go != null) Destroy(go);
        }
        _activeAnnotations.Clear();
    }

    /// <summary>Joue l'effet visuel de succès à la position de l'étape.</summary>
    public void PlaySuccessFeedback(AssemblyStep step)
    {
        if (successEffectPrefab == null) return;

        Transform parent = annotationAnchor != null ? annotationAnchor : transform;
        GameObject fx = Instantiate(successEffectPrefab, parent);
        fx.transform.localPosition = step.targetPosition;

        // Auto-destruction après 2 secondes
        Destroy(fx, 2f);
    }
}