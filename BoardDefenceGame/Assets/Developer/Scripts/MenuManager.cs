using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    public GameObject mainMenuGO;
    public GameObject moneyPanel;
    public GameObject starPanel;
    public GameObject itemBuyButtonGO;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        mainMenuGO.SetActive(true);
        moneyPanel.SetActive(false);
        starPanel.SetActive(true);
    }

    public void StartButton()
    {
        mainMenuGO.SetActive(false);
        moneyPanel.SetActive(true);
        starPanel.SetActive(false);

        Globals.itemClickActive = true;
        SlotBuyManager.Instance.StartGame();
        GameEventSystem.RPGStartEvent();

        ItemCreatorSubPanel.Instance.FirstItemCreator();
        EnergyManager.Instance.StartGame();

        itemBuyButtonGO.SetActive(true);
        WaveProgressManager.Instance.waveProgress.gameObject.SetActive(true);
        WaveProgressManager.Instance.WaveProgressStart(Globals.chapterCount);
    }

    public void OpenSkillMenu()
    {
        mainMenuGO.SetActive(false);
    }
}
