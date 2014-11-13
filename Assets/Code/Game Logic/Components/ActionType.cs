using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    /// <summary>
    /// Represents the type of behavior that this action will trigge
    /// </summary>
    [Flags]
    public enum ActionType
    {
        PlaySoundEffect = 0,
        SpawnEntity = 1 << 0,
        DespawnEntity = 1 << 1,
        DespawnAllWithTag = 1 << 2,
        UnityAPICall_SendMessage = 1 << 3,
    }
}