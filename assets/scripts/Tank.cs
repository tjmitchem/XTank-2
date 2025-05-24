using Godot;
public partial class Tank : RigidBody2D
{
	[Export]
	private int rotation_steps = 10;
	
	[Export]
	private int speed_multiplier = 50;

	private int speed = 0;
	private Vector2 target;
	private float target_rotation = 0.0f;
	private int rotation_dir = 1;
	private int rotation_counter;
	private float rotation_stepsize = 0.0f;
	private bool rotating = false;
	
	public override void _Ready()
	{
		target = Position;
		rotation_counter = rotation_steps;
	}
	
	private bool is_numeric_key(int value)
	{
		if (value == 96) { // Check if key is numpad 0 or "`"
			return true;
		} else { // Check if key is between 0 and 9
			return ((value >= (int)Godot.Key.Key0) && (value <= (int)Godot.Key.Key9));
		}
	}
	
	private int keycode_to_speed(int keycode)
	{
		if (keycode == 96) { // Numpad 0 or "`"
			return 0;
		} else {
			return (speed_multiplier * (keycode - 48));
		}
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		base._IntegrateForces(state);
	
		// Handle rotation, if required
		if (rotating)
		{
			RotationDegrees += rotation_stepsize;
			rotation_counter -= 1;
			
			if (rotation_counter == 0)
			{
				rotating = false;
			}
		}

		// Set our speed
		LinearVelocity = Transform.X * 1 * speed;
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
				speed = keycode_to_speed((int)keyEvent.Keycode);
			}
		}

		if (@event.IsActionPressed("click"))
		{
			target = GetGlobalMousePosition();
			target_rotation = float.RadiansToDegrees(Position.AngleToPoint(target));
			
			float rotation_diff = 0.0f;
			if (RotationDegrees > target_rotation)
			{
				rotation_diff = RotationDegrees - target_rotation;
				rotation_dir = -1;
			}
			else
			{
				rotation_diff = target_rotation - RotationDegrees;
				rotation_dir = 1;
			}
			
			if (rotation_diff >= 180)
			{
				rotation_diff = 360 - rotation_diff;
				rotation_dir = -rotation_dir;
			}

			rotation_stepsize = (rotation_diff / rotation_steps) * rotation_dir;

			if (rotation_stepsize != 0)
			{
				rotation_counter = rotation_steps;
				rotating = true;
			}
		}

		return;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		KinematicCollision2D collision_info = MoveAndCollide((LinearVelocity * (float)delta));
		if (collision_info != null)
		{
			LinearVelocity = LinearVelocity.Bounce(collision_info.GetNormal());
		}
		
		return;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return;
	}
}
