using UnityEngine;

[CreateAssetMenu(fileName = "MonsterStat", menuName = "Scriptable Objects/MonsterStat")]
public class MonsterStat : ScriptableObject
{
    public GameObject monsterPrefab;
    public float moveSpeed;
    //����
    public float attackRange;
    public float attackDamage;
    public float attackDelay;
    public float attackTimer = 0f;

    //���� ����
    public int attackVarious;

    public int hp;
}
