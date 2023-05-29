using System;
using FirstGodotGame.Util;
using Godot;

public partial class Game : Node2D
{
	[Signal]
	public delegate void ScoreUpdatedEventHandler(int newScore);

	/// <summary>
	/// Time that the escape key needs to be held before the game exits 
	/// </summary>
	[Export]
	public float QuitTime = 1f;

	[Export] 
	public PackedScene EnemyScene, PlayerScene, HUDScene;

	[Export, ExportCategory("Sounds")]
	public AudioStream GameOverSound;
	[Export]
	public AudioStream Music;

	public int Score = 0;

	private Player _player;
	private double _quitTime = 0;
	private AudioStreamPlayer _gameOverSound, _musicPlayer;
	private bool _gameStarted = false;
	private Timer _enemySpawnTimer;
	private Tween _deathMusicEffect;
	private HUD _hud;
	
	public override void _Ready()
	{
		_player = PlayerScene.Instantiate<Player>();
		AddChild(_player);
		_player.Hit += OnPlayerHit;

		_hud = HUDScene.Instantiate<HUD>();
		_hud.Game = this;
		_hud.Player = _player;
		_hud.StartGame += StartGame;
		AddChild(_hud);
		
		_enemySpawnTimer = GetNode<Timer>("EnemySpawnTimer");
		_enemySpawnTimer.Stop();

		// Create sound players
		_gameOverSound = new AudioStreamPlayer();
		_gameOverSound.Stream = GameOverSound;
		AddChild(_gameOverSound);

		_musicPlayer = new AudioStreamPlayer();
		_musicPlayer.Stream = Music;
		AddChild(_musicPlayer);
		_musicPlayer.Play();
	}

	private void OnPlayerHit()
	{
		_enemySpawnTimer.Stop();
		_deathMusicEffect = CreateTween();
		_deathMusicEffect.TweenProperty(_musicPlayer, new NodePath(AudioStreamPlayer.PropertyName.PitchScale), 0.4f, 1);
		_gameOverSound.Play();

		_gameStarted = false;
	}

	private void StartGame()
	{
		// Make all current mobs disappear
		GetTree().CallGroup("enemies", Enemy.MethodName.Disappear);
		
		Score = 0;
		EmitSignal(SignalName.ScoreUpdated, 0);
		
		var viewportRect = GetViewportRect();
		_player.Start(new Vector2(Mathf.Lerp(viewportRect.Position.X, viewportRect.End.X, 0.5f), Mathf.Lerp(viewportRect.Position.Y, viewportRect.End.Y, 0.5f)));
		if (Math.Abs(_musicPlayer.PitchScale - 1) > 0.01)
		{
			if (_deathMusicEffect is not null)
			{
				_deathMusicEffect.Stop();
				_deathMusicEffect = null;
			}
			CreateTween().TweenProperty(_musicPlayer, new NodePath(AudioStreamPlayer.PropertyName.PitchScale), 1, .4);
		}

		_enemySpawnTimer.WaitTime = 2.5;
		_enemySpawnTimer.Start();
		
		_hud.GameStarted();
		_gameStarted = true;
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
		
		if (!_gameStarted && Input.IsActionJustPressed("start"))
			StartGame();
	}

	private void OnSpawnEnemy()
	{
		if (!_player.Visible)
			return;
		
		// Spawn enemy node
		Enemy mob = EnemyScene.Instantiate<Enemy>();
		
		// Set the mob's position to a random location.
		var mobSpawnLocation = GetViewportRect().Grow(60).GetPointAlongPerimeter(GD.Randf());
		mob.Position = mobSpawnLocation;
		mob.SetTarget(_player);
		
		// Add score callback
		mob.Destroyed += delegate
		{
			if (!_gameStarted)
				return;
			Score++;
			EmitSignal(SignalName.ScoreUpdated, Score);
		};

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
		
		// Speed up time before next spawn
		_enemySpawnTimer.WaitTime = Mathf.Max(1, _enemySpawnTimer.WaitTime - 0.1);
	}
}
