using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

/// <summary>
/// Basic launch command processor (Multiplay prefers passing IP and port along)
/// </summary>
public class ApplicationData
{
    /// <summary>
    /// Commands Dictionary
    /// Supports flags and single variable args (eg. '-argument', '-variableArg variable')
    /// </summary>
    private Dictionary<String, Action<String>> m_CommandDictionary = new ();

    private const String k_IPCmd        = "ip";
    private const String k_PortCmd      = "port";
    private const String k_QueryPortCmd = "queryPort";

    public static String IP() => PlayerPrefs.GetString(k_IPCmd);

    public static Int32 Port() => PlayerPrefs.GetInt(k_PortCmd);

    public static Int32 QPort() => PlayerPrefs.GetInt(k_QueryPortCmd);

    //Ensure this gets instantiated Early on
    public ApplicationData()
    {
        SetIP("127.0.0.1");
        SetPort("7777");
        SetQueryPort("7787");
        m_CommandDictionary["-" + k_IPCmd]        = SetIP;
        m_CommandDictionary["-" + k_PortCmd]      = SetPort;
        m_CommandDictionary["-" + k_QueryPortCmd] = SetQueryPort;
        ProcessCommandLinearguments(Environment.GetCommandLineArgs());
    }

    private void ProcessCommandLinearguments(String[] args)
    {
        StringBuilder sb = new ();
        sb.AppendLine("Launch Args: ");

        for (Int32 i = 0; i < args.Length; i++)
        {
            String arg     = args[i];
            String nextArg = "";

            if (i + 1 < args.Length) // if we are evaluating the last item in the array, it must be a flag
            {
                nextArg = args[i + 1];
            }

            if (EvaluatedArgs(arg, nextArg))
            {
                sb.Append(arg);
                sb.Append(" : ");
                sb.AppendLine(nextArg);
                i++;
            }
        }

        Debug.Log(sb);
    }

    /// <summary>
    /// Commands and values come in the args array in pairs, so we
    /// </summary>
    private Boolean EvaluatedArgs(String arg, String nextArg)
    {
        if (!IsCommand(arg)) { return false; }

        if (IsCommand(nextArg)) // If you have need for flags, make a separate dict for those.
        {
            return false;
        }

        m_CommandDictionary[arg].Invoke(nextArg);
        return true;
    }

    private void SetIP(String ipArgument) { PlayerPrefs.SetString(k_IPCmd, ipArgument); }

    private void SetPort(String portArgument)
    {
        if (Int32.TryParse(portArgument, out Int32 parsedPort)) { PlayerPrefs.SetInt(k_PortCmd, parsedPort); }
        else { Debug.LogError($"{portArgument} does not contain a parseable port!"); }
    }

    private void SetQueryPort(String qPortArgument)
    {
        if (Int32.TryParse(qPortArgument, out Int32 parsedQPort)) { PlayerPrefs.SetInt(k_QueryPortCmd, parsedQPort); }
        else { Debug.LogError($"{qPortArgument} does not contain a parseable query port!"); }
    }

    private Boolean IsCommand(String arg) => !String.IsNullOrEmpty(arg) && m_CommandDictionary.ContainsKey(arg) && arg.StartsWith("-");
}