using TMPro;
using UnityEngine;

public class CellText : MonoBehaviour
{
    [SerializeField] private TMP_Text xCoor;
    [SerializeField] private TMP_Text yCoor;

    public void SetText(sbyte xCoor, sbyte yCoor)
    {
        if (xCoor == 0)
        {
            this.yCoor.text = (yCoor + 1).ToString();
        }
        if (yCoor == 0)
        {
            this.xCoor.text = ((char)('a' + xCoor)).ToString();
        }
    }

    public void Init(sbyte xCoor, sbyte yCoor, Color color)
    {
        SetText(xCoor, yCoor);
        SetColor(color);

        gameObject.SetActive(true);
    }

    public void SetColor(Color color)
    {
        xCoor.color = color;
        yCoor.color = color;
    }
}
