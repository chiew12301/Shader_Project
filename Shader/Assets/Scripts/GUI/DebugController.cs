using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    public int ccccc = 0;
    private bool showConsole;
    private bool showHelp;

    private string input;

    public static DebugCommand DEBUG_TESTING;
    public static DebugCommand<int> DEBUG_VALUE;
    public static DebugCommand HELP;
    public static DebugCommand HIDE;

    public List<object> commandList;

    private Vector2 scroll;

    public void OnToggleDebug(InputValue value)
    {
        this.showConsole = !this.showConsole;
    }

    public void OnReturn(InputValue value)
    {
        if(this.showConsole)
        {
            this.HandleInput();
            this.input = "";
        }
    }

    private void Awake() 
    {
        DEBUG_TESTING = new DebugCommand("dt", "Debug a text.", "dt", () =>
        {
            this.TestForDebug();
        }); 

        DEBUG_VALUE = new DebugCommand<int>("dv", "Debug a value.", "dv", (x) =>
        {
            this.TestForDebug(x);
        });

        HELP = new DebugCommand("help", "Show All Command.", "help", () =>
        {
            this.showHelp = true;
        });

        HIDE = new DebugCommand("hide", "Hide Console.", "hide", () =>
        {
            this.showConsole = false;
            this.showHelp = false;
        });

        this.commandList = new List<object>{
            DEBUG_TESTING,
            DEBUG_VALUE,
            HELP,
            HIDE,
        };
    }

    private void OnGUI()
    {
        if(!this.showConsole) return;

        float y = 0.0f;

        if(this.showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 100), "");

            Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * this.commandList.Count);

            this.scroll = GUI.BeginScrollView(new Rect(0, y + 5.0f, Screen.width, 90), this.scroll, viewport);
            for(int i = 0; i < this.commandList.Count; i++)
            {
                DebugCommandBase c = this.commandList[i] as DebugCommandBase;

                string label = $"{c.GetFormat()} - {c.GetDescription()}";

                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();

            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        this.input = GUI.TextField(new Rect(10.0f, y + 5.0f, Screen.width-20.0f, 20.0f), this.input);
    }

    private void HandleInput()
    {
        string[] properties = this.input.Split(' ');

        for(int i = 0; i < this.commandList.Count; i++)
        {
            DebugCommandBase cb = this.commandList[i] as DebugCommandBase;

            if(this.input.Contains(cb.GetCommondID()))
            {
                if(this.commandList[i] as DebugCommand != null)
                {
                    (this.commandList[i] as DebugCommand).Invoke();
                }
                else if(this.commandList[i] as DebugCommand<int> != null)
                {
                    (this.commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
            }
        }
    }

    public void TestForDebug()
    {
        Debug.Log("Debuged");
    }

    public void TestForDebug(int value)
    {
        Debug.Log("Debuged " + value);
        ccccc += value;
    }
}
