using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Template;

namespace template
{
    struct Ray 
    {

        public Ray(float3 O, float3 D) 
        {
            this.D = D;
            this.O = O;
            t = 0f;
        }

        public float3 O { get; set; }
        public float3 D;
        public float t;





    }
}
