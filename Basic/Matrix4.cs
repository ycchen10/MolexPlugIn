
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;

namespace Basic
{
    public class Matrix4
    {
        public double[,] matrix = new double[4, 4];
        public Matrix4()
        {
            matrix[0, 0] = 1.0; matrix[0, 1] = 0.0; matrix[0, 2] = 0.0; matrix[0, 3] = 0.0;
            matrix[1, 0] = 0.0; matrix[1, 1] = 1.0; matrix[1, 2] = 0.0; matrix[1, 3] = 0.0;
            matrix[2, 0] = 0.0; matrix[2, 1] = 0.0; matrix[2, 2] = 1.0; matrix[2, 3] = 0.0;
            matrix[3, 0] = 0.0; matrix[3, 1] = 0.0; matrix[3, 2] = 0.0; matrix[3, 3] = 1.0;
        }

        public Matrix4(double[,] mat)
        {
            matrix[0, 0] = mat[0, 0]; matrix[0, 1] = mat[0, 1]; matrix[0, 2] = mat[0, 2]; matrix[0, 3] = mat[0, 3];
            matrix[1, 0] = mat[1, 0]; matrix[1, 1] = mat[1, 1]; matrix[1, 2] = mat[1, 2]; matrix[1, 3] = mat[1, 3];
            matrix[2, 0] = mat[2, 0]; matrix[2, 1] = mat[2, 1]; matrix[2, 2] = mat[2, 2]; matrix[2, 3] = mat[2, 3];
            matrix[3, 0] = mat[3, 0]; matrix[3, 1] = mat[3, 1]; matrix[3, 2] = mat[3, 2]; matrix[3, 3] = mat[3, 3];
        }

        public Matrix4(double m11, double m12, double m13, double m14,
                     double m21, double m22, double m23, double m24,
                     double m31, double m32, double m33, double m34,
                     double m41, double m42, double m43, double m44)
        {
            matrix[0, 0] = m11; matrix[0, 1] = m12; matrix[0, 2] = m13; matrix[0, 3] = m14;
            matrix[1, 0] = m21; matrix[1, 1] = m22; matrix[1, 2] = m23; matrix[1, 3] = m24;
            matrix[2, 0] = m31; matrix[2, 1] = m32; matrix[2, 2] = m33; matrix[2, 3] = m34;
            matrix[3, 0] = m41; matrix[3, 1] = m42; matrix[3, 2] = m43; matrix[3, 3] = m44;
        }

        public Matrix4(Matrix4 mat)
        {
            matrix[0, 0] = mat.matrix[0, 0]; matrix[0, 1] = mat.matrix[0, 1]; matrix[0, 2] = mat.matrix[0, 2]; matrix[0, 3] = mat.matrix[0, 3];
            matrix[1, 0] = mat.matrix[1, 0]; matrix[1, 1] = mat.matrix[1, 1]; matrix[1, 2] = mat.matrix[1, 2]; matrix[1, 3] = mat.matrix[1, 3];
            matrix[2, 0] = mat.matrix[2, 0]; matrix[2, 1] = mat.matrix[2, 1]; matrix[2, 2] = mat.matrix[2, 2]; matrix[2, 3] = mat.matrix[2, 3];
            matrix[3, 0] = mat.matrix[3, 0]; matrix[3, 1] = mat.matrix[3, 1]; matrix[3, 2] = mat.matrix[3, 2]; matrix[3, 3] = mat.matrix[3, 3];
        }

        public void SetScale(double sx, double sy, double sz)
        {
            this.Identity();
            matrix[0, 0] *= sx;
            matrix[1, 1] *= sy;
            matrix[2, 2] *= sz;
        }

        public double GetScale()
        {
            return Math.Sqrt(matrix[0, 0] * matrix[0, 0] + matrix[0, 1] * matrix[0, 1] + matrix[0, 2] * matrix[0, 2]);
        }

        public void Identity()
        {
            matrix[0, 0] = 1.0; matrix[0, 1] = 0.0; matrix[0, 2] = 0.0; matrix[0, 3] = 0.0;
            matrix[1, 0] = 0.0; matrix[1, 1] = 1.0; matrix[1, 2] = 0.0; matrix[1, 3] = 0.0;
            matrix[2, 0] = 0.0; matrix[2, 1] = 0.0; matrix[2, 2] = 1.0; matrix[2, 3] = 0.0;
            matrix[3, 0] = 0.0; matrix[3, 1] = 0.0; matrix[3, 2] = 0.0; matrix[3, 3] = 1.0;
        }

