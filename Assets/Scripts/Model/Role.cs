public class Player{
    public int HpCount = 1;                //生命值数
    public int AttackRange = 2;            //攻击范围
    public int MaxBombCount = 3;           //糖弹数量
    public int HasPlacedBombCount = 0;    
    public int playerSpeed = 3;            //移速基准
    public bool IsAlive => HpCount > 0;           //是否存活

    public int PlayerSkinId = 1;
    public int BombSkinId = 1; // TODO: 糖泡皮肤自定义，需要动态修改糖泡Prefab中的精灵资源

    public Player() { }

    public bool CanPlace() {
        return MaxBombCount > HasPlacedBombCount;
    }

    public void DecreaseHP() {
        if (IsAlive) HpCount -= 1;
    }
}

// 怪物类
public class Monster {
    public int hpCount = 1;                //生命值数
    public int attactRange = 1;            //攻击范围
    public int maxBombCount = 2;           //糖弹数量
    public int playerSpeed = 10;           //移速基准
    public bool is_alive = true;           //是否存活

    public int monsterId = 1;
}