using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{
    [SerializeField] Text myText;

    [SerializeField] ButtonListControl buttonControl;


    // Assign Button Text (level name)
    public void SetText(string _text)
    {
        myText.text = _text;
    }


    public void OnClick()
    {
        buttonControl.ButtonClicked(myText.ToString());
    }
}
