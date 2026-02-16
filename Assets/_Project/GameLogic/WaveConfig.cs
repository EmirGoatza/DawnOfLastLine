using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "ScriptableObjects/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [System.Serializable]
    public struct EnemyGroup
    {
        public int count;
        public float delayAfter;
    }

    [Header("Paramètres de Progression")]
    public float scaleFactor = 0.5f; // Nombre d'ennemis supplémentaires par vague
    public float delayScaleFactor = 0.95f;  // Réduction du délai entre les groupes à chaque vague
    public float minDelayAllowed = 0.5f; // Délai minimum entre les groupes
    
    [Header("Valeurs par Défaut (Génération)")]
    public int baseEnemyCount = 3; 
    public float baseDelayBetweenGroups = 2f;

    // Génère dynamiquement la structure de la vague
    public List<EnemyGroup> GenerateWaveLayout(int waveNumber)
    {
        List<EnemyGroup> generatedGroups = new List<EnemyGroup>();
        
        // La taille de la liste correspond au numéro de la vague pour le moment, mais cela peut être ajusté pour des configurations plus complexes
        for (int i = 0; i < waveNumber; i++)
        {
            EnemyGroup group;
            group.count = baseEnemyCount + GetExtraEnemies(waveNumber);
            group.delayAfter = GetScaledDelay(baseDelayBetweenGroups, waveNumber);
            
            generatedGroups.Add(group);
        }

        return generatedGroups;
    }

    public int GetExtraEnemies(int waveNumber) => Mathf.RoundToInt((waveNumber - 1) * scaleFactor);

    public float GetScaledDelay(float baseDelay, int waveNumber)
    {
        float newDelay = baseDelay * Mathf.Pow(delayScaleFactor, waveNumber - 1);
        return Mathf.Max(newDelay, minDelayAllowed);
    }
}