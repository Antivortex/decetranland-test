using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public TextMeshProUGUI armyWins;
    public Button goToMenu;

    public void Populate(Army army)
    {
        armyWins.text = $"Army {army.ArmyIndex+1} wins!";
    }

    void Awake()
    {
        goToMenu.onClick.AddListener( GoToMenu );
    }

    void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}