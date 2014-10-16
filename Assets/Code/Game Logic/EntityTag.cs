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
    }
}