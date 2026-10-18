using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface ISessionRepository
{
    void Save(SessionData data);
    List<SessionData> LoadRecent(int count);
}

public class JsonSessionRepository : ISessionRepository
{
    [Serializable]
    public class SessionFile
    {
        public List<SessionData> sessions = new List<SessionData>();
    }

    private const int MaxSessions = 50;

    // Looked up only when needed, because Unity doesn't allow this during object creation.
    private string FilePath => Path.Combine(Application.persistentDataPath, "sessions.json");

    public void Save(SessionData data)
    {
        SessionFile file = ReadFile();
        file.sessions.Add(data);
        while (file.sessions.Count > MaxSessions) file.sessions.RemoveAt(0);

        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(file, true));
        }
        catch (Exception e)
        {
            Debug.LogError("Could not save sessions: " + e.Message);
        }
    }

    // Newest first.
    public List<SessionData> LoadRecent(int count)
    {
        SessionFile file = ReadFile();
        List<SessionData> recent = new List<SessionData>();
        for (int i = file.sessions.Count - 1; i >= 0 && recent.Count < count; i--)
            recent.Add(file.sessions[i]);
        return recent;
    }

    private SessionFile ReadFile()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                SessionFile file = JsonUtility.FromJson<SessionFile>(File.ReadAllText(FilePath));
                if (file != null && file.sessions != null) return file;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Could not read sessions: " + e.Message);
        }
        return new SessionFile();
    }
}