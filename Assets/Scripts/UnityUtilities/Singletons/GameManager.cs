namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/Singletons/Game Manager")]
    public class GameManager : Singleton<GameManager>
    {
        public Player player { get; private set; }

        protected override void SetDefaults()
        {
            //get references
            player = FindObjectOfType<Player>();

            //if there is a player, lock mouse
            if (player)
            {
                Utility.LockMouse(CursorLockMode.Locked);
            }
        }
    }
}