using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatHandler : MonoBehaviour
{
    [SerializeField]
    StatCard entityStats;
    [SerializeField]
    int level = 1;
    [SerializeField]
    int exp = 0;
    public int Exp { get { return exp; } }

    public int ExpToNextLv {
        get { return (int)(Mathf.Pow(level+1, 3) / ((float)entityStats.growthSpeed / 10)); }
    }

    public int Level {
        get { return level; }
    }

    public string entityName {
        get { return entityStats.name; }
    }

    int maxHP {
        get { return Mathf.FloorToInt(((2 * entityStats.baseHP + hpBonuses) * level / 10f + level + 10 )* ((int)entityStats.hpProficiency*.1f)); }
    }
    public int MaxHP {
        get { return maxHP; }
    }
    int str {
        get { return Mathf.FloorToInt(((2 * entityStats.str + strBonuses) * level / 10f + 5) * (int)entityStats.strProficiency*.1f); }
    }
    public int Str {
        get { return str; }
    }
    int intl {
        get { return Mathf.FloorToInt(((2 * entityStats.intl + intlBonuses) * level / 10f + 5) * (int)entityStats.intlProficiency * .1f); }
    }
    public int Intl {
        get { return intl; }
    }
    int dex {
        get { return Mathf.FloorToInt(((2 * entityStats.dex + dexBonuses) * level / 10f + 5) * (int)entityStats.dexProficiency * .1f); }
    }
    public int Dex {
        get { return dex; }
    }
    int def {
        get { return Mathf.FloorToInt(((2 * entityStats.def + defBonuses) * level / 10f + 5) * (int)entityStats.defProficiency * .1f); }
    }
    public int Def {
        get { return def; }
    }

    int expYield {
        get { return (int)((float)entityStats.baseExpYield * (float)level / (float)7); }
    }
    public int ExpYield { get { return expYield; } }

    [SerializeField]
    int hpBonuses,strBonuses,intlBonuses,dexBonuses,defBonuses;

    public int curHP;

    AdventureMenuHandler aMH;
    EntityManager entityManager;



    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
        aMH = GameObject.Find("Game Manager").GetComponent<AdventureMenuHandler>();
        entityManager = GetComponent<EntityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (exp > ExpToNextLv) {
            exp -= ExpToNextLv;
            level++;
            aMH.NewLogMessage(string.Format("{0} leveled up!", entityName));
            aMH.NewLogMessage(string.Format("{0} is now level {1}!", entityName, level));
        }
    }



    public void TakeDamage(int baseDamage, int damageStat, int oppLevel) {
        int dmgToTake = Mathf.FloorToInt(((((2 * (float)oppLevel)) + 2) * (float)baseDamage * ((float)damageStat / (float)def)) / 15 + 2);
        if (dmgToTake > 0) { GetComponent<EntityManager>().regenCounter = 0; }
        curHP -= dmgToTake;
        if (dmgToTake >= 0) { aMH.NewLogMessage(string.Format("{0} took {1} damage!", entityStats.name, dmgToTake)); }
        else if(dmgToTake < 0) { aMH.NewLogMessage(string.Format("{0} healed {1}!", entityStats.name, dmgToTake)); }
        
    }

    public void TakeFlatDamage(int dmgToTake) {
        curHP -= dmgToTake;
        if (dmgToTake > 0) { aMH.NewLogMessage(string.Format("{0} took {1} damage!", entityStats.name, dmgToTake)); }
        else if (dmgToTake < 0) { aMH.NewLogMessage(string.Format("{0} healed {1}!", entityStats.name, Mathf.Abs(dmgToTake))); }
    }


    public void GainExp(int amtToGain) {
        exp += amtToGain;
        aMH.NewLogMessage(string.Format("{0} gained {1} experience points.", entityName, amtToGain));
    }

    public void GainLevel(int amtToGain) {
        level += amtToGain;
        exp = 0;
    }

    public void UpdateStats() {
        for (int i = 0; i < entityManager.EntityArmor.Length; i++) {
            hpBonuses += entityManager.EntityArmor[i] != null ? entityManager.EntityArmor[i].statBoosts[0] : 0;
            strBonuses += entityManager.EntityArmor[i] != null ? entityManager.EntityArmor[i].statBoosts[1] : 0;
            dexBonuses += entityManager.EntityArmor[i] != null ? entityManager.EntityArmor[i].statBoosts[2] : 0;
            intlBonuses += entityManager.EntityArmor[i] != null ? entityManager.EntityArmor[i].statBoosts[3] : 0;
            defBonuses += entityManager.EntityArmor[i] != null ? entityManager.EntityArmor[i].statBoosts[4] : 0;
        }
        
    }

}
