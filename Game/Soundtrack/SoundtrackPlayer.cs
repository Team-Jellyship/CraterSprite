using Godot;

namespace CraterSprite.Game;

public partial class SoundtrackPlayer : AudioStreamPlayer
{
    public static SoundtrackPlayer instance { get; private set; }

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        VolumeLinear = 0.25f;
    }

    public void StartRandomSong()
    {
        SetStream(CraterMath.ChooseRandom(GameMode.GameMode.instance.settings.songsList));
        Play();
    }

    public void StartSong(AudioStream song)
    {
        SetStream(song);
        Play();
    }
}