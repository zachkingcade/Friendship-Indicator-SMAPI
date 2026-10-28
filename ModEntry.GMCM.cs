
using StardewModdingAPI.Events;
using FriendshipIndicator.Integrations;

namespace FriendshipIndicator;

internal sealed partial class ModEntry
{
    private void OnGameLaunched(
        object? sender,
        GameLaunchedEventArgs e)
    {
        var gmcm = this.Helper.ModRegistry
            .GetApi<IGenericModConfigMenuApi>(
                "spacechase0.GenericModConfigMenu"
            );

        if (gmcm is null)
            return;

        gmcm.Register(
            mod: this.ModManifest,
            reset: () => this.config = new ModConfig(),
            save: () => this.Helper.WriteConfig(this.config)
        );

        //---------------------------------------------
        // GENERAL SETTINGS
        //---------------------------------------------

        gmcm.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "General Settings"
        );

        gmcm.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Enable Chat Alerts",
            tooltip: () =>
                "Display friendship changes in the in-game chat.",
            getValue: () => this.config.enableChatAlerts,
            setValue: value => this.config.enableChatAlerts = value
        );

        gmcm.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Enable NPC Alerts",
            tooltip: () =>
                "Display floating friendship changes above nearby NPCs.",
            getValue: () => this.config.enableNpcAlerts,
            setValue: value => this.config.enableNpcAlerts = value
        );

        gmcm.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Show Friendship Loss",
            tooltip: () =>
                "Show notifications when friendship points decrease.",
            getValue: () => this.config.enableLossAlerts,
            setValue: value => this.config.enableLossAlerts = value
        );

        gmcm.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Always Show Chat Alerts",
            tooltip: () =>
                "Also send friendship changes to chat when the NPC is nearby.",
            getValue: () => this.config.alwaysChatAlert,
            setValue: value => this.config.alwaysChatAlert = value
        );

        gmcm.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Minimum Chat Change",
            tooltip: () =>
                "Minimum friendship points gained or lost before showing a chat alert. Set to 0 for all changes.",
            getValue: () => this.config.minimumChangeChatAlerts,
            setValue: value => this.config.minimumChangeChatAlerts = value,
            min: 0,
            max: 250,
            interval: 5
        );

        gmcm.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Minimum NPC Change",
            tooltip: () =>
                "Minimum friendship points gained or lost before showing floating text. Set to 0 for all changes.",
            getValue: () => this.config.minimumChangeNpcAlerts,
            setValue: value => this.config.minimumChangeNpcAlerts = value,
            min: 0,
            max: 250,
            interval: 5
        );

        //---------------------------------------------
        // NPC ALERT APPEARANCE
        //---------------------------------------------

        gmcm.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "NPC Alert Appearance"
        );

        gmcm.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Show Positive/Negative Sign",
            tooltip: () =>
                "Include a + or - sign before the floating friendship number.",
            getValue: () => this.config.showSignOnNpcAlerts,
            setValue: value => this.config.showSignOnNpcAlerts = value
        );

        gmcm.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Alert Duration",
            tooltip: () =>
                "How many seconds the floating notification stays on screen.",
            getValue: () => this.config.npcAlertDurationInSeconds,
            setValue: value => this.config.npcAlertDurationInSeconds = value,
            min: 0.5f,
            max: 10f,
            interval: 0.25f,
            formatValue: value => $"{value:0.00}s"
        );

        gmcm.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Float Distance",
            tooltip: () =>
                "How many pixels the floating notification travels upward.",
            getValue: () => this.config.npcAlertDistanceToFloatInPixels,
            setValue: value => this.config.npcAlertDistanceToFloatInPixels = value,
            min: 0f,
            max: 200f,
            interval: 5f,
            formatValue: value => $"{value:0} px"
        );

        gmcm.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Start Fading At",
            tooltip: () =>
                "Percentage of the animation completed before fading begins. 100% means no gradual fading.",
            getValue: () => this.config.npcAlertPercentageStartFade,
            setValue: value => this.config.npcAlertPercentageStartFade = value,
            min: 0,
            max: 100,
            interval: 5,
            formatValue: value => $"{value}%"
        );

        //---------------------------------------------
        // ADVANCED - CHAT WORDING
        //---------------------------------------------

        gmcm.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Advanced - Chat Wording"
        );

        gmcm.AddParagraph(
            mod: this.ModManifest,
            text: () =>
                "Customize the words used in friendship gain and loss messages."
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Gained Word",
            tooltip: () =>
                "Word describing positive friendship changes. Default: gained.",
            getValue: () => this.config.gainedString,
            setValue: value => this.config.gainedString = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Lost Word",
            tooltip: () =>
                "Word describing negative friendship changes. Default: lost.",
            getValue: () => this.config.lossString,
            setValue: value => this.config.lossString = value
        );

        //---------------------------------------------
        // ADVANCED - FRIENDSHIP GAIN CHAT
        //---------------------------------------------

        gmcm.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Friendship Gain Chat Text"
        );

        gmcm.AddParagraph(
            mod: this.ModManifest,
            text: () =>
                "Default: Player has gained 20 friendship with Abigail!"
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Before Player Name",
            tooltip: () =>
                "Text inserted before the player's name in gain messages.",
            getValue: () => this.config.gainedChatAlertPrePlayersNameText,
            setValue: value => this.config.gainedChatAlertPrePlayersNameText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "After Player Name",
            tooltip: () =>
                "Text between the player's name and the gained word. Default: ' has '.",
            getValue: () => this.config.gainedChatAlertBeforeDifferenceText,
            setValue: value => this.config.gainedChatAlertBeforeDifferenceText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Before NPC Name",
            tooltip: () =>
                "Text between the friendship amount and NPC name. Default: ' friendship with '.",
            getValue: () => this.config.gainedChatAlertInbetweenDifferenceAndNpcNameText,
            setValue: value => this.config.gainedChatAlertInbetweenDifferenceAndNpcNameText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "After NPC Name",
            tooltip: () =>
                "Text appended after the NPC name. Default: !",
            getValue: () => this.config.gainedChatAlertAfterNpcNameText,
            setValue: value => this.config.gainedChatAlertAfterNpcNameText = value
        );

        //---------------------------------------------
        // ADVANCED - FRIENDSHIP LOSS CHAT
        //---------------------------------------------

        gmcm.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Friendship Loss Chat Text"
        );

        gmcm.AddParagraph(
            mod: this.ModManifest,
            text: () =>
                "Default: Player has lost 20 friendship with Abigail!"
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Before Player Name",
            tooltip: () =>
                "Text inserted before the player's name in loss messages.",
            getValue: () => this.config.lossChatAlertPrePlayersNameText,
            setValue: value => this.config.lossChatAlertPrePlayersNameText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "After Player Name",
            tooltip: () =>
                "Text between the player's name and the lost word. Default: ' has '.",
            getValue: () => this.config.lossChatAlertBeforeDifferenceText,
            setValue: value => this.config.lossChatAlertBeforeDifferenceText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Before NPC Name",
            tooltip: () =>
                "Text between the friendship amount and NPC name.",
            getValue: () => this.config.lossChatAlertInbetweenDifferenceAndNpcNameText,
            setValue: value => this.config.lossChatAlertInbetweenDifferenceAndNpcNameText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "After NPC Name",
            tooltip: () =>
                "Text appended after the NPC name in loss messages.",
            getValue: () => this.config.lossChatAlertAfterNpcNameText,
            setValue: value => this.config.lossChatAlertAfterNpcNameText = value
        );

        //---------------------------------------------
        // ADVANCED - FLOATING NPC TEXT
        //---------------------------------------------

        gmcm.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Advanced - Floating NPC Text"
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Gain Prefix",
            tooltip: () =>
                "Text displayed before a positive floating friendship number.",
            getValue: () => this.config.gainedNpcAlertIPreText,
            setValue: value => this.config.gainedNpcAlertIPreText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Gain Suffix",
            tooltip: () =>
                "Text displayed after a positive floating friendship number.",
            getValue: () => this.config.gainedNpcAlertIPostText,
            setValue: value => this.config.gainedNpcAlertIPostText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Loss Prefix",
            tooltip: () =>
                "Text displayed before a negative floating friendship number.",
            getValue: () => this.config.lossNpcAlertIPreText,
            setValue: value => this.config.lossNpcAlertIPreText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Loss Suffix",
            tooltip: () =>
                "Text displayed after a negative floating friendship number.",
            getValue: () => this.config.lossNpcAlertIPostText,
            setValue: value => this.config.lossNpcAlertIPostText = value
        );

        //---------------------------------------------
        // ADVANCED - MEETING NEW NPCs
        //---------------------------------------------

        gmcm.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Advanced - New Friendship Text"
        );

        gmcm.AddParagraph(
            mod: this.ModManifest,
            text: () =>
                "Default: Player has met Abigail!"
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Before Player Name",
            tooltip: () =>
                "Text inserted before the player's name when meeting an NPC.",
            getValue: () => this.config.metChatAlertPrePlayersNameText,
            setValue: value => this.config.metChatAlertPrePlayersNameText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "Between Names",
            tooltip: () =>
                "Text between the player's name and the newly met NPC's name.",
            getValue: () => this.config.metChatAlertBetweenNamesText,
            setValue: value => this.config.metChatAlertBetweenNamesText = value
        );

        gmcm.AddTextOption(
            mod: this.ModManifest,
            name: () => "After NPC Name",
            tooltip: () =>
                "Text appended after the newly met NPC's name.",
            getValue: () => this.config.metChatAlertAfterNpcNameText,
            setValue: value => this.config.metChatAlertAfterNpcNameText = value
        );
    }
}
