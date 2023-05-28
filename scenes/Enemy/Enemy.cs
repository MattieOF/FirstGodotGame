using Godot;
using System;

public partial class Enemy : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		string[] animations = sprite.SpriteFrames.GetAnimationNames();
		sprite.Play(animations[GD.Randi() % animations.Length]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnLeaveScreen()
	{
		QueueFree();
	}
}
