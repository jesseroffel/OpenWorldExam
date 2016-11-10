﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CombatSystem : MonoBehaviour {
    [Header("Player Objects")]
    public FirstPersonControler FirstPersonControlerScript;
    public Animator PlayerAnimator;
    public Player_Health PlayerStats;
    [Header("Combat Prefabs")]
    public Transform HitRegBlock;
    public GameObject ArrowPrefab;
    public GameObject MagicMisslePrefab;
    public GameObject AoEPrefab;

    [Header("Player GameObjects")]
    public Transform ArrowSpawn;
    public Transform MagicMissleSpawn;
    public Transform MagicAreaOfEffectSpawn;
    public GameObject SwordModel;
    public GameObject StaffModel;
    public GameObject BowModel;
    //public Transform HandPosition;
    //public Transform BackPosition01;
    //public Transform BackPosition02;

    [Header("Attack Settings")]
    public float Attack01Swing = 0.5f;
    public float AttackBuildup = 0.5f;
    public int MagicSpell = 1;
    
    private int maxspells = 0;
    private float spelldowncool = 0;

    private float propulsionForce = 10.0f;
    private float NextAttack = 0.0f;
    private float WaitForSpawn = 0.3f;
    private float AttackTime = 0.0f;
    private bool PrepareAttack = false;
    private bool Attacking = false;
    private int CombatState = 1;
    private int AttackOrder = 0;

    //Switching
    public float SwitchSpeed = 1.0f;
    private float SwitchDisable = 0;
    private bool FinishSwitch = false;
    private bool SwitchChosen = false;
    private int SwitchChosenOption = 0;
    private int[] StyleOrder = { 2, 1, 3 };

    private Spell currentspell;
    private float MagicSwitchDelay = 0;
    private float DelayAdd = 1.5f;

    // UI
    [Header("Combat Switch UI")]
    public GameObject SwitchCombatPanel;
    private CombatSwitchUI CombatSwitchUIScript;

    [Header("Magic Switch UI")]
    public GameObject SwitchMagicSpellWindow;
    private ChangeSpellUI ChangeSpellUI;

    [Header("Bow Strength UI")]
    public Image StrengthBar;
    public GameObject StrengthPanel;

    private float BowStrengh = 0;
    private bool HoldingDown = false;
    private bool WindowOpen = false;

    private float AttackPower = 15.0f;

    private Quaternion SwordRot = new Quaternion(50, 0, 0, 0);

    enum CombatStyle { NoCombat = 0, Melee = 1, Range = 2, Magic = 3 };
    enum MagicStyle { None = 0, Missle = 1, AoE = 2}

    // Use this for initialization
    void Start() {
        //myTransform = gameObject.position;
        if (PlayerAnimator == null) { Debug.LogError("Animator 'PlayerAnimator' is null, set reference"); }
        if (HitRegBlock == null) { Debug.LogError("Transform 'HitRegBlock' is null, set reference"); }
        if (FirstPersonControlerScript == null) { Debug.LogError("Player_move 'playermovescript' is null, set reference"); }
        if (StrengthBar == null) { Debug.LogError("Text 'StrenghText' is null, set reference"); }
        if (StrengthPanel == null) { Debug.LogError("GameObject 'StrenghPanel' is null, set reference"); }

        if (ChangeSpellUI == null) { ChangeSpellUI = SwitchMagicSpellWindow.GetComponent<ChangeSpellUI>(); }

        maxspells = Spellbook.SpellbookObject.AmountSpells;
        currentspell = Spellbook.SpellbookObject.GetSpellByID(MagicSpell);
        ChangeSpellUI.SetSpellName(currentspell.SpellName);
        ChangeSpellUI.SetSpellImage(currentspell.Sprite);
    }
	
	// Update is called once per frame
	void Update () {

	    if (CrossPlatformInputManager.GetButton("Fire1") && Time.time > NextAttack)
        {
            if (FirstPersonControlerScript.GetInConversation()) {
                //Debug.Log("Conversation confirm, make animation of this!");
            } else
            {
                SetAttackAnimation();
            }
        }
        if (CrossPlatformInputManager.GetButtonUp("Fire1"))
        {
            HoldingDown = false;
        }

        if (CrossPlatformInputManager.GetButtonDown("SwitchSpell") && CombatState == (int)CombatStyle.Magic && Time.time > MagicSwitchDelay)
        {
            SwitchSpell();
        }

        if (CrossPlatformInputManager.GetButton("SwitchCombat") && Time.time > SwitchDisable)
        {
            SwitchCombatStyle();
        }

        if (CrossPlatformInputManager.GetButtonUp("SwitchCombat") )
        {
            WindowOpen = false;
            SwitchCombatPanel.SetActive(false);
        }

        if (PrepareAttack)
        {
            // Melee
            if (Time.time > AttackTime)
            {
                Attacking = true;
                if (CombatState != (int)CombatStyle.Range) { PrepareAttack = false; }
                NextAttack = Time.time + Attack01Swing;
                //Vector3 Addpos = transform.position + (transform.forward);
                
                switch (CombatState)
                {
                    case (int)CombatStyle.Melee:
                        Transform HitDec = (Transform)Instantiate(HitRegBlock, transform.position + (transform.forward), transform.rotation);
                        HitDec.transform.parent = transform;
                        switch (AttackOrder)
                        {
                            case 1:
                                
                                HitDec.transform.position += new Vector3(0, 0.4f, 0);
                                HitDec.GetComponent<HitRegistrator>().SetSettings(1,Attack01Swing, AttackPower, transform.forward * propulsionForce);
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                        }
                        break;
                    case (int)CombatStyle.Range:
                        if (HoldingDown)
                        {
                            if (StrengthPanel) {
                                if (StrengthPanel.activeSelf == false) { StrengthPanel.SetActive(true); }
                            }
                            string Text = "";
                            if (BowStrengh < 1) {
                                BowStrengh += 0.01f;
                                if (BowStrengh > 1) { BowStrengh = 1; }
                                
                            }
                            if (StrengthBar) { StrengthBar.fillAmount = BowStrengh; }
                        } else
                        {
                            PrepareAttack = false;
                            GameObject Projectile = Instantiate(ArrowPrefab);
                            Projectile.transform.position = ArrowSpawn.position;
                            Projectile.transform.rotation = ArrowSpawn.rotation;

                            Rigidbody rb = Projectile.GetComponent<Rigidbody>();
                            rb.velocity = (ArrowSpawn.forward * 50) * BowStrengh;

                            Projectile.GetComponent<HitRegistrator>().SetSettings(2, 10, 10, transform.forward * propulsionForce);
                            if (StrengthPanel) { StrengthPanel.SetActive(false); }
                            BowStrengh = 0;
                            FirstPersonControlerScript.SetSlowWalk(false);
                        }
                        
                        break;
                    case (int)CombatStyle.Magic:
                        if (spelldowncool == 1.0f) {
                            int type = currentspell.SpellType;

                            if (PlayerStats.Mana >= currentspell.ManaCost)
                            {
                                spelldowncool = 0;
                                PlayerStats.ChangeMana(-currentspell.ManaCost);
                                switch (type)
                                {
                                    case 1:
                                        DelayAdd = 0.01f;
                                        GameObject Missle = Instantiate(MagicMisslePrefab);
                                        Missle.transform.position = MagicMissleSpawn.position;
                                        Missle.transform.rotation = MagicMissleSpawn.rotation;
                                        Missle.GetComponent<HitRegistrator>().SetSettings(
                                            3,
                                            currentspell.AliveTime,
                                            currentspell.Change,
                                            transform.forward * propulsionForce,
                                            currentspell.ID);
                                        break;
                                    case 2:
                                        DelayAdd = 0.005f;
                                        GameObject AoE = Instantiate(AoEPrefab);
                                        AoE.transform.position = MagicAreaOfEffectSpawn.position;
                                        AoE.transform.rotation = MagicAreaOfEffectSpawn.rotation;
                                        Vector3 empty = new Vector3(0, 0, 0);
                                        AoE.GetComponent<HitRegistrator>().SetSettings(
                                            3,
                                            currentspell.AliveTime,
                                            currentspell.Change,
                                            empty,
                                            currentspell.ID);
                                        break;
                                    case 3:
                                        PlayerStats.health += currentspell.Change;
                                        break;
                                }
                            }
                        }
                        
                        break;
                }
            }
        }
        if (Attacking)
        {
            if (Time.time > NextAttack)
            {
                Attacking = false;
                AttackOrder = 0;
                if (FirstPersonControlerScript) { FirstPersonControlerScript.CanMove = true; }
            }
        }

        if (spelldowncool != 1)
        {
            spelldowncool += DelayAdd;
            if (spelldowncool > 1) { spelldowncool = 1; }
            ChangeSpellUI.SetFill(spelldowncool);
        }
    }

    void SetCombatState(int value)
    {
        CombatState = value;
    }

    void SwitchCombatStyle()
    {
        bool SwitchCheck = false;
        int oldstyle = CombatState;
        int chosen = 0;
        if (SwitchCombatPanel)
        {
            if (!WindowOpen) {
                WindowOpen = true;
                SwitchCombatPanel.SetActive(true);
            }
        }
        if (CrossPlatformInputManager.GetButton("SwitchLeft"))
        {
            SwitchCheck = true;
            SwitchDisable = Time.time + SwitchSpeed;
            CombatState = StyleOrder[0];
            int mid = StyleOrder[1];
            int left = StyleOrder[0];
            StyleOrder[0] = mid;
            StyleOrder[1] = left;
            chosen = 1;
        }
        //if (CrossPlatformInputManager.GetButton("SwitchMiddle"))
        //{
        //    CombatState = StyleOrder[1];
        //}
        if (CrossPlatformInputManager.GetButton("SwitchRight"))
        {
            SwitchCheck = true;
            SwitchDisable = Time.time + SwitchSpeed;
            CombatState = StyleOrder[2];
            int mid = StyleOrder[1];
            int right = StyleOrder[2];
            StyleOrder[2] = mid;
            StyleOrder[1] = right;
            chosen = 3;
        }

        //Debug.Log("Left: " + StyleOrder[0] + " Mid: " + StyleOrder[1] + " Right: " + StyleOrder[2]);

        //CombatState++;
        //if (CombatState == 4) { CombatState = 1; }

        //Set old weapon inactive
        if (SwitchCheck)
        {
            switch (oldstyle)
            {
                case (int)CombatStyle.Melee:
                    SwordModel.SetActive(false);
                    break;
                case (int)CombatStyle.Range:
                    BowModel.SetActive(false);
                    break;
                case (int)CombatStyle.Magic:
                    StaffModel.SetActive(false);
                    ChangeSpellUI.SetMagicMode(false);
                    break;
                default:
                    Debug.LogWarning("[PLAYER] Invalid combatstyle, Model SetActive(false) failed");
                    break;
            }
            //Set new weapon active
            switch (CombatState)
            {
                case (int)CombatStyle.Melee:
                    SwordModel.SetActive(true);
                    Debug.Log("[PLAYER] Combat: Melee Mode");
                    break;
                case (int)CombatStyle.Range:
                    BowModel.SetActive(true);
                    Debug.Log("[PLAYER] Combat: Range Mode");
                    break;
                case (int)CombatStyle.Magic:
                    StaffModel.SetActive(true);
                    ChangeSpellUI.SetMagicMode(true);
                    Debug.Log("[PLAYER] Combat: Magic Mode");
                    break;
                default:
                    Debug.LogWarning("[PLAYER] Invalid combatstyle, Model SetActive(true) failed");
                    break;
            }
        }
        if (WindowOpen && SwitchCheck) {
            if (SwitchCombatPanel)
            {
                if (CombatSwitchUIScript == null)
                {
                    CombatSwitchUIScript = SwitchCombatPanel.GetComponent<CombatSwitchUI>();
                    CombatSwitchUIScript.SwitchStyles(chosen);
                }
                else
                {
                    CombatSwitchUIScript.SwitchStyles(chosen);
                }
                chosen = 0;
                WindowOpen = false;
                SwitchCombatPanel.SetActive(false);
            }
        }
    }

    void SwitchSpell()
    {
        MagicSpell++;
        if (MagicSpell > maxspells)
        {
            MagicSpell = 1;
        }
        MagicSwitchDelay = Time.time + 0.5f;
        currentspell = Spellbook.SpellbookObject.GetSpellByID(MagicSpell);
        ChangeSpellUI.SetSpellName(currentspell.SpellName);
        ChangeSpellUI.SetSpellImage(currentspell.Sprite);
    }

    void SetAttackAnimation()
    {
        NextAttack = Time.time + AttackBuildup;
        HoldingDown = true;
        PrepareAttack = true;
        //if (FirstPersonControlerScript) { FirstPersonControlerScript.CanMove = false; }
        //Set animation
        if (CombatState == 1)
        {
            AttackOrder++;
            AttackTime = Time.time + WaitForSpawn;
            switch (AttackOrder)
            {
                case 1:
                    PlayerAnimator.SetTrigger("AttackMelee01Trigger");
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }
        if (CombatState == 2)
        {
            FirstPersonControlerScript.SetSlowWalk(true);
        }
    }
}
