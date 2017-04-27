using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NewBehaviourScript : MonoBehaviour {

    public GameObject BallPrefab;
    public Sprite[] BallSprites;

    private GameObject FirstBall;
    private List<GameObject> RemovableBallList;
    private GameObject LastBall;
    private String CurrentBallName;

	// Use this for initialization
	void Start () {
	    DropBalls(150);
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButton(0) && FirstBall == null) {
	        OnDragStart();
	    } else if (Input.GetMouseButtonUp(0)) {
	        OnDragEnd();
	    } else if (FirstBall != null) {
	        OnDragging();
	    }
	}

     private void  DropBalls(int count) {
        for (var i = 0; i < count; i++) {
            var randomX = Random.Range(-0.5f, 0.5f);
            float randomZ = Random.Range(-20.0f, 20.0f);
            var ball = Instantiate(BallPrefab, new Vector3(randomX, 30.0f, 0.0f), Quaternion.identity);
            ball.transform.eulerAngles = new Vector3(0, 0, randomZ);
            int spriteId = Random.Range(0, 4);
            ball.name = "ball" + spriteId;
            var ballTexture = ball.GetComponent<SpriteRenderer>();
            ballTexture.sprite = BallSprites[spriteId];
        }
    }

    private void OnDragStart() {
        var currentCollider = GetCurrentCollider();
        if (currentCollider != null) {
            var currentColliderObject = currentCollider.gameObject;
            /* "ball" で始まってたら・・・ */
            if (currentColliderObject.name.IndexOf("ball") != -1) {
                RemovableBallList = new List<GameObject>();
                FirstBall = currentColliderObject;
                CurrentBallName = currentColliderObject.name;
            }
        }
    }

    private Collider2D GetCurrentCollider() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        return hit.collider;
    }

    private void OnDragEnd() {
        if (FirstBall != null) { /* 最初のボールじゃなくて */
            if (RemovableBallList.Count >= 3) { /* 消すボールのリストに３つはいっている */
                foreach (GameObject obj in RemovableBallList) { /* 全部のボールを消して１つ追加する */
                    Destroy(obj);
                    DropBalls(1);
                }
            }
        } else { /* 消すボールのリストに３つはいってないないとき */
            foreach (GameObject obj in RemovableBallList) {
                var listedBall = obj;
                listedBall.name = listedBall.name.Substring(0, 1);
                RemovableBallList = new List<GameObject>();
            }
        }
        FirstBall = null;
    }

    private void OnDragging() {
        var currentCollider = GetCurrentCollider();
        if (currentCollider != null) {
            var currentColliderObject = currentCollider.gameObject;
            if (currentColliderObject.name == CurrentBallName) {
                Debug.Log("YESSSSSS!");
                PushToList(currentColliderObject);
            }
        }
    }

    private void PushToList(GameObject obj) {
        LastBall = obj;
        RemovableBallList.Add(obj);
        obj.name = "_" + obj.name;
        ChangeColor(obj);
    }
    private void ChangeColor(GameObject obj) {
        var ballTexture = obj.GetComponent<SpriteRenderer>();
        ballTexture.color = new Color(255.0f, 255.0f, 255.0f, 0.5f);

    }
}
