shader_type canvas_item;

void vertex() {
	UV = (MODEL_MATRIX * vec4(VERTEX, 0.0f, 1.0f)).xy / 32.0f;
}

void fragment() {
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
