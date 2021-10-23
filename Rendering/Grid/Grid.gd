extends Node2D


export(Color) var grid_color = Color.blue setget set_grid_color
export(int) var grid_width = 1 setget set_grid_width


var size : Vector2 setget set_size
var chunk_size : Vector2 setget set_chunk_size


func set_grid_color(color: Color):
	grid_color = color
	update()


func set_grid_width(width: int):
	grid_width = width
	update()


func set_size(s: Vector2):
	size = s
	update()


func set_chunk_size(s: Vector2):
	chunk_size = s
	update()


func _draw() -> void:
	var chunks_amount = (size/chunk_size).round();

	for x in range(chunks_amount.x):
		draw_line(Vector2(x * chunk_size.x, 0), Vector2(x * chunk_size.x, size.y), grid_color, grid_width)

	for y in range(chunks_amount.y):
		draw_line(Vector2(0, y * chunk_size.y), Vector2(size.x, y * chunk_size.y), grid_color, grid_width)
	
	draw_line(Vector2(size.x, 0), size, grid_color)
	draw_line(Vector2(0, size.y), size, grid_color)