        public void SetRow(int index, UVector vec)
        {
            matrix[index, 0] = vec.m_vecX; matrix[index, 1] = vec.m_vecY; matrix[index, 2] = vec.m_vecZ;
        }

        public void SetCol(int index, UVector vec)
        {
            matrix[0, index] = vec.m_vecX; matrix[1, index] = vec.m_vecY; matrix[2, index] = vec.m_vecZ;
        }

        public void Translate(double x, double y, double z)
        {
            matrix[3, 0] += x;
            matrix[3, 1] += y;
            matrix[3, 2] += z;
        }

        public void Translate(UVector pos)
        {
            matrix[3, 0] += pos.m_vecX;
            matrix[3, 1] += pos.m_vecY;
            matrix[3, 2] += pos.m_vecZ;
        }

        public void Translate(Point3d pos)
        {
            matrix[3, 0] += pos.X;
            matrix[3, 1] += pos.Y;
            matrix[3, 2] += pos.Z;
        }

        public void RolateWithX(double ang)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.matrix[1, 1] = Math.Cos(ang);
            mat.matrix[1, 2] = Math.Sin(ang);
            mat.matrix[2, 1] = -Math.Sin(ang);
            mat.matrix[2, 2] = Math.Cos(ang);
            PostMultiply(mat);
        }

