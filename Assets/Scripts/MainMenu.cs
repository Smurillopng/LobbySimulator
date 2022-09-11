using TMPro;
using UnityEngine;
using Utils;

public class MainMenu : MonoBehaviour
{
    private TMP_InputField _inputField;
    
    public void ExitGame()
    {
        this.Log("Exiting game");
        Application.Quit();
    }
}
