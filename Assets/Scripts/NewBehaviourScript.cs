using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using NUnit.Framework.Internal.Execution;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NewBehaviourScript : MonoBehaviour {

    public GameObject BallPrefab;
    public Sprite[] BallSprites;
    public GameObject Timer;
    public GameObject Scores;
    public GameObject A;
    public GameObject B;

    private GameObject _firstBall;
    private List<GameObject> _removableBallList;
    private GameObject _lastBall;
    private String _currentBallName;
    private Boolean _isPlaying;
    private Text _timerText;
    private int _timeLimit = 60;
    private int _countTime = 3;
    private int _point;
    private int _Ascore;
    private int _Bscore;

	// Use this for initialization
	void Start () {
	    _timerText = Timer.GetComponent<Text>();
	    StartCoroutine(CountDown());
	}
	
	// Update is called once per frame
	void Update () {
	    if (_isPlaying) {
	        if (Input.GetMouseButton(0) && _firstBall == null) {
	            OnDragStart();
	        } else if (Input.GetMouseButtonUp(0)) { /* _firstBall に何か入ってる */
	            OnDragEnd();
	        } else if (_firstBall != null) { /* _firstBall に何か入ってる */
	            OnDragging();
	        }
	    }
	}

    IEnumerator CountDown() {
        var count = _countTime;
        while (count > 0) {
            _timerText.text = count.ToString();
            yield return new WaitForSeconds (1.0f);
            count -= 1;
        }
        DropBalls(50);
        _timerText.text = "START!";
        _isPlaying = true;
        yield return new WaitForSeconds (1.0f);
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer() {
        var count = _timeLimit;
        while (count > 0) {
            _timerText.text = count.ToString();
            yield return new WaitForSeconds (1.0f);
            count -= 1;
        }
        _timerText.text = "Finished";
        OnDragEnd();
        _isPlaying = false;
    }

    public void Reset() {
        SceneManager.LoadScene("Main");
    }

    public void Shuffle() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player") ) {
            float randomX = Random.Range(-20.0f, 20.0f);
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX * 100, 4000.0f));
           /* obj.GetComponent<Rigidbody2D>().AddTorque(10.0f);*/
        }
        Debug.Log("SHUFFLE!");
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
                    _point ++;
                    Scores.GetComponent<Text>().text = _point.ToString();
                    if (obj.name == "_" + "Ball0") {
                        _Ascore++;
                    } else if (obj.name == "_" + "Ball3") {
                        _Bscore++;
                    }
                    if (_Ascore >= 5) {
                        A.GetComponent<Text>().color = Color.red;
                    }
                    if (_Bscore >= 10) {
                        B.GetComponent<Text>().color = Color.blue;
                    }
                    Debug.Log("Ascore : " + _Ascore);
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
                if (_lastBall != currentColliderObject) {
                    var dist = Vector3.Distance(_lastBall.transform.position, currentColliderObject.transform.position);
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
        Debug.Log("まって〜！");
        yield return new WaitForSeconds(time);
    }
}
