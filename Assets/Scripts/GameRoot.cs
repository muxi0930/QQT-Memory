using UnityEngine;

public class GameRoot: MonoBehaviour {
    private GameFlowManger gameRuntimeFlow;

    private void Start(){ Debug.Log("游戏启动"); Init(); }
    private void Init() {
        new GameFlowManger();
    }
    
    public void BeginGame() {
        new GameFlowManger();
    }
}
