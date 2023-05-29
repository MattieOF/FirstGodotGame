using FirstGodotGame.Util;
using Godot;

public partial class Game : Node2D
{
	/// <summary>
	/// Time that the escape key needs to be held before the game exits 
	/// </summary>
	[Export]
	public float QuitTime = 1f;
	
	[Export]
	public PackedScene EnemyScene { get; set; }
	
	[Export]
	public PackedScene PlayerScene { get; set; }

	private Player _player;
	private double _quitTime = 0;
	
	public override void _Ready()
	{
		_player = PlayerScene.Instantiate<Player>();
		var viewportRect = GetViewportRect();
		AddChild(_player);
		_player.Start(new Vector2(Mathf.Lerp(viewportRect.Position.X, viewportRect.End.X, 0.5f), Mathf.Lerp(viewportRect.Position.Y, viewportRect.End.Y, 0.5f)));
	}

	public override void _Process(double delta)
	{
		// Check for quit
		if (Input.IsActionPressed("quit"))
		{
			_quitTime += delta;
			if (_quitTime >= QuitTime)
				GetTree().Quit();
		}
		else
			_quitTime = 0;
	}

	public void OnSpawnEnemy()
	{
		Enemy mob = EnemyScene.Instantiate<Enemy>();

		var mobSpawnLocation = GetViewportRect().Grow(60).GetPointAlongPerimeter(GD.Randf());
		
		// Set the mob's direction perpendicular to the path direction.
		var directionVector = (mobSpawnLocation - _player.Position).Normalized();
		float direction = Mathf.Atan2(directionVector.X, directionVector.Y);

		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation;

		// Add some randomness to the direction.
		direction += (float)GD.RandRange(-Mathf.Pi / 6, Mathf.Pi / 6);
		mob.Rotation = direction;

		// Choose the velocity.
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}
}
