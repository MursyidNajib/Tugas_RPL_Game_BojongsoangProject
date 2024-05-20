using UnityEngine;
using System.Collections;
using System;

public class MapLoad : MonoBehaviour {
    public Transform generatedWallFolder, generatedEnemyFolder, generatedBulletFolder, spawnLocation, powerUp, player1;
    public int level;
    public Transform wall, iron, bush, ice, water;
    public AudioSource levelStarting;
    
    private bool multiplayer = false;
    private int currentLevel;

    void Start (){
        LoadMap(level);
        Application.targetFrameRate = 60;
        QualitySettings.antiAliasing = 0;
        QualitySettings.shadowCascades = 0;
        QualitySettings.vSyncCount = 1;
        QualitySettings.SetQualityLevel(2);
    }

    void Update(){
        if (currentLevel != level){
            LoadMap(level);
        }
        CheckInput();
    }

    private void CheckInput(){
        if (Input.GetKeyDown(KeyCode.R)){
            multiplayer = !multiplayer;
            LoadMap(1);
        }
    }
    // message receiver (getter) from "EnemySpawn"
    public void GetMultiplayer(ArgsPointer<bool> pointer){
        pointer.Args = new bool[] { multiplayer };
    }

    private void LoadMap(bool won){
        if (won){
            LoadMap(++level);
        }
    }

    private void LoadMap(int lev){
        currentLevel = lev;
        level = lev;

        // Reset data
        DeleteChilds(generatedWallFolder);
        DeleteChilds(generatedEnemyFolder);
        DeleteChilds(generatedBulletFolder);

        player1.SendMessage("ResetPosition");
        player1.GetComponent<Animator>().SetBool("hit", false);
        player1.SendMessage("SetShooting", false);
        player1.SendMessage("SetShooting", false);
        player1.SendMessage("SetShield", 6);

        // Enemy spawning reset
        spawnLocation.SendMessage("Reset");

        // Read map file
        //string[] m = System.IO.File.ReadAllLines(@"Assets/Maps/map" + currentLevel + ".txt");
        string[] m = System.IO.File.ReadAllLines(@"Bojongsoang_Project_Data/Maps/map" + currentLevel + ".txt");
        GenerateObjects(m);

        // powerUp reset
        powerUp.SendMessage("Reset");

        // play a sound
        levelStarting.Play();
    }

    private void DeleteChilds(Transform folder){
        Transform[] ts = folder.GetComponentsInChildren<Transform>();
        foreach (var t in ts){
            if (!t.gameObject.name.Contains("Generated")){
                Destroy(t.gameObject);
            }
        }
    }

    private void GenerateObjects(string[] m){
        for (int i = 0; i < 26; i++){
            for (int j = 0; j < 40; j++){
                Transform t = null;
                if (m[i][j] == 'o'){
                    t = Instantiate(wall, new Vector3(j - 17, 13 - (i + 1), 0), wall.rotation) as Transform;
                }else if (m[i][j] == 'Q'){
                    t = Instantiate(iron, new Vector3(j - 17, 13 - (i + 1), 0), wall.rotation) as Transform;
                }else if (m[i][j] == 'b'){
                    t = Instantiate(bush, new Vector3(j - 17, 13 - (i + 1), 0), wall.rotation) as Transform;
                }else if (m[i][j] == 'i'){
                    t = Instantiate(ice, new Vector3(j - 17, 13 - (i + 1), 0), wall.rotation) as Transform;
                }else if (m[i][j] == 'w'){
                    t = Instantiate(water, new Vector3(j - 17, 13 - (i + 1), 0), wall.rotation) as Transform;
                }
                
                if (m[i][j] != '.')
                {
                    t.parent = generatedWallFolder;
                }
            }
        }
    }
}
