using System.Collections.Generic;
using System.Reflection;
using TerraLands.Utils;

namespace TerraLands.TLPrefixes
{
    /// <summary>
    /// Class full of all of the TLPrefixes. Has a few helper methods for getting IDs, types, or text.
    /// </summary>
    public static class TLPrefixList
    {
        #region Prefixes
        public static readonly TLPrefix Default = new TLPrefix(
            prefixID: 0,
            "Default",
            PrefixType.None,
            rngWeight: 0f
            );

        public static readonly TLPrefix Cathartic = new TLPrefix(
            prefixID: 1,
            "Cathartic",
            PrefixType.Ranged,
            dmgMult: 1.05f
            );

        public static readonly TLPrefix DaDerp = new TLPrefix(
            prefixID: 2,
            "Da derp",
            PrefixType.Ranged,
            dmgMult: 0.8f,
            critChanceMod: -5,
            useSpeedMult: 1.34f,
            velocityMult: 1.34f,
            accMult: 0.75f
            );

        public static readonly TLPrefix Macro = new TLPrefix(
            prefixID: 3,
            "Macro",
            PrefixType.Ranged,
            dmgMult: 1.25f,
            critChanceMod: 5,
            useSpeedMult: 0.5f,
            velocityMult: 0.75f,
            scaleMult: 1.34f,
            knockMult: 1.5f
            );

        public static readonly TLPrefix Implicit = new TLPrefix(
            prefixID: 4,
            "Implicit",
            PrefixType.Special
            );
        #endregion

        #region Usable Methods
        /// <summary>
        /// Returns the TLPrefix instance of the given ID.
        /// </summary>
        public static TLPrefix GetInstanceWithID(int ID)
        {
            FieldInfo[] fieldInfoList = typeof(TLPrefixList).GetFields();
            foreach (FieldInfo field in fieldInfoList)
            {
                if (field.FieldType == typeof(TLPrefix))
                {
                    if (((TLPrefix)field.GetValue(field)).TLPrefixID == ID)
                    {
                        return (TLPrefix)field.GetValue(field);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a list of available TL Prefixes that all share the type of PrefixType passed through.
        /// Will return an empty list if none exist or if an error occurs.
        /// </summary>
        public static List<TLPrefix> GetListOfPrefixesWithType(PrefixType prefixType)
        {
            FieldInfo[] fieldInfoArray = typeof(TLPrefixList).GetFields();
            List<TLPrefix> listOfAvailablePrefixes = new List<TLPrefix>();
            foreach (FieldInfo field in fieldInfoArray)
            {
                if (field.FieldType == typeof(TLPrefix))
                {
                    if (((TLPrefix)field.GetValue(field)).prefixType == prefixType)
                    {
                        listOfAvailablePrefixes.Add((TLPrefix)field.GetValue(field));
                    }
                }
            }
            return listOfAvailablePrefixes;
        }
        #endregion
    }
}
