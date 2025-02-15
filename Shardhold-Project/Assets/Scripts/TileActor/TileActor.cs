using UnityEngine;

abstract public class TileActor : MonoBehaviour
{
    public enum ObjType{
        placeholder
    }

    public enum TileActorType
    {
        EnemyUnit,
        Structure,
        Trap,
    }

    [SerializeField] public TileActorType TAtype;
    [SerializeField] private int attackRange;
    [SerializeField] private int damage;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private (int, int)? currentTile = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
