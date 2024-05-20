using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using System.Collections;
using TMPro;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class Player : MonoBehaviour{
    public int shieldTime = 0;
    public int killCount = 0;
    public int level = 1;
    public Transform bulletWeak, bulletFast, bulletStrong;
    public Animator shieldAnim;

    //savefileproperties
    public int rank, highscoredesu;
    public string name;

    //level
    public TextMeshProUGUI levelText;
    public int levelStage = 1;

    //score
    public TextMeshProUGUI scoreText;
    public int score = 0;

    //lives
    public TextMeshProUGUI livesText;
    public int lives = 3;

    public TextMeshProUGUI scoreText1;
    public TextMeshProUGUI scoreText2;
    public TextMeshProUGUI scoreText3;
    private List<Score> highScores= new List<Score>();
    public InputField enterName;
    public string database;

    void Start(){
        database = "URI=file:score.db";
        level = 1;
        displayLevel();
        displayScore();
        displayLives();
        Load();
        ShowScore();
        name = PlayerPrefs.GetString("Name");
        Debug.Log(name);
        CreateDB();
    }

    void Update(){
        if (level == 1){
            gameObject.SendMessage("SetBullet", bulletWeak);
            gameObject.SendMessage("SetMaxBullets", 1);
        }
        if (level == 2){
            gameObject.SendMessage("SetBullet", bulletFast);
            gameObject.SendMessage("SetMaxBullets", 1);
        }
        if (level == 3){
            gameObject.SendMessage("SetBullet", bulletFast);
            gameObject.SendMessage("SetMaxBullets", 2);
        }
        if (level == 4){
            gameObject.SendMessage("SetBullet", bulletStrong);
            gameObject.SendMessage("SetMaxBullets", 2);
        }
        gameObject.GetComponent<Animator>().SetInteger("level", level);
    }

    public void CreateDB(){
        using (var connection = new SqliteConnection(database)){
            connection.Open();
            using(var command = connection.CreateCommand()){
                command.CommandText = "CREATE TABLE IF NOT EXISTS highScore(name STRING, score INT);";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public void Save(){
        using (var connection = new SqliteConnection(database)){
            connection.Open();
            using(var command = connection.CreateCommand()){
                command.CommandText = "INSERT INTO highScore(name, score) VALUES ('"+ name +"','"+ score +"');";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }   
    }

    public void Load(){
        highScores.Clear();
        using (var connection = new SqliteConnection(database)){
            connection.Open();
            using (var command = connection.CreateCommand()){
                command.CommandText = "SELECT * FROM highScore ORDER BY score DESC;";
                command.ExecuteNonQuery();
                using (var reader = command.ExecuteReader()){
                    while (reader.Read()){
                        highScores.Add(new Score(reader.GetString(0),reader.GetInt32(1)));
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
    }

    public void ShowScore(){
        highscoredesu = highScores[0].getScore();
        name = highScores[0].getName();
        rank = 1 ;
        scoreText1.text = rank.ToString()+"."+name+" "+highscoredesu.ToString();

        if(highScores[1]!=null){
            highscoredesu = highScores[1].getScore();
            name = highScores[1].getName();
            rank = 2 ;
            scoreText2.text = rank.ToString()+"."+name+" "+highscoredesu.ToString();
        }
        
        if(highScores[2]!=null){
            highscoredesu = highScores[2].getScore();
            name = highScores[2].getName();
            rank = 3 ;
            scoreText3.text = rank.ToString()+"."+name+" "+highscoredesu.ToString();
        }
    }

    public void scoreMin1(){
        score -=1;
        displayScore();
    }

    public void score1(){
        score += 10;
        destroy();
        displayScore();
    }

    public void score2(){
        score += 20;
        destroy();
        displayScore();
    }

    public void score3(){
        score += 50;
        destroy();
        displayScore();
    }

    public void score0(){
        score -= 100;
        destroy();
        displayScore();
    }

    public void displayScore(){
        PlayerPrefs.SetInt("Score", score);
        scoreText.text = score.ToString();
    }

    public void displayLevel(){
        levelText.text = levelStage.ToString();
    }

    public void displayLives(){
        livesText.text = lives.ToString();
    }

    // Bonus taken
    public void OnTriggerEnter2D(Collider2D collision){
        Transform other = collision.GetComponent<Transform>();
        
        if (other.name.Contains("PowerUp")){
            int bonus = Mathf.RoundToInt(other.GetComponent<Animator>().GetFloat("bonus"));
            if (bonus == 1){
                level++;
            }
            if (bonus == 2){
                other.gameObject.SendMessage("DestroyAllTanks");
            }
            if (bonus == 3){
                other.gameObject.SendMessage("FreezeTime");
            }
            if (bonus == 4){
                SetShield(15);
            }
            if (bonus == 5){
                lives++;
                displayLives();
            }
            other.gameObject.SendMessage("HidePowerUp");
        }
    }

    // Message receiver from "Map load" and/ "this"
    private void SetShield(int time){
        if (shieldTime <= 0){
            shieldTime = time;
            StartCoroutine(ShieldEnumerator());
        }
        shieldTime = time;
        shieldAnim.SetBool("isOn", true);
        gameObject.GetComponent<Animator>().SetBool("shield", true);
    }

    IEnumerator ShieldEnumerator(){
        while (shieldTime > 0){
            yield return new WaitForSeconds(1);
            shieldTime--;
        }
        if (shieldTime <= 0){
            shieldAnim.SetBool("isOn", false);
            gameObject.GetComponent<Animator>().SetBool("shield", false);
        }
    }

    // message receiver from "load map"
    public void SetLevel(int level){
        this.level = level;
    }

    // message receiver from "BulletTankDestroy"
    public void Hit(){
        lives--;
        if(lives==0){
            Save();
        }
        livesText.text = lives.ToString();
    }

    public void destroy(){
        killCount++;
        if(killCount%20==   0){
            levelStage+=1;
            displayLevel();
        }
    }

    // message receiver from "BulletTankDestroy"
    public void GetLives(ArgsPointer<int> pointer){
        pointer.Args = new int[] { lives };
    }

    // message receiver from "load map"
    public void SetLives(int lives){
        this.lives = lives;
    }

    //kill +(10,20,20,50)
    //death -100
    //using bomb doenst give you score (this is not bomberman)
    //tiap nembak -1(LU OLANG PIKIL OE KAYA YA?)
    //tiap jalan 40 tile -1(DIESEL LAGI MAHAL)
}
