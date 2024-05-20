﻿using UnityEngine;
using System.Collections;

public class EnemySpawning : MonoBehaviour {
    public Transform eagle, generatedEnemyFolder, easyTank, fastTank, mediumTank, strongTank;
    public int next;
    
    private int[] tanks;    
    private Transform trans;
    private Animator anim;
    
    System.Random r;

    void Start(){
        trans = gameObject.GetComponent<Transform>();
        anim = gameObject.GetComponent<Animator>();
        r = new System.Random();
        tanks = new int[20];
        for (int i=0;i<20;i++){
            tanks[i] = r.Next(50) % 4 + 1;
        }
        Reset();
    }

    public void Reset(){
        if (trans == null) trans = gameObject.GetComponent<Transform>();
        trans.position = new Vector3(-16, 12, 0);
        next = 0;
    }

    void Update(){
        int tankCount = generatedEnemyFolder.GetComponentsInChildren<Transform>().Length;

        // 4 tanks and 1 folder also counts
        if (next < 20 && tankCount < 5) {
            anim.SetBool("spawn", true);
        }else if (next >= 20 && tankCount <= 1){
            eagle.SendMessage("LoadMap", true);
        }
    }

    // Called from animation event
    private void SpawnEnemy(){
        anim.SetBool("spawn", false);
        Transform t = null;

        if (tanks[next] == 1){
            t = Instantiate(easyTank, trans.position, easyTank.rotation) as Transform;
        }
        else if (tanks[next] == 2){
            t = Instantiate(fastTank, trans.position, fastTank.rotation) as Transform;
        }
        else if (tanks[next] == 3){
            t = Instantiate(mediumTank, trans.position, mediumTank.rotation) as Transform;
        }
        else if (tanks[next] == 4){
            t = Instantiate(strongTank, trans.position, strongTank.rotation) as Transform;
            t.SendMessage("SetLives", 5);
        }
        PushPosition();
        t.parent = generatedEnemyFolder;
        t.SendMessage("SetIsTemplate", false);
    
        // every four enemies, one get bonus 
        if ((next + 1) % 4 == 0){
            t.SendMessage("SetBonus", r.Next(50) % 5 + 1);
        }
        next++;
    }

    private void PushPosition(){
        trans.position += new Vector3(19, 0, 0);
        if (trans.position.x > 19) trans.position = new Vector3(-16, 12, 0);
    }
}
