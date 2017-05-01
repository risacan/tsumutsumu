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

    private GameObject _firstBall;
    private List<GameObject> _removableBallList;
    private GameObject _lastBall;
    private String _currentBallName;

	// Use this for initialization
	void Start () {
	    DropBalls(50);
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButton(0) && _firstBall == null) {
	        OnDragStart();
	    } else if (Input.GetMouseButtonUp(0)) { /* _firstBall に何か入ってる */
	        OnDragEnd();
	    } else if (_firstBall != null) { /* _firstBall に何か入ってる */
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
            ball.name = "Ball" + spriteId;
            var ballTexture = ball.GetComponent<SpriteRenderer>();
            ballTexture.sprite = BallSprites[spriteId];
            StartCoroutine(WaitForSeconds(1.0f));

        }
    }

    private void OnDragStart() {
        var currentCollider = GetCurrentCollider();
        if (currentCollider != null) {
            var currentColliderObject = currentCollider.gameObject;
            /* "ball" で始まってたら・・・ */
            if (currentColliderObject.name.IndexOf("Ball") != -1) {
                _removableBallList = new List<GameObject>();
                _firstBall = currentColliderObject; /* _firstBall に最初のボールを入れる */
                _currentBallName = currentColliderObject.name;
                PushToList(currentColliderObject);
            }
        }
    }

    private Collider2D GetCurrentCollider() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        return hit.collider;
    }

    private void OnDragEnd() {
        if (_firstBall != null) {
            /* 最初のボールじゃなくて */
            if (_removableBallList.Count >= 3) {
                /* 消すボールのリストに３つはいっている */
                foreach (GameObject obj in _removableBallList) {
                    /* 全部のボールを消して１つ追加する */
                    Destroy(obj);
                    DropBalls(1);
                }
            } else {
                /* 1つか2つはいってる */
                foreach (GameObject obj in _removableBallList) {
                    /* 全部のボールを消して１つ追加する */
                    var listedBall = obj;
                    ChangeColor(obj, 1.0f);
                    _removableBallList = new List<GameObject>();
                    listedBall.name = listedBall.name.Substring(1, 5);
                }
            }
        }
        Debug.Log(_removableBallList);
        _firstBall = null;
    }

    private void OnDragging() {
        var currentCollider = GetCurrentCollider();
        if (currentCollider != null) {
            var currentColliderObject = currentCollider.gameObject;
            if (currentColliderObject.name == _currentBallName) {
                Debug.Log("YESSSSSS SAME BALLNAME!");
                if (_lastBall != currentColliderObject) {
                    Debug.Log("Gonna show distance");
                    var dist = Vector3.Distance(_lastBall.transform.position, currentColliderObject.transform.position);
                    Debug.Log("distance =" + dist);
                    if (dist <= 4.0f) {
                        PushToList(currentColliderObject);
                    }
                }
            }
        }
    }

    private void PushToList(GameObject obj) {
        _lastBall = obj;
        _removableBallList.Add(obj);
        obj.name = "_" + obj.name;
        ChangeColor(obj, 0.5f);
    }

    private void ChangeColor(GameObject obj, float transparency) {
        var ballTexture = obj.GetComponent<SpriteRenderer>();
        ballTexture.color = new Color(255.0f, 255.0f, 255.0f, transparency);

    }

    IEnumerator WaitForSeconds(float time) {
        yield return new WaitForSeconds(time);
    }
}
