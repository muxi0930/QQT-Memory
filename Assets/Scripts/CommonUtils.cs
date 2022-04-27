using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonUtils {
    public static CommonUtils Instance => _instance ??= new CommonUtils();

    public string getPlayerInfo(int playerId) {
        return "TODO：从配置文件中读取";
    }

    public static void loadSenceSync(string name) {
        Debug.Log("正在同步切换场景到" + name);
        SceneManager.LoadScene(name);
    }
    
    private const int MapFixFloat = 4;
    
    // 世界坐标转内存糖弹网格坐标
    public static (int, int) WorldPosition2GridIndex(Vector3 pos) {
        var x = (int)Math.Floor((pos.x * 10) / MapFixFloat + 0.5f);
        var y = (int)Math.Floor((pos.y * 10) / MapFixFloat + 0.5f);
        return (x, y);
    }
    // 根据世界坐标获取修正坐标
    private CommonUtils() { }
    private static CommonUtils _instance = null;
    public static Vector3 FixWorldPositon(Vector3 currentPos) {
        var x = currentPos.x * 10;
        var y = currentPos.y * 10;
        var _x = Math.Floor(x / MapFixFloat + 0.5f) * MapFixFloat / 10.0;
        var _y = Math.Floor(y / MapFixFloat + 0.5f) * MapFixFloat / 10.0;
        return new Vector3((float)_x, (float)_y, currentPos.z);
    }
    
    // 读取player默认属性
    public static Player getPlayerInfo() {
        return new Player(); //TODO 配置文件导入
    }

    public static GameObject getDefaultBomb() {
        return Resources.Load<GameObject>("Prefabs/Prefab_Bomb_Circle"); // TODO: 从对象池模块中复制已加载内存的预制体
    }
}
