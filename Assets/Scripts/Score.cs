using System;

class Score{
    public string name;
    public int score;

    public Score(string name, int score){
        this.name = name;
        this.score = score;
    }

    public int getScore(){
        return score;
    }

    public string getName(){
        return name;
    }
}