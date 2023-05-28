using Godot;
using System;

public partial class Player : Area2D
{
	[Export]
	public int Speed = 400;

	private Vector2          _screenSize;
	private AnimatedSprite2D _sprite;

	[Signal]
	public delegate void HitEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		_sprite = GetNode<AnimatedSprite2D>("Sprite");
		
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		// Get input
		if (Input.IsActionPressed("move_right"))
			velocity.X += 1;
		if (Input.IsActionPressed("move_left"))
			velocity.X -= 1;
		if (Input.IsActionPressed("move_up")) // Going up is negative... 🤢
			velocity.Y -= 1;
		if (Input.IsActionPressed("move_down"))
			velocity.Y += 1;

		// Update sprite and normalise velocity
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			_sprite.Animation = velocity.X == 0 ? "up" : "walk"; // Set anim depending on direction
			_sprite.Play();
		}
		else
		{
			_sprite.Stop();
		}
		
		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, _screenSize.X),
			y: Mathf.Clamp(Position.Y, 0, _screenSize.Y)
		);
	}
	
	private void OnBodyEntered(PhysicsBody2D body)
	{
		Hide(); // Player disappears after being hit.
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("Collision").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
	
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("Collision").Disabled = false;
	}
}
