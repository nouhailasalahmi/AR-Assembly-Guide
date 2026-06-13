# AR Assembly Guide - Guide d'Assemblage en Réalité Augmentée

Ce projet est une application de **Réalité Augmentée (AR)** développée avec **Unity 6** et **Vuforia Engine**. Elle est conçue pour guider pas à pas un opérateur dans l'assemblage d'un mécanisme mécanique complexe (piston, bielle et vilebrequin). 

L'application suit en temps réel la position et l'orientation des pièces réelles à l'aide de marqueurs physiques (*Image Targets*), compare l'écart avec les tolérances définies et attribue un score d'efficacité à l'utilisateur à la fin de la procédure.

---

## 🚀 Fonctionnalités Clés

* **Suivi Spatial Précis (Vuforia)** : Utilisation de marqueurs d'images 2D pour repérer et suivre les pièces réelles (vilebrequin, bielle, piston).
* **Architecture Guidée par États (State Machine)** : Gestion robuste du flux d'étapes (Accueil, Étape en cours, Feedback succès/erreur, Écran final).
* **Chargement Dynamique (JSON)** : Les étapes de montage, tolérances, instructions et annotations 3D sont lues de façon dynamique depuis un fichier de configuration JSON.
* **Algorithme de Validation Spatiale** : Compare la distance (en cm) et la rotation angulaire (en degrés) de l'objet suivi par rapport à la cible.
* **Trois Modes de Validation** :
  * `Auto` : Validation automatique dès que la pièce physique entre dans les tolérances.
  * `Button` : L'opérateur clique sur un bouton de l'interface utilisateur pour valider son geste.
  * `Gesture` : Détection de gestes tactiles spécifiques sur l'écran (glissement, tapotement).
* **Système d'Évaluation (Scoring)** : Attribution d'un score final sur 100 points calculé à partir du temps de montage et des erreurs commises, assorti d'une appréciation finale (*Excellent, Bien, Correct, À améliorer, Insuffisant*).
* **Annotations 3D Dynamiques** : Affichage d'aides visuelles (flèches, contours en surbrillance) pour désigner l'emplacement d'assemblage de chaque pièce.

---

## 📂 Structure du Code Source (Scripts C#)

Tous les scripts se situent dans le dossier [Assets/Scripts](file:///c:/Users/lenovo/My%20project/Assets/Scripts/) :

* **[AssemblyStep.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/AssemblyStep.cs)** : Modèle de données désérialisant les étapes de montage depuis le JSON. Définit les tolérances, positions cibles et modes de validation.
* **[ProcedureLoader.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/ProcedureLoader.cs)** : Charge et analyse le fichier JSON de procédure situé dans les ressources.
* **[AssemblyStateMachine.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/AssemblyStateMachine.cs)** : Gère le cycle de vie global de l'application et les transitions entre les étapes.
* **[ValidationModule.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/ValidationModule.cs)** : Calcule les écarts de translation et de rotation angulaire des objets pour valider l'étape.
* **[ScoringManager.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/ScoringManager.cs)** : Enregistre le temps écoulé, comptabilise les erreurs et calcule le score final de l'opérateur.
* **[UIManager.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/UIManager.cs)** : Pilote tous les panneaux d'affichage (HUD de progression, popups d'erreurs, résultats).
* **[GestureDetector.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/GestureDetector.cs)** : Gère l'interprétation des gestes utilisateur via le nouveau système d'Input Unity.
* **[AnnotationManager.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/AnnotationManager.cs)** : Instancie et place les modèles 3D d'aide et de guidage visuel.
* **[TimerDisplay.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/TimerDisplay.cs)** : Affiche le compte à rebours ou chronomètre en temps réel sur l'interface.
* **[TrackingStatusMonitor.cs](file:///c:/Users/lenovo/My%20project/Assets/Scripts/TrackingStatusMonitor.cs)** : Surveille la qualité de détection de la caméra et alerte l'opérateur en cas de perte de tracking.

---

## ⚙️ Configuration de la Procédure (Format JSON)

La séquence d'assemblage est définie dans le fichier [procedure_montage.json](file:///c:/Users/lenovo/My%20project/Assets/Resources/procedure_montage.json). Voici un exemple de structure :

```json
{
  "procedureName": "Assemblage du mécanisme piston-bielle-vilebrequin",
  "mechanismId": "PROC_MOTEUR_ASSEMBLAGE_V1",
  "steps": [
    {
      "id": 1,
      "instruction": "Placer le vilebrequin dans sa position de référence.",
      "annotationPrefabName": "Arrow_Vilebrequin",
      "targetPosition": { "x": 0.0, "y": 0.0, "z": 0.0 },
      "targetRotation": { "x": 0.0, "y": 0.0, "z": 0.0 },
      "positionTolerance": 0.05,
      "rotationTolerance": 10.0,
      "maxDurationSeconds": 60,
      "validationMode": 0
    }
  ]
}
```

---

## 🛠️ Installation et Lancement du Projet

### 1. Prérequis
* **Unity Hub** avec l'éditeur **Unity 6** (version `6000.x` recommandée).
* Une webcam (pour tester dans l'éditeur) ou un appareil mobile Android/iOS pour le déploiement.

### 2. Téléchargement et Ouverture
1. Clonez ce dépôt GitHub :
   ```bash
   git clone https://github.com/nouhailasalahmi/AR-Assembly-Guide.git
   ```
2. Ouvrez le dossier du projet dans **Unity Hub**.
3. Ouvrez la scène principale : `Assets/Scenes/MainScene.unity`.

### 3. Utilisation des Marqueurs
Les marqueurs d'images (*Image Targets*) nécessaires pour que la caméra détecte les pièces sont situés dans le dossier `Assets/Markers/` :
* [vilebrequin.png](file:///c:/Users/lenovo/My%20project/Assets/Markers/vilebrequin.png)
* [bielle.png](file:///c:/Users/lenovo/My%20project/Assets/Markers/bielle.png)
* [piston.png](file:///c:/Users/lenovo/My%20project/Assets/Markers/piston.png)

*Vous pouvez les imprimer ou les afficher sur un autre écran pour tester le tracking avec votre webcam.*

---

## 📄 Rapport de Projet LaTeX

Le code source LaTeX du rapport de ce projet est disponible à la racine du fichier [Rapport_AR_Assembly_Guide.tex](file:///c:/Users/lenovo/My%20project/Rapport_AR_Assembly_Guide.tex). Il contient la modélisation technique complète, les équations géométriques de calcul d'écart 3D et le fonctionnement de l'interface et du scoring.

Pour générer le rapport au format PDF, compilez le fichier avec n'importe quel éditeur LaTeX (comme Texmaker, Overleaf ou Visual Studio Code avec l'extension LaTeX Workshop) en utilisant le compilateur `pdflatex`.

---

## 📝 Licence
Projet réalisé dans un but académique et de recherche par **Nouhaila Salahmi, wissal EL ABDOUSSI et ESSOUFI Hassan**.
