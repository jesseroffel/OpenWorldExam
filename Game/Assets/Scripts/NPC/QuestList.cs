﻿using LitJson;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class QuestList : MonoBehaviour {
    public int QuestCount = 0;
    public int ActiveQuestCount = 0;
    public int LastAccessedQuest = 0;
    private List<QuestObject> DatabaseQuests = new List<QuestObject>();
    private List<QuestObject> ActiveQuests = new List<QuestObject>();

    void Start () {

        //QuestData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Quests.json"));
        //ConstructQuestDatabase();
        DatabaseQuests.Add(new QuestObject(
            DatabaseQuests.Count + 1,           // QuestID
            "Sword quest",                      // Quest Title
            new int[] { 1 },                 // Quest Type(s)
            true,                               // Quest Starter?
            false,                              // Quest Progresser?
            true,                               // Quest Completer?
            1,                                  // Quest Progression Start State
            0,                                  // Quest Total Progressions
            new int[] { 0, 2 },                 // Other Quest ID unlock(s) at completion, leave empty if none
            false,                              // Requires Quest ID to start?
            new int[] { },                      // Required Quest ID(s) to start, leave empty if none
            false,                              // Requires Kill to complete?
            new int[] { },                      // Required Enemy kill amount to complete, leave empty if none
            new int[] { },                      // Required EnemyID(s) for kill tracking, leave empty if none
            true,                              // Requires item to complete?
            new int[] { 1 },                      // Required item id(s) to complete, leave empty if none
            new string[] { "Hey you, I haven't seen you before..?", "oh well doesn't matter I suppose..", "I need a guy like you for a small task!", "I lost my lovely sword yesterday and I can't go search for it myself.." , "Can you do it for me instead? You would help an old man right?" },      // Dialogue strings about quest
            "I can't give you this quest",                             // Dialogue when start conditions are not met
            "I trust you.. Please search for my sword",                            // Dialogue when quest is already started
            "Oh goodness, Thanks alot good man, I can't express my happyness right now!",                         // Dialogue when completing quest
            "I am sorry I haven't anything to give you as a reward but THANK YOU!!"                         // Dialogue after completion quest
        ));
        DatabaseQuests.Add(new QuestObject(
            DatabaseQuests.Count+1,             // QuestID
            "Kill quest",                       // Quest Title
            new int[] { 2 },                 // Quest Type(s)
            true,                               // Quest Starter?
            false,                              // Quest Progresser?
            true,                               // Quest Completer?
            1,                                  // Quest Progression Start State
            0,                                  // Quest Total Progressions
            new int[] { 0, 2 },                 // Other Quest ID unlock(s) at completion, leave empty if none
            true,                              // Requires Quest ID to start?
            new int[] { 1 },                      // Required Quest ID(s) to start, leave empty if none
            true,                              // Requires Kill to complete?
            new int[] { 5 },                      // Required Enemy kill amount to complete, leave empty if none
            new int[] { 1 },                      // Required EnemyID(s) for kill tracking, leave empty if none
            false,                              // Requires item to complete?
            new int[] { },                      // Required item id(s) to complete, leave empty if none
            new string[] { "Those slimes... I hate them", "Could you do me a favour?", "Can you kill 5 slimes just to show them who's boss?!" },      // Dialogue strings about quest
            "I don´t trust  you enough (Complete other quests)",                             // Dialogue when start conditions are not met
            "Good luck, I hope you can kill em all",                            // Dialogue when quest is already started
            "Thank you for helping me out, here have a small reward",                         // Dialogue when completing quest
            "Once again, thank you, see you around later."                         // Dialogue after completion quest
        ));
        QuestCount = DatabaseQuests.Count;
    }

    public bool CheckCompletedQuest(int questid)
    {
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_QuestID == questid)
            {
                if (activequest.m_QuestCompleted) { return true; } else { return false; }
            }
        }
        return false;
    }

    public QuestObject GetInformation(int questid)
    {
    
        foreach (QuestObject quest in DatabaseQuests)
        {
           if (quest.m_QuestID == questid)
            {
                LastAccessedQuest = questid;
                return quest;
            }
        }
        QuestObject obj = null;
        return obj;
    }

    public bool AddActiveQuest(int questid)
    {
        bool alreadyadded = false;
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_QuestID == questid)
            {
                Debug.LogWarning("[QUEST] Quest Already added!?");
                LastAccessedQuest = questid;
                return true;
            }
        }
        if (!alreadyadded)
        {
            foreach (QuestObject quest in DatabaseQuests)
            {
                if (quest.m_QuestID == questid)
                {
                    ActiveQuests.Add(quest);
                    LastAccessedQuest = questid;
                    ActiveQuestCount = ActiveQuests.Count;
                    Debug.Log("[QUEST] Set Active Quest: " + quest.m_QuestID);
                    return true;
                }
            }
            
        }
        return false;
    }

    public bool FinishQuest(int questid)
    {
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_QuestID == questid)
            {
                activequest.m_QuestCompleted = true;
                activequest.m_QuestStarted = false;
                activequest.m_QuestRequirementsMet = false;
                return true;
            }
        }
        return false;
    }

    public bool StartQuest(int questid)
    {
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_QuestID == questid)
            {
                activequest.m_QuestStarted = true;
                return true;
            }
        }
        return false;
    }

    public bool RegisterItemID(int itemid)
    {
        bool Registereditemid = false;
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_RequiresItem)
            {
                int[] itemsid = activequest.m_RequiredItemID;
                int countids = itemsid.Length;
                for (int i = 0; i < countids; i++)
                {
                    if(itemsid[i] == itemid)
                    {
                        activequest.m_CurrentItemID[i] = itemid;
                        Registereditemid = true;
                        Debug.Log("[QUEST] Registered itemid: " + itemid + " at listposition: " + i + " for Quest: " + activequest.m_QuestTitle);
                    }
                }

                bool allitemscheck = true;
                if (activequest.m_CurrentItemID.Length > 1)
                {
                    for (int i = 0; i < countids; i++)
                    {
                        if (activequest.m_CurrentItemID[i] != activequest.m_RequiredItemID[i])
                        {
                            allitemscheck = false;
                            break;
                        }
                    }
                } else {
                    if (activequest.m_CurrentItemID[0] != activequest.m_RequiredItemID[0])
                    {
                        allitemscheck = false;
                    }
                }
                
                if (allitemscheck)
                {
                    activequest.m_QuestRequirementsMet = true;
                    Debug.Log("[QUEST] " + activequest.m_QuestTitle + " has it's all requirements met!");
                }
            }
        }
        if (Registereditemid) { return true; } else { return false; }
    }

    /*void ConstructQuestDatabase()
    {
        for (int i = 0; i < QuestData.Count; i++)
        {
            QuestData[i]["questtypes"]
            List<int> questtypeslist = new List<int>();
            //questtypeslist.AddRange((int)QuestData[i]["questtypes"]);
            
            DatabaseQuests.Add(new QuestInformation(
                (int)QuestData[i]["id"],
                (string)QuestData[i]["title"],
                (int)QuestData[i]["questtype"],
                (bool)QuestData[i]["queststarter"],
                (bool)QuestData[i]["questprogresser"],
                (bool)QuestData[i]["questcompleter"],
                (int)QuestData[i]["questtotalprogressions"],
                (int)QuestData[i]["progresionquestid"],
                (int)QuestData[i]["questunlocks"],
                (bool)QuestData[i]["requiresquestunlock"],
                (int)QuestData[i]["requiredquestid"],
                (bool)QuestData[i]["requireskill"],
                (int)QuestData[i]["requireskillamount"],
                (int)QuestData[i]["requiredenemyid"],
                (bool)QuestData[i]["requiresitem"],
                (int)QuestData[i]["requireditemid"],
                (string)QuestData[i]["questdialogue"],
                (string)QuestData[i]["questdeny"],
                (string)QuestData[i]["questgiven"],
                (string)QuestData[i]["questcomplete"],
                (string)QuestData[i]["afterword"]

                ));   
    }*/
}
