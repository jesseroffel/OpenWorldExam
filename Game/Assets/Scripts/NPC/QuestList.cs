﻿using UnityEngine;
using System.Collections.Generic;

public class QuestList : MonoBehaviour {
    public static QuestList QuestListObject;
    [Header("Quest Log Script Object")]
    public HandleQuestlog QuestLogScript;
    [Header("Inventory Reference")]
    public Inventory inventoryscript;
    [Header("Quest List Stats")]
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
            "A sticky situation",              // Quest Title
            new int[] { 1 },                    // Quest Type(s)
            true,                               // Quest Starter?
            false,                              // Quest Progresser?
            true,                               // Quest Completer?
            1,                                  // Quest Progression Start State
            0,                                  // Quest Total Progressions
            new int[] { 3 },                 // Other Quest ID unlock(s) at completion, leave empty if none
            false,                              // Requires Quest ID to start?
            new int[] { },                      // Required Quest ID(s) to start, leave empty if none
            true,                              // Requires Kill to complete?
            new int[] {10 },                      // Required Enemy kill amount to complete, leave empty if none
            new int[] { 1},                      // Required EnemyID(s) for kill tracking, leave empty if none
            false,                              // Requires item to complete?
            new int[] { },                      // Required item id(s) to complete, leave empty if none
            new string[] { "I hope you understand what the mayor said to you, that creature in the tower..","Anyway, you will need some killing experience, so I got a little task for you.","Close to this town there are slimes roaming around, you need to kill a few.. hmm let's say..","Yes, kill 10 slimes for me to prove you're skilled enough to begin this adventure" },      // Dialogue strings about quest
            "",                             // Dialogue when start conditions are not met
            "Remember, don't come back before you killed 10 slimes!",                            // Dialogue when quest is already started
            "Ah look at you, I can see you beating that creature before no time",                         // Dialogue when completing quest
            "Speak to all the people in town, some have tasks for you but they may need your trust first"                         // Dialogue after completion quest
        ));
        DatabaseQuests.Add(new QuestObject(
            DatabaseQuests.Count + 1,           // QuestID
            "The Lost Item",              // Quest Title
            new int[] { 1 },                    // Quest Type(s)
            true,                               // Quest Starter?
            false,                              // Quest Progresser?
            true,                               // Quest Completer?
            1,                                  // Quest Progression Start State
            0,                                  // Quest Total Progressions
            new int[] { 4 },                 // Other Quest ID unlock(s) at completion, leave empty if none
            false,                              // Requires Quest ID to start?
            new int[] { },                      // Required Quest ID(s) to start, leave empty if none
            false,                              // Requires Kill to complete?
            new int[] { },                      // Required Enemy kill amount to complete, leave empty if none
            new int[] { },                      // Required EnemyID(s) for kill tracking, leave empty if none
            true,                              // Requires item to complete?
            new int[] { 1 },                      // Required item id(s) to complete, leave empty if none
            new string[] { "Hey you, I have not seen you before..?", "Oh well, doesn't matter, I suppose..", "I need a guy like you for a small task!", "I lost my lovely sword yesterday and I can't go search for it myself.." , "Could you do it for me instead? You would help an old man right?" },      // Dialogue strings about quest
            "I can't give you this quest",                             // Dialogue when start conditions are not met
            "I trust you.. Please search for my sword",                            // Dialogue when quest is already started
            "Oh goodness, Thanks alot good man, I can't express my happyness right now!",                         // Dialogue when completing quest
            "I am sorry, I don't have anything to give you as a reward but I can say one thing... THANK YOU!!"                         // Dialogue after completion quest
        ));
        DatabaseQuests.Add(new QuestObject(
            DatabaseQuests.Count + 1,           // QuestID
            "Slimy start",                      // Quest Title
            new int[] { 2 },                    // Quest Type(s)
            true,                               // Quest Starter?
            false,                              // Quest Progresser?
            true,                               // Quest Completer?
            1,                                  // Quest Progression Start State
            0,                                  // Quest Total Progressions
            new int[] { 2 },                    // Other Quest ID unlock(s) at completion, leave empty if none
            true,                               // Requires Quest ID to start?
            new int[] { 1 },                    // Required Quest ID(s) to start, leave empty if none
            true,                               // Requires Kill to complete?
            new int[] { 5 },                    // Required Enemy kill amount to complete, leave empty if none
            new int[] { 1 },                    // Required EnemyID(s) for kill tracking, leave empty if none
            false,                              // Requires item to complete?
            new int[] { },                      // Required item id(s) to complete, leave empty if none
            new string[] { "Those slimes...", "Could you do me a favour?", "Can you kill 5 slimes just to show them who's boss?!" },      // Dialogue strings about quest
            "I don´t trust  you enough (Complete other quests)",                // Dialogue when start conditions are not met
            "Good luck, please come back when you'v killed at least 5 slimes!",                            // Dialogue when quest is already started
            "Thank you for helping me out, here have a small reward",           // Dialogue when completing quest
            "Ah great, you've killed 5 slimes... ( GAME OVER, THANKS FOR PLAYING )"                      // Dialogue after completion quest
        ));
        QuestCount = DatabaseQuests.Count;

        QuestListObject = this;

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

    public bool CheckInventory(QuestObject quest)
    {
        if (inventoryscript == null) { inventoryscript = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>(); }
        if (inventoryscript)
        {
            if (quest != null)
            {
                if (quest.m_RequiresItem)
                {
                    int[] id = quest.m_RequiredItemID;
                    int count = id.Length;
                    bool endcheck = true;
                    for (int i = 0; i < count; i++)
                    {
                        bool contains = inventoryscript.InventoryContains(id[i]);
                        if (!contains) { endcheck = false; }
                    }
                    if (endcheck) { return true; } else { return false; }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        } else
        {
            return false;
        }
    }

    public void SetQuestLogActive(int questid)
    {
        bool check = false;
        if (QuestLogScript == null) { QuestLogScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HandleQuestlog>();  }
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_QuestID == questid)
            {
                QuestLogScript.AddQuestToList(
                    activequest.m_QuestID,
                    activequest.m_QuestTitle,
                    activequest.m_QuestGivenDialogue,
                    activequest.m_RequiredItemID,
                    activequest.m_RequiredEnemyID,
                    activequest.m_RequiresKillAmount,
                    activequest.m_CurrentItemID,
                    activequest.m_CurrentKillAmount);
                check = true;
            }
        }
        if (check)
        {
            
        } else
        {
            Debug.Log("[QUESTLOG] ERROR: Couldn't add to Questlog, check quest");
        }
    }

    public void UpdateQuestLog(int questid)
    {
        if (QuestLogScript == null) { QuestLogScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HandleQuestlog>(); }
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_QuestID == questid)
            {
                QuestLogScript.UpdateLog(activequest.m_QuestID, activequest.m_CurrentItemID, activequest.m_CurrentKillAmount);
            }
        }
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
                    Debug.Log("[QUEST] Quest ID Active: " + quest.m_QuestID);
                    return true;
                }
            }
            
        }
        return false;
    }

    public bool FinishQuest(int questid)
    {
        if (inventoryscript == null) { inventoryscript = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>(); }
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_QuestID == questid)
            {
                activequest.m_QuestCompleted = true;
                activequest.m_QuestStarted = false;
                activequest.m_QuestRequirementsMet = false;

                bool checkred = activequest.m_RequiresItem;
                if (checkred)
                {
                    int[] items = activequest.m_CurrentItemID;
                    int count = items.Length;
                    for (int i = 0; i < count; i++)
                    {
                        inventoryscript.RemoveItem(activequest.m_CurrentItemID[i]);
                    }
                }
                
                QuestLogScript.RemoveQuestFromList(questid);
                CheckUnlock(questid);

                Debug.Log("[QUEST] Quest ID Completed: " + questid);
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

    public bool RegisterItemIDWithQuest(int questid)
    {
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_RequiresItem)
            {
                if (activequest.m_QuestID == questid)
                {
                    RegisterItemID(activequest.m_RequiredItemID[0]);
                    return true;
                }
            }

        }
        return false;
    }

    public bool CheckUnlock(int questid)
    {
        if (inventoryscript == null) { inventoryscript = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>(); }
        if (QuestLogScript == null) { QuestLogScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HandleQuestlog>(); }
        bool unlocked = false;
        string title = "";
        int[] requnlockcheckid;
        foreach (QuestObject unlockquest in DatabaseQuests)
        {
            if (unlockquest.m_RequiresQuestUnlock)
            {
                int QUC = unlockquest.m_RequiredQuestID.Length;
                requnlockcheckid = new int[QUC];
                for (int i = 0; i < QUC; i++)
                {
                    if (unlockquest.m_RequiredQuestID[i] == questid)
                    {
                        title = unlockquest.m_QuestTitle;
                        QuestLogScript.DisplayQuestUnlock(title);
                        unlocked = true;
                    }
                }
            }
        }
        if (unlocked) { return true; } else { return false; }
    }
   
    public bool RegisterItemID(int itemid)
    {
        if (inventoryscript == null) { inventoryscript = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>(); }
        if (QuestLogScript == null) { QuestLogScript = GameObject.FindGameObjectWithTag("HUD").GetComponent<HandleQuestlog>(); }
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
                        UpdateQuestLog(activequest.m_QuestID);
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

    public bool RegisterKillID(int enemyid)
    {
        bool RegisteredKill = false;
        foreach (QuestObject activequest in ActiveQuests)
        {
            if (activequest.m_RequiresKill)
            {
                // Check Enemy to quest, check reqenemy position in [], update currentkill at potion []
                int[] reqEnemyIDS = activequest.m_RequiredEnemyID;
                int countreqeids = reqEnemyIDS.Length;
                for (int i = 0; i < countreqeids; i++)
                {
                    if (reqEnemyIDS[i] == enemyid)
                    {
                        int killcount = activequest.m_CurrentKillAmount[i];
                        activequest.m_CurrentKillAmount[i] = killcount+1;
                        RegisteredKill = true;
                        UpdateQuestLog(activequest.m_QuestID);
                        Debug.Log("[QUEST] Registered Kill EnemyID: " + enemyid + " for Quest: " + activequest.m_QuestTitle);
                    }
                }
                
                // Check if quest is complete
                bool allkillscheck = true;
                if (activequest.m_CurrentKillAmount.Length > 1)
                {
                    for (int i = 0; i < countreqeids; i++)
                    {
                        if (activequest.m_CurrentKillAmount[i] != activequest.m_RequiresKillAmount[i])
                        {
                            allkillscheck = false;
                            break;
                        }
                    }
                }
                else
                {
                    if (activequest.m_CurrentKillAmount[0] != activequest.m_RequiresKillAmount[0])
                    {
                        allkillscheck = false;
                    }
                }
                // Handle Quest Completion
                if (allkillscheck)
                {
                    activequest.m_QuestRequirementsMet = true;
                    Debug.Log("[QUEST] " + activequest.m_QuestTitle + " has it's all requirements met!");
                }
            }
        }
        if (RegisteredKill) { return true; } else { return false; }
        
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
