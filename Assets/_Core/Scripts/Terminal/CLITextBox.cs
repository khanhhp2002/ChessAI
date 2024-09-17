using TMPro;
using UnityEngine;

public class CLITextBox : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void SetText(string value, CLITextBoxType cLITextBoxType)
    {
        switch (cLITextBoxType)
        {
            case CLITextBoxType.Command:
                text.text = "> ";
                break;
            case CLITextBoxType.Response:
                text.text = "   ";
                break;
            case CLITextBoxType.Error:
                text.color = Color.red;
                break;
        }

        text.text += value;
    }

}
