using Godot;

public partial class Enemy : RigidBody2D
{
	[Signal]
	public delegate void DestroyedEventHandler();
	
	[Export, ExportCategory("Enemy Properties")]
	public Vector2 SpeedRange = new(150, 250);

	[Export]
	public Vector2 AngleRadiansRange = new(0.35f, 0.35f);
	
	public bool Alive = true;
	
	private float _spawnTimer = .5f;

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
		if (_spawnTimer > 0)
			_spawnTimer -= (float)delta;
	}

	private void OnLeaveScreen()
	{
		if (_spawnTimer <= 0 && Alive)
		{
			EmitSignal(SignalName.Destroyed);
			QueueFree();
		}
	}

	public void SetTarget(Node2D target)
	{
		LinearVelocity = (target.Position - Position).Normalized() * (float)GD.RandRange(SpeedRange.X, SpeedRange.Y);
		LinearVelocity = LinearVelocity.Rotated((float)GD.RandRange(AngleRadiansRange.X, AngleRadiansRange.Y));
		GetNode<AnimatedSprite2D>("Sprite").Rotation = Mathf.Atan2(LinearVelocity.Y, LinearVelocity.X);
	}

	public void Disappear()
	{
		Alive = false;
		var tween = CreateTween();
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		tween.TweenProperty(sprite, new NodePath(Node2D.PropertyName.Scale), Vector2.Zero, .6);
		tween.TweenProperty(sprite, new NodePath(Node2D.PropertyName.Rotation), 7, .6);
		tween.TweenCallback(Callable.From(QueueFree)).SetDelay(1);
	}
}
