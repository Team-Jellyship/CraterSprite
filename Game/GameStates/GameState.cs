using System.Collections.Generic;
using CraterSprite.Match3;
using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.Game.GameMode;

public class GameState
{
	public virtual void EnterState(GameMode mode) {}

	public virtual void ExitState() {}

	public virtual void NotifyGemDestroyed(GameMode mode, int destroyerPlayerIndex, Vector2 offset) {}
	
	public virtual void SetWinner(GameMode mode, int playerIndex) {}

	public float transitionTime = 0.0f;
	public string stateName;
	public bool canSetWinner = false;
}
