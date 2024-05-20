using UnityEngine;
using System.Collections;
using System;
using Mono.Data.Sqlite;
using UnityEngine.SceneManagement;


public class Eagle : MonoBehaviour {
    public AudioSource eagleDestroy, gameOver;
    public Transform player1;
    public string database;

    void Start(){
        database = "URI=file:score.db";
    }

    public void OnTriggerEnter2D(Collider2D collider){
        Transform other = collider.GetComponent<Transform>();
        if (other.name.Contains("Bullet") && !other.GetComponent<Animator>().GetBool("hit") && !gameObject.GetComponent<Animator>().GetBool("isDestroyed")){
            other.GetComponent<Animator>().SetBool("hit", true);
            gameObject.GetComponent<Animator>().SetBool("isDestroyed", true);
            eagleDestroy.Play();
            this.DoAfter(1, () => gameOver.Play());
        }
    }

    private void FinishGame(){
        save();
        player1.SendMessage("SetLevel", 1);
        player1.SendMessage("SetLives", 3);
        player1.SendMessage("SetIsTemplate", false);
        gameObject.GetComponent<Animator>().SetBool("isDestroyed", false);
        //gameObject.SendMessage("LoadMap", 1);
        SceneManager.LoadScene("main menu");
    }

    public void save(){
        using (var connection = new SqliteConnection(database)){
            connection.Open();
            string name = PlayerPrefs.GetString("Name");
            int score = PlayerPrefs.GetInt("Score");
            using(var command = connection.CreateCommand()){
                command.CommandText = "INSERT INTO highScore(name, score) VALUES ('"+ name +"','"+ score +"');";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}
