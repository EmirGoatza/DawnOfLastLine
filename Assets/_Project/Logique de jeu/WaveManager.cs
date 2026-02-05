using UnityEngine;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {
    [Header("Paramètres de manche")]
    public int currentWave = 0;

    public int GetEnemyCount() {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void Update() {
        if (GetEnemyCount() == 0) {
            NextWave();
        }
    }

    void NextWave() {
        currentWave++;
        Debug.Log("// Manche " + currentWave + " commencée !");
    }
}