using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TNT.Constants
{
    public static class Layers
    {
        public static readonly int PLAYER = LayerMask.GetMask("Player");
        public static readonly int ENEMY = LayerMask.GetMask("Enemy");
        public static readonly int PLAYER_SPELL = LayerMask.GetMask("PlayerSpell");
        public static readonly int ENEMY_SPELL = LayerMask.GetMask("EnemeSpell");
    }
}