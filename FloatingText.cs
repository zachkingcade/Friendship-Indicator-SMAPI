using Microsoft.Xna.Framework;
using StardewValley;

    namespace FriendshipIndicator;
    internal class FloatingText
    {
        public NPC Npc { get; init; } = null!;
        public string Text { get; init; } = "";
        public Color Color { get; init; }

        public float Age { get; set; } = 0f;
        public float Duration { get; init; } = 1.5f;
    }