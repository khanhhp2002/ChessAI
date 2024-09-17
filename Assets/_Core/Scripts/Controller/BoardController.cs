using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : Singleton<BoardController>
{
    [SerializeField] private Sprite[] pieceSprites;
    [SerializeField] private ChessPiece chessPieceBrefab;
    [SerializeField] private ChessPieceColor playerColor;
    [SerializeField] private float moveDelay = 1f;

    private List<string> movesHistory = new List<string>();
    private ChessPieceColor turn = ChessPieceColor.White;

    bool canWhiteCastleKingSide = true;
    bool canWhiteCastleQueenSide = true;

    bool canBlackCastleKingSide = true;
    bool canBlackCastleQueenSide = true;

    private string BuildMoveCommand()
    {
        return string.Join(" ", movesHistory);
    }

    [ContextMenu("Spawn Black Queen")]
    public void SpawnBlackQueen()
    {
        ChessPiece blackQueen = Instantiate(chessPieceBrefab);
        int index = (int)ChessPieceType.Queen + 6;
        blackQueen.Init(ChessPieceType.Queen, ChessPieceColor.Black, pieceSprites[index]);
        GridController.Instance.GetCell("a1").SetChessPiece(blackQueen);
    }

    [ContextMenu("Spawn White Queen")]
    public void SpawnWhiteQueen()
    {
        ChessPiece whiteQueen = Instantiate(chessPieceBrefab);
        int index = (int)ChessPieceType.Queen;
        whiteQueen.Init(ChessPieceType.Queen, ChessPieceColor.White, pieceSprites[index]);
        GridController.Instance.GetCell("a8").SetChessPiece(whiteQueen);
    }

    [ContextMenu("New Game")]
    public void NewGame()
    {
        CleanBoard();

        SpawnPiece(ChessPieceType.Rook, ChessPieceColor.White, "a1");
        SpawnPiece(ChessPieceType.Knight, ChessPieceColor.White, "b1");
        SpawnPiece(ChessPieceType.Bishop, ChessPieceColor.White, "c1");
        SpawnPiece(ChessPieceType.Queen, ChessPieceColor.White, "d1");
        SpawnPiece(ChessPieceType.King, ChessPieceColor.White, "e1");
        SpawnPiece(ChessPieceType.Bishop, ChessPieceColor.White, "f1");
        SpawnPiece(ChessPieceType.Knight, ChessPieceColor.White, "g1");
        SpawnPiece(ChessPieceType.Rook, ChessPieceColor.White, "h1");

        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(ChessPieceType.Pawn, ChessPieceColor.White, $"{(char)('a' + i)}2");
            SpawnPiece(ChessPieceType.Pawn, ChessPieceColor.Black, $"{(char)('a' + i)}7");
        }

        SpawnPiece(ChessPieceType.Rook, ChessPieceColor.Black, "a8");
        SpawnPiece(ChessPieceType.Knight, ChessPieceColor.Black, "b8");
        SpawnPiece(ChessPieceType.Bishop, ChessPieceColor.Black, "c8");
        SpawnPiece(ChessPieceType.Queen, ChessPieceColor.Black, "d8");
        SpawnPiece(ChessPieceType.King, ChessPieceColor.Black, "e8");
        SpawnPiece(ChessPieceType.Bishop, ChessPieceColor.Black, "f8");
        SpawnPiece(ChessPieceType.Knight, ChessPieceColor.Black, "g8");
        SpawnPiece(ChessPieceType.Rook, ChessPieceColor.Black, "h8");

        StockfishEngineController.Instance.SendCommand("setoption name Threads value 4");
        StockfishEngineController.Instance.SendCommand("setoption name Hash value 1000);");
        StockfishEngineController.Instance.SendCommand("ucinewgame");
        StockfishEngineController.Instance.SendCommand("position startpos");
        StockfishEngineController.Instance.SendCommand("go depth 10");
    }

    private void CleanBoard()
    {
        if (movesHistory.Count == 0) return;
        movesHistory.Clear();
        GridController.Instance.CleanBoard();
    }

    private void SpawnPiece(ChessPieceType type, ChessPieceColor color, string cellName)
    {
        ChessPiece piece = Instantiate(chessPieceBrefab);
        int index = (int)type + (int)color * 6;
        piece.Init(type, color, pieceSprites[index]);
        piece.Move(GridController.Instance.GetCell(cellName)).Forget();
        piece.gameObject.name = $"{color} {type}";
    }

    public async UniTask MoveChessPiece(ChessPiece chessPiece, Cell to)
    {
        if (to == null) return;

        if (chessPiece.Color != turn) return;

        Cell from = chessPiece.CurrentCell;

        bool isMove = await chessPiece.Move(to);


        if (isMove)
        {
            turn = turn == ChessPieceColor.White ? ChessPieceColor.Black : ChessPieceColor.White; // switch turn
            movesHistory.Add($"{from.name}{to.name}");
            StockfishEngineController.Instance.SendCommand($"position startpos  moves {BuildMoveCommand()}");
            StockfishEngineController.Instance.SendCommand("go depth 10");
        }
    }

    public async UniTaskVoid MoveChessPiece(string from, string to)
    {
        Cell fromCell = GridController.Instance.GetCell(from);
        Cell toCell = GridController.Instance.GetCell(to);

        if (fromCell.GetChessPiece() == null) return;

        if (fromCell.GetChessPiece().Color != playerColor) return;

        await MoveChessPiece(fromCell.GetChessPiece(), toCell);
    }
}
