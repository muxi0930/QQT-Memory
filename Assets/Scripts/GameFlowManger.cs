using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

class GBomb {
    public GameObject gameObject;
    public int attactRange;
    public Player master;
    public GBomb(int attactRange, GameObject gameObject, Player player) {
        this.gameObject = gameObject;
        this.attactRange = attactRange;
        this.master = player;
    }

    public void ExplodeByChain() {
        gameObject.GetComponent<BombController>().ExplodeByChainToCtl();
    }
}

public class GameFlowManger {
    private static GBomb[,] BombGrid;
    private static int[,] _hasAddedMap;
    private static Queue<GBomb> _chainQueue;

    private Vector3 PlayerBrithPosition;

    
    public GameFlowManger() {
        Cursor.visible = true;
        BombGrid = new GBomb[300, 300];
        _hasAddedMap = new int[300, 300];
        _chainQueue = new Queue<GBomb>();
        // 初始化并激活场景
        CommonUtils.loadSenceSync("Scene_MainGameFlow"); // TODO: 地图模块开发
        SceneManager.sceneLoaded += LoadedAndInit;
        
    }

    private static void LoadedAndInit(Scene scene, LoadSceneMode mode) {
        var playerPrefab = Resources.Load<GameObject>("Prefabs/Prefab_Player"); // TODO: 从对象池模块中复制已加载内存的预制体
        foreach (var o in scene.GetRootGameObjects()) {
            if (o.name != "Players") continue;
            var player = GameObject.Instantiate(playerPrefab, o.transform);
            player.transform.position = new Vector3(14, 12, 0); // TODO: 从地图模块的配置文件读取
            player.GetComponent<PlayerConrtoller>()._player = CommonUtils.getPlayerInfo();
        }
        SceneManager.SetActiveScene(scene);
    }
    
    public static bool CanPlace(Vector3 pos) {
        var (fixedX, fixedY) = CommonUtils.WorldPosition2GridIndex(pos);
        return BombGrid[fixedX, fixedY] == null;
    }

    public static void PlaceBomb(Vector3 pos, int range, GameObject bomb, Player player) {
        var (x, y) = CommonUtils.WorldPosition2GridIndex(pos);
        BombGrid[x, y] = new GBomb(range, bomb, player);
        Debug.Log("Place Bomb at " + x + ", " + y);
    }

    public static void ExplodeBomb(int x, int y) {
        BombGrid[x, y].ExplodeByChain();
        BombGrid[x, y].master.HasPlacedBombCount--;
        BombGrid[x, y] = null;
        // Debug.Log("Bomb Explode at " + x + ", " + y);
    }

    public static void ChainExplode(Vector3 pos) {
        _hasAddedMap = new int[300, 300];
        var (x, y) = CommonUtils.WorldPosition2GridIndex(pos);
        var waitingExplode = new ArrayList {BombGrid[x, y]};
        _hasAddedMap[x, y] = 1;
        
        _chainQueue.Enqueue(BombGrid[x, y]);
        while (_chainQueue.Count != 0) {
            var gBomb = _chainQueue.Dequeue();
            var gBombRange = gBomb.attactRange;
            var (gBombX, gBombY) = CommonUtils.WorldPosition2GridIndex(gBomb.gameObject.transform.position);
            for (int i = gBombX - gBombRange; i <= gBombX + gBomb.attactRange; i++) {
                if (i != x && _hasAddedMap[i, gBombY] != 1 && BombGrid[i, gBombY] != null) {
                    _chainQueue.Enqueue(BombGrid[i, gBombY]);
                    waitingExplode.Add(BombGrid[i, gBombY]);
                    _hasAddedMap[i, gBombY] = 1;
                }
            }
            for (int j = gBombY - gBombRange; j <= gBombY + gBomb.attactRange; j++) {
                if (j != y && _hasAddedMap[gBombX, j] != 1 && BombGrid[gBombX, j] != null) {
                    _chainQueue.Enqueue(BombGrid[gBombX, j]);
                    waitingExplode.Add(BombGrid[gBombX, j]);
                    _hasAddedMap[gBombX, j] = 1;
                }
            }
        }

        string str = "Chain EXPLODE: ";
        foreach (var gbomb in waitingExplode) {
            var (a1, a2) = CommonUtils.WorldPosition2GridIndex((gbomb as GBomb).gameObject.transform.position);
            str += "(" + a1 + "," + a2 + ") ";
        }
        Debug.Log(str);
        foreach (var gbomb in waitingExplode) {
            var (a1, a2) = CommonUtils.WorldPosition2GridIndex((gbomb as GBomb).gameObject.transform.position);
            ExplodeBomb(a1, a2);
        }
    }

    public static bool CheckGameOver() {
        return false; // TODO:游戏结束返回房间
    }
}
