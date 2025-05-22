extends RigidBody2D

var speed = 0
var target = position
var target_rotation = 0
@export var rotation_steps = 10
var rotation_dir = 1
var rotation_counter = rotation_steps
var rotation_stepsize = 0.0
var rotating = false

func is_numeric_key(value):
	if value == 96:
		return true
	else:
		return ((value >= KEY_0) && (value <= KEY_9))
		
		
func keycode_to_speed(code):
	if code == 96:
		return 0
	
	return(50 * (code - 48))
	

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
		
		var rotation_diff = 0.0
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
	var collision_info = move_and_collide(linear_velocity * delta)
	if collision_info:
		linear_velocity = linear_velocity.bounce(collision_info.get_normal())
