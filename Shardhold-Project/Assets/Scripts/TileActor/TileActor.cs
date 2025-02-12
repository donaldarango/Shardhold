using UnityEngine;

abstract public class TileActor : MonoBehaviour
{
    public enum ObjType{
        placeholder
    }

    public enum TileActorType
    {
        enemyUnit,
        structure,
        trap,
    }

    private int attackRange;
    private int damage;
    private int currentHealth;
    private int maxHealth;
    private (int, int)? currentTile = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
