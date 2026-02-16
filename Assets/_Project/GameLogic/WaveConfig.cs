using UnityEngine;


[CreateAssetMenu(fileName = "WaveConfig", menuName = "ScriptableObjects/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Header("Paramètres de Progression")]
    public float scaleFactor = 0.5f;

    [Tooltip("1 = pas de changement. 0.9 = réduit le délai de 10% par vague.")]
    public float delayScaleFactor = 0.95f; 
    public float minDelayAllowed = 0.5f; // Sécurité pour ne pas tomber à 0s
    
    // Calcul du nombre d'ennemis supplémentaires à ajouter
    public int GetExtraEnemies(int waveNumber) => Mathf.RoundToInt((waveNumber - 1) * scaleFactor);

    // Calcul du nouveau délai
    public float GetScaledDelay(float baseDelay, int waveNumber)
    {
        float newDelay = baseDelay * Mathf.Pow(delayScaleFactor, waveNumber - 1);
        return Mathf.Max(newDelay, minDelayAllowed);
    }
}
