using Godot;
public partial class Tank : RigidBody2D
{
	[Export]
	private int _rotationSteps = 10;
	
	[Export]
	private int _speedMultiplier = 50;

	private int _speed;
	private Vector2 _target;
	private float _targetRotation;
	private int _rotationDir = 1;
	private int _rotationCounter;
	private float _rotationStepSize;
	private bool _rotating;
	
	public override void _Ready()
	{
		_speed = 0;
		_target = Position;
		_targetRotation = 0.0f;
		_rotationCounter = _rotationSteps;
		_rotationStepSize = 0.0f;
		_rotating = false;
	}
	
	private bool is_numeric_key(int val)
	{
		if (val == 96) { // Check if key is numpad 0 or "`"
			return true;
		} else { // Check if key is between 0 and 9
			return ((val >= (int)Godot.Key.Key0) && (val <= (int)Godot.Key.Key9));
		}
	}
	
	private int keycode_to_speed(int keycode)
	{
		if (keycode == 96) { // Numpad 0 or "`"
			return 0;
		} else {
			return (_speedMultiplier * (keycode - 48));
		}
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		base._IntegrateForces(state);
	
		// Handle rotation, if required
		if (_rotating)
		{
			RotationDegrees += _rotationStepSize;
			_rotationCounter -= 1;
			
			if (_rotationCounter == 0)
			{
				_rotating = false;
			}
		}

		// Set our speed
		LinearVelocity = Transform.X * 1 * _speed;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
		{
			if (keyEvent.Keycode == Godot.Key.Escape)
			{
				GD.Print("Quitting");
				GetTree().Quit();
			}
			
			if (is_numeric_key((int)keyEvent.Keycode))
			{
				_speed = keycode_to_speed((int)keyEvent.Keycode);
			}
		}

		if (@event.IsActionPressed("click"))
		{
			_target = GetGlobalMousePosition();
			_targetRotation = float.RadiansToDegrees(Position.AngleToPoint(_target));
			
			float rotationDiff;
			if (RotationDegrees > _targetRotation)
			{
				rotationDiff = RotationDegrees - _targetRotation;
				_rotationDir = -1;
			}
			else
			{
				rotationDiff = _targetRotation - RotationDegrees;
				_rotationDir = 1;
			}
			
			if (rotationDiff >= 180)
			{
				rotationDiff = 360 - rotationDiff;
				_rotationDir = -_rotationDir;
			}

			_rotationStepSize = (rotationDiff / _rotationSteps) * _rotationDir;

			if (_rotationStepSize != 0)
			{
				_rotationCounter = _rotationSteps;
				_rotating = true;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		KinematicCollision2D collisionInfo = MoveAndCollide((LinearVelocity * (float)delta));
		if (collisionInfo != null)
		{
			LinearVelocity = LinearVelocity.Bounce(collisionInfo.GetNormal());
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
