using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public enum Map
{
    Default,
}

public enum GameMode
{
    Default,
}

public enum GameQueue
{
    Casual,
    Competitive,
}

public class MatchplayUser
{
    public UserData Data { get; }

    public Action<String> OnNameChanged;

    public MatchplayUser()
    {
        String tempId = Guid.NewGuid().ToString();

        Data = new UserData(userName: "Player", userAuthId: tempId, clientId: 0, userGamePreferences: new GameInfo());
    }

    public String Name
    {
        get => Data.userName;
        set
        {
            Data.userName = value;
            OnNameChanged?.Invoke(obj: Data.userName);
        }
    }

    public String AuthId
    {
        get => Data.userAuthId;
        set => Data.userAuthId = value;
    }

    public Map MapPreferences
    {
        get => Data.userGamePreferences.map;
        set => Data.userGamePreferences.map = value;
    }

    public GameMode GameModePreferences
    {
        get => Data.userGamePreferences.gameMode;
        set => Data.userGamePreferences.gameMode = value;
    }

    public GameQueue QueuePreference
    {
        get => Data.userGamePreferences.gameQueue;
        set => Data.userGamePreferences.gameQueue = value;
    }

    public override String ToString()
    {
        StringBuilder userData = new (value: "MatchplayUser: ");
        userData.AppendLine(value: $"- {Data}");
        return userData.ToString();
    }
}

[Serializable]
public class UserData
{
    public String   userName;
    public String   userAuthId;
    public UInt64   clientId;
    public GameInfo userGamePreferences;

    public Int32 characterId = -1;

    public UserData(String userName, String userAuthId, UInt64 clientId, GameInfo userGamePreferences)
    {
        this.userName            = userName;
        this.userAuthId          = userAuthId;
        this.clientId            = clientId;
        this.userGamePreferences = userGamePreferences;
    }
}

[Serializable]
public class GameInfo
{
    public Map       map;
    public GameMode  gameMode;
    public GameQueue gameQueue;

    public Int32  MaxUsers = 20;
    public String ToSceneName => ConvertToScene(map: map);

    private const String multiplayCasualQueue      = "casual-queue";
    private const String multiplayCompetitiveQueue = "competitive-queue";

    private static readonly Dictionary<String, GameQueue> multiplayToLocalQueueNames = new()
    {
        { multiplayCasualQueue, GameQueue.Casual }, { multiplayCompetitiveQueue, GameQueue.Competitive },
    };

    public override String ToString()
    {
        StringBuilder sb = new ();
        sb.AppendLine(value: "GameInfo: ");
        sb.AppendLine(value: $"- map:        {map}");
        sb.AppendLine(value: $"- gameMode:   {gameMode}");
        sb.AppendLine(value: $"- gameQueue:  {gameQueue}");
        return sb.ToString();
    }

    public static String ConvertToScene(Map map)
    {
        switch (map)
        {
            case Map.Default: return "Gameplay";

            default:
                Debug.LogWarning(message: $"{map} - is not supported.");
                return "";
        }
    }

    public String ToMultiplayQueue()
    {
        return gameQueue switch
        {
            GameQueue.Casual      => multiplayCasualQueue,
            GameQueue.Competitive => multiplayCompetitiveQueue,
            _                     => multiplayCasualQueue,
        };
    }

    public static GameQueue ToGameQueue(String multiplayQueue)
    {
        if (!multiplayToLocalQueueNames.ContainsKey(key: multiplayQueue))
        {
            Debug.LogWarning(message: $"No QueuePreference that maps to {multiplayQueue}");
            return GameQueue.Casual;
        }

        return multiplayToLocalQueueNames[key: multiplayQueue];
    }
}