using Godot;
using System.Collections.Generic;
using CraterSprite;
using CraterSprite.Match3;
using CraterSprite.UI.HUD;

public partial class Match3Display : AutobindUiElement
{
    [Export]
    private Godot.Collections.Array<ColorRect> displays
    {
        set => _colorDisplays = CraterFunctions.ConvertArray(value);
        get => CraterFunctions.ConvertToGodotArray(_colorDisplays);
    }
    private List<ColorRect> _colorDisplays = [];


    protected override void Bind(PlayerState playerState)
    {
        BindContainer(playerState.container);
    }

    private void BindContainer(Match3Container container)
    {
        container.onOrbsChanged.AddListener(ContainerChanged);
        ContainerChanged(container.orbs);
    }

    private void ContainerChanged(List<MatchType> container)
    {
        GD.Print($"[HUD] Displaying orbs: {string.Join(", ", container)}");
        for (var i = 0; i < displays.Count; ++i)
        {
            displays[i].Color = i < container.Count ? Match3RecipeTable.GetColor(container[i]) : new Color(0.0f, 0.0f, 0.0f, 0.2f);
        }
    }
}
