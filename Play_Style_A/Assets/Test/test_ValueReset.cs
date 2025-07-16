using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_ValueReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteKey("mutateNum");
        PlayerPrefs.DeleteKey("mutateRow");
        PlayerPrefs.DeleteKey("runRatio");
        PlayerPrefs.DeleteKey("v1Ratio");
        PlayerPrefs.DeleteKey("h0Ratio");
        PlayerPrefs.DeleteKey("forwardXBias");
        PlayerPrefs.DeleteKey("isSameSignXZ");
        PlayerPrefs.DeleteKey("jumpRatio");
        PlayerPrefs.DeleteKey("coverageDeg");
        PlayerPrefs.DeleteKey("itemDeg");
        PlayerPrefs.DeleteKey("interactionDeg");
        PlayerPrefs.DeleteKey("itemPri");
        PlayerPrefs.DeleteKey("decisionVel");
        PlayerPrefs.DeleteKey("directionAcc");
        PlayerPrefs.DeleteKey("avoidInc");
        PlayerPrefs.DeleteKey("nearestItemRecent");
        PlayerPrefs.DeleteKey("repCount");
        PlayerPrefs.DeleteKey("near_mutateNum");
        PlayerPrefs.DeleteKey("near_mutateRow");
        PlayerPrefs.DeleteKey("near_runRatio");
        PlayerPrefs.DeleteKey("near_v1Ratio");
        PlayerPrefs.DeleteKey("near_h0Ratio");
        PlayerPrefs.DeleteKey("near_forwardXBias");
        PlayerPrefs.DeleteKey("near_isSameSignXZ");
        PlayerPrefs.DeleteKey("near_jumpRatio");
        PlayerPrefs.DeleteKey("near_coverageDeg");
        PlayerPrefs.DeleteKey("near_itemDeg");
        PlayerPrefs.DeleteKey("near_interactionDeg");
        PlayerPrefs.DeleteKey("near_itemPri");
        PlayerPrefs.DeleteKey("near_decisionVel");
        PlayerPrefs.DeleteKey("near_directionAcc");
        PlayerPrefs.DeleteKey("near_avoidInc");
        PlayerPrefs.SetInt("restartCount", 0);
        PlayerPrefs.DeleteKey("fixNum");
        PlayerPrefs.DeleteKey("visitedPositions");
        PlayerPrefs.Save(); // ïœçXÇämíË
        PlayerPrefs.SetInt("RestartCount", 0);
        PlayerPrefs.SetInt("A", 0);
        PlayerPrefs.SetInt("IsARegenerated", -1);
        PlayerPrefs.SetString("VisitedPositions", "");
        int a = PlayerPrefs.GetInt("RestartCount", 0);
        int b = PlayerPrefs.GetInt("A", 0);
        int c = PlayerPrefs.GetInt("IsARegenerated", -1);
        string d = PlayerPrefs.GetString("VisitedPositions", "");
        Debug.Log(a + b + c + d);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
