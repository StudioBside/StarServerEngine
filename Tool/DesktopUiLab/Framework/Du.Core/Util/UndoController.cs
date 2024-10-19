namespace Du.Core.Util;

using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Du.Core.Interfaces;

public sealed class UndoController
{
    private readonly LinkedList<IDormammu> commands = new();
    private readonly int limit;
    private LinkedListNode<IDormammu>? currentNode;

    public UndoController(int limit = 10)
    {
        this.limit = limit;
        this.UndoCommand = new RelayCommand(this.Undo, () => this.currentNode is not null);
        this.RedoCommand = new RelayCommand(this.Redo, this.CanRedo);
    }

    public IRelayCommand UndoCommand { get; }
    public IRelayCommand RedoCommand { get; }

    public void Add(IDormammu command)
    {
        // 현재 노드 이후의 모든 명령을 제거
        if (this.currentNode != null)
        {
            while (this.currentNode.Next != null)
            {
                this.commands.RemoveLast();
            }
        }

        // 새로운 명령 추가
        this.commands.AddLast(command);
        this.currentNode = this.commands.Last;

        // 제한된 개수를 초과하는 경우 가장 오래된 명령 제거
        while (this.commands.Count > this.limit)
        {
            this.commands.RemoveFirst();
        }

        this.UndoCommand.NotifyCanExecuteChanged();
        this.RedoCommand.NotifyCanExecuteChanged();
    }

    public void Undo()
    {
        if (this.currentNode == null)
        {
            return;
        }

        this.currentNode.Value.Undo();
        this.currentNode = this.currentNode.Previous;

        this.UndoCommand.NotifyCanExecuteChanged();
        this.RedoCommand.NotifyCanExecuteChanged();
    }

    public void Redo()
    {
        if (this.CanRedo() == false)
        {
            return;
        }

        this.currentNode = this.currentNode?.Next ?? this.commands.First!;
        this.currentNode.Value.Redo();

        this.UndoCommand.NotifyCanExecuteChanged();
        this.RedoCommand.NotifyCanExecuteChanged();
    }

    //// --------------------------------------------------------------------------------

    private bool CanRedo()
    {
        return this.commands.Count > 0 &&
            (this.currentNode is null || this.currentNode.Next is not null);
    }
}
