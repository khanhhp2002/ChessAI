using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class StockfishEngineController : Singleton<StockfishEngineController>
{
    private Process stockfishProcess;
    private StreamWriter stockfishWriter;
    private Thread stockfishListenerThread;
    private string stockfishPath;
    private bool isListening = true;
    private string currentCommand;

    public Action<string> onGetResponse;
    public Action<string> onSentCommand;


    void Awake()
    {
        stockfishPath = Path.Combine(Application.streamingAssetsPath, "stockfish", "stockfish-windows-x86-64-avx2.exe");

        StartEngine();
    }

    private void Start()
    {
        SendCommand("uci");  // Initialize the UCI protocol
    }

    public void StartEngine()
    {
        stockfishProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = stockfishPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        // Start the process
        stockfishProcess.Start();
        stockfishWriter = stockfishProcess.StandardInput;

        // Listen to the stockfish process in a separate thread
        stockfishListenerThread = new Thread(ListenToStockfish);
        stockfishListenerThread.Start();
    }

    [ContextMenu("New Game")]
    public void NewGame()
    {
        SendCommand("ucinewgame");
        SendCommand("isready");
    }

    [ContextMenu("Position")]
    public void Position()
    {
        SendCommand("position startpos");
        SendCommand("go depth 1");
        //position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 moves g2g4 d7d5 f1g2 c8g4 c2c4
        SendCommand("position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 moves g2g4 d7d5 f1g2 c8g4 c2c4");
    }

    [ContextMenu("Evaluate")]
    public void Evaluate()
    {
        SendCommand("eval");
    }

    void StartStockfish()
    {
        stockfishProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = stockfishPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        stockfishProcess.Start();
        stockfishWriter = stockfishProcess.StandardInput;
        UnityEngine.Debug.Log("Stockfish started successfully.");
    }

    public void SendCommand(string command)
    {
        stockfishWriter.WriteLine(command);
        stockfishWriter.Flush();

        currentCommand = command;

        onSentCommand?.Invoke(command);
    }

    void ListenToStockfish()
    {
        using (StreamReader stockfishReader = stockfishProcess.StandardOutput)
        {
            while (isListening && !stockfishReader.EndOfStream)
            {
                string responseLine = stockfishReader.ReadLine();

                HandleResponse(responseLine);
            }
        }
    }

    void HandleResponse(string response)
    {
        if (response == null) return;

        if (response.StartsWith("uciok"))
        {

        }
        else if (response.Equals("readyok"))
        {

        }
        else if (response.StartsWith("option"))
        {

        }
        else if (response.StartsWith("bestmove")) // bestmove g1f3 ponder e7e5
        {
            if (response.Contains("none"))
            {
                UnityEngine.Debug.Log("No move found");
                MainThreadDispatcher.RunTaskInMainThread(() =>
                {
                    BoardController.Instance.NewGame();
                }, 5f).Forget();
                return;
            }
            string from = response.Split(' ')[1].Substring(0, 2);
            string to = response.Split(' ')[1].Substring(2, 2);
            MainThreadDispatcher.RunTaskInMainThread(() =>
            {
                BoardController.Instance.MoveChessPiece(from, to).Forget();
            }, 1f).Forget();
        }
        else
        {

        }

        MainThreadDispatcher.RunTaskInMainThread(() =>
        {
            onGetResponse?.Invoke(response);
        }).Forget();
    }

    void OnApplicationQuit()
    {
        // Send quit command and close the Stockfish process
        isListening = false;
        SendCommand("quit");

        // Join the listener thread to clean up
        if (stockfishListenerThread != null && stockfishListenerThread.IsAlive)
        {
            stockfishListenerThread.Join();
        }

        stockfishProcess.Close();
    }
}
