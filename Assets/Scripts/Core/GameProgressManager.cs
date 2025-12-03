using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CaseProgress
{
    public string caseID;
    public int infosFound;
    public int cluesFound;
    public bool isCompleted;

    
}

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("Casos disponibles")]
    public CaseData[] allCases;

    private Dictionary<string, CaseProgress> progressDict = new Dictionary<string, CaseProgress>();
    private string lastCompletedCaseID;

    [HideInInspector] public string lastPlayedCaseID;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllProgress();
         
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -------------------------------------------------------------------
    // GET / CREATE PROGRESS
    // -------------------------------------------------------------------
    public CaseProgress GetProgress(string caseID)
    {
        if (string.IsNullOrEmpty(caseID))
            return null;

        if (!progressDict.ContainsKey(caseID))
        {
            progressDict[caseID] = new CaseProgress
            {
                caseID = caseID,
                infosFound = 0,
                cluesFound = 0,
                isCompleted = false
            };
        }

        return progressDict[caseID];
    }

    // -------------------------------------------------------------------
    // UPDATE PROGRESS
    // -------------------------------------------------------------------
    public void AddInfoFound(string caseID)
    {
        var progress = GetProgress(caseID);
        progress.infosFound++;
        SaveProgress(caseID);
    }

    public void AddClueFound(string caseID)
    {
        var progress = GetProgress(caseID);
        progress.cluesFound++;
        SaveProgress(caseID);
    }

    public void UpdateCaseProgress(string caseID, int infos, int clues, bool completed)
    {
        var progress = GetProgress(caseID);

        progress.infosFound = infos;
        progress.cluesFound = clues;
        progress.isCompleted = completed;

        if (completed)
            lastCompletedCaseID = caseID;

        SaveProgress(caseID);
       
    }

    public void MarkCaseCompleted(string caseID)
    {
        var progress = GetProgress(caseID);
        progress.isCompleted = true;
        lastCompletedCaseID = caseID;

        SaveProgress(caseID);
       
    }

    // -------------------------------------------------------------------
    // RANK SYSTEM
    // -------------------------------------------------------------------


   

    public bool AreAllCasesCompleted()
    {
        if (allCases == null || allCases.Length == 0) return false;

        foreach (var c in allCases)
        {
            if (c == null) return false;

            var p = GetProgress(c.caseID);
            if (p == null || !p.isCompleted)
                return false;
        }

        return true;
    }

    public int GetCompletedCasesCount()
    {
        int count = 0;
        if (allCases == null) return 0;

        foreach (var c in allCases)
        {
            if (c == null) continue;
            var p = GetProgress(c.caseID);
            if (p != null && p.isCompleted) count++;
        }

        return count;
    }

    public CaseData GetLastCompletedCase()
    {
        if (string.IsNullOrEmpty(lastCompletedCaseID))
            return null;

        foreach (var c in allCases)
        {
            if (c != null && c.caseID == lastCompletedCaseID)
                return c;
        }

        return null;
    }

    // -------------------------------------------------------------------
    // SAVE / LOAD
    // -------------------------------------------------------------------
    private void SaveProgress(string caseID)
    {
        var progress = GetProgress(caseID);

        PlayerPrefs.SetInt($"{caseID}_InfosFound", progress.infosFound);
        PlayerPrefs.SetInt($"{caseID}_CluesFound", progress.cluesFound);
        PlayerPrefs.SetInt($"{caseID}_Completed", progress.isCompleted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadAllProgress()
    {
        if (allCases == null) return;

        foreach (var c in allCases)
        {
            if (c == null || string.IsNullOrEmpty(c.caseID)) continue;

            var progress = GetProgress(c.caseID);

            progress.infosFound = PlayerPrefs.GetInt($"{c.caseID}_InfosFound", 0);
            progress.cluesFound = PlayerPrefs.GetInt($"{c.caseID}_CluesFound", 0);
            progress.isCompleted = PlayerPrefs.GetInt($"{c.caseID}_Completed", 0) == 1;

            progressDict[c.caseID] = progress;
        }
    }

    // -------------------------------------------------------------------
    // RESET
    // -------------------------------------------------------------------
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        progressDict.Clear();
        lastCompletedCaseID = null;
    }
}
