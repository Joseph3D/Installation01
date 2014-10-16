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
        AIEntity = 1,
        Player = 2,
        Enemy = 3,
        SpawnPoint = 4,
        Projectile = 5,
        SceneryObject = 6,
        Surface_Dirt = 7,
        Surface_Glass = 8,
        Surface_Metal = 9,
        Surface_Gore = 10,
        Surface_ForerunnerHardLight = 11,
        Surface_Gravel = 12,
    }
}