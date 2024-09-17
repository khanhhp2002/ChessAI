using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour
{
    [SerializeField] private ChessPieceType type;
    [SerializeField] private ChessPieceColor color;
    [SerializeField] private Image image;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    private Cell currentCell;

    public Cell CurrentCell { get => currentCell; set => currentCell = value; }
    public ChessPieceType Type { get => type; set => type = value; }
    public ChessPieceColor Color { get => color; set => color = value; }
    public AspectRatioFitter AspectRatioFitter { get => aspectRatioFitter; set => aspectRatioFitter = value; }

    public void Init(ChessPieceType type, ChessPieceColor color, Sprite image)
    {
        this.type = type;
        this.color = color;
        this.image.sprite = image;
    }

    public async UniTask<bool> Move(Cell to)
    {
        if (to == null) return false;

        ChessPiece chessPiece = to.GetChessPiece();
        if (chessPiece != this && chessPiece != null)
        {
            if (chessPiece.Color == this.Color)
            {
                this.transform.SetParent(currentCell.transform);
                await LMotion.Create(transform.localPosition, Vector3.zero, 0f)
                    .WithEase(Ease.InOutCubic)
                    .BindWithState(transform, (location, target) => target.localPosition = location)
                    .ToUniTask();
                return false;
            }
            Destroy(chessPiece.gameObject);
            to.SetChessPiece(null);
        }

        currentCell?.SetChessPiece(null);

        this.transform.SetParent(to.transform);
        Debug.Log($"Moved {Color} {Type} to {to.name}");

        await LMotion.Create(transform.localPosition, Vector3.zero, 0f)
            .WithEase(Ease.InOutCubic)
            .BindWithState(transform, (location, target) => target.localPosition = location)
            .ToUniTask();

        currentCell = to;
        to.SetChessPiece(this);

        return true;
    }

    public async UniTask<bool> Move(Cell to, float delay)
    {
        if (to == null) return false;

        ChessPiece chessPiece = to.GetChessPiece();
        if (chessPiece != this && chessPiece != null)
        {
            if (chessPiece.Color == this.Color) return false;
            Destroy(chessPiece.gameObject);
            to.SetChessPiece(null);
        }

        currentCell?.SetChessPiece(null);

        this.transform.SetParent(to.transform);
        Debug.Log($"Moved {Color} {Type} to {to.name}");

        await LMotion.Create(transform.localPosition, Vector3.zero, delay)
            .WithEase(Ease.InOutCubic)
            .BindWithState(transform, (location, target) => target.localPosition = location)
            .ToUniTask();

        currentCell = to;
        to.SetChessPiece(this);

        return true;
    }

    private bool IsMoveValid(Cell to)
    {
        return true;
    }
}

public enum ChessPieceType
{
    Pawn = 0,
    Rook = 1,
    Knight = 2,
    Bishop = 3,
    Queen = 4,
    King = 5
}

public enum ChessPieceColor
{
    White = 0,
    Black = 1
}
