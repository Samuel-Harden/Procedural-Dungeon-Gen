using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonListControl : MonoBehaviour
{
    [SerializeField] GameObject buttonTemplate;


    private void Start()
    {
        {
            for (int i = 0; i < 20; i++)
            {
                GameObject button = Instantiate(buttonTemplate) as GameObject;

                button.SetActive(true);

                button.GetComponent<ButtonListButton>().SetText("Button No: " + i);

                button.transform.SetParent(buttonTemplate.transform.parent, false);
            }
        }
    }


    public void ButtonClicked(string _text)
    {
        Debug.Log(_text);
    }
}
