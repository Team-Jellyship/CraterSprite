using Godot;
using System;
using System.Linq;

namespace CraterSprite;

public partial class MenuControl : CharacterBody2D
{
	[Signal] public delegate void UpEventHandler();
	[Signal] public delegate void DownEventHandler();
	
	[Signal] public delegate void SelectEventHandler();
	[Signal] public delegate void BackEventHandler();
	[Signal] public delegate void CreditsEventHandler();
	
	[Signal] public delegate void P1ReadyEventHandler();
	[Signal] public delegate void P2ReadyEventHandler();
	
	/**
	* <summary>Move the menu cursor up</summary>
	*/
	public void up()
	{
		EmitSignal(SignalName.Up);
	}
	
	/**
	* <summary>Move the menu cursor down</summary>
	*/
	public void down()
	{
		EmitSignal(SignalName.Down);
	}
	
	/**
	* <summary>Used to make a selection on the pause menu</summary>
	*/
	public void select()
	{
		EmitSignal(SignalName.Select);
	}
	
	/**
	* <summary>Press this button to opt out of the main menu or return back to the game from the pause menu.</summary>
	*/
	public void back()
	{
		EmitSignal(SignalName.Back);
	}
	
	/**
	* <summary>Press this button on the main menu to bring up the credits screen. Press again to close it.</summary>
	*/
	public void credits()
	{
		EmitSignal(SignalName.Credits);
	}
	
	
	/**
	* <summary>Allows Player 1 to ready up at the main menu by pressing start.</summary>
	*/
	public void StartP1()
	{
		EmitSignal(SignalName.P1Ready);
	}
	
	/**
	* <summary>Allows Player 2 to ready up at the main menu by pressing start.</summary>
	*/
	public void StartP2()
	{
		EmitSignal(SignalName.P2Ready);
	}
}
