
namespace FriendshipIndicator;

public class ModConfig
{
    //---------------------------------------------
    // GENERAL SETTINGS
    //---------------------------------------------
    public bool enableChatAlerts { get; set; } = true;
    public bool enableNpcAlerts { get; set; } = true;
    public bool enableLossAlerts { get; set; } = true;
    public bool showSignOnNpcAlerts { get; set; } = true;
    public bool alwaysChatAlert { get; set; } = false;
    public int minimumChangeChatAlerts { get; set; } = 0;
    public int minimumChangeNpcAlerts { get; set; } = 0;


    //---------------------------------------------
    // FLOATING NPC ALERT SETTINGS
    //---------------------------------------------
    public float npcAlertDurationInSeconds { get; set; } = 1.5f;
    public float npcAlertDistanceToFloatInPixels { get; set; } = 45f;
    public int npcAlertPercentageStartFade { get; set; } = 0;


    //---------------------------------------------
    // ADVANCED - SHARED CHAT TEXT
    //---------------------------------------------

    public string gainedString { get; set; } = "gained";
    public string lossString { get; set; } = "lost";


    //---------------------------------------------
    // ADVANCED - FRIENDSHIP GAIN CHAT TEXT
    //---------------------------------------------

    public string gainedChatAlertPrePlayersNameText { get; set; } = "";

    public string gainedChatAlertBeforeDifferenceText { get; set; } = " has ";

    public string gainedChatAlertInbetweenDifferenceAndNpcNameText { get; set; }
        = " friendship with ";

    public string gainedChatAlertAfterNpcNameText { get; set; } = "!";


    //---------------------------------------------
    // ADVANCED - FRIENDSHIP LOSS CHAT TEXT
    //---------------------------------------------

    public string lossChatAlertPrePlayersNameText { get; set; } = "";

    public string lossChatAlertBeforeDifferenceText { get; set; } = " has ";

    public string lossChatAlertInbetweenDifferenceAndNpcNameText { get; set; }
        = " friendship with ";

    public string lossChatAlertAfterNpcNameText { get; set; } = "!";


    //---------------------------------------------
    // ADVANCED - FLOATING NPC TEXT
    //---------------------------------------------

    public string gainedNpcAlertIPreText { get; set; } = "";
    public string gainedNpcAlertIPostText { get; set; } = "";

    public string lossNpcAlertIPreText { get; set; } = "";
    public string lossNpcAlertIPostText { get; set; } = "";


    //---------------------------------------------
    // ADVANCED - NEW FRIENDSHIP CHAT TEXT
    //---------------------------------------------

    public string metChatAlertPrePlayersNameText { get; set; } = "";

    public string metChatAlertBetweenNamesText { get; set; }
        = " has met ";

    public string metChatAlertAfterNpcNameText { get; set; } = "!";
}
