using System;
using System.Collections.Generic;
using UnityEngine;
using static MapGenerator;

class LightningBolt : Spell
{
    public override TargetType type => TargetType.Tile;
    public override int range => 4;
    public override int damage => 3;
    public override int heal => 0;
    public override bool friendlyFire => false;
    public override string cardName => "Lightning Bolt";

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