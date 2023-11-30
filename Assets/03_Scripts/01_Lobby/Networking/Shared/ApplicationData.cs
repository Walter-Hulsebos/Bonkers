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

    public static String IP() => PlayerPrefs.GetString(key: k_IPCmd);

    public static Int32 Port() => PlayerPrefs.GetInt(key: k_PortCmd);

    public static Int32 QPort() => PlayerPrefs.GetInt(key: k_QueryPortCmd);

    //Ensure this gets instantiated Early on
    public ApplicationData()
    {
        SetIP(ipArgument: "127.0.0.1");
        SetPort(portArgument: "7777");
        SetQueryPort(qPortArgument: "7787");
        m_CommandDictionary[key: "-" + k_IPCmd]        = SetIP;
        m_CommandDictionary[key: "-" + k_PortCmd]      = SetPort;
        m_CommandDictionary[key: "-" + k_QueryPortCmd] = SetQueryPort;
        ProcessCommandLinearguments(args: Environment.GetCommandLineArgs());
    }

    private void ProcessCommandLinearguments(String[] args)
    {
        StringBuilder sb = new ();
        sb.AppendLine(value: "Launch Args: ");

        for (Int32 i = 0; i < args.Length; i++)
        {
            String arg     = args[i];
            String nextArg = "";

            if (i + 1 < args.Length) // if we are evaluating the last item in the array, it must be a flag
            {
                nextArg = args[i + 1];
            }

            if (EvaluatedArgs(arg: arg, nextArg: nextArg))
            {
                sb.Append(value: arg);
                sb.Append(value: " : ");
                sb.AppendLine(value: nextArg);
                i++;
            }
        }

        Debug.Log(message: sb);
    }

    /// <summary>
    /// Commands and values come in the args array in pairs, so we
    /// </summary>
    private Boolean EvaluatedArgs(String arg, String nextArg)
    {
        if (!IsCommand(arg: arg)) { return false; }

        if (IsCommand(arg: nextArg)) // If you have need for flags, make a separate dict for those.
        {
            return false;
        }

        m_CommandDictionary[key: arg].Invoke(obj: nextArg);
        return true;
    }

    private void SetIP(String ipArgument) { PlayerPrefs.SetString(key: k_IPCmd, value: ipArgument); }

    private void SetPort(String portArgument)
    {
        if (Int32.TryParse(s: portArgument, result: out Int32 parsedPort)) { PlayerPrefs.SetInt(key: k_PortCmd, value: parsedPort); }
        else { Debug.LogError(message: $"{portArgument} does not contain a parseable port!"); }
    }

    private void SetQueryPort(String qPortArgument)
    {
        if (Int32.TryParse(s: qPortArgument, result: out Int32 parsedQPort)) { PlayerPrefs.SetInt(key: k_QueryPortCmd, value: parsedQPort); }
        else { Debug.LogError(message: $"{qPortArgument} does not contain a parseable query port!"); }
    }

    private Boolean IsCommand(String arg) => !String.IsNullOrEmpty(value: arg) && m_CommandDictionary.ContainsKey(key: arg) && arg.StartsWith(value: "-");
}