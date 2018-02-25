using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{
    [SerializeField] Text fileName;
    [SerializeField] ButtonListControl buttonControl;


    // Assign Button Text (level name)
    public void SetText(string _fileName)
    {
        string remove = "Assets/Resources/MapData/";
        fileName.text = _fileName;

        fileName.text = fileName.text.Replace(remove, "");
    }


    // pass the level name to be loaded
    public void OnClick()
    {
        buttonControl.ButtonClicked(fileName.text);
    }
}
