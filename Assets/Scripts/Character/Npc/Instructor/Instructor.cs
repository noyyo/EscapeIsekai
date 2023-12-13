using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Instructor : CustomSingleton<Instructor>
{
   public int rank = 0;
    [SerializeField]GameObject effect;
    [SerializeField]GameObject rankUpUI;
    [SerializeField] TextMeshProUGUI text;
    PlayerInputSystem playerInputSystem;
    private void Awake()
    {
        playerInputSystem = GameManager.Instance.Player.GetComponent<PlayerInputSystem>();
        effect = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/RankUpEffect"));
        rankUpUI = Instantiate(Resources.Load<GameObject>("Prefabs/UI/RankUpUI"),GameManager.Instance.Ui_Manager.Canvas.transform);
        rankUpUI.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(TurnOff);
        text = rankUpUI.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        rankUpUI.SetActive(false);
    }
    private void GameFail()
    {
        MinigameManager.Instance.ChangeSuccess -= GameFailorSuc;
    }
    private void GameSuc()
    {
        if (rank < 5)
        {
            playerInputSystem.PlayerActions.Disable();
            rank++;
            Cursor.lockState = CursorLockMode.None;
            rankUpUI.SetActive(true);
            text.text = $"남은기회 : {5-rank}";
            SoundManager.Instance.CallPlaySFX(ClipType.NPCSFX, "RankUpSound", this.transform, false);
            effect.transform.position = GameManager.Instance.Player.transform.position;
            effect.GetComponent<ParticleSystem>().Play();
            GameManager.Instance.Player.GetComponent<Player>().Playerconditions.Power += 2;
        }
        MinigameManager.Instance.ChangeSuccess -= GameFailorSuc;
    }
    public void GameFailorSuc(int val)
    {
        if (val == 1)
            GameSuc();
        if (val == -1)
            GameFail();
    }
    public void TurnOff()
    {
        playerInputSystem.PlayerActions.Enable();
        rankUpUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
