using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoresScript : MonoBehaviour{
    public GameObject rank;
    public GameObject score;
    public GameObject scoreName;

    public void SetScore(string name, string Score, string rank){
        this.rank.GetComponent<Text>().text = rank;
        this.scoreName.GetComponent<Text>().text = name;
        this.rank.GetComponent<Text>().text = rank;
    }
}