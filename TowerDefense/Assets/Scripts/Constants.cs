using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Constant helper variables
    /// </summary>
    public static class Constants
    {
        public static readonly Color RedColor = new Color(1f, 0f, 0f, 0f);
        public static readonly Color BlackColor = new Color(0f, 0f, 0f, 0f);
        public static readonly Color BackgroundColor = new Color(132/255f, 147/255f, 48/255f, 0f);
        public static readonly int CannonCost = 50;
        public static readonly int CarrotAward = 10;
        public static readonly int InitialEnemyHealth = 50;
        public static readonly int EnemyAward = 10;
        public static readonly int Ball = 50;
        public static readonly float MinDistanceForCannonToShoot = 10f;
        
    }
}
