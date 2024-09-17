using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    [SerializeField] private Image backGround;
    [SerializeField] private CellText cellText;

    private ChessPiece chessPiece;
    private string cellName;
    private static ChessPiece selectedChessPiece;
    private static Cell currentHoverCell;

    public ChessPiece GetChessPiece()
    {
        return chessPiece;
    }

    public void SetColor(Color color)
    {
        backGround.color = color;
    }

    public void SetName(sbyte x, sbyte y)
    {
        cellName = $"{(char)('a' + x)}{y + 1}";
        gameObject.name = cellName;
    }

    public void Init(Color backgroundColor, Color textColor, sbyte xCoor, sbyte yCoor)
    {
        SetColor(backgroundColor);
        SetName(xCoor, yCoor);

        if (xCoor == 0 || yCoor == 0)
            cellText.Init(xCoor, yCoor, textColor);
        else
            cellText.gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    public void SetChessPiece(ChessPiece chessPiece)
    {
        this.chessPiece = chessPiece;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Clicked on cell: " + cellName);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (chessPiece != null && selectedChessPiece == null)
        {
            selectedChessPiece = chessPiece;
        }
        else if (selectedChessPiece != null && selectedChessPiece.CurrentCell != currentHoverCell)
        {
            MoveChessPiece();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectedChessPiece != null && selectedChessPiece.CurrentCell != currentHoverCell)
        {
            MoveChessPiece();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentHoverCell = this;
    }

    private void MoveChessPiece()
    {
        BoardController.Instance.MoveChessPiece(selectedChessPiece, currentHoverCell).Forget();
        selectedChessPiece = null;
    }
}
