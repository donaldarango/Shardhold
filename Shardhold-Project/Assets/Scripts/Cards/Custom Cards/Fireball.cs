using UnityEngine;
using static MapGenerator;

class Fireball : Spell
{
    public override TargetType type => TargetType.Quadrant;
    public override int range => 2;
    public override int damage => 2;
    public override int heal => 0;
    public override bool friendlyFire => true;
    public override string cardName => "Fireball";

    public override void Play()
    {
        base.Play();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
