using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Object = System.Object;

public class BombController : MonoBehaviour {
    private CapsuleCollider2D _capsuleCollider2D;
    private Animator _animator;
    private AudioSource _audioSource;
    
    private float _createTime;
    private bool _hasExplode;
    private const float MapFixFloat = 0.4f;
    
    public float bombExplodeTime = 3f;
    public int bombRange = 5;
    public GameObject animFireLeft;
    public GameObject animFireRight;
    public GameObject animFireUp;
    public GameObject animFireDown;

    
    // Start is called before the first frame update
    void Start() {
        _animator = gameObject.GetComponentInChildren<Animator>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        _capsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();

        // 使用触发器实现人物放置糖炮后还未走开时，二者并无碰撞的效果
        _capsuleCollider2D.isTrigger = true;
        // 清除继承自Prefab的状态
        _createTime = Time.time;
    }

    // Update is called once per frame
    void Update() {
        // 处理爆炸
        HandleExplode();
        // 处理销毁
        HandleDestory();
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        _capsuleCollider2D.isTrigger = false;
    }

    private void HandleExplode() {
        if (Time.time - _createTime > bombExplodeTime && !_animator.GetBool("is_explode")) {
            _animator.SetBool("is_explode", true);
            _capsuleCollider2D.isTrigger = true;
            if (!_hasExplode && !_audioSource.isPlaying) {
                GameFlowManger.ChainExplode(transform.position);
                _audioSource.Play();
                // 处理焰火
                for (int i = 1; i <= bombRange; i++) {
                    Instantiate(animFireLeft, transform.position - new Vector3(+0.4f*i, 0f, 0), Quaternion.identity);
                    Instantiate(animFireRight, transform.position - new Vector3(-0.4f*i, 0f, 0), Quaternion.identity);
                    Instantiate(animFireUp, transform.position - new Vector3(0f, 0.4f*i, 0), Quaternion.identity);
                    Instantiate(animFireDown, transform.position - new Vector3(0f, -0.4f*i, 0), Quaternion.identity);
                }

            }
            _hasExplode = true;
        }
    }

    private void HandleDestory() {
        var animInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (animInfo.IsName("anim_bomb_explode_center") && animInfo.normalizedTime > 1f) {
            Destroy(gameObject);
        }
    }

    public void ExplodeByChainToCtl() {
        _animator.SetBool("is_explode", true);
        _audioSource.Play();
        for (int i = 1; i <= bombRange; i++) {
            Instantiate(animFireLeft, transform.position - new Vector3(+0.4f*i, 0f, 0), Quaternion.identity);
            Instantiate(animFireRight, transform.position - new Vector3(-0.4f*i, 0f, 0), Quaternion.identity);
            Instantiate(animFireUp, transform.position - new Vector3(0f, 0.4f*i, 0), Quaternion.identity);
            Instantiate(animFireDown, transform.position - new Vector3(0f, -0.4f*i, 0), Quaternion.identity);
        }
    }
}
