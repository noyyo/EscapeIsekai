using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image hpBar_Image;
    public Image stamina_Image;
    public Image hunger_Image;
    public Image powerUp_Image;
    public Image slashSkill_Image;
    public Image SuperJump_Image;
    public Image Throw_Image;
    public Image NoStamina_Image;
    public Image Shield_Image;
    public Image Roll_Image;
    public Image[] Locks = new Image[3];

    private void Start()
    {
        Player player = GameManager.Instance.Player.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("플레이어를 찾지 못했습니다.");
        }
        Playerconditions conditions = player.Playerconditions;
        conditions.health.ConditionUpdated += hpUpdate;
        conditions.stamina.ConditionUpdated += staminaUpdate;
        conditions.hunger.ConditionUpdated += hungerUpdate;
        conditions.powerUp.ConditionUpdated += powerUPUpdate;
        conditions.skill.ConditionUpdated += slashSkillUpdate;
        conditions.superJump.ConditionUpdated += superJumpUpdate;
        conditions.throwskill.ConditionUpdated += ThrowUpdate;
        conditions.noStamina.ConditionUpdated += noStanminaUpdate;
        conditions.shield.ConditionUpdated += shieldUpdate;
        conditions.rollCoolTime.ConditionUpdated += rollUpdate;
    }
    private void hpUpdate(Condition condition) => hpBar_Image.fillAmount = condition.GetPercentage();
    private void staminaUpdate(Condition condition) => stamina_Image.fillAmount = condition.GetPercentage();
    private void hungerUpdate(Condition condition) => hunger_Image.fillAmount = condition.GetPercentage();
    private void powerUPUpdate(Condition condition) => powerUp_Image.fillAmount = condition.GetPercentage();
    private void slashSkillUpdate(Condition condition) => slashSkill_Image.fillAmount = condition.GetPercentage();
    private void superJumpUpdate(Condition condition) => SuperJump_Image.fillAmount = condition.GetPercentage();
    private void ThrowUpdate(Condition condition) => Throw_Image.fillAmount = condition.GetPercentage();
    private void noStanminaUpdate(Condition condition) => NoStamina_Image.fillAmount = condition.GetPercentage();
    private void shieldUpdate(Condition condition) => Shield_Image.fillAmount = condition.GetPercentage();
    private void rollUpdate(Condition condition) => Roll_Image.fillAmount = condition.GetPercentage();
}
