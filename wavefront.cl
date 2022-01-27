
#define EYE (float3)(0, 10, 0)
#define VEC (float3)(0,	0, 1)
#define DIST 2.0f


__kernel void device_function(write_only image2d_t a)
{
	
	int idx = get_global_id(0);
	int idy = get_global_id(1);

	int2 dim = get_image_dim(a);

	//printf("%d %d\n", dim);

	int id = idx + dim.x * idy;

	if (id >= (dim.x * dim.y)) return;


	// generate ray
	float3 E = EYE;
	float3 center = E + (DIST * VEC);

	float3 P0 = center + (float3)(-1, 1, 0);
	float3 P1 = center + (float3)(1, 1, 0);
	float3 P2 = center + (float3)(-1, -1, 0);

	float u = (float)idx / dim.x;
	float v = (float)idy / dim.y;


	//printf("%f %f\n", u, v);


	float3 O = P0 + u * (P1 - P0) + v * (P2 - P0);
	float3 D = O - E;

	D = normalize(D);


	//printf("X:%f Y:%f Z:%f\n",D.x,D.y,D.z);

	//printf("O- X: % f Y : % f Z : % f\n",O.x,O.y,O.z);

	// sphere
	float t = 0;
	float r2 = 25; // radius of 5
	float3 pos = (float3)(0, 10, 20);
	float4 col = (float4)(1, 0, 0, 1);

	int2 pix = (int2)(idx, idy);

	// intersect circle

	float3 C = pos - O;

	//printf("X:%f Y:%f Z:%f\n", C.x, C.y, C.z);

	t = dot(C, D);



	float3 Q = C - t * D;
	float p2 = dot(Q, Q);



	//printf("%f %f\n", r2, p2);

	if (p2 > r2) {
		write_imagef(a, pix, 0.f);
		return; 
	}
	


	t -= sqrt(r2 - p2);

	//printf("test\n");

	// display pixel

	if (t <= 0) {
		write_imagef(a, pix, 0.f);
		return;
	}
		
	write_imagef(a, pix, col);
}





__kernel void generate_rays(float3 E, float d, float3 V)
{

	int idx = get_global_id(0);
	int idy = get_global_id(1);



	float3 center = E + d * V;


	float3 P0 = center + (float3)(-1, 1, 0);
	float3 P1 = center + (float3)(1, 1, 0);
	float3 P2 = center + (float3)(-1, -1, 0);


	float u = (float)idx;
	float v = (float)idy;
	float3 Puv = P0 + u * (P1 - P0) + v * (P2 - P0);
	float3 D = Puv - E;



}



__kernel void i_accept_floats(float* f) 
{
	int id = get_global_id(0);

	if (id == 0) {
	
		f = float[100];
	
	}

}