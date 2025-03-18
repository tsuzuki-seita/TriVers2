using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ScoreModel
{
    private int score;
    private Subject<int> scoreSubject = new Subject<int>();
    public IObservable<int> OnScoreChanged => scoreSubject;
    public void AddScore(int amount) {
        score += amount;
        scoreSubject.OnNext(score);
    }
}
