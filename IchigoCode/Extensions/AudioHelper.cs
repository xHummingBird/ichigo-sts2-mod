using MegaCrit.Sts2.Core.Commands;

namespace Ichigo.IchigoCode.Extensions;

public static class AudioHelper
{
    private static readonly Random rng = new Random();

    private static readonly string[] attackSfx =
    {
        "res://Ichigo/sounds/attack.wav",
        "res://Ichigo/sounds/attack_1.wav",
        "res://Ichigo/sounds/attack_2.wav",
        "res://Ichigo/sounds/attack_3.wav",
        "res://Ichigo/sounds/attack_4.wav",
        "res://Ichigo/sounds/attack_5.wav",
        "res://Ichigo/sounds/attack_6.wav",
        "res://Ichigo/sounds/attack_7.wav",
    };
    
    private static readonly string[] damagedSfx =
    {
        "res://Ichigo/sounds/hit.wav",
        "res://Ichigo/sounds/hit_1.wav",
        "res://Ichigo/sounds/hit_2.wav",
    };
    
    private static readonly string[] highDamagedSfx =
    {
        "res://Ichigo/sounds/hit_hard.wav",
        "res://Ichigo/sounds/hit_hard_1.wav",
    };
    
    private static readonly string[] criticalDamagedSfx =
    {
        "res://Ichigo/sounds/hit_critical.wav",
        "res://Ichigo/sounds/hit_critical_1.wav",
    };
    
    private static readonly string[] victorySfx =
    {
        "res://Ichigo/sounds/victory_2.mp3",
        "res://Ichigo/sounds/victory.wav",
        "res://Ichigo/sounds/victory_1.wav",
        "res://Ichigo/sounds/victory_3.wav",
    };

    private static readonly string[] gameoverSfx =
    {
        "res://Ichigo/sounds/gameover.wav",
        "res://Ichigo/sounds/gameover_1.wav",
        "res://Ichigo/sounds/gameover_2.wav",
    };
    
    private static readonly string[] phraseSfx =
    {
        "res://Ichigo/sounds/phrase.wav",
        "res://Ichigo/sounds/phrase_1.wav",
        "res://Ichigo/sounds/phrase_2.wav",
        "res://Ichigo/sounds/phrase_3.wav",
        "res://Ichigo/sounds/phrase_4.wav",
        "res://Ichigo/sounds/phrase_5.wav",
        "res://Ichigo/sounds/phrase_6.wav",
        "res://Ichigo/sounds/phrase_7.wav",
        "res://Ichigo/sounds/phrase_8.wav",
        "res://Ichigo/sounds/phrase_9.wav",
        "res://Ichigo/sounds/phrase_10.wav",
        "res://Ichigo/sounds/phrase_11.wav",
        "res://Ichigo/sounds/phrase_12.wav",
        "res://Ichigo/sounds/phrase_13.wav",
        "res://Ichigo/sounds/phrase_14.wav",
    };

    private static readonly string[] getsugaSfx =
    {
        "res://Ichigo/sounds/attack.wav",
        "res://Ichigo/sounds/attack_1.wav",
        "res://Ichigo/sounds/attack_2.wav",
        "res://Ichigo/sounds/attack_3.wav",
        "res://Ichigo/sounds/attack_4.wav",
        "res://Ichigo/sounds/attack_5.wav",
        "res://Ichigo/sounds/attack_6.wav",
        "res://Ichigo/sounds/attack_7.wav",
        "res://Ichigo/sounds/getsuga_voice_1.mp3",
        "res://Ichigo/sounds/getsuga_voice_2.wav",
        "res://Ichigo/sounds/getsuga_voice_3.wav",
        "res://Ichigo/sounds/getsuga_voice_4.wav",
    };
    
    private static readonly string[] bankaiSfx =
    {
        "res://Ichigo/sounds/bankai.mp3",
        "res://Ichigo/sounds/bankai_1.mp3",
    };
    
    private static readonly string[] bankaiBeforeSfx =
    {
        "res://Ichigo/sounds/bankai_after_1.wav",
        "res://Ichigo/sounds/bankai_after_2.wav",
        "res://Ichigo/sounds/bankai_after_3.wav",
        "res://Ichigo/sounds/bankai_after_4.wav",
        "res://Ichigo/sounds/bankai_after_5.wav",
        "res://Ichigo/sounds/bankai_after_6.wav",
        "res://Ichigo/sounds/bankai_after_7.wav",
        "res://Ichigo/sounds/bankai_after_8.wav",
    };
    
    public static void PlayRandomAttack()
    {
        PlayRandom(attackSfx);
    }
    
    public static void PlayRandomBankai()
    {
        PlayRandom(bankaiSfx);
    }

    public static void PlayRandomPhrase()
    {
        PlayRandom(phraseSfx);
    }
    
    public static void PlayRandomDamaged()
    {
        PlayRandom(damagedSfx);
    }

    public static void PlayRandomDamagedHigh()
    {
        PlayRandom(highDamagedSfx);
    }

    public static void PlayRandomGameover()
    {
        PlayRandom(gameoverSfx);
    }

    public static void PlayRandomDamagedCritical()
    {
        PlayRandom(criticalDamagedSfx);
    }
    
    public static void PlayRandomVictory()
    {
        PlayRandom(victorySfx);
    }

    public static void PlayRandomGetsuga()
    {
        PlayRandom(getsugaSfx);
    }

    public static void PlayRandomBankaiBefore()
    {
        PlayRandom(bankaiBeforeSfx);
    }

    public static void PlayRandom(string[] pool)
    {
        int index = rng.Next(pool.Length);
        SfxCmd.Play(pool[index], 0.7f);
    }
}