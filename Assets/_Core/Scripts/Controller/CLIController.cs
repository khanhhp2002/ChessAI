using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CLIController : MonoBehaviour
{
    [SerializeField] private CLITextBox textBoxBrefabs;
    [SerializeField] private float delay = .1f;
    private Queue<Command> commandQueue = new Queue<Command>();

    private class Command
    {
        public string command;
        public CLITextBoxType type;
    }

    bool isProcessing = false;

    void Awake()
    {
        StockfishEngineController.Instance.onSentCommand += OnSentCommand;
        StockfishEngineController.Instance.onGetResponse += OnReceivedResponse;
    }

    public void OnSentCommand(string command)
    {
        commandQueue.Enqueue(new Command { command = command, type = CLITextBoxType.Command });

        if (!isProcessing)
        {
            DisplayCLIAsync().Forget();
        }
    }

    public void OnReceivedResponse(string response)
    {
        commandQueue.Enqueue(new Command { command = response, type = CLITextBoxType.Response });

        if (!isProcessing)
        {
            DisplayCLIAsync().Forget();
        }
    }

    private async UniTaskVoid DisplayCLIAsync()
    {
        isProcessing = true;
        while (commandQueue.Count > 0)
        {
            Command command = commandQueue.Dequeue();
            await UniTask.Delay((int)(delay * 1000));
            ExecuteTask(command);
        }
        isProcessing = false;
    }

    private async void ExecuteTask(Command command)
    {
        CLITextBox textBox = GetCLITextBox();
        textBox.SetText(command.command, command.type);
        await UniTask.CompletedTask;
    }

    private CLITextBox GetCLITextBox()
    {
        return Instantiate(textBoxBrefabs, transform);
    }
}
