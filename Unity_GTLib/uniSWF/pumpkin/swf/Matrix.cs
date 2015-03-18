namespace pumpkin.swf
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// 
        /// </summary>
        private static Vector3 transformVector3Static_res = new Vector3();
        /// <summary>
        /// 
        /// </summary>
        public float a;
        /// <summary>
        /// 
        /// </summary>
        public float b;
        /// <summary>
        /// 
        /// </summary>
        public float c;
        /// <summary>
        /// 
        /// </summary>
        public float d;
        /// <summary>
        /// 
        /// </summary>
        public float tx;
        /// <summary>
        /// 
        /// </summary>
        public float ty;

        /// <summary>
        /// 
        /// </summary>
        public Matrix()
        {
            this.a = 1f;
            this.b = 0f;
            this.c = 0f;
            this.d = 1f;
            this.tx = 0f;
            this.ty = 0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        public Matrix(float a, float b, float c, float d, float tx, float ty)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.tx = tx;
            this.ty = ty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Matrix setMatrix(Matrix matrix)
        {
            this.a = matrix.a;
            this.b = matrix.b;
            this.c = matrix.c;
            this.d = matrix.d;
            this.tx = matrix.tx;
            this.ty = matrix.ty;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix clone()
        {
            return new Matrix(this.a, this.b, this.c, this.d, this.tx, this.ty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public void concat(Matrix m)
        {
            float num = (this.a * m.a) + (this.b * m.c);
            this.b = (this.a * m.b) + (this.b * m.d);
            this.a = num;
            float num2 = (this.c * m.a) + (this.d * m.c);
            this.d = (this.c * m.b) + (this.d * m.d);
            this.c = num2;
            float num3 = ((this.tx * m.a) + (this.ty * m.c)) + m.tx;
            this.ty = ((this.tx * m.b) + (this.ty * m.d)) + m.ty;
            this.tx = num3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float getRotation()
        {
            Matrix matrix = this;
            float num = Mathf.Atan((-matrix.c / matrix.a));
            float num2 = this.getScaleX();
            float num3 = Mathf.Atan((-this.c / this.a));
            float num4 = Mathf.Acos((this.a / num2));
            float num5 = num4 * Mathf.Rad2Deg;
            if ((num5 > 90) && (num3 > 0))
            {
                num = (360 - num5) * Mathf.PI;
            }
            else if ((num5 < 90) && (num3 < 0))
            {
                num = (360 - num5) * Mathf.PI;
            }
            else
            {
                num = num4;
            }
            return (num * Mathf.Rad2Deg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float getScaleX()
        {
            if (this.a < 0f)
            {
                return -( Mathf.Sqrt ((this.a * this.a) + (this.c * this.c)));
            }
            return  Mathf.Sqrt((this.a * this.a) + (this.c * this.c));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float getScaleY()
        {
            if (this.d < 0f)
            {
                return -( Mathf.Sqrt(((this.b * this.b) + (this.d * this.d))));
            }
            return  Mathf.Sqrt(((this.b * this.b) + (this.d * this.d)));
        }

        /// <summary>
        /// 
        /// </summary>
        public void identity()
        {
            this.a = 1f;
            this.b = 0f;
            this.c = 0f;
            this.d = 1f;
            this.tx = 0f;
            this.ty = 0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix invert()
        {
            float num = (this.a * this.d) - (this.b * this.c);
            if (num == 0f)
            {
                this.a = this.b = this.c = this.d = 0f;
                this.tx = -this.tx;
                this.ty = -this.ty;
            }
            else
            {
                num = 1f / num;
                float num3 = this.d * num;
                this.d = this.a * num;
                this.a = num3;
                this.b *= -num;
                this.c *= -num;
                float num4 = (-this.a * this.tx) - (this.c * this.ty);
                this.ty = (-this.b * this.tx) - (this.d * this.ty);
                this.tx = num4;
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Matrix mult(Matrix m)
        {
            return new Matrix 
            { 
                a = (this.a * m.a) + (this.b * m.c), 
                b = (this.a * m.b) + (this.b * m.d), 
                c = (this.c * m.a) + (this.d * m.c), 
                d = (this.c * m.b) + (this.d * m.d), 
                tx = ((this.tx * m.a) + (this.ty * m.c)) + m.tx, 
                ty = ((this.tx * m.b) + (this.ty * m.d)) + m.ty 
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rad"></param>
        public void rotate(float rad)
        {
            float num = Mathf.Cos(rad);
            float num2 = Mathf.Sin(rad);
            float num3 = (this.a * num) - (this.b * num2);
            this.b = (this.a * num2) + (this.b * num);
            this.a = num3;
            float num4 = (this.c * num) - (this.d * num2);
            this.d = (this.c * num2) + (this.d * num);
            this.c = num4;
            float num5 = (this.tx * num) - (this.ty * num2);
            this.ty = (this.tx * num2) + (this.ty * num);
            this.tx = num5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inSX"></param>
        /// <param name="inSY"></param>
        public void scale(float inSX, float inSY)
        {
            this.a *= inSX;
            this.b *= inSY;
            this.c *= inSX;
            this.d *= inSY;
            this.tx *= inSX;
            this.ty *= inSY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inTa"></param>
        public void setRotation(float inTa)
        {
            this.setRotation(inTa, 1f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inTa"></param>
        /// <param name="inScale"></param>
        public void setRotation(float inTa, float inScale)
        {
            float num = inScale;
            this.a = Mathf.Cos(inTa) * num;
            this.c = Mathf.Sin(inTa) * num;
            this.b = -this.c;
            this.d = this.a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            object[] objArray1 = new object[] { "matrix(", this.a, ", ", this.b, ", ", this.c, ", ", this.d, ", ", this.tx, ", ", this.ty, ")" };
            return string.Concat(objArray1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inPos"></param>
        /// <returns></returns>
        public Vector2 transformPoint(Vector2 inPos)
        {
            return new Vector2(((inPos.x * this.a) + (inPos.y * this.c)) + this.tx, 
                ((inPos.x * this.b) + (inPos.y * this.d)) + this.ty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectIn"></param>
        /// <returns></returns>
        public Rect transformRect(Rect rectIn)
        {
            Matrix matrix = this;
            float num = (matrix.a * rectIn.x) + (matrix.c * rectIn.y);
            float num2 = num;
            float num3 = (matrix.b * rectIn.x) + (matrix.d * rectIn.y);
            float num4 = num;
            float num5 = (matrix.a * (rectIn.x + rectIn.width)) + (matrix.c * rectIn.y);
            float num6 = (matrix.b * (rectIn.x + rectIn.width)) + (matrix.d * rectIn.y);
            if (num5 < num)
            {
                num = num5;
            }
            if (num6 < num3)
            {
                num3 = num6;
            }
            if (num5 > num2)
            {
                num2 = num5;
            }
            if (num6 > num4)
            {
                num4 = num6;
            }
            num5 = (matrix.a * (rectIn.x + rectIn.width)) + (matrix.c * (rectIn.y + rectIn.height));
            num6 = (matrix.b * (rectIn.x + rectIn.width)) + (matrix.d * (rectIn.y + rectIn.height));
            if (num5 < num)
            {
                num = num5;
            }
            if (num6 < num3)
            {
                num3 = num6;
            }
            if (num5 > num2)
            {
                num2 = num5;
            }
            if (num6 > num4)
            {
                num4 = num6;
            }
            num5 = (matrix.a * rectIn.x) + (matrix.c * (rectIn.y + rectIn.height));
            num6 = (matrix.b * rectIn.x) + (matrix.d * (rectIn.y + rectIn.height));
            if (num5 < num)
            {
                num = num5;
            }
            if (num6 < num3)
            {
                num3 = num6;
            }
            if (num5 > num2)
            {
                num2 = num5;
            }
            if (num6 > num4)
            {
                num4 = num6;
            }
            return new Rect(num + matrix.tx, num3 + matrix.ty, num2 - num, num4 - num3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inPos"></param>
        /// <returns></returns>
        public Vector3 transformVector3(Vector3 inPos)
        {
            return new Vector3(((inPos.x * this.a) + (inPos.y * this.c)) + this.tx, ((inPos.x * this.b) + (inPos.y * this.d)) + this.ty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inX"></param>
        /// <param name="inY"></param>
        /// <param name="inZ"></param>
        /// <returns></returns>
        public Vector3 transformVector3(float inX, float inY, float inZ)
        {
            return new Vector3(((inX * this.a) + (inY * this.c)) + this.tx, ((inX * this.b) + (inY * this.d)) + this.ty, inZ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inX"></param>
        /// <param name="inY"></param>
        /// <param name="inZ"></param>
        /// <returns></returns>
        internal Vector3 transformVector3Static(float inX, float inY, float inZ)
        {
            transformVector3Static_res.x = ((inX * this.a) + (inY * this.c)) + this.tx;
            transformVector3Static_res.y = ((inX * this.b) + (inY * this.d)) + this.ty;
            transformVector3Static_res.z = inZ;
            return transformVector3Static_res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inX"></param>
        /// <param name="inY"></param>
        public void translate(float inX, float inY)
        {
            this.tx += inX;
            this.ty += inY;
        }
    }
}

