using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FriendshipIndicator;
internal sealed class ModEntry : Mod
{

    class Config
    {
        public bool enableChatAlerts = true;
        public bool enableNpcAlerts = true;
        public bool enableLossAlerts = true;
        public bool showSignOnNpcAlerts = true;
        public bool alwaysChatAlert = false;

        public int minimumChangeChatAlerts = 0;
        public int minimumChangeNpcAlerts = 0;

        public float npcAlertDurationInSeconds = 1.5f;

        public float npcAlertDistanceToFloatInPixels = 45f;
        public int npcAlertPercentageStartFade = 0;

        //ADVANCED SETTINGS
        public string gainedString = "gained";
        public string lossString = "lost";
        public string gainedChatAlertPrePlayersNameText = "";
        public string gainedChatAlertBeforeDifferenceText = " has ";

        public string gainedChatAlertInbetweenDifferenceAndNpcNameText = " friendship with ";
        public string gainedChatAlertAfterNpcNameText = "!";

        public string gainedNpcAlertIPreText = "";
        public string gainedNpcAlertIPostText = "";

        public string lossChatAlertPrePlayersNameText = "";
        public string lossChatAlertBeforeDifferenceText = " has ";

        public string lossChatAlertInbetweenDifferenceAndNpcNameText = " friendship with ";
        public string lossChatAlertAfterNpcNameText = "!";

        public string lossNpcAlertIPreText = "";
        public string lossNpcAlertIPostText = "";

        public string metChatAlertPrePlayersNameText = "";
        public string metChatAlertBetweenNamesText = " has met ";
        public string metChatAlertAfterNpcNameText = "!";
    }
    //member varibles
    private readonly List<FloatingText> floatingTexts = new();

    private Config config = new();
    private Dictionary<string, int> friendships = new();

    //Constructor/ SMAPI entry point
    public override void Entry(IModHelper helper)
    {
        //Game State Events
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        
        //Update Events
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
        helper.Events.GameLoop.UpdateTicked += this.UpdateFloatingTexts;
    }

//-----------------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------------
// Friendship Handling
//-----------------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------------

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        //This is just in case, clear old values, possibly from another save
        this.friendships.Clear();

        //Store our initial friendship data on load
        foreach (var pair in Game1.player.friendshipData.Pairs)
        {
            this.friendships[pair.Key] = pair.Value.Points;
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        //only run when world context is ready and every 10 ticks
        if (!Context.IsWorldReady || !e.IsMultipleOf(10)){
            return;
        }

        //short circuit the friendship existing first since that is most likely
        //This way of short circuiting at multiplay layers instead of doing and statements is more runtime efficent
        foreach (var pair in Game1.player.friendshipData.Pairs)
        {
            if (this.friendships.ContainsKey(pair.Key)){
                if(this.friendships[pair.Key] != pair.Value.Points)
                {
                    FriendshipChangeHandler(this.friendships[pair.Key], pair.Value.Points, pair.Key);

                    this.friendships[pair.Key] = pair.Value.Points;
                }
            } else {
                FriendshipChangeHandler(-1, pair.Value.Points, pair.Key);

                this.friendships[pair.Key] = pair.Value.Points;
            }   
        }
    }

    private void FriendshipChangeHandler(int originalValue, int newValue, string friendshipName)
    {
        //check if on current map
        NPC npc = Game1.getCharacterFromName(friendshipName);

        bool currentMap = npc.currentLocation == Game1.currentLocation;

        int difference = newValue - originalValue;

        if(difference < 0 && !config.enableLossAlerts) return;

        if(originalValue == -1)
        {
            onFriendshipChangeNewFriendship(friendshipName);
        }

        // Allowing both to hit is intentional
        if (currentMap)
        {
            onFriendshipChangeCurrentMap(difference, npc);
        } else
        {
            onFriendshipChangeOutsideCurrentMapOrUnknown(difference, friendshipName);
        }
    }
    private void onFriendshipChangeNewFriendship(string friendshipName)
    {
        if (!config.enableChatAlerts) return;

        Game1.chatBox.addInfoMessage($"{config.metChatAlertPrePlayersNameText}{Game1.player.Name}{config.metChatAlertBetweenNamesText}{friendshipName}{config.metChatAlertAfterNpcNameText}");
    }

    private void onFriendshipChangeCurrentMap(int difference, NPC npc)
    {
        if (!config.enableNpcAlerts) return;

        if (config.minimumChangeNpcAlerts > Math.Abs(difference)) return;

        string preText;
        string postText;

        if(difference > 0)
        {
            preText = config.gainedNpcAlertIPreText;
            postText = config.gainedNpcAlertIPostText;
        } else
        {
            preText = config.lossNpcAlertIPreText;
            postText = config.lossNpcAlertIPostText;
        }

        this.AddChangeText(npc, difference, preText, postText);

        if (config.alwaysChatAlert && config.minimumChangeChatAlerts <= Math.Abs(difference))
        {
            Game1.chatBox.addInfoMessage(this.constructChatAlertString(difference, npc.Name));
        }
    }

    private void onFriendshipChangeOutsideCurrentMapOrUnknown(int difference, string friendshipName)
    {
        if (!config.enableChatAlerts) return;

        if (config.minimumChangeChatAlerts > Math.Abs(difference)) return;

        Game1.chatBox.addInfoMessage(this.constructChatAlertString(difference, friendshipName));
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        //We clear in case they open another save file, we don't want to keep a previous saves friendship points
        this.friendships.Clear();
    }

    private string constructChatAlertString(int difference, string friendshipName)
    {
        string changeType = difference > 0? config.gainedString : config.lossString;

        if(difference > 0)
        {
            return $"{config.gainedChatAlertPrePlayersNameText}{Game1.player.Name}{config.gainedChatAlertBeforeDifferenceText}{changeType} {Math.Abs(difference)}{config.gainedChatAlertInbetweenDifferenceAndNpcNameText}{friendshipName}{config.gainedChatAlertAfterNpcNameText}";
        } else
        {
            return $"{config.lossChatAlertPrePlayersNameText}{Game1.player.Name}{config.lossChatAlertBeforeDifferenceText}{changeType} {Math.Abs(difference)}{config.lossChatAlertInbetweenDifferenceAndNpcNameText}{friendshipName}{config.lossChatAlertAfterNpcNameText}";
        }
        
    }

//-----------------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------------
// Text Alert Handling
//-----------------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------------

    private void AddChangeText(NPC npc, int change, string preText, string postText)
    {
        if (change == 0)
            return;

        Color color = change > 0 ? Color.LightGreen: Color.IndianRed;
        String sign = change > 0 ? "+" : "-";

        if (!config.showSignOnNpcAlerts)
        {
            sign = "";
        } 

        string text = $"{preText}{sign}{Math.Abs(change)}{postText}";

        this.floatingTexts.Add(new FloatingText
        {
            Npc = npc,
            Text = text,
            Color = color,
            Duration = config.npcAlertDurationInSeconds
        });
    }

    
    private void UpdateFloatingTexts(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            this.floatingTexts.Clear();
            return;
        }

        foreach (var popup in this.floatingTexts)
        {
            popup.Age += 1f / 60f;
        }

        this.floatingTexts.RemoveAll(
            popup => popup.Age >= popup.Duration
        );
    }

    private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        foreach (var popup in this.floatingTexts)
        {

            float progress = popup.Age / popup.Duration;
            float fadeStart = config.npcAlertPercentageStartFade / 100f;
            float normalizeProgress = (progress - fadeStart)/(1f-fadeStart);
            float opacity = Math.Clamp(1f - normalizeProgress, 0f, 1f);

            // Start above the NPC's head.
            Vector2 worldPosition = popup.Npc.Position
                + new Vector2(32f, -40f);

            // Float upward by 45 pixels.
            worldPosition.Y -= config.npcAlertDistanceToFloatInPixels * progress;

            Vector2 screenPosition = Game1.GlobalToLocal(
                Game1.viewport,
                worldPosition
            );

            // Center text horizontally.
            Vector2 textSize = Game1.smallFont.MeasureString(popup.Text);

            screenPosition.X -= textSize.X / 2f;

            e.SpriteBatch.DrawString(
                Game1.smallFont,
                popup.Text,
                screenPosition,
                popup.Color * opacity
            );
        }
    }

}