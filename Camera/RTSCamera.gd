extends Camera2D
class_name RTSCamera2D

export var pan_speed = 10.0
export var speed = 20.0
export var zoom_amount = 0.25

export var zoom_min : Vector2 = Vector2(0.1, 0.1)
export var zoom_max : Vector2 = Vector2(3, 3)

var mouse_pos : Vector2
var zoom_factor = 1.0
var correct_position : Vector2

var pressed : bool

var movement : Vector2

signal zoom_changed
signal position_changed


func _ready():
	current = true
	limit_bottom = 180
	limit_left = 0
	limit_right = 320
	limit_top = 0
	
	var result = get_viewport().connect("size_changed", self, "_on_Window_size_changed")
	
	if not result == OK:
		print("RTS Camera was not able to connect to viewport.")
	
	_on_Window_size_changed()


func _on_Window_size_changed():
	
	var max_zoom = Vector2((limit_right-limit_left) / get_viewport_rect().size.x, (limit_bottom-limit_top) / get_viewport_rect().size.y)
	zoom_max = Vector2(max_zoom.x, max_zoom.x) if max_zoom.x < max_zoom.y else Vector2(max_zoom.y, max_zoom.y)
	zoom.x = clamp(zoom.x, zoom_min.x, zoom_max.x)
	zoom.y = clamp(zoom.y, zoom_min.y, zoom_max.y)


func _process(delta):
	#smooth movement
	var previous_pos = position
	var inpx = (int(Input.is_action_pressed("ui_right"))
					   - int(Input.is_action_pressed("ui_left")))
	var inpy = (int(Input.is_action_pressed("ui_down"))
					   - int(Input.is_action_pressed("ui_up")))
	position.x = lerp(position.x, position.x + inpx * speed * zoom.x,speed * delta)
	position.y = lerp(position.y, position.y + inpy * speed * zoom.y,speed * delta)

	if Input.is_key_pressed(KEY_CONTROL):
#	if not pressed:
		var margin_left =   (get_viewport_rect().size.x * (1 - drag_margin_left))/2
		var margin_top =    (get_viewport_rect().size.y * (1 - drag_margin_top   ))/2
		var margin_right =  (get_viewport_rect().size.x + get_viewport_rect().size.x*drag_margin_right )/2
		var margin_bottom = (get_viewport_rect().size.y + get_viewport_rect().size.y*drag_margin_bottom)/2
		
		if mouse_pos.x < margin_left: 
			position.x = position.x - abs(mouse_pos.x - margin_left)/margin_left * pan_speed * zoom.x
		elif mouse_pos.x > margin_right:
			position.x = position.x + abs(mouse_pos.x - margin_right)/margin_left *  pan_speed * zoom.x
		if mouse_pos.y < margin_top:
			position.y = position.y - abs(mouse_pos.y - margin_top)/margin_top * pan_speed * zoom.y
		elif mouse_pos.y > margin_bottom:
			position.y = position.y + abs(mouse_pos.y - margin_bottom)/margin_top * pan_speed * zoom.y
	
	position = Vector2(
		clamp(position.x, limit_left + (get_viewport_rect().size/2).x * zoom.x, limit_right - (get_viewport_rect().size/2).x * zoom.x),
		clamp(position.y, limit_top + (get_viewport_rect().size/2).y * zoom.y, limit_bottom - (get_viewport_rect().size/2).y * zoom.y)
	)
	
	correct_position = global_position
	if not position == previous_pos:
		emit_signal("position_changed")


func _input(event):
	var previous_position = position
	
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_WHEEL_UP or event.button_index == BUTTON_WHEEL_DOWN:
			if event.is_pressed():
			
				var previous_mouse_position = get_global_mouse_position()
				var previous_zoom = zoom
				
				if event.button_index == BUTTON_WHEEL_UP:
					zoom_factor = -zoom_amount
				if event.button_index == BUTTON_WHEEL_DOWN:
					zoom_factor = zoom_amount
				
				zoom.x = max(zoom.x + zoom_factor, zoom_min.x)
				zoom.y = max(zoom.y + zoom_factor, zoom_min.y)
				
				zoom.x = stepify(zoom.x, zoom_amount/2)
				zoom.y = stepify(zoom.y, zoom_amount/2)
				
				zoom.x = clamp(zoom.x, zoom_min.x, zoom_max.x)
				zoom.y = clamp(zoom.y, zoom_min.y, zoom_max.y)
				
				if not zoom == previous_zoom:
					emit_signal("zoom_changed")
				
				position -= get_global_mouse_position() - previous_mouse_position
				if not previous_position == position:
					emit_signal("position_changed")
				
			else:
				zoom_factor = 1
		if event.button_index == BUTTON_MIDDLE:
			if event.pressed:
				pressed = true
			else:
				pressed = false
	
	if event is InputEventMouseMotion:
		if pressed:
			position -= event.relative / (Vector2.ONE/zoom)
			emit_signal("position_changed")
	
	if event is InputEventMouse:
		mouse_pos = event.position

