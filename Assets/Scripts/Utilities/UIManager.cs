namespace redd096
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject pauseMenu = default;
        [SerializeField] GameObject endMenu = default;
        [SerializeField] Text counter_Text = default;

        public void PauseMenu(bool active)
        {
            //show/hide pause menu
            if(pauseMenu)
                pauseMenu.SetActive(active);
        }

        public void EndMenu(bool active)
        {
            //show/hide end menu
            if (endMenu)
                endMenu.SetActive(active);
        }

        public void UpdateEnemyCounter(int enemyCounter)
        {
            //update enemy counter
            counter_Text.text = enemyCounter.ToString("F0");
        }
    }
}