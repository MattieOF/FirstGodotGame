using Godot;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int Speed = 400;

	private AnimatedSprite2D _sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("Sprite");

		GetNode<CollisionShape2D>("Collider").Disabled = true;
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

		var screenSize = GetViewportRect().Size;
		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 50, screenSize.X - 50),
			y: Mathf.Clamp(Position.Y, 50, screenSize.Y - 50)
		);
	}
	
	private void OnBodyEntered(PhysicsBody2D body)
	{
		Hide(); // Player disappears after being hit.
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("Collider").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
	
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("Collider").Disabled = false;
	}
}
