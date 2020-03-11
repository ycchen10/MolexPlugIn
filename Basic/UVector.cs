using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NXOpen;

namespace Basic
{
    public class UVector
    {
        public  double m_vecX;
        public  double m_vecY;
        public  double m_vecZ;

        public UVector(double vecX, double vecY, double vecZ)
        {
            m_vecX = vecX;
            m_vecY = vecY;
            m_vecZ = vecZ;
        }

        public UVector(UVector vec)
        {
            m_vecX = vec.m_vecX;
            m_vecY = vec.m_vecY;
            m_vecZ = vec.m_vecZ;
        }
        public UVector(Vector3d vec)
        {
            m_vecX = vec.X;
            m_vecY = vec.Y;
            m_vecZ = vec.Z;
        }
        public UVector()
        {
        }

        public double X
        {
            get { return m_vecX; }
            set { m_vecX = value; }
        }

        public double Y
        {
            get { return m_vecY; }
            set { m_vecY = value; }
        }

        public double Z
        {
            get { return m_vecZ; }
            set { m_vecZ = value; }
        }

        public void Set(double x, double y, double z)
        {
            m_vecX = x;
            m_vecY = y;
            m_vecZ = z;
        }

        public bool IsZero()
        {
            return Math.Abs(Dot(this)) < 0.000001;
        }

        public double Dot(UVector vec)
        {
            return vec.m_vecX * m_vecX + vec.m_vecY * m_vecY + vec.m_vecZ * m_vecZ;
        }

        public static double Angle(UVector vec1, UVector vec2)
        {
            return Math.Acos(vec1 * vec2 / (Math.Sqrt(vec1 * vec1) * Math.Sqrt(vec2 * vec2)));
        }

        public static double Space(UVector vec1, UVector vec2)
        {
            return Math.Sqrt((vec1 - vec2) * (vec1 - vec2));
        }

        static public UVector operator -(UVector vec)
        {
            return (new UVector(-vec.m_vecX, -vec.m_vecY, -vec.m_vecZ));
        }

        public static UVector operator + (UVector vec1, UVector vec2)
        {
            return new UVector(vec1.m_vecX + vec2.m_vecX, vec1.m_vecY + vec2.m_vecY, vec1.m_vecZ + vec2.m_vecZ);
        }

        public static UVector operator -(UVector vec1, UVector vec2)
        {
            return new UVector(vec1.m_vecX - vec2.m_vecX, vec1.m_vecY - vec2.m_vecY, vec1.m_vecZ - vec2.m_vecZ);
        }
        /// <summary>
        /// 叉乘（得到右手定则的第三轴）
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static UVector operator ^(UVector vec1, UVector vec2)
        {
            return new UVector(vec1.m_vecY * vec2.m_vecZ - vec1.m_vecZ * vec2.m_vecY, vec1.m_vecZ * vec2.m_vecX - vec1.m_vecX * vec2.m_vecZ, vec1.m_vecX * vec2.m_vecY - vec1.m_vecY * vec2.m_vecX);
        }

        public static double operator *(UVector vec1, UVector vec2)
        {
            return vec1.m_vecX * vec2.m_vecX + vec1.m_vecY * vec2.m_vecY + vec1.m_vecZ * vec2.m_vecZ;
        }

        public static UVector operator *(double a, UVector vec)
        {
            return new UVector(a * vec.m_vecX, a * vec.m_vecY, a * vec.m_vecZ);
        }

        public static UVector operator /(UVector vec, double a)
        {
            return new UVector(vec.m_vecX / a, vec.m_vecY / a, vec.m_vecZ / a);
        }
        
        public UVector Add(UVector vec)
        {
            return new UVector(m_vecX + vec.m_vecX, m_vecY + vec.m_vecY, m_vecZ + vec.m_vecZ);         
        }

        public UVector PostSubstract(UVector vec)
        {
            return new UVector(m_vecX - vec.m_vecX, m_vecY - vec.m_vecY, m_vecZ - vec.m_vecZ);
        }

        public UVector PreSubstract(UVector vec)
        {
            return new UVector(vec.m_vecX - m_vecX, vec.m_vecY - m_vecY, vec.m_vecZ - m_vecZ);
        }

        public UVector PreProduct(UVector vec)
        {
            return new UVector(vec.m_vecY * m_vecZ - vec.m_vecZ * m_vecY, vec.m_vecZ * m_vecX - vec.m_vecX * m_vecZ, vec.m_vecX * m_vecY - vec.m_vecY * m_vecX);
        }

        public UVector PostProduct(UVector vec)
        {
            return new UVector(m_vecY * vec.m_vecZ - m_vecZ * vec.m_vecY, m_vecZ * vec.m_vecX - m_vecX * vec.m_vecZ, m_vecX * vec.m_vecY - m_vecY * vec.m_vecX);
        }

        public void Norm()
        {
            double dis = Math.Sqrt(Dot(this));

            m_vecX /= dis;
            m_vecY /= dis;
            m_vecZ /= dis;
        }

        public static double Dis(UVector v1, UVector v2)
        {
            return Math.Sqrt((v2.m_vecX - v1.m_vecX) * (v2.m_vecX - v1.m_vecX) + (v2.m_vecY - v1.m_vecY) * (v2.m_vecY - v1.m_vecY) + (v2.m_vecZ - v1.m_vecZ) * (v2.m_vecZ - v1.m_vecZ));
        }

    }
}
