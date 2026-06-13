using UnityEngine;

/// <summary>
/// Représente une étape de montage désérialisée depuis le JSON de procédure.
/// </summary>
[System.Serializable]
public class AssemblyStep
{
    public int id;
    public string instruction;          // Texte affiché à l'opérateur
    public string annotationPrefabName; // Nom du prefab flèche/annotation à instancier
    public Vector3 targetPosition;      // Position cible de la pièce (world space)
    public Vector3 targetRotation;      // Rotation cible (euler)
    public float positionTolerance;     // Tolérance en mètres (ex: 0.05)
    public float rotationTolerance;     // Tolérance en degrés (ex: 10)
    public float maxDurationSeconds;    // Temps max alloué pour cette étape (0 = illimité)
    public ValidationMode validationMode; // Bouton, Geste, ou Auto
}

public enum ValidationMode
{
    Button,     // L'opérateur appuie sur "Valider"
    Gesture,    // Détection de geste (tap, swipe)
    Auto        // Validation automatique dès que position/rotation sont dans la tolérance
}