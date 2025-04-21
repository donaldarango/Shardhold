using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MapGenerator;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "Spell", menuName = "Scriptable Objects/Spell")]
class Spell : Card
{
    [Header("Spell Stats")]
    public int heal;
    public bool friendlyFire;
    public bool unincludeUnits;

    public GameObject animation;
    public Vector3 animationOffset = new Vector3(0.0f, 0.0f, 0.0f);

    public override CardType cardType => CardType.Spell;

    private void OnEnable()
    {
        //MapGenerator.PlayCard += OnPlayCard;
        Debug.Log("Test");
    }

    private void OnDisable()
    {
        //MapGenerator.PlayCard -= OnPlayCard;
    }

    //public void BeginPlay(HashSet<(int, int)> tiles)
    //{
    //    base.coordSet = tiles;
    //    Play();
    //}


    public override void Play(HashSet<(int, int)> tiles)
    {
        base.Play(tiles);

        SoundFXManager.instance.PlaySoundFXClip(audioClip, SoundFXManager.instance.transform, audioVolume);

        coordSet = tiles;
        Debug.Log("play called:");
        if (coordSet == null) { return; }
        if (damage > 0) { damageArea(); }
        if (heal > 0) { healArea(); }
    }

    public void damageArea()
    {
        Debug.Log("coordset : " + coordSet.Count);
        foreach(var tile in coordSet)
        {
            MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
            TileActor actor = target.GetCurrentTileActor();
            PlaySpellAnimation(target.GetTileCenter());
            Debug.Log("tile - " + target.name + " | " + tile.Item1 + " | " + tile.Item2);
            if(actor && actor.GetTileActorType() != TileActor.TileActorType.Trap && (actor.GetTileActorType() != TileActor.TileActorType.Structure || friendlyFire))
            {
                Debug.Log("attacking - " + actor.name + " | " + damage);
                actor.TakeDamage(damage); //hurt ally units if friendly fire is enabled.
            }
        }
    }

    public void healArea()
    {
        foreach (var tile in coordSet)
        {
            if (tile.Item1 == -1 && tile.Item2 == -1)
            {
                var Base = GameObject.Find("Base");
                Base.GetComponent<Base>().Heal(heal);
                PlaySpellAnimation(new Vector3(0,0,0));
            }
            else
            {
                MapTile target = MapManager.Instance.GetTile(tile.Item1, tile.Item2);
                TileActor actor = target.GetCurrentTileActor();

                if (actor && actor.GetTileActorType() != TileActor.TileActorType.Trap && (actor.GetTileActorType() != TileActor.TileActorType.EnemyUnit || friendlyFire))
                {
                    actor.SetCurrentHealth(Math.Min(actor.tileActorStats.maxHealth, actor.GetCurrentHealth() + heal)); //no overheal. heal enemy units if friendly fire is enabled 
                }
            }
        }

        if (!unincludeUnits)
        {
            foreach(GameObject uicard in Deck.Instance.UIHand)
            {
                if(uicard != null)
                {
                    var unit = uicard.GetComponent<AllyUnit>();
                    if (unit)
                    {
                        unit.currentHealth = Mathf.Min(unit.stats.hp, unit.currentHealth + heal);
                        var ui = unit.GetComponent<CardUI>();
                        if (ui)
                        {
                            ui.updateHealth(unit.currentHealth);
                        }
                    }
                }
            }
        }
    }

    private void PlaySpellAnimation(Vector3 tileCoords)
    {
        Vector3 pos = tileCoords + animationOffset;
        if (animation != null)
        {
            GameObject obj = Instantiate(animation, pos, Quaternion.identity);
            SpellAnimation spellAnimation = obj.GetComponent<SpellAnimation>();
            if (spellAnimation == null)
            {
                Debug.LogWarning("Prefab does not have SpellAnimation script attached.");
            }
        }
    }
}

