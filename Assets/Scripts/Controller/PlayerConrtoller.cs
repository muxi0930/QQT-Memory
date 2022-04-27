using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerConrtoller : MonoBehaviour {
    private Animator animator;
    private Rigidbody2D rg2D;
    private int[] UserInput;
    private int UserInputIndex;
    private Dictionary<int, KeyCode> keyDictionary;
    private AudioSource audioSource;

    public Player _player;
    public Text Text;
    public GameObject Bomb;
    public BombController BombController;
    private void Start() {
        UserInput = new[] {0, 0, 0, 0};
        keyDictionary = new Dictionary<int, KeyCode> {
            {0, KeyCode.UpArrow}, {1, KeyCode.RightArrow}, {2, KeyCode.DownArrow}, {3, KeyCode.LeftArrow}
        };

        rg2D = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        
        if (_player == null) _player = new Player();
        if (Bomb == null) {
            Bomb = CommonUtils.getDefaultBomb();
            BombController = Bomb.GetComponent<BombController>();
            BombController.bombRange = _player.AttackRange;
        }

    }

    // Update is called once per frame
    private void Update() {
        if (_player.IsAlive) {
            HandleMovement();
            HandleCreateBomb();
        }
    }

    private void HandleCreateBomb() {
        var targetPosition = transform.position - new Vector3(0, 0.25f, 0);
        targetPosition = CommonUtils.FixWorldPositon(targetPosition);
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (GameFlowManger.CanPlace(targetPosition) && _player.CanPlace()) {
                // 修复放置坐标对其
                var _bomb = Instantiate(Bomb, targetPosition, Quaternion.identity);
                _player.HasPlacedBombCount++;
                GameFlowManger.PlaceBomb(targetPosition, _player.AttackRange, _bomb, _player);
                audioSource.Play();
            }
        }
    }
    
    // 处理人物移动
    private void HandleMovement() {
        // 处理移动
        var priorAxis = getPriorAxis();
        var isMoveLeft = priorAxis == KeyCode.LeftArrow ? -1 : 0;
        var isMoveRight = priorAxis == KeyCode.RightArrow ? 1 : 0;
        var isMoveUp = priorAxis == KeyCode.UpArrow ? 1 : 0;
        var isMoveDown = priorAxis == KeyCode.DownArrow ? -1 : 0;
        var isIdel = priorAxis == KeyCode.Alpha9 ? 0 : 1;

        rg2D.velocity = new Vector2(_player.playerSpeed * (isMoveLeft + isMoveRight) * isIdel,
            _player.playerSpeed * (isMoveUp + isMoveDown) * isIdel);

        // 处理动画事件
        SetMovementBool(isMoveLeft, isMoveRight, isMoveUp, isMoveDown);
    }

    private void HandleDeath() {
        if (!_player.IsAlive) {
            // TODO：播放死亡动画，any状态到死亡动画状态，设置动画参数触发
            GameFlowManger.CheckGameOver();
        }
    }
    
    // 处理按键输入顺序，返回优先输入轴，确保多方向输入时移动正确
    private KeyCode getPriorAxis() {
        if (Input.GetKeyUp(KeyCode.UpArrow)) UserInput[0] = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow)) UserInput[1] = 0;
        if (Input.GetKeyUp(KeyCode.DownArrow)) UserInput[2] = 0;
        if (Input.GetKeyUp(KeyCode.LeftArrow)) UserInput[3] = 0;
        if (Input.GetKeyDown(KeyCode.UpArrow)) UserInput[0] = ++UserInputIndex;
        if (Input.GetKeyDown(KeyCode.RightArrow)) UserInput[1] = ++UserInputIndex;
        if (Input.GetKeyDown(KeyCode.DownArrow)) UserInput[2] = ++UserInputIndex;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) UserInput[3] = ++UserInputIndex;

        var index = Array.IndexOf(UserInput, UserInput.Max());
        // 处理无移动情况
        if (UserInput[index] == 0) return KeyCode.Alpha9;
        // 实现移动后保留角色面向功能
        if (UserInput[index] != 0) animator.SetFloat("LastDirect", index);
        return keyDictionary[index];
    }

    // 通知前台处理动画面向
    private void SetMovementBool(int isMoveLeft, int isMoveRight, int isMoveUp, int isMoveDown) {
        var isIdle = !Convert.ToBoolean(isMoveLeft + isMoveRight) & !Convert.ToBoolean(isMoveUp + isMoveDown);
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("LookLeft", false);
        animator.SetBool("LookRight", false);
        animator.SetBool("LookUp", false);
        animator.SetBool("LookDown", false);
        if (!isIdle) {
            animator.SetBool("LookLeft", Convert.ToBoolean(isMoveLeft));
            animator.SetBool("LookRight", Convert.ToBoolean(isMoveRight));
            animator.SetBool("LookUp", Convert.ToBoolean(isMoveUp));
            animator.SetBool("LookDown", Convert.ToBoolean(isMoveDown));
        }
    }

    // 动画IDLE在0帧时调用
    private void setDirtAnimEvent() {
        var f = animator.GetFloat("LastDirect");
        // Debug.Log(f*0.25f + 0.02f );
        animator.Play("IDLE", 0, f * 0.25f + 0.02f);
        animator.speed = 0;
    }

    private void BeDamaged() {
        _player.HpCount -= 1;
        //TODO:播放扣血动画
        
        HandleDeath();
    }
}