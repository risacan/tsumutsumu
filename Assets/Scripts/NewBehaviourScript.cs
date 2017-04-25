using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    public GameObject BallPrefab;
    public Sprite[] BallSprites;

	// Use this for initialization
	void Start () {
	    DropBalls(55);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

     public void DropBalls(int count) {
        for (var i = 0; i < count; i++) {
            var ball = Instantiate(BallPrefab);
            var randomX = Random.Range(-2.0f, 2.0f);
            var randomZ = Random.Range(-40.0f, 40.0f);
            int spriteId = Random.Range(0, 3);
            ball.name = "ball" + spriteId;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
