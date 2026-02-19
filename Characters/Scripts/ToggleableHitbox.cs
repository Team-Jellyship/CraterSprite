using Godot;

namespace CraterSprite;

public partial class ToggleableHitbox : Area2D
{
    [Export] private bool _on = true;
    [Export] private CollisionShape2D _onHitbox;
    [Export] private CollisionShape2D _offHitbox;

    private bool _enabled = true;

    public void Enable()
    {
        _enabled = true;
        Callable.From(() =>
        {
            if (_on)
            {
                _onHitbox.Disabled = false;
            }
            else
            {
                _offHitbox.Disabled = false;
            }
        }).CallDeferred();
    }

    public void Disable()
    {
        _enabled = false;
        Callable.From(() =>
        {
            _onHitbox.Disabled = true;
            _offHitbox.Disabled = true;
        }).CallDeferred();
    }

    private void SetOff()
    {
        _on = false;
        if (!_enabled)
        {
            return;
        }
        _offHitbox.Disabled = false;
        _onHitbox.Disabled = true;
    }

    private void SetOn()
    {
        _on = true;
        if (!_enabled)
        {
            return;
        }
        _onHitbox.Disabled = false;
        _offHitbox.Disabled = true;
    }
}