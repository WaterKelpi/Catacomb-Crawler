using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AdventureMenuHandler : MonoBehaviour {

    bool isPaused;
    public bool IsPaused {
        get { return isPaused; }
    }
    [SerializeField]
    Slider playerHealthSlider;


    EntityStatHandler playerStats;
    FloorManager floorManager;
    InventoryManager playerInventory;
    EntityManager playerEntityManager;


    [SerializeField]
    RectTransform pauseHUD;


    int  curOthersSel,curOthersContextSel,curGroundSel,curGroundContextSel,curRestSel,curRestContextSel;

    [SerializeField]
    TextMeshProUGUI curFloor, curLevel, curHP;

    [SerializeField]
    TextMeshProUGUI gameLog;


    [Header("Pause Menu")]
    [SerializeField]//Main Panel
    RectTransform pausePanel;
    [SerializeField]//Text
    TextMeshProUGUI pauseMenuText, team1Hp, team2Hp, team3Hp, team4Hp, goldCounter, bellyGauge;
    int curMenuSelection;

    [Header("Moves")]
    [SerializeField]//Main Panel
    RectTransform movesPanel;
    [SerializeField]//Text
    TextMeshProUGUI movesText, movesContextText,movesSummaryText;
    int curMoveSel, curMoveContextSel;

    [Header("Inventory")]
    [SerializeField]//Main Panel
    RectTransform inventoryPanel;
    [SerializeField]//Sub Panels
    RectTransform invContextPanel;
    [SerializeField]//Text
    TextMeshProUGUI inventoryText, invContextText, invPageText;
    int curInvSelection, curInvContextSel, curInvPage;

    [Header("Stats")]
    [SerializeField]//Main Panel
    RectTransform statsPanel;
    //[SerializeField]//Sub Panels
    [SerializeField]//Text
    TextMeshProUGUI statsText;
    int curStatsPage, curStatsConextSel;
    bool statsContext;

    [Header("Other")]
    [SerializeField]//Main Panel
    RectTransform othersPanel;
    //[SerializeField]//Sub Panels
    [SerializeField]//Text
    TextMeshProUGUI othersText;


    [Header("Ground")]
    [SerializeField]//Main Panel
    RectTransform groundPanel;
    //[SerializeField]//Sub Panels
    [SerializeField]//Text
    TextMeshProUGUI groundText;

    [Header("Rest")]
    [SerializeField]//Main Panel
    RectTransform restPanel;
    //[SerializeField]//Sub Panels
    [SerializeField]//Text
    TextMeshProUGUI restText;


    public List<string> hiddenLog = new List<string>();

    [SerializeField]
    menuType curMenu;
    
    bool inputHeld,invContext;

    private void Awake() {
        playerStats = GameObject.Find("objPlayer").GetComponent<EntityStatHandler>();
        playerInventory = GameObject.Find("objPlayer").GetComponent<InventoryManager>();
        floorManager = GameObject.Find("Game Manager").GetComponent<FloorManager>();
        playerEntityManager = GameObject.Find("objPlayer").GetComponent<EntityManager>();
        ChangeMenu();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = isPaused ? 0 : 1;

        if (playerStats.MaxHP != playerHealthSlider.maxValue) { playerHealthSlider.maxValue = playerStats.MaxHP; }
        if (playerStats.curHP != playerHealthSlider.value) { playerHealthSlider.value = playerStats.curHP; }

        //Setting Cur HP
        curHP.text = "HP  " + playerStats.curHP.ToString().PadLeft(playerStats.MaxHP.ToString().Length, ' ') + "/" + playerStats.MaxHP.ToString();
        //Set Cur Floor
        curFloor.text = floorManager.FloorNum.ToString() + "F";
        //Set Cur Level
        curLevel.text = "Lv " + playerStats.Level.ToString();

        if (Input.GetKeyDown(KeyCode.R)) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }


        if (Input.GetButtonDown("Start") && GameObject.Find("objPlayer").GetComponent<EntityMovement>().CurTurn && !GameObject.Find("objPlayer").GetComponent<EntityMovement>().isMoving) {
            if (!isPaused) { isPaused = true; 
            }else if (isPaused) {
                if (curMenu == menuType.paused) {
                    isPaused = false;
                }
                
            }
        }

        if (isPaused) {
            if (pauseHUD.gameObject.activeInHierarchy == false) { pauseHUD.gameObject.SetActive(true); }
            pauseHUD.gameObject.SetActive(true);
            
            
            #region Menu Navigation
            
            if (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0) {
                inputHeld = false;
            }
            switch (curMenu) {
                case menuType.paused:
                    if (Input.GetAxisRaw("Vertical") != 0 && !inputHeld) {
                        inputHeld = true;
                        curMenuSelection -= 1 * (int)Mathf.Sign(Input.GetAxisRaw("Vertical"));
                    }
                    curMenuSelection = curMenuSelection < 0 ? 6 : curMenuSelection > 6 ? 0 : curMenuSelection;
                    if (Input.GetButtonDown("RightButton")) {
                        switch (curMenuSelection) {
                            //Moves
                            case 0:
                                curMenu = menuType.moves;
                                ChangeMenu();
                                break;
                                //Items
                            case 1:
                                curMenu = menuType.inventory;
                                ChangeMenu();
                                break;
                                //Stats
                            case 2:
                                curMenu = menuType.stats;
                                ChangeMenu();
                                break;
                                //Others
                            case 3:
                                curMenu = menuType.others;
                                ChangeMenu();
                                break;
                                //Ground
                            case 4:
                                curMenu = menuType.ground;
                                ChangeMenu();
                                break;
                                //Rest
                            case 5:
                                curMenu = menuType.rest;
                                ChangeMenu();
                                break;
                                //Exit
                            case 6:
                                isPaused = false;
                                break;
                        }
                    }
                    break;
                case menuType.moves:
                    if (Input.GetAxisRaw("Vertical") != 0 && !inputHeld) {
                        inputHeld = true;
                        curMoveSel -= 1 * (int)Mathf.Sign(Input.GetAxisRaw("Vertical"));
                    }
                    curMoveSel = curMoveSel < 0 ? playerEntityManager.SpecialMoves.Count - 1 : curMoveSel > playerEntityManager.SpecialMoves.Count - 1 ? 0 : curMoveSel;
                    if (Input.GetButtonDown("RightButton")) {
                        if (playerEntityManager.UseSMove(curMoveSel, GameObject.Find("objPlayer").GetComponent<EntityMovement>().CurDirection)) {
                            GameObject.Find("objPlayer").GetComponent<EntityMovement>().canMove = false;
                            CloseMenu();
                        }
                    }

                    if (Input.GetButtonDown("BottomButton")) {
                        ChangeMenu();
                        curMenu = menuType.paused;
                    }
                    break;
                case menuType.inventory:
                    if (Input.GetAxisRaw("Vertical") != 0 && !inputHeld) {
                        if (!invContext) {
                            inputHeld = true;
                            curInvSelection -= 1 * (int)Mathf.Sign(Input.GetAxisRaw("Vertical"));
                        }
                        else {
                            inputHeld = true;
                            curInvContextSel -= 1 * (int)Mathf.Sign(Input.GetAxisRaw("Vertical"));
                        }
                        
                    }
                    if (Input.GetAxisRaw("Horizontal") != 0 && !inputHeld) {
                        if (!invContext) {
                            inputHeld = true;
                            curInvPage += 1 * (int)Mathf.Sign(Input.GetAxisRaw("Horizontal"));
                            curInvSelection += 10 * (int)Mathf.Sign(Input.GetAxisRaw("Horizontal"));
                            if (curInvSelection < 0) { curInvSelection = 0; }
                            if (curInvSelection > playerInventory.Inventory.Count) { curInvSelection = playerInventory.Inventory.Count; }
                        }

                    }
                    curInvPage = Mathf.Clamp(curInvPage, 0, Mathf.Clamp(playerInventory.Inventory.Count/10,0,2));
                    //Make sure inventory panel is being shown
                    if (inventoryPanel.gameObject.activeInHierarchy == false) { inventoryPanel.gameObject.SetActive(true); }
                    //Makesure inventory context panel is being shown when it's supposed to be
                    if (invContext && invContextPanel.gameObject.activeInHierarchy == false) { invContextPanel.gameObject.SetActive(true); }
                    else if (!invContext && invContextPanel.gameObject.activeInHierarchy == true) { invContextPanel.gameObject.SetActive(false); }
                    //Loop inventory panel selection
                    if (curInvSelection < 0 + (curInvPage * 10)) {
                        if (playerInventory.Inventory.Count >= 10 + (curInvPage * 10)) {
                            curInvSelection = 9 + (curInvPage * 10);
                        }
                        else {
                            curInvSelection = playerInventory.InventoryCount.Count - 1;
                        }
                    }
                    if (curInvSelection > 9 + (curInvPage * 10) || curInvSelection > playerInventory.Inventory.Count - 1) {
                        curInvSelection = 0 + (curInvPage * 10);
                    }
                    if (Input.GetButtonDown("BottomButton")) {
                        if (!invContext) {
                            curMenu = menuType.paused;
                            ChangeMenu();
                        }
                        else {
                            invContext = false;
                        }
                        
                    }
                    //Right Button Clicked
                    if (Input.GetButtonDown("RightButton")) {
                        //Out of Context Menu
                        if (playerInventory.Inventory.Count == 0) {
                            break;
                        }
                        if (!invContext) {
                            invContext = true;
                            curInvContextSel = 0;
                        }
                        else { //Inside Context Menu
                            string itemName = playerInventory.Inventory[curInvSelection].itemName;
                            if (invContextText.text.Contains(">Consume")){//Try to Eat
                                if (playerEntityManager.ConsumeItem(curInvSelection)) {
                                    NewLogMessage(playerStats.entityName + " consumed " + itemName);
                                    invContext = false;
                                    CloseMenu();
                                    GameObject.Find("objPlayer").GetComponent<EntityMovement>().EndTurn();
                                }
                            }
                            if (invContextText.text.Contains(">Equip")) {
                                if (playerEntityManager.EquipItem(curInvSelection)) {
                                    NewLogMessage(playerStats.entityName + " equiped " + itemName);
                                    invContext = false;
                                }
                            }
                            if (invContextText.text.Contains(">Unequip")) {
                                if (playerEntityManager.UnequipItem(curInvSelection)) {
                                    NewLogMessage(playerStats.entityName + " unequiped " + itemName);
                                }
                            }
                            if (invContextText.text.Contains(">Throw")) {
                                EntityMovement pm = GameObject.Find("objPlayer").GetComponent<EntityMovement>();
                                if (playerEntityManager.ThrowItem(curInvSelection,pm.CurDirection)) {
                                    pm.canMove = false;
                                    invContext = false;
                                    CloseMenu();
                                }
                            }
                            if (invContextText.text.Contains(">Exit")) {
                                invContext = false;
                            }
                        }

                    }
                    break;
                case menuType.stats:
                    if (Input.GetButtonDown("BottomButton")) {
                        ChangeMenu();
                        curMenu = menuType.paused;
                    }
                    if (Input.GetAxisRaw("Horizontal") != 0 && !inputHeld) {
                        if (!statsContext) {
                            inputHeld = true;
                            curStatsPage += 1 * (int)Mathf.Sign(Input.GetAxisRaw("Horizontal"));
                            
                            /*if (curInvSelection < 0) { curInvSelection = 0; }
                            if (curInvSelection > playerInventory.Inventory.Count) { curInvSelection = playerInventory.Inventory.Count; }*/
                        }

                    }
                    curStatsPage = Mathf.Clamp(curStatsPage, 0, 1);
                    break;
                case menuType.others:
                    if (Input.GetButtonDown("BottomButton")) {
                        ChangeMenu();
                        curMenu = menuType.paused;
                    }
                    break;
                case menuType.ground:
                    if (Input.GetButtonDown("BottomButton")) {
                        ChangeMenu();
                        curMenu = menuType.paused;
                    }
                    break;
                case menuType.rest:
                    if (Input.GetButtonDown("BottomButton")) {
                        ChangeMenu();
                        curMenu = menuType.paused;
                    }
                    break;
            }
            #endregion

            #region Menu Drawing
            //Handle Menu Drawing
            switch (curMenu) {
                case menuType.paused:
                    if (pausePanel.gameObject.activeInHierarchy == false) { pausePanel.gameObject.SetActive(true); }
                    team1Hp.text = playerStats.entityName + "\t" + playerStats.curHP.ToString().PadLeft(playerStats.MaxHP.ToString().Length, ' ') + "/" + playerStats.MaxHP.ToString();
                    goldCounter.text = "Money:" + playerInventory.Gold.ToString().PadLeft(17, ' ') + "G";
                    bellyGauge.text = "Belly: " + playerEntityManager.curBelly + "/" + playerEntityManager.maxBelly;
                    pauseMenuText.text = "";
                    //Draw Options
                    for (int i = 0; i < 7; i++) {
                        if (i == curMenuSelection) { pauseMenuText.text += ">"; } else { pauseMenuText.text += "  "; }
                        switch (i) {
                            case 0:
                                pauseMenuText.text += "Moves";
                                break;
                            case 1:
                                pauseMenuText.text += "Inventory";
                                break;
                            case 2:
                                pauseMenuText.text += "Stats";
                                break;
                            case 3:
                                pauseMenuText.text += "Others";
                                break;
                            case 4:
                                pauseMenuText.text += "Ground";
                                break;
                            case 5:
                                pauseMenuText.text += "Rest";
                                break;
                            case 6:
                                pauseMenuText.text += "Exit";
                                break;
                            default:
                                pauseMenuText.text += "ERROR";
                                break;
                        }
                        pauseMenuText.text += "\n";
                    }
                    break;
                case menuType.moves:
                    if (movesPanel.gameObject.activeInHierarchy == false) { movesPanel.gameObject.SetActive(true); }
                    if (playerEntityManager.SpecialMoves.Count > 0) {
                        movesText.text = "";
                        for (int i = 0; i < playerEntityManager.SpecialMoves.Count; i++) {
                            if (i == curMoveSel) { movesText.text += ">"; }
                            movesText.text += playerEntityManager.SpecialMoves[i].name;
                            movesText.text += i == (playerEntityManager.SpecialMoves.Count - 1) ? "" : "\n";
                        }
                    }else {
                        movesText.text = "Currently under construction";
                    }
                    
                    break;
                case menuType.inventory:
                    invPageText.text = "Page: " + (curInvPage+1) + "/" + Mathf.Clamp(Mathf.CeilToInt((float)playerInventory.Inventory.Count / 10),1,3).ToString();
                    if (playerInventory.Inventory.Count == 0) { inventoryText.text = ""; break; }
                        inventoryText.text = "";
                        for (int i = 0 + (curInvPage * 10); i < (playerInventory.Inventory.Count < 10+(curInvPage * 10) ? playerInventory.Inventory.Count : 10 + (curInvPage * 10)); i++) {
                            if (i == curInvSelection) { inventoryText.text += ">"; }
                            inventoryText.text += playerInventory.Inventory[i].itemName + "\t" + (playerInventory.InventoryCount[i] != 1 ? playerInventory.InventoryCount[i].ToString() : "");
                            if (i < 9 + (curInvPage * 10)) { inventoryText.text += "\n"; }
                        }
                    //Context Options
                    if (invContext) {
                        invContextText.text = "";
                        if (curInvContextSel == 0) { invContextText.text += ">"; }
                        invContextText.text += playerInventory.Inventory[curInvSelection].consumable && !invContextText.text.Contains("Consume") ?
                            "Consume" : playerInventory.Inventory[curInvSelection].throwable && !invContextText.text.Contains("Throw") ?
                            "Throw" : playerInventory.Inventory[curInvSelection].useable && !invContextText.text.Contains("Use") ?
                            "Use" : playerInventory.Inventory[curInvSelection].equipable && !invContextText.text.Contains("Equip") ?
                            "Equip" : !invContextText.text.Contains("Exit") ? "Exit" : "";
                        invContextText.text += "\n";
                        if (curInvContextSel == 1) { invContextText.text += ">"; }
                        invContextText.text += playerInventory.Inventory[curInvSelection].throwable && !invContextText.text.Contains("Throw") ?
                            "Throw" : playerInventory.Inventory[curInvSelection].useable && !invContextText.text.Contains("Use") ?
                            "Use" : playerInventory.Inventory[curInvSelection].equipable && !invContextText.text.Contains("Equip") ?
                            "Equip" : !invContextText.text.Contains("Exit") ? "Exit" : "";
                        invContextText.text += "\n";
                        if (curInvContextSel == 2) { invContextText.text += ">"; }
                        invContextText.text += playerInventory.Inventory[curInvSelection].useable && !invContextText.text.Contains("Use") ?
                            "Use" : playerInventory.Inventory[curInvSelection].equipable && !invContextText.text.Contains("Equip") ?
                            "Equip" : !invContextText.text.Contains("Exit") ? "Exit" : "";
                        invContextText.text += "\n";
                        if (curInvContextSel == 3) { invContextText.text += ">"; }
                        invContextText.text += playerInventory.Inventory[curInvSelection].equipable && !invContextText.text.Contains("Equip") ?
                            "Equip" : !invContextText.text.Contains("Exit") ? "Exit" : "";
                        invContextText.text += "\n";
                        if (curInvContextSel == 4) { invContextText.text += ">"; }
                        invContextText.text += !invContextText.text.Contains("Exit") ? "Exit" : "";

                        if (playerInventory.InventoryEquip[curInvSelection] == true) {
                            invContextText.text = invContextText.text.Replace("Equip", "Unequip");
                        }
                            
                            
                    }

                    break;
                case menuType.stats:
                    if (statsPanel.gameObject.activeInHierarchy == false) { statsPanel.gameObject.SetActive(true); }
                    statsText.text = "";
                    switch (curStatsPage) {
                        case 0:
                            statsText.text += "Level:    " + playerStats.Level + "\n";
                            statsText.text += "Exp To Next Level: " + (playerStats.ExpToNextLv - playerStats.Exp) + "\n";
                            statsText.text += "HP:    " + playerStats.curHP + "/" + playerStats.MaxHP + "\n";
                            statsText.text += "Strength:    " + playerStats.Str + "\n";
                            statsText.text += "Dexterity:    " + playerStats.Dex + "\n";
                            statsText.text += "Intelligence:    " + playerStats.Intl + "\n";
                            statsText.text += "Defense:    " + playerStats.Def + "\n";
                            statsText.text += "Speed:    " + playerEntityManager.speed *100 + "%";
                            break;
                        case 1:
                            statsText.text += "Head: " + (playerEntityManager.EntityArmor[0] != null ? playerEntityManager.EntityArmor[0].itemName:"") + "\n";
                            statsText.text += "Chest: " + (playerEntityManager.EntityArmor[1] != null ? playerEntityManager.EntityArmor[1].itemName : "") + "\n";
                            statsText.text += "Hands: " + (playerEntityManager.EntityArmor[2] != null ? playerEntityManager.EntityArmor[2].itemName : "") + "\n";
                            statsText.text += "Legs: " + (playerEntityManager.EntityArmor[3] != null ? playerEntityManager.EntityArmor[3].itemName : "") + "\n";
                            statsText.text += "Feet: " + (playerEntityManager.EntityArmor[4] != null ? playerEntityManager.EntityArmor[4].itemName : "") + "\n";
                            statsText.text += "Accessory 1: " + (playerEntityManager.EntityArmor[5] != null ? playerEntityManager.EntityArmor[5].itemName : "") + "\n";
                            statsText.text += "Accessory 2: " + (playerEntityManager.EntityArmor[6] != null ? playerEntityManager.EntityArmor[6].itemName : "") + "\n";
                            statsText.text += "Weapons: " + (playerEntityManager.EntityArmor[7] != null ? playerEntityManager.EntityArmor[7].itemName : "Unarmed") + "\n";
                            break;
                        default:
                            statsText.text = "You shouldn't be seeing this, it done broke";
                            break;

                    }
                    
                    break;
                case menuType.others:
                    if (othersPanel.gameObject.activeInHierarchy == false) { othersPanel.gameObject.SetActive(true); }
                    break;
                case menuType.ground:
                    if (groundPanel.gameObject.activeInHierarchy == false) { groundPanel.gameObject.SetActive(true); }
                    groundText.text = "";
                    if (playerEntityManager.ObjOntopOf != null) { groundText.text = string.Format("There is a {0} at your feet", playerEntityManager.ObjOntopOf.GetComponent<ItemScript>().Item.itemName); }
                    else { groundText.text = "There is nothing at your feet"; }
                    break;
                case menuType.rest:
                    if (restPanel.gameObject.activeInHierarchy == false) { restPanel.gameObject.SetActive(true); }
                    break;
            }
            #endregion

        }
        else {
            if (pauseHUD.gameObject.activeInHierarchy == true) { pauseHUD.gameObject.SetActive(false); }
        }



    }


    public void NewLogMessage(string newMsg) {
        hiddenLog.Add(newMsg);
        gameLog.text = "";
        for (int i = 3; i > 0; i--) {
            if (hiddenLog.Count - i < 0) {
                gameLog.text += " \n";
                continue; }
            gameLog.text += hiddenLog[hiddenLog.Count - i]+ "\n";
        }
    }

    void ChangeMenu() {
        pausePanel.gameObject.SetActive(false);
        movesPanel.gameObject.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
        statsPanel.gameObject.SetActive(false);
        othersPanel.gameObject.SetActive(false);
        groundPanel.gameObject.SetActive(false);
        restPanel.gameObject.SetActive(false);
    }

    void CloseMenu() {
        ChangeMenu();
        curMenu = menuType.paused;
        isPaused = false;
    }
}