        public void RolateWithY(double ang)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.matrix[0, 0] = Math.Cos(ang);
            mat.matrix[0, 2] = -Math.Sin(ang);
            mat.matrix[2, 0] = Math.Sin(ang);
            mat.matrix[2, 2] = Math.Cos(ang);
            PostMultiply(mat);
        }

        public void RolateWithZ(double ang)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.matrix[0, 0] = Math.Cos(ang);
            mat.matrix[0, 1] = Math.Sin(ang);
            mat.matrix[1, 0] = -Math.Sin(ang);
            mat.matrix[1, 1] = Math.Cos(ang);
            PostMultiply(mat);
        }

        public static Matrix4 operator *(Matrix4 mat1, Matrix4 mat2)
        {
            Matrix4 matr = new Matrix4();
            matr.matrix[0, 0] = mat1.matrix[0, 0] * mat2.matrix[0, 0] + mat1.matrix[1, 0] * mat2.matrix[0, 1] + mat1.matrix[2, 0] * mat2.matrix[0, 2] + mat1.matrix[3, 0] * mat2.matrix[0, 3];
            matr.matrix[1, 0] = mat1.matrix[0, 0] * mat2.matrix[1, 0] + mat1.matrix[1, 0] * mat2.matrix[1, 1] + mat1.matrix[2, 0] * mat2.matrix[1, 2] + mat1.matrix[3, 0] * mat2.matrix[1, 3];
            matr.matrix[2, 0] = mat1.matrix[0, 0] * mat2.matrix[2, 0] + mat1.matrix[1, 0] * mat2.matrix[2, 1] + mat1.matrix[2, 0] * mat2.matrix[2, 2] + mat1.matrix[3, 0] * mat2.matrix[2, 3];
            matr.matrix[3, 0] = mat1.matrix[0, 0] * mat2.matrix[3, 0] + mat1.matrix[1, 0] * mat2.matrix[3, 1] + mat1.matrix[2, 0] * mat2.matrix[3, 2] + mat1.matrix[3, 0] * mat2.matrix[3, 3];

            matr.matrix[0, 1] = mat1.matrix[0, 1] * mat2.matrix[0, 0] + mat1.matrix[1, 1] * mat2.matrix[0, 1] + mat1.matrix[2, 1] * mat2.matrix[0, 2] + mat1.matrix[3, 1] * mat2.matrix[0, 3];
            matr.matrix[1, 1] = mat1.matrix[0, 1] * mat2.matrix[1, 0] + mat1.matrix[1, 1] * mat2.matrix[1, 1] + mat1.matrix[2, 1] * mat2.matrix[1, 2] + mat1.matrix[3, 1] * mat2.matrix[1, 3];
            matr.matrix[2, 1] = mat1.matrix[0, 1] * mat2.matrix[2, 0] + mat1.matrix[1, 1] * mat2.matrix[2, 1] + mat1.matrix[2, 1] * mat2.matrix[2, 2] + mat1.matrix[3, 1] * mat2.matrix[2, 3];
            matr.matrix[3, 1] = mat1.matrix[0, 1] * mat2.matrix[3, 0] + mat1.matrix[1, 1] * mat2.matrix[3, 1] + mat1.matrix[2, 1] * mat2.matrix[3, 2] + mat1.matrix[3, 1] * mat2.matrix[3, 3];

            matr.matrix[0, 2] = mat1.matrix[0, 2] * mat2.matrix[0, 0] + mat1.matrix[1, 2] * mat2.matrix[0, 1] + mat1.matrix[2, 2] * mat2.matrix[0, 2] + mat1.matrix[3, 2] * mat2.matrix[0, 3];
            matr.matrix[1, 2] = mat1.matrix[0, 2] * mat2.matrix[1, 0] + mat1.matrix[1, 2] * mat2.matrix[1, 1] + mat1.matrix[2, 2] * mat2.matrix[1, 2] + mat1.matrix[3, 2] * mat2.matrix[1, 3];
            matr.matrix[2, 2] = mat1.matrix[0, 2] * mat2.matrix[2, 0] + mat1.matrix[1, 2] * mat2.matrix[2, 1] + mat1.matrix[2, 2] * mat2.matrix[2, 2] + mat1.matrix[3, 2] * mat2.matrix[2, 3];
            matr.matrix[3, 2] = mat1.matrix[0, 2] * mat2.matrix[3, 0] + mat1.matrix[1, 2] * mat2.matrix[3, 1] + mat1.matrix[2, 2] * mat2.matrix[3, 2] + mat1.matrix[3, 2] * mat2.matrix[3, 3];

            matr.matrix[0, 3] = mat1.matrix[0, 3] * mat2.matrix[0, 0] + mat1.matrix[1, 3] * mat2.matrix[0, 1] + mat1.matrix[2, 3] * mat2.matrix[0, 2] + mat1.matrix[3, 3] * mat2.matrix[0, 3];
            matr.matrix[1, 3] = mat1.matrix[0, 3] * mat2.matrix[1, 0] + mat1.matrix[1, 3] * mat2.matrix[1, 1] + mat1.matrix[2, 3] * mat2.matrix[1, 2] + mat1.matrix[3, 3] * mat2.matrix[1, 3];
            matr.matrix[2, 3] = mat1.matrix[0, 3] * mat2.matrix[2, 0] + mat1.matrix[1, 3] * mat2.matrix[2, 1] + mat1.matrix[2, 3] * mat2.matrix[2, 2] + mat1.matrix[3, 3] * mat2.matrix[2, 3];
            matr.matrix[3, 3] = mat1.matrix[0, 3] * mat2.matrix[3, 0] + mat1.matrix[1, 3] * mat2.matrix[3, 1] + mat1.matrix[2, 3] * mat2.matrix[3, 2] + mat1.matrix[3, 3] * mat2.matrix[3, 3];

            return matr;
        }

        public void PostMultiply(Matrix4 mat)
        {
            double c11 = mat.matrix[0, 0] * matrix[0, 0] + mat.matrix[1, 0] * matrix[0, 1] + mat.matrix[2, 0] * matrix[0, 2] + mat.matrix[3, 0] * matrix[0, 3];
            double c21 = mat.matrix[0, 0] * matrix[1, 0] + mat.matrix[1, 0] * matrix[1, 1] + mat.matrix[2, 0] * matrix[1, 2] + mat.matrix[3, 0] * matrix[1, 3];
            double c31 = mat.matrix[0, 0] * matrix[2, 0] + mat.matrix[1, 0] * matrix[2, 1] + mat.matrix[2, 0] * matrix[2, 2] + mat.matrix[3, 0] * matrix[2, 3];
            double c41 = mat.matrix[0, 0] * matrix[3, 0] + mat.matrix[1, 0] * matrix[3, 1] + mat.matrix[2, 0] * matrix[3, 2] + mat.matrix[3, 0] * matrix[3, 3];

            double c12 = mat.matrix[0, 1] * matrix[0, 0] + mat.matrix[1, 1] * matrix[0, 1] + mat.matrix[2, 1] * matrix[0, 2] + mat.matrix[3, 1] * matrix[0, 3];
            double c22 = mat.matrix[0, 1] * matrix[1, 0] + mat.matrix[1, 1] * matrix[1, 1] + mat.matrix[2, 1] * matrix[1, 2] + mat.matrix[3, 1] * matrix[1, 3];
            double c32 = mat.matrix[0, 1] * matrix[2, 0] + mat.matrix[1, 1] * matrix[2, 1] + mat.matrix[2, 1] * matrix[2, 2] + mat.matrix[3, 1] * matrix[2, 3];
            double c42 = mat.matrix[0, 1] * matrix[3, 0] + mat.matrix[1, 1] * matrix[3, 1] + mat.matrix[2, 1] * matrix[3, 2] + mat.matrix[3, 1] * matrix[3, 3];

            double c13 = mat.matrix[0, 2] * matrix[0, 0] + mat.matrix[1, 2] * matrix[0, 1] + mat.matrix[2, 2] * matrix[0, 2] + mat.matrix[3, 2] * matrix[0, 3];
            double c23 = mat.matrix[0, 2] * matrix[1, 0] + mat.matrix[1, 2] * matrix[1, 1] + mat.matrix[2, 2] * matrix[1, 2] + mat.matrix[3, 2] * matrix[1, 3];
            double c33 = mat.matrix[0, 2] * matrix[2, 0] + mat.matrix[1, 2] * matrix[2, 1] + mat.matrix[2, 2] * matrix[2, 2] + mat.matrix[3, 2] * matrix[2, 3];
            double c43 = mat.matrix[0, 2] * matrix[3, 0] + mat.matrix[1, 2] * matrix[3, 1] + mat.matrix[2, 2] * matrix[3, 2] + mat.matrix[3, 2] * matrix[3, 3];

            double c14 = mat.matrix[0, 3] * matrix[0, 0] + mat.matrix[1, 3] * matrix[0, 1] + mat.matrix[2, 3] * matrix[0, 2] + mat.matrix[3, 3] * matrix[0, 3];
            double c24 = mat.matrix[0, 3] * matrix[1, 0] + mat.matrix[1, 3] * matrix[1, 1] + mat.matrix[2, 3] * matrix[1, 2] + mat.matrix[3, 3] * matrix[1, 3];
            double c34 = mat.matrix[0, 3] * matrix[2, 0] + mat.matrix[1, 3] * matrix[2, 1] + mat.matrix[2, 3] * matrix[2, 2] + mat.matrix[3, 3] * matrix[2, 3];
            double c44 = mat.matrix[0, 3] * matrix[3, 0] + mat.matrix[1, 3] * matrix[3, 1] + mat.matrix[2, 3] * matrix[3, 2] + mat.matrix[3, 3] * matrix[3, 3];

            matrix[0, 0] = c11; matrix[0, 1] = c12; matrix[0, 2] = c13; matrix[0, 3] = c14;
            matrix[1, 0] = c21; matrix[1, 1] = c22; matrix[1, 2] = c23; matrix[1, 3] = c24;
            matrix[2, 0] = c31; matrix[2, 1] = c32; matrix[2, 2] = c33; matrix[2, 3] = c34;
            matrix[3, 0] = c41; matrix[3, 1] = c42; matrix[3, 2] = c43; matrix[3, 3] = c44;
        }

        public void PreMultiply(Matrix4 mat)
        {
            double c11 = matrix[0, 0] * mat.matrix[0, 0] + matrix[1, 0] * mat.matrix[0, 1] + matrix[2, 0] * mat.matrix[0, 2] + matrix[3, 0] * mat.matrix[0, 3];
            double c21 = matrix[0, 0] * mat.matrix[1, 0] + matrix[1, 0] * mat.matrix[1, 1] + matrix[2, 0] * mat.matrix[1, 2] + matrix[3, 0] * mat.matrix[1, 3];
            double c31 = matrix[0, 0] * mat.matrix[2, 0] + matrix[1, 0] * mat.matrix[2, 1] + matrix[2, 0] * mat.matrix[2, 2] + matrix[3, 0] * mat.matrix[2, 3];
            double c41 = matrix[0, 0] * mat.matrix[3, 0] + matrix[1, 0] * mat.matrix[3, 1] + matrix[2, 0] * mat.matrix[3, 2] + matrix[3, 0] * mat.matrix[3, 3];

            double c12 = matrix[0, 1] * mat.matrix[0, 0] + matrix[1, 1] * mat.matrix[0, 1] + matrix[2, 1] * mat.matrix[0, 2] + matrix[3, 1] * mat.matrix[0, 3];
            double c22 = matrix[0, 1] * mat.matrix[1, 0] + matrix[1, 1] * mat.matrix[1, 1] + matrix[2, 1] * mat.matrix[1, 2] + matrix[3, 1] * mat.matrix[1, 3];
            double c32 = matrix[0, 1] * mat.matrix[2, 0] + matrix[1, 1] * mat.matrix[2, 1] + matrix[2, 1] * mat.matrix[2, 2] + matrix[3, 1] * mat.matrix[2, 3];
            double c42 = matrix[0, 1] * mat.matrix[3, 0] + matrix[1, 1] * mat.matrix[3, 1] + matrix[2, 1] * mat.matrix[3, 2] + matrix[3, 1] * mat.matrix[3, 3];

            double c13 = matrix[0, 2] * mat.matrix[0, 0] + matrix[1, 2] * mat.matrix[0, 1] + matrix[2, 2] * mat.matrix[0, 2] + matrix[3, 2] * mat.matrix[0, 3];
            double c23 = matrix[0, 2] * mat.matrix[1, 0] + matrix[1, 2] * mat.matrix[1, 1] + matrix[2, 2] * mat.matrix[1, 2] + matrix[3, 2] * mat.matrix[1, 3];
            double c33 = matrix[0, 2] * mat.matrix[2, 0] + matrix[1, 2] * mat.matrix[2, 1] + matrix[2, 2] * mat.matrix[2, 2] + matrix[3, 2] * mat.matrix[2, 3];
            double c43 = matrix[0, 2] * mat.matrix[3, 0] + matrix[1, 2] * mat.matrix[3, 1] + matrix[2, 2] * mat.matrix[3, 2] + matrix[3, 2] * mat.matrix[3, 3];

            double c14 = matrix[0, 3] * mat.matrix[0, 0] + matrix[1, 3] * mat.matrix[0, 1] + matrix[2, 3] * mat.matrix[0, 2] + matrix[3, 3] * mat.matrix[0, 3];
            double c24 = matrix[0, 3] * mat.matrix[1, 0] + matrix[1, 3] * mat.matrix[1, 1] + matrix[2, 3] * mat.matrix[1, 2] + matrix[3, 3] * mat.matrix[1, 3];
            double c34 = matrix[0, 3] * mat.matrix[2, 0] + matrix[1, 3] * mat.matrix[2, 1] + matrix[2, 3] * mat.matrix[2, 2] + matrix[3, 3] * mat.matrix[2, 3];
            double c44 = matrix[0, 3] * mat.matrix[3, 0] + matrix[1, 3] * mat.matrix[3, 1] + matrix[2, 3] * mat.matrix[3, 2] + matrix[3, 3] * mat.matrix[3, 3];

            matrix[0, 0] = c11; matrix[0, 1] = c12; matrix[0, 2] = c13; matrix[0, 3] = c14;
            matrix[1, 0] = c21; matrix[1, 1] = c22; matrix[1, 2] = c23; matrix[1, 3] = c24;
            matrix[2, 0] = c31; matrix[2, 1] = c32; matrix[2, 2] = c33; matrix[2, 3] = c34;
            matrix[3, 0] = c41; matrix[3, 1] = c42; matrix[3, 2] = c43; matrix[3, 3] = c44;
        }
        /// <summary>
        /// 逆矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetInversMatrix()
        {
            double[] mat = {matrix[0,0],matrix[0,1],matrix[0,2],matrix[0,3],
                          matrix[1,0],matrix[1,1],matrix[1,2],matrix[1,3],
                          matrix[2,0],matrix[2,1],matrix[2,2],matrix[2,3],
                          matrix[3,0],matrix[3,1],matrix[3,2],matrix[3,3]};
            double[] inversMat = new double[16];


            NXOpen.UF.UFSession ufSession = NXOpen.UF.UFSession.GetUFSession();
            ufSession.Mtx4.Invert(mat, inversMat);
            return new Matrix4(inversMat[0], inversMat[1], inversMat[2], inversMat[3],
                           inversMat[4], inversMat[5], inversMat[6], inversMat[7],
                           inversMat[8], inversMat[9], inversMat[10], inversMat[11],
                           inversMat[12], inversMat[13], inversMat[14], inversMat[15]);
        }


        public Matrix4 GetOrthInvers()
        {
            return new Matrix4(matrix[0, 0], matrix[1, 0], matrix[2, 0], 0.0,
                           matrix[0, 1], matrix[1, 1], matrix[2, 1], 0.0,
                           matrix[0, 2], matrix[1, 2], matrix[2, 2], 0.0,
                           -(matrix[3, 0] * matrix[0, 0] + matrix[3, 1] * matrix[0, 1] + matrix[3, 2] * matrix[0, 2]),
                           -(matrix[3, 0] * matrix[1, 0] + matrix[3, 1] * matrix[1, 1] + matrix[3, 2] * matrix[1, 2]),
                           -(matrix[3, 0] * matrix[2, 0] + matrix[3, 1] * matrix[2, 1] + matrix[3, 2] * matrix[2, 2]),
                           1);
        }

        public UVector GetPendiAxis(UVector vec)
        {
            double dx = Math.Abs(vec.m_vecX);
            double dy = Math.Abs(vec.m_vecY);
            double dz = Math.Abs(vec.m_vecZ);
            if (dz < dx)
            {
                if (dz < dy)
                    return new UVector(-vec.m_vecY, vec.m_vecX, 0.0);
                else
                    return new UVector(vec.m_vecZ, 0.0, -vec.m_vecX);
            }
            else
            {
                if (dx < dy)
                    return new UVector(0.0, -vec.m_vecZ, vec.m_vecY);
                else
                    return new UVector(vec.m_vecZ, 0.0, -vec.m_vecX);
            }
        }

        public void TransformToZAxis(UVector pt, UVector axis)
        {
            UVector vx = GetPendiAxis(axis);
            UVector vy = axis ^ vx;

            Matrix4 mat2 = new Matrix4();
            mat2.SetRow(0, vx);
            mat2.SetRow(1, vy);
            mat2.SetRow(2, axis);
            mat2.SetRow(3, pt);
            /*mat2.SetCol(0, vx);
            mat2.SetCol(1, vy);
            mat2.SetCol(2, axis);
            mat2.SetRow(3, pt);*/
            PostMultiply(mat2.GetOrthInvers());
        }

        public void TransformToZAxis(Point3d pt1, Vector3d axis1)
        {
            UVector pt = new UVector();
            pt.X = pt1.X;
            pt.Y = pt1.Y;
            pt.Z = pt1.Z;

            UVector axis = new UVector();
            axis.X = axis1.X;
            axis.Y = axis1.Y;
            axis.Z = axis1.Z;

            UVector vx = GetPendiAxis(axis);
            UVector vy = axis ^ vx;

            Matrix4 mat2 = new Matrix4();
            mat2.SetRow(0, vx);
            mat2.SetRow(1, vy);
            mat2.SetRow(2, axis);
            mat2.SetRow(3, pt);
            /*mat2.SetCol(0, vx);
            mat2.SetCol(1, vy);
            mat2.SetCol(2, axis);
            mat2.SetRow(3, pt);*/
            PostMultiply(mat2.GetOrthInvers());
        }

        public void TransformToZAxis(UVector pt, UVector vx, UVector vy)
        {
            Matrix4 mat2 = new Matrix4();
            mat2.SetRow(0, vx);
            mat2.SetRow(1, vy);
            mat2.SetRow(2, vx ^ vy);
            mat2.SetRow(3, pt);
            /*mat2.SetCol(0, vx);
            mat2.SetCol(1, vy);
            mat2.SetCol(2, axis);
            mat2.SetRow(3, pt);*/
            PostMultiply(mat2.GetOrthInvers());
        }

        public void TransformToZAxis(Point3d pt1, Vector3d vx1, Vector3d vy1)
        {
            UVector pt = new UVector();
            pt.X = pt1.X;
            pt.Y = pt1.Y;
            pt.Z = pt1.Z;

            UVector vx = new UVector();
            vx.X = vx1.X;
            vx.Y = vx1.Y;
            vx.Z = vx1.Z;

            UVector vy = new UVector();
            vy.X = vy1.X;
            vy.Y = vy1.Y;
            vy.Z = vy1.Z;

            Matrix4 mat2 = new Matrix4();
            mat2.SetRow(0, vx);
            mat2.SetRow(1, vy);
            mat2.SetRow(2, vx ^ vy);
            mat2.SetRow(3, pt);
            /*mat2.SetCol(0, vx);
            mat2.SetCol(1, vy);
            mat2.SetCol(2, axis);
            mat2.SetRow(3, pt);*/
            PostMultiply(mat2.GetOrthInvers());
        }

        public void TransformToCsys(CoordinateSystem csys, ref Matrix4 mat)
        {
            mat.Identity();
            Vector3d xVec = new Vector3d();
            Vector3d yVec = new Vector3d();
            csys.GetDirections(out xVec, out yVec);
            mat.TransformToZAxis(csys.Origin, xVec, yVec);
        }
        public static Matrix4 CalRelativeMatrix(UVector vx1, UVector vy1, UVector vz1, UVector origin1,
                                        UVector vx2, UVector vy2, UVector vz2, UVector origin2)
        {
            Matrix4 mat1 = new Matrix4();
            mat1.SetRow(0, vx1);
            mat1.SetRow(1, vy1);
            mat1.SetRow(2, vz1);
            mat1.SetRow(3, origin1);

            Matrix4 mat2 = new Matrix4();
            mat2.SetRow(0, vx2);
            mat2.SetRow(1, vy2);
            mat2.SetRow(2, vz2);
            mat2.SetRow(3, origin2);
            return mat1 * mat2.GetOrthInvers();
        }


        public void ApplyVec(ref UVector vec)
        {
            double x = matrix[0, 0] * vec.m_vecX + matrix[1, 0] * vec.m_vecY + matrix[2, 0] * vec.m_vecZ;
            double y = matrix[0, 1] * vec.m_vecX + matrix[1, 1] * vec.m_vecY + matrix[2, 1] * vec.m_vecZ;
            double z = matrix[0, 2] * vec.m_vecX + matrix[1, 2] * vec.m_vecY + matrix[2, 2] * vec.m_vecZ;

            vec.m_vecX = x;
            vec.m_vecY = y;
            vec.m_vecZ = z;
        }

        public void ApplyVec(ref Vector3d vec)
        {
            double x = matrix[0, 0] * vec.X + matrix[1, 0] * vec.Y + matrix[2, 0] * vec.Z;
            double y = matrix[0, 1] * vec.X + matrix[1, 1] * vec.Y + matrix[2, 1] * vec.Z;
            double z = matrix[0, 2] * vec.X + matrix[1, 2] * vec.Y + matrix[2, 2] * vec.Z;

            vec.X = x;
            vec.Y = y;
            vec.Z = z;
        }

        public void ApplyVec(ref double x, ref double y, ref double z)
        {
            double xNew = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z;
            double yNew = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z;
            double zNew = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z;

            x = xNew;
            y = yNew;
            z = zNew;
        }

        public void ApplyAbsVec(ref UVector vec)
        {
            double x = Math.Abs(matrix[0, 0] * vec.m_vecX) + Math.Abs(matrix[1, 0] * vec.m_vecY) + Math.Abs(matrix[2, 0] * vec.m_vecZ);
            double y = Math.Abs(matrix[0, 1] * vec.m_vecX) + Math.Abs(matrix[1, 1] * vec.m_vecY) + Math.Abs(matrix[2, 1] * vec.m_vecZ);
            double z = Math.Abs(matrix[0, 2] * vec.m_vecX) + Math.Abs(matrix[1, 2] * vec.m_vecY) + Math.Abs(matrix[2, 2] * vec.m_vecZ);

            vec.m_vecX = x;
            vec.m_vecY = y;
            vec.m_vecZ = z;
        }

        public void ApplyPos(ref UVector pt)
        {
            double x = matrix[0, 0] * pt.m_vecX + matrix[1, 0] * pt.m_vecY + matrix[2, 0] * pt.m_vecZ + matrix[3, 0];
            double y = matrix[0, 1] * pt.m_vecX + matrix[1, 1] * pt.m_vecY + matrix[2, 1] * pt.m_vecZ + matrix[3, 1];
            double z = matrix[0, 2] * pt.m_vecX + matrix[1, 2] * pt.m_vecY + matrix[2, 2] * pt.m_vecZ + matrix[3, 2];

            pt.m_vecX = x;
            pt.m_vecY = y;
            pt.m_vecZ = z;
        }

        public void ApplyAbsPos(ref UVector pt)
        {
            double x = matrix[0, 0] * pt.m_vecX + matrix[1, 0] * pt.m_vecY + matrix[2, 0] * pt.m_vecZ + matrix[3, 0];
            double y = matrix[0, 1] * pt.m_vecX + matrix[1, 1] * pt.m_vecY + matrix[2, 1] * pt.m_vecZ + matrix[3, 1];
            double z = matrix[0, 2] * pt.m_vecX + matrix[1, 2] * pt.m_vecY + matrix[2, 2] * pt.m_vecZ + matrix[3, 2];

            pt.m_vecX = Math.Abs(x);
            pt.m_vecY = Math.Abs(y);
            pt.m_vecZ = Math.Abs(z);
        }

        public void ApplyPos(ref double x, ref double y, ref double z)
        {
            double x1 = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z + matrix[3, 0];
            double y1 = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z + matrix[3, 1];
            double z1 = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z + matrix[3, 2];
            x = x1;
            y = y1;
            z = z1;
        }


        public void ApplyPos(ref Point3d pt)
        {
            double x = matrix[0, 0] * pt.X + matrix[1, 0] * pt.Y + matrix[2, 0] * pt.Z + matrix[3, 0];
            double y = matrix[0, 1] * pt.X + matrix[1, 1] * pt.Y + matrix[2, 1] * pt.Z + matrix[3, 1];
            double z = matrix[0, 2] * pt.X + matrix[1, 2] * pt.Y + matrix[2, 2] * pt.Z + matrix[3, 2];

            pt.X = x;
            pt.Y = y;
            pt.Z = z;
        }

        public void ApplyAbsPos(ref double x, ref double y, ref double z)
        {
            double x1 = matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z + matrix[3, 0];
            double y1 = matrix[0, 1] * x + matrix[1, 1] * y + matrix[2, 1] * z + matrix[3, 1];
            double z1 = matrix[0, 2] * x + matrix[1, 2] * y + matrix[2, 2] * z + matrix[3, 2];
            x = Math.Abs(x1);
            y = Math.Abs(y1);
            z = Math.Abs(z1);
        }

        public void GetXAxis(ref UVector xAxis)
        {
            xAxis.m_vecX = this.matrix[0, 0];
            xAxis.m_vecY = this.matrix[1, 0];
            xAxis.m_vecZ = this.matrix[2, 0];
            xAxis.Norm();
        }
        public void GetYAxis(ref UVector yAxis)
        {
            yAxis.m_vecX = this.matrix[0, 1];
            yAxis.m_vecY = this.matrix[1, 1];
            yAxis.m_vecZ = this.matrix[2, 1];
            yAxis.Norm();
        }
        public void GetZAxis(ref UVector zAxis)
        {
            zAxis.m_vecX = this.matrix[0, 0];
            zAxis.m_vecY = this.matrix[1, 0];
            zAxis.m_vecZ = this.matrix[2, 0];
            zAxis.Norm();
        }

        public Vector3d GetXAxis()
        {
            Vector3d xAxis = new Vector3d();

            xAxis.X = this.matrix[0, 0];
            xAxis.Y = this.matrix[1, 0];
            xAxis.Z = this.matrix[2, 0];

            return xAxis;

        }
        public Vector3d GetYAxis()
        {
            Vector3d yAxis = new Vector3d();
            yAxis.X = this.matrix[0, 1];
            yAxis.Y = this.matrix[1, 1];
            yAxis.Z = this.matrix[2, 1];

            return yAxis;
        }
        public Vector3d GetZAxis()
        {
            Vector3d zAxis = new Vector3d();
            zAxis.X = this.matrix[0, 2];
            zAxis.Y = this.matrix[1, 2];
            zAxis.Z = this.matrix[2, 2];

            return zAxis;
        }

        public Matrix3x3 GetMatrix3()
        {
            Matrix3x3 mat = new Matrix3x3();

            mat.Xx = this.matrix[0, 0];
            mat.Xy = this.matrix[1, 0];
            mat.Xz = this.matrix[2, 0];


            mat.Yx = this.matrix[0, 1];
            mat.Yy = this.matrix[1, 1];
            mat.Yz = this.matrix[2, 1];


            mat.Zx = this.matrix[0, 2];
            mat.Zy = this.matrix[1, 2];
            mat.Zz = this.matrix[2, 2];


            return mat;
        }
        /// <summary>
        /// 获取矩阵圆心点
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public Point3d GetCenter(Matrix4 mat)
        {
            Point3d temp = new Point3d(0, 0, 0);
            Matrix4 inver = mat.GetInversMatrix();
            inver.ApplyPos(ref temp);
            return temp;

        }
        public double Xx
        {
            get { return this.matrix[0, 0]; }
        }
        public double Xy
        {
            get { return this.matrix[1, 0]; }
        }
        public double Xz
        {
            get { return this.matrix[2, 0]; }
        }

        public double Yx
        {
            get { return this.matrix[0, 1]; }
        }
        public double Yy
        {
            get { return this.matrix[1, 1]; }
        }
        public double Yz
        {
            get { return this.matrix[2, 1]; }
        }

        public double Zx
        {
            get { return this.matrix[0, 2]; }
        }
        public double Zy
        {
            get { return this.matrix[1, 2]; }
        }
        public double Zz
        {
            get { return this.matrix[2, 2]; }
        }
    }
}
