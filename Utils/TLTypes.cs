using Terraria.ID;

namespace TerraLands.Utils
{
    public enum PrefixType
    {
        None,
        Ranged,
        Melee,
        Magic,
        Summon,
        Special,
    }

    public enum SuffixRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum TLRarities
    {
        Common = ItemRarityID.White,
        Uncommon = ItemRarityID.Green,
        Rare = ItemRarityID.Blue,
        Epic = ItemRarityID.Purple,
        Legendary = ItemRarityID.Quest,
        Seraph = ItemRarityID.Pink,
        Perlescent = ItemRarityID.Cyan,
        Effervescent = ItemRarityID.Expert
    }

    public enum ElementType
    {
        Default = -1,
        None,
        Explosive,
        Fire,
        Shock,
        Corrosive,
        Cryo
    }
}
