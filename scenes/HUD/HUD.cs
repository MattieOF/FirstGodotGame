using Godot;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	public Player Player;
	public Game Game;
	
	private Control _baseContainer;
	private Label _mainText;
	private Button _playButton;
	
	public override void _Ready()
	{
		_baseContainer = GetNode<Control>("MenuCenterContainer");
		_playButton = GetNode<Button>("MenuCenterContainer/MenuHFlow/PlayButton");
		_mainText = GetNode<Label>("TopPanelBG/MainText");
		Player.Hit += PlayerDied;
		Game.ScoreUpdated += delegate(int score) { _mainText.Text = $"Score: {score}"; };
	}

	public void GameStarted()
	{
		_baseContainer.Visible = false;
	}
	
	private void PlayerDied()
	{
		_baseContainer.Visible = true;
		_mainText.Text = $"RIP. Final score: {Game.Score}";
		_playButton.Text = "Retry";
	}

	private void OnStartButtonPressed() => EmitSignal(SignalName.StartGame);

	private void Quit() => GetTree().Quit();
}
