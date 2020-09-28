using System.Reflection;
using TerraLands.Buffs;
using TerraLands.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TerraLands.TLPrefixes
{
    public static class TLSuffixList
    {
        public static readonly TLSuffix Default = new TLSuffix(
            suffixID: -1,
            "Defaulcy",
            suffixRarity: SuffixRarity.Common
            );

        public static readonly TLSuffix Omnipercipiency = new TLSuffix(
            suffixID: 1,
            "Omnipercipiency",
            suffixRarity: SuffixRarity.Legendary,
            delegate (Player player)
            {
                player.AddBuff(ModContent.BuffType<Omnipercipiency>(), 5);
            }
            );

        #region Usable Methods
        /// <summary>
        /// Returns the TLSuffix instance of the given ID.
        /// </summary>
        public static TLSuffix GetInstanceWithID(int ID)
        {
            FieldInfo[] fieldInfoList = typeof(TLSuffixList).GetFields();
            foreach (FieldInfo field in fieldInfoList)
            {
                if (field.FieldType == typeof(TLSuffix))
                {
                    if (((TLSuffix)field.GetValue(field)).suffixID == ID)
                    {
                        return (TLSuffix)field.GetValue(field);
                    }
                }
            }
            return null;
        }

        public static TLSuffix GetRandomSuffix()
        {
            FieldInfo[] fieldInfoList = typeof(TLSuffixList).GetFields();
            TLSuffix chosenSuffix = null;
            while (chosenSuffix == null)
            {
                FieldInfo randomInfo = fieldInfoList[Main.rand.Next(0, fieldInfoList.Length)];
                if (randomInfo.FieldType == typeof(TLSuffix) && ((TLSuffix)randomInfo.GetValue(randomInfo)).suffixID != Default.suffixID)
                {
                    chosenSuffix = (TLSuffix)randomInfo.GetValue(randomInfo);
                }
            }
            return chosenSuffix;
        }
        #endregion
    }
}
