using System;
using Cysharp.Threading.Tasks;
public class MainThreadDispatcher
{
    public static async UniTask RunTaskInMainThread(Action action)
    {
        await UniTask.SwitchToMainThread();

        action?.Invoke();
    }

    public static async UniTask RunTaskInMainThread(Action action, float delay)
    {
        await UniTask.SwitchToMainThread();
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        action?.Invoke();
    }
}

