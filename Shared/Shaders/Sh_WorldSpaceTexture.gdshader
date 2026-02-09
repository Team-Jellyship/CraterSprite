shader_type canvas_item;

vec2 GetScaleMatrix(mat4 matrix){
	return vec2(
		sqrt(matrix[0].x * matrix[0].x + matrix[1].x * matrix[1].x),
		sqrt(matrix[0].y * matrix[0].y + matrix[1].y * matrix[1].y));
}

void vertex() {
	// UV = (MODEL_MATRIX * vec4(VERTEX, 0.0f, 1.0f)).xy / 32.0f;
	UV = VERTEX * GetScaleMatrix(MODEL_MATRIX) / 32.0f - vec2(0.5f, 1.0f);
}

void fragment() {
}


//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
