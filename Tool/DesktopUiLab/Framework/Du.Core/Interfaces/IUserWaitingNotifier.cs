namespace Du.Core.Interfaces;

using System;

/// <summary>
/// UI를 block한 후 사용자에게 기다리라는 메시지를 노출하는 역할.
/// </summary>
public interface IUserWaitingNotifier
{
    Task<IDisposable> StartWait(string message);
    void StopWait();
}
