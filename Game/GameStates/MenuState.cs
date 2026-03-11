using Godot;

namespace CraterSprite.Game.GameMode;

public class MenuState : GameState
{
    public PackedScene menuScene;

    private Control _menuInstance;
        
    public override void EnterState(GameMode mode)
    {
        _menuInstance = menuScene.Instantiate<Control>();
        mode.menuRoot.AddChild(_menuInstance);
    }

    public override void ExitState()
    {
        _menuInstance.Free();
        _menuInstance = null;
    }
}