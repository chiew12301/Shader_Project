using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommandBase
{
    private string m_commandID;
    private string m_commandDescription;
    private string m_commandFormat;

    public string GetCommondID() => this.m_commandID;
    public string GetDescription() => this.m_commandDescription;
    public string GetFormat() => this.m_commandFormat;

    public DebugCommandBase(string id, string description, string format)
    {
        this.m_commandID = id;
        this.m_commandDescription = description;
        this.m_commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
        this.command = command;
    }
    
    public void Invoke()
    {
        this.command?.Invoke();
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> command;

    public DebugCommand(string id, string description, string format, Action<T> command) : base(id, description, format)
    {
        this.command = command;
    }
    
    public void Invoke(T value)
    {
        this.command.Invoke(value);
    }
}
