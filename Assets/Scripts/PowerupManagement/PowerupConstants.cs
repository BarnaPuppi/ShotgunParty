using UnityEngine;

public static class PowerupConstants {
    public static class Heal {
        public const float healthRegenerated = 25f;
        public const float duration = 6f;
        public const float healIntervalDuration = 0.55f;
    }

    public static class Rage {
        public const float targetKnockbackReduction = 0.8f;
        public const float damageDealtMultiplier = 1.15f;
        public const float reloadReduction = 0.67f;
        public const float duration = 11f;
    }

    public static class Haste {
        public const float speedMultiplier = 1.4f;
        public const float jumpPowerMultiplier = 1.15f;
        public const float knockbackMultipiler = 1.12f;
        public const float duration = 10f;
    }

    public static class Jetpack {
        public const float jetpackPower = 55f;
        public const float duration = 1.6f;
    }

    public static class Armour {
        public const float damageReduction = 0.77f;
        public const float knockbackReduction = 0.7f;
        public const float speedReduction = 0.8f;
        public const float duration = 9f;
    }

    public static class Freeze {
        public const float speedReduction = 0.45f;
        public const float duration = 10f;
        public const float radius = 2.3f;
    }
    
    public static class Teleport {
        public const float distance = 4.5f;
    }
}