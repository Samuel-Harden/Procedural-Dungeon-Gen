using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] Canvas quitMenu;
    [SerializeField] Canvas newLevelMenu;
    [SerializeField] Canvas loadLevelMenu;


    // Use this for initialization
    void Start()
    {
        quitMenu.enabled = false;
        newLevelMenu.enabled = false;
        loadLevelMenu.enabled = false;
    }


    public void exitPress()
    {
        quitMenu.enabled = true;

        newLevelMenu.enabled = false;
        loadLevelMenu.enabled = false;
    }


    public void noPress()
    {
        quitMenu.enabled = false;
    }


    public void NewLevelPress()
    {
        newLevelMenu.enabled = true;

        quitMenu.enabled = false;
        loadLevelMenu.enabled = false;
    }


    public void LoadLevelPress()
    {
        loadLevelMenu.enabled = true;

        quitMenu.enabled = false;
        newLevelMenu.enabled = false;
    }


    public void startEditor()
    {
        SceneManager.LoadScene("LevelEditor");
    }


    public void exitEditor()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}