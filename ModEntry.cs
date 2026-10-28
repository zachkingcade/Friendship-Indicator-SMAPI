using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FriendshipIndicator;
internal sealed class ModEntry : Mod
{

    private class FloatingText
    {
        public NPC Npc { get; init; } = null!;
        public string Text { get; init; } = "";
        public Color Color { get; init; }

        public float Age { get; set; } = 0f;
        public float Duration { get; init; } = 1.5f;
    }

    private readonly List<FloatingText> floatingTexts = new();

    private Dictionary<string, int> friendships = new();
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
        helper.Events.GameLoop.UpdateTicked += this.UpdateFloatingTexts;

        // Subscribe to an event that happens after a save loads.
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

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
        Game1.chatBox.addInfoMessage($"{Game1.player.Name} has met {friendshipName}!");
    }

    private void onFriendshipChangeCurrentMap(int difference, NPC npc)
    {
        this.AddChangeText(npc, difference);
    }

    private void onFriendshipChangeOutsideCurrentMapOrUnknown(int difference, string friendshipName)
    {
        string changeType = difference > 0? "gained" : "lost";

        Game1.chatBox.addInfoMessage($"{Game1.player.Name} has {changeType} {Math.Abs(difference)} friendship with {friendshipName}!");
    }

    private void AddChangeText(NPC npc, int change)
    {
        if (change == 0)
            return;

        Color color = change > 0 ? Color.LightGreen: Color.IndianRed;
        String sign = change > 0 ? "+" : "-";

        string text = $"{sign}{Math.Abs(change)}";

        this.floatingTexts.Add(new FloatingText
        {
            Npc = npc,
            Text = text,
            Color = color
        });
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        //We clear in case they open another save file, we don't want to keep a previous saves friendship points
        this.friendships.Clear();
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
            float opacity = 1f - progress;

            // Start above the NPC's head.
            Vector2 worldPosition = popup.Npc.Position
                + new Vector2(32f, -40f);

            // Float upward by 45 pixels.
            worldPosition.Y -= 45f * progress;

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