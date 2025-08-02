using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class SoundCollections
    {
        private readonly Dictionary<string, SoundEffect> _soundEffects;
        private readonly Music _backgroundMusic;
        private static SoundCollections? _instance;

        private SoundCollections()
        {
            _backgroundMusic = SplashKit.LoadMusic("BackgroundMusic", "main_menu.mp3");

            _soundEffects = new Dictionary<string, SoundEffect>()
            {
                {"LeverToggle", SplashKit.LoadSoundEffect("LeverToggle", "lever.mp3")},
                {"FireBoyJump", SplashKit.LoadSoundEffect("FireBoyJump", "fireboy_jump.wav")},
                {"WaterGirlJump", SplashKit.LoadSoundEffect("WaterGirlJump", "watergirl_jump.wav")},
                {"Death", SplashKit.LoadSoundEffect("Death", "dead.mp3")},
                {"ExitReached", SplashKit.LoadSoundEffect("ExitReached", "door.wav")},
                {"DiamondCollect", SplashKit.LoadSoundEffect("DiamondCollect", "diamond_collected.mp3")},
                {"MainMenu", SplashKit.LoadSoundEffect("MainMenu", "main_menu.mp3")},
                {"Navigation", SplashKit.LoadSoundEffect("Navigation" , "navigation.wav")}
            };
        }

        public static SoundCollections Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundCollections();
                }
                return _instance;
            }
        }

        public void PlaySound(string soundName)
        {
            if (_soundEffects.ContainsKey(soundName))
            {
                SplashKit.PlaySoundEffect(_soundEffects[soundName]);
            }
        }

        public void StopSound(string soundName)
        {
            if (_soundEffects.ContainsKey(soundName))
            {
                SplashKit.StopSoundEffect(_soundEffects[soundName]);
            }
        }

        public void PlayBackgroundMusic()
        {
            if (!SplashKit.MusicPlaying())
            {
                SplashKit.PlayMusic(_backgroundMusic);
            }
        }

        public void StopBackgroundMusic()
        {
            if (SplashKit.MusicPlaying())
            {
                SplashKit.StopMusic();
            }
        }
    }
}