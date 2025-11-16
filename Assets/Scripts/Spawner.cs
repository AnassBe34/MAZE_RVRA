using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("Références")]
    public GameObject enemyPrefab;

    [Header("Zone de spawn (relatif à areaCenter)")]
    public Vector3 areaCenter = Vector3.zero;
    public Vector3 areaSize = new Vector3(50f, 0f, 50f); // largeur x profondeur

    [Header("Paramètres")]
    public float minDistanceBetweenEnemies = 2f;

    // internal
    private List<GameObject> activeEnemies = new List<GameObject>();

    // ----- Fonction publique appelée par GameManager -----
    public void SpawnFixedNumber(int count)
    {
        // nettoyage
        activeEnemies.RemoveAll(e => e == null);

        int attemptsTotal = 0;
        int maxTotalAttempts = count * 20;

        for (int i = 0; i < count; i++)
        {
            bool spawned = false;
            int attempts = 0;
            while (!spawned && attempts < 20 && attemptsTotal < maxTotalAttempts)
            {
                attempts++;
                attemptsTotal++;

                Vector3 pos = GetRandomPositionOnGround();

                if (IsFarFromOtherEnemies(pos))
                {
                    GameObject obj = Instantiate(enemyPrefab, pos, Quaternion.identity);
                    activeEnemies.Add(obj);
                    spawned = true;
                }
            }
            if (!spawned)
            {
                Debug.LogWarning($"Spawner: impossible de spawn l'objet #{i} après plusieurs essais.");
            }
        }
    }

    // retourne une position aléatoire dans la box et ajustée au sol
    Vector3 GetRandomPositionOnGround()
    {
        float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
        float z = Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f);
        Vector3 samplePos = areaCenter + new Vector3(x, 10f, z); // start y au-dessus

        // Raycast vers le bas pour trouver le sol (max 50m)
        RaycastHit hit;
        if (Physics.Raycast(samplePos, Vector3.down, out hit, 50f))
        {
            return hit.point + Vector3.up * 0.5f; // remonter légèrement pour éviter l'intersection
        }
        else
        {
            // Si pas de sol détecté, on renvoie une position avec y = areaCenter.y
            return areaCenter + new Vector3(x, areaCenter.y + 0.5f, z);
        }
    }

    bool IsFarFromOtherEnemies(Vector3 pos)
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null) continue;
            if (Vector3.Distance(enemy.transform.position, pos) < minDistanceBetweenEnemies)
                return false;
        }
        return true;
    }
}
