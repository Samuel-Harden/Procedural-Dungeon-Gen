using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListControl : MonoBehaviour
{
    [SerializeField] GameObject buttonTemplate;
    [SerializeField] LoadSaveManager loadSaveManager;

    private List<GameObject> buttons;


    private void Start()
    {
        buttons = new List<GameObject>();
    }


    public void SetLoadList(List<string> _savedlevels)
    {
        if (buttons.Count != 0)
            ClearButtons();

        foreach(string levelName in _savedlevels)
        {
            GameObject button = Instantiate(buttonTemplate) as GameObject;

            button.SetActive(true);

            button.GetComponent<ButtonListButton>().SetText(levelName);

            button.transform.SetParent(buttonTemplate.transform.parent, false);

            buttons.Add(button);
        }
    }


    // Clear any Buttons
    private void ClearButtons()
    {
        for (int i = buttons.Count - 1; i >= 0; i--)
        {
            Destroy(buttons[i], 0.2f);
        }

        buttons.Clear();
    }


    public void ButtonClicked(string _fileName)
    {
        // Close menus

        // Call LoadSave manager and load level
        loadSaveManager.LoadLevel(_fileName);
    }
}
