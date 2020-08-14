using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatCard", menuName = "New Stat Card")]


public class StatCard : ScriptableObject
{
    public int baseHP;
    public statProficiency hpProficiency = statProficiency.average;
    public int str;
    public statProficiency strProficiency = statProficiency.average;
    public int dex;
    public statProficiency dexProficiency = statProficiency.average;
    public int intl;
    public statProficiency intlProficiency = statProficiency.average;
    public int def;
    public statProficiency defProficiency = statProficiency.average;

    public int baseExpYield;

    public growthSpeeds growthSpeed;

    public void GainExp() { }
}
