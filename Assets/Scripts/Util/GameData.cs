using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string UId;
    public string UName;
    public int coin;
    public int ball100Score;
    public int ClassicScore;
    public int shootingScore;
    public int IngameRewardLevel;
    public int IngameRewardScore;
    public int TotalStarCount;
    public bool SweetAim;
    public bool UseNoAds;
    public int SelectBall;
    public int SelectMonster;
    public int SelectCharBall;
    public int CurGameStage;
    public string GiftGetTime;
    public string GetCoinADsState;
    public bool isGiftTime;
    public bool isGetCoinTime;
    public string DailyTime;
    public int DailyCount;
    public bool isDailyAds;
    public bool isDailyRewardGet;
    public bool isDailyAdd;
    public List<int> ingameItems;
    public List<int> stageStars;
    public List<int> stageScores;
    public List<int> stageLocks;
    public List<int> stageChapterList;
    public List<int> charBallList;
    public List<int> housingStateList;

    
    public List<bool> Achievements;
    
    public void Print()
    {
        string deb = "";
        deb+=("UId = " + UId)+"\n";
        deb += ("UName = " + UName) + "\n";
        deb += ("coin = " + coin) + "\n";
        deb += ("ball100Score = " + ball100Score) + "\n";
        deb += ("ClassicScore = " + ClassicScore) + "\n";
        deb += ("ShootingScore = " + shootingScore) + "\n";
        deb += ("IngameRewardLevel = " + IngameRewardLevel) + "\n";
        deb += ("IngameRewardScore = " + IngameRewardScore) + "\n";
        deb += ("SweetAim = " + SweetAim) + "\n";
        deb += ("SelectBall = " + SelectBall) + "\n";
        deb += ("CurGameStage = " + CurGameStage) + "\n";
        deb += ("GiftGetTime = " + GiftGetTime) + "\n";
        deb += ("GetCoinADsState = " + GetCoinADsState) + "\n";
        deb += "\n";
        for (int idx = 0; idx < charBallList.Count; idx++)
        {
            deb += (string.Format("ballList[{0}] = {1}", idx, charBallList[idx]));
        }
        for (int idx = 0; idx < ingameItems.Count; idx++)
        {
            deb += (string.Format("ingameItems[{0}] = {1}", idx, ingameItems[idx]));
        }
        
        deb += "\n";
        for (int idx = 0; idx < stageStars.Count; idx++)
        {
            deb += (string.Format("stageStars[{0}] = {1}", idx, stageStars[idx]));
        }
        deb += "\n";
        for (int idx = 0; idx < stageScores.Count; idx++)
        {
            deb += (string.Format("stageScores[{0}] = {1}", idx, stageScores[idx]));
        }
        deb += "\n";
        for (int idx = 0; idx < stageLocks.Count; idx++)
        {
            deb += (string.Format("stageLocks[{0}] = {1}", idx, stageLocks[idx]));
        }
        deb += "\n";
        
        for (int idx = 0; idx < Achievements.Count; idx++)
        {
            deb += (string.Format("Achievements[{0}] = {1}", idx, Achievements[idx]));
        }

        Debug.Log(deb);
    }


}
