using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManagerXR : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject spherePrefab;
    public int numberOfSpheres = 10;
    public GameObject environmentParent;

    [Header("Timer Settings")]
    public float gameTime = 60f;
    private float timer;
    private bool gameStarted = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    void Start()
    {
        Debug.Log("[GameManagerXR] Start() lancé");
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame()
    {
        // Attendre que le WFC ait fini de générer
        yield return new WaitForSeconds(2f);

        StartGame(); // 🔥 On lance vraiment le jeu ici
    }

    void StartGame()
    {
        timer = gameTime;
        gameStarted = true;

        Debug.Log("[GameManagerXR] Jeu démarré avec timer = " + timer);

        SpawnSpheres();
        UpdateTimerUI();
    }

    void Update()
    {
        if (!gameStarted)
            return;

        timer -= Time.deltaTime;

        if (timer < 0f)
            timer = 0f;

        UpdateTimerUI();

        if (timer <= 0f)
        {
            gameStarted = false;
            EndGame();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null)
        {
            Debug.LogWarning("[GameManagerXR] ⚠️ Le champ Timer Text n'est pas assigné !");
            return;
        }

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void SpawnSpheres()
    {
        if (spherePrefab == null || environmentParent == null)
        {
            Debug.LogWarning("[GameManagerXR] Prefab ou environnement non assigné !");
            return;
        }

        Bounds bounds = GetEnvironmentBounds();
        float ballRadius = spherePrefab.GetComponent<SphereCollider>().radius * spherePrefab.transform.localScale.y;

        // 🔹 Trouver le GameObject du plan
        GameObject plane = GameObject.Find("Plane");
        if (plane == null)
        {
            Debug.LogError("[GameManagerXR]  Aucun GameObject nommé 'Plane' trouvé dans la scène !");
            return;
        }

        float planeY = plane.transform.position.y;

        for (int i = 0; i < numberOfSpheres; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool validPos = false;
            int attempts = 0;

            // On tente plusieurs fois pour éviter de spawn sur un TriggerWall
            while (!validPos && attempts < 30)
            {
                attempts++;

                // 1️⃣ Choisit une position aléatoire sur la surface du plan
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomZ = Random.Range(bounds.min.z, bounds.max.z);

                spawnPos = new Vector3(randomX, planeY + ballRadius, randomZ);

                // 2️⃣ Vérifie s’il y a collision avec un objet (comme un TriggerWall)
                Collider[] overlaps = Physics.OverlapSphere(spawnPos, ballRadius);
                validPos = true;

                foreach (Collider col in overlaps)
                {
                    if (col.CompareTag("TriggerWall")) //  Évite les cubes TriggerWall
                    {
                        validPos = false;
                        break;
                    }
                }
            }

            if (validPos)
            {
                Instantiate(spherePrefab, spawnPos, Quaternion.identity, environmentParent.transform);
                Debug.Log($"[GameManagerXR]  Sphère {i + 1} spawnée à {spawnPos}");
            }
            else
            {
                Debug.LogWarning($"[GameManagerXR]  Impossible de trouver une position sûre pour la sphère {i + 1}");
            }
        }
    }


    Bounds GetEnvironmentBounds()
    {
        Renderer[] renderers = environmentParent.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(environmentParent.transform.position, Vector3.one * 10);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }

    void EndGame()
    {
        Debug.Log("[GameManagerXR] Fin du jeu !");
        if (timerText != null)
            timerText.text = "Temps écoulé !";
    }
}
