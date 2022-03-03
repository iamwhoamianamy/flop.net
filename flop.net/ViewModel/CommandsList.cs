using System;
using System.Collections.Generic;

namespace flop.net.ViewModel;

public sealed class CommandsList
{
    /// <summary>
    /// Список команд по порядку выполнения
    /// </summary>
    private readonly LinkedList<CommandFlop> commandCollection = new();
    /// <summary>
    /// Текущая выполненная команда
    /// </summary>
    private LinkedListNode<CommandFlop> currentCommand;

    /// <summary>
    /// Метод, для команды, выполнения предыдущей команды
    /// </summary>
    public void ExecutePreviousCommand()
    {
        if (currentCommand.Previous != null)
        {
            currentCommand = currentCommand?.Previous;
            currentCommand.Value.Execute(null);
        }
    }

    /// <summary>
    /// Метод, для команды, выполнения следующей, существующей, команды команды
    /// </summary>
    public void ExecuteNextCommand()
    {
        if (currentCommand.Next != null)
        {
            currentCommand = currentCommand?.Next;
            currentCommand.Value.Execute(null);
        }
    }

    public bool IsLastCommand => currentCommand?.Previous == null;
    public bool IsFirstCommand => currentCommand?.Next == null;

    /// <summary>
    /// Метод для обработки выполненной команды
    /// </summary>
    public void CommandExecuted(object sender, EventArgs e)
    {
        var command = (CommandFlop) sender;

        if (IsFirstCommand)
        {
            commandCollection.AddLast(command);
        }
        else if(!command.Equals(currentCommand.Next.Value))
        {
            commandCollection.AddAfter(currentCommand, command);
        }
    }

    //
    // Singleton
    //

    static readonly CommandsList instance = new ();
    static CommandsList() { }
    public static CommandsList Instance => instance;
}