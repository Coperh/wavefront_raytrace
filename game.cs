﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Cloo;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using template;

namespace Template {

	class Game
	{


		// when GLInterop is set to true, the fractal is rendered directly to an OpenGL texture
		bool GLInterop = true;

		// load the OpenCL program; this creates the OpenCL context
		static OpenCLProgram ocl = new OpenCLProgram( "../../wavefront.cl" );

		// find the kernel named 'device_function' in the program
		OpenCLKernel kernel = new OpenCLKernel( ocl, "device_function" );


		OpenCLKernel generate_rays = new OpenCLKernel(ocl, "generate_rays");

		//static int width = 100;


		// create a regular buffer; by default this resides on both the host and the device
		//OpenCLBuffer<int> buffer = new OpenCLBuffer<int>( ocl, width * width);


		




		public Surface screen;

		static int width = 1280, height = 720;

		// create an OpenGL texture to which OpenCL can send data
		OpenCLImage<int> image = new OpenCLImage<int>(ocl, width, height);


		OpenCLBuffer<Ray> rays = new OpenCLBuffer<Ray>(ocl, width * height);

		OpenCLBuffer<float> floats;


		Stopwatch timer = new Stopwatch();
		float t = 21.5f;




		static float3 E = new float3(0, 10f, 0), V = new float3(0, 0, 1f);
		static float d = 2;





		public void Init()
		{
			// nothing here

			OpenCLKernel i_accept_floats = new OpenCLKernel(ocl, "i_accept_floats");

			i_accept_floats.SetArgument(0, floats);

			long[] workSize = { 100, 1 };

			i_accept_floats.Execute(workSize);

		}
		public void Tick()
		{
			GL.Finish();
			// clear the screen
			screen.Clear( 0 );



			kernel.SetArgument(0, image);


			long[] workSize = { width, height };
			long[] localSize = { 32, 16 };


			
			// lock the OpenGL texture for use by OpenCL
			kernel.LockOpenGLObject(image.texBuffer);
			// execute the kernel
			kernel.Execute(workSize, localSize);
			// unlock the OpenGL texture so it can be used for drawing a quad
			kernel.UnlockOpenGLObject(image.texBuffer);

			


			//generate rays
			/*
			generate_rays.SetArgument(0, E);
			generate_rays.SetArgument(1, d);
			generate_rays.SetArgument(2, V);

			rays.CopyToDevice();


			generate_rays.Execute(workSize, localSize);
			*/

			//Console.WriteLine(screen.width);
			//Console.WriteLine(screen.height);





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