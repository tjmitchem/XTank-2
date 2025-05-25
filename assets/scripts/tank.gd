extends RigidBody2D

@export var rotation_steps: int = 10
@export var speed_multiplier: int      = 50

var speed: int      = 0
var target: Vector2 = position
var target_rotation: float = 0.0
var rotation_dir: int        = 1
var rotation_counter: int    = rotation_steps
var rotation_stepsize: float = 0.0
var rotating: bool           = false

func is_numeric_key(value) -> bool:
	if value == 96:
		return true
	else:
		return ((value >= KEY_0) && (value <= KEY_9))
		
		
func keycode_to_speed(code) -> int:
	if code == 96:
		return 0
	
	return(speed_multiplier * (code - 48))
	

func _integrate_forces(state):
	# Handle rotating, if required
	if rotating:
		rotation_degrees += rotation_stepsize
		rotation_counter -= 1

		if rotation_counter == 0:
			rotating = false
			rotation_counter = rotation_steps

	# Set our speed	
	linear_velocity = transform.x * 1 * speed	


func _input(event):
	if event is InputEventKey && event.pressed:
		if event.keycode == KEY_ESCAPE:
			print("Quitting")
			get_tree().quit()
		elif is_numeric_key(event.keycode):
			speed = keycode_to_speed(event.keycode)

	if event.is_action_pressed("click"):
		target = get_global_mouse_position()
		target_rotation = rad_to_deg(position.angle_to_point(target))
		
		var rotation_diff: float = 0.0
		if (rotation_degrees > target_rotation):
			rotation_diff = rotation_degrees - target_rotation
			rotation_dir = -1
		else:
			rotation_diff = target_rotation - rotation_degrees
			rotation_dir = 1
			
		if rotation_diff >= 180:
			rotation_diff = 360 - rotation_diff
			rotation_dir = -rotation_dir
			
		rotation_stepsize = (rotation_diff / rotation_steps) * rotation_dir
		
		if rotation_stepsize != 0:
			rotation_counter = rotation_steps
			rotating = true
		

func _physics_process(delta):
	var collision_info: KinematicCollision2D = move_and_collide(linear_velocity * delta)
	
	if collision_info:
		linear_velocity = linear_velocity.bounce(collision_info.get_normal())
