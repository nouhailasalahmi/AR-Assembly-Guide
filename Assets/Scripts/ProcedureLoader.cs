using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper pour la désérialisation du fichier JSON de procédure.
/// </summary>
[System.Serializable]
public class AssemblyProcedure
{
    public string procedureName;
    public string mechanismId;
    public List<AssemblyStep> steps;
}

/// <summary>
/// Charge et expose la procédure de montage depuis un fichier JSON dans Resources/.
/// </summary>
public class ProcedureLoader : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Nom du fichier JSON dans Assets/Resources/ (sans extension)")]
    public string jsonFileName = "procedure_montage";

    private AssemblyProcedure _procedure;
    public AssemblyProcedure Procedure => _procedure;

    void Awake()
    {
        LoadProcedure();
    }

    /// <summary>
    /// Charge le JSON et désérialise la procédure.
    /// </summary>
    public void LoadProcedure()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile == null)
        {
            Debug.LogError($"[ProcedureLoader] Fichier JSON introuvable : Resources/{jsonFileName}.json");
            return;
        }

        _procedure = JsonUtility.FromJson<AssemblyProcedure>(jsonFile.text);
        Debug.Log($"[ProcedureLoader] Procédure chargée : {_procedure.procedureName} — {_procedure.steps.Count} étapes");
    }
}