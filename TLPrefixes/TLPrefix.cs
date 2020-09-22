using TerraLands.Utils;

namespace TerraLands.TLPrefixes
{
    /// <summary>
    /// Class used for the creation of custom TerraLands prefixes. Must be declared with an ID, Prefix Text Value, and Prefix Type. All stat multipliers are optional.
    /// </summary>
    public class TLPrefix
    {
        public readonly int TLPrefixID;

        public readonly string prefixText;

        public readonly PrefixType prefixType;

        /// <summary>
        /// The "rarity" of the prefix. 1f is normal, anything <1f is rarer, and anything >1f is more common
        /// </summary>
        public readonly float weight;

        /// <summary>
        /// How much the weapon's damage is multiplied by.
        /// </summary>
        public readonly float damageMultiplier;

        /// <summary>
        /// A weapon's crit chance will increase/decrease by this.
        /// </summary>
        public readonly int critChanceModification;

        /// <summary>
        /// How much more damage a crit does is multiplied by this.
        /// </summary>
        public readonly float critDamageMultiplier;

        /// <summary>
        /// How much the weapon's use speed is multiplied by.
        /// Remember that INCREASING this multiplier increases speed, and vice versa for DECREASING.
        /// </summary>
        public readonly float useSpeedMultiplier;

        /// <summary>
        /// If the weapon has a projectile, its velocity is multiplied by this.
        /// </summary>
        public readonly float projVelocityMultiplier;

        /// <summary>
        /// If the weapon has a projectile, its scale is multiplied by this.
        /// </summary>
        public readonly float scaleMultiplier;

        /// <summary>
        /// How much the weapon's knockback is multiplied by.
        /// </summary>
        public readonly float knockbackMultiplier;

        /// <summary>
        /// For weapons with projectiles; what the projectile's accuracy is multiplied by.
        /// </summary>
        public readonly float accuracyMultiplier;

        public TLPrefix(int prefixID, string preText, PrefixType preType, float rngWeight = 1f, float dmgMult = 1f, int critChanceMod = 0, float critDmgMult = 1f, float useSpeedMult = 1f, float velocityMult = 1f, float scaleMult = 1f, float knockMult = 1f, float accMult = 1f)
        {
            TLPrefixID = prefixID;
            prefixText = preText;
            prefixType = preType;
            weight = rngWeight;
            damageMultiplier = dmgMult;
            critChanceModification = critChanceMod;
            critDamageMultiplier = critDmgMult;
            projVelocityMultiplier = velocityMult;
            useSpeedMultiplier = useSpeedMult;
            scaleMultiplier = scaleMult;
            knockbackMultiplier = knockMult;
            accuracyMultiplier = accMult;
        }
    }
}
