using UnityEngine;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    public GameObject startUIScreen;
    public GameObject gameManager; // ou tout autre objet que tu veux activer après
    public Button startButton;

    void Start()
    {
        // Affiche l'écran de démarrage et désactive le reste du jeu
        startUIScreen.SetActive(true);
        if (gameManager != null)
            gameManager.SetActive(false);

        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    void OnStartButtonClicked()
    {
        // Masque l'écran et lance le jeu
        startUIScreen.SetActive(false);

        if (gameManager != null)
            gameManager.SetActive(true);
    }
}
