using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Cloo;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace Template {

	class Game
	{


		// when GLInterop is set to true, the fractal is rendered directly to an OpenGL texture
		bool GLInterop = true;

		// load the OpenCL program; this creates the OpenCL context
		static OpenCLProgram ocl = new OpenCLProgram( "../../wavefront.cl" );

		// find the kernel named 'device_function' in the program
		OpenCLKernel kernel = new OpenCLKernel( ocl, "device_function" );


		OpenCLKernel generate_primary_rays = new OpenCLKernel(ocl, "generate_primary_rays");

		OpenCLKernel cast_rays = new OpenCLKernel(ocl, "cast_rays");

		//static int width = 100;


		// create a regular buffer; by default this resides on both the host and the device
		//OpenCLBuffer<int> buffer = new OpenCLBuffer<int>( ocl, width * width);







		public Surface screen;

		static int width = 1280, height = 720;

		// create an OpenGL texture to which OpenCL can send data
		OpenCLImage<int> image = new OpenCLImage<int>(ocl, width, height);


		


		Stopwatch timer = new Stopwatch();
		float t = 21.5f;





		OpenCLBuffer<float> primary_rays = new OpenCLBuffer<float>(ocl, width * height * 6);


		OpenCLBuffer<int> dims = new OpenCLBuffer<int>(ocl, 3);

		static float3 E = new float3(0, 10f, 0), V = new float3(0, 0, 1f);
		static float d = 2;





		public void Init()
		{
			// nothing here


			dims[0] = width;
			dims[1] = height;
			dims[2] = width * height;


			//generate rays



			primary_rays.CopyToDevice();
			dims.CopyToDevice();


			generate_primary_rays.SetArgument(0, primary_rays);
			generate_primary_rays.SetArgument(1, dims);

			generate_primary_rays.SetArgument(2, E);
			generate_primary_rays.SetArgument(3, d);
			generate_primary_rays.SetArgument(4, V);



			long[] workSize = { width, height };
			long[] localSize = { 16, 16 };



			generate_primary_rays.Execute(workSize, localSize);


			cast_rays.SetArgument(0, primary_rays);
			cast_rays.SetArgument(1, dims);
			cast_rays.SetArgument(2, image);

			//Console.WriteLine(screen.width);
			//Console.WriteLine(screen.height);

		}
		public void Tick()
		{
			GL.Finish();
			// clear the screen
			screen.Clear(0);



			kernel.SetArgument(0, image);


			long[] workSize = { width, height };
			long[] localSize = { 16, 16 };




			if (false)
			{
				kernel.SetArgument(0, image);
				// lock the OpenGL texture for use by OpenCL
				kernel.LockOpenGLObject(image.texBuffer);
				// execute the kernel
				kernel.Execute(workSize, localSize);
				// unlock the OpenGL texture so it can be used for drawing a quad
				kernel.UnlockOpenGLObject(image.texBuffer);
			}
			else {

				// lock the OpenGL texture for use by OpenCL
				cast_rays.LockOpenGLObject(image.texBuffer);
				// execute the kernel
				cast_rays.Execute(workSize, localSize);
				// unlock the OpenGL texture so it can be used for drawing a quad
				cast_rays.UnlockOpenGLObject(image.texBuffer);



			}








		}
		
		public void Render() 
		{
				
			// use OpenGL to draw a quad using the texture that was filled by OpenCL
			if (GLInterop)
			{
				GL.LoadIdentity();
				GL.BindTexture( TextureTarget.Texture2D, image.OpenGLTextureID );
				GL.Begin( PrimitiveType.Quads );
				GL.TexCoord2( 0.0f, 1.0f ); GL.Vertex2( -1.0f, -1.0f );
				GL.TexCoord2( 1.0f, 1.0f ); GL.Vertex2(  1.0f, -1.0f );
				GL.TexCoord2( 1.0f, 0.0f ); GL.Vertex2(  1.0f,  1.0f );
				GL.TexCoord2( 0.0f, 0.0f ); GL.Vertex2( -1.0f,  1.0f );
				GL.End();
			}
			}
		}

	} // namespace Template