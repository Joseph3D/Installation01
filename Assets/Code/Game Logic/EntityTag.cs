using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    [Flags]
    public enum EntityTag
    {
        GameEntity = 0,
        AIEntity = 1 << 0,
        Player = 1 << 1,
        Enemy = 1 << 2,
        SpawnPoint = 1 << 3,
        Projectile = 1 << 4,
        SceneryObject = 1 << 5,
        Surface_Dirt = 1 << 6,
        Surface_Glass = 1 << 7,
        Surface_Metal = 1 << 8,
        Surface_Gore = 1 << 9,
        Surface_ForerunnerHardLight = 1 << 10,
        Surface_Gravel = 1 << 11,
        Surface_Grass = 1 << 12,
    }
}