using System;
using System.Numerics;
using Rhino.Geometry;

using Plane = Rhino.Geometry.Plane;

namespace PhysX.GH
{
    public static class Util
    {
        private static Random random = new Random();


        public static double Distance(Point3d A, Point3d B)
        {
            return Math.Sqrt(Math.Pow(A.X - B.X, 2.0) + Math.Pow(A.Y - B.Y, 2.0) + Math.Pow(A.Z - B.Z, 2.0));
        }


        public static double DistanceXY(Point3d A, Point3d B)
        {
            return Math.Sqrt(Math.Pow(A.X - B.X, 2.0) + Math.Pow(A.Y - B.Y, 2.0));
        }


        public static double DistanceSquared(Point3d A, Point3d B)
        {
            return Math.Pow(A.X - B.X, 2.0) + Math.Pow(A.Y - B.Y, 2.0) + Math.Pow(A.Z - B.Z, 2.0);
        }


        public static double DistanceSquaredXY(Point3d A, Point3d B)
        {
            return Math.Pow(A.X - B.X, 2.0) + Math.Pow(A.Y - B.Y, 2.0);
        }


        public static double DistancePointLine(Point3d M, Point3d A, Point3d B)
        {
            return Vector3d.CrossProduct(M - A, M - B).Length / (A - B).Length;
        }


        public static Point3d ClosestPointOnLine(Point3d M, Point3d A, Point3d B)
        {
            double distance = DistancePointLine(M, A, B);

            Vector3d AB = B - A;
            if ((M - A) * AB < 0) AB *= -1;

            return A + AB / AB.Length * Math.Sqrt(DistanceSquared(M, A) - distance * distance);
        }


        public static Point3d ClosestPointOnLine(Point3d M, Point3d A, Point3d B, ref double distance)
        {
            distance = DistancePointLine(M, A, B);
            Vector3d AB = B - A;
            if ((M - A) * AB < 0) AB *= -1;

            return A + AB / AB.Length * Math.Sqrt(DistanceSquared(M, A) - distance * distance);
        }


        public static void ResetRandom(int seed) { random = seed < 0 ? new Random() : new Random(seed); }
        public static int GetRandomInteger() { return random.Next(); }
        public static int GetRandomInteger(int minValue, int maxValue) { return random.Next(minValue, maxValue); }
        public static int GetRandomInteger(int maxValue) { return random.Next(maxValue); }
        public static double GetRandomDouble() { return random.NextDouble(); }
        public static double GetRandomDouble(double minValue, double maxValue) { return minValue + random.NextDouble() * (maxValue - minValue); }
        public static double GetRandomDouble(double maxValue) { return random.NextDouble() * maxValue; }


        public static Point3d GetRandomPoint(double minX = 0.0, double maxX = 1.0, double minY = 0.0, double maxY = 1.0, double minZ = 0.0, double maxZ = 1.0)
        {
            double x = minX + (maxX - minX) * random.NextDouble();
            double y = minY + (maxY - minY) * random.NextDouble();
            double z = minZ + (maxZ - minZ) * random.NextDouble();
            return new Point3d(x, y, z);
        }


        public static Vector3d GetRandomUnitVector()
        {
            double phi = 2.0 * Math.PI * random.NextDouble();
            double theta = Math.Acos(2.0 * random.NextDouble() - 1.0);
            double x = Math.Sin(theta) * Math.Cos(phi);
            double y = Math.Sin(theta) * Math.Sin(phi);
            double z = Math.Cos(theta);
            return new Vector3d(x, y, z);
        }


        public static Vector3d GetRandomUnitVectorXY()
        {
            double angle = 2.0 * Math.PI * random.NextDouble();
            double x = Math.Cos(angle);
            double y = Math.Sin(angle);
            return new Vector3d(x, y, 0.0);
        }


        public static Vector3d GetRandomVector(double minLength = 0.0, double maxLength = 1.0)
        {
            return GetRandomUnitVector() * (minLength + random.NextDouble() * (maxLength - minLength));
        }


        public static Vector3d GetRandomVectorXY(double minLength = 0.0, double maxLength = 1.0)
        {
            return GetRandomUnitVectorXY() * (minLength + random.NextDouble() * (maxLength - minLength));
        }


        public static Vector3d ComputePlaneNormal(Point3d A, Point3d B, Point3d C)
        {
            Vector3d normal = Vector3d.CrossProduct(B - A, C - A);
            normal.Unitize();
            return normal;
        }


        private const double degreeToRadianFactor = Math.PI / 180.0;
        private const double radianToDegreeFactor = 180.0 / Math.PI;

        public static double ToRadian(double degree)
        {
            return degree * degreeToRadianFactor;
        }


        public static double ToDegree(double radian)
        {
            return radian * radianToDegreeFactor;
        }


        public static double AngleBetweenTwoTriangles(Point3d A, Point3d B, Point3d M, Point3d N)
        {
            return AngleBetweenTwoVectors(
                M - ClosestPointOnLine(M, A, B),
                N - ClosestPointOnLine(N, A, B));
        }


        public static double AngleBetweenTwoUnitVectors(Vector3d a, Vector3d b)
        {
            return Math.Acos(a.X * b.X + a.Y * b.Y + a.Z * b.Z);
        }


        public static double AngleBetweenTwoUnitVectors(Vector3d a, Vector3d b, Vector3d z, bool positive = true)
        {
            double angle = Math.Acos(a.X * b.X + a.Y * b.Y + a.Z * b.Z);

            if (z * Vector3d.CrossProduct(a, b) < 0.0)
            {
                if (positive) angle = 2.0 * Math.PI - angle;
                else angle = -angle;
            }
            return angle;
        }


        public static double AngleBetweenTwoVectors(Vector3d a, Vector3d b)
        {
            double temp = (a.X * b.X + a.Y * b.Y + a.Z * b.Z) / (a.Length * b.Length);
            if (temp > 1.0) temp = 1.0;
            else if (temp < -1.0) temp = -1.0;
            return Math.Acos(temp);
        }


        public static double AngleBetweenTwoVectors(Vector3d a, Vector3d b, Vector3d z, bool positive = true)
        {
            double temp = (a.X * b.X + a.Y * b.Y + a.Z * b.Z) / (a.Length * b.Length);
            if (temp > 1.0) temp = 1.0;
            else if (temp < -1.0) temp = -1.0;

            double angle = Math.Acos(temp);

            if (z * Vector3d.CrossProduct(a, b) < 0.0)
            {
                if (positive) angle = 2.0 * Math.PI - angle;
                else angle = -angle;
            }

            return angle;
        }


        public static void CleanMesh(this Mesh mesh)
        {
            mesh.Vertices.CombineIdentical(true, true);
            mesh.Vertices.CullUnused();
            mesh.Weld(3.1415926535897931);
            mesh.UnifyNormals();
            mesh.FaceNormals.ComputeFaceNormals();
            mesh.Normals.ComputeNormals();
        }


        public static Mesh UnjoinFaces(this Mesh mesh)
        {
            Mesh m = new Mesh();

            foreach (MeshFace face in mesh.Faces)
            {
                int i = m.Vertices.Count;
                m.Vertices.Add(mesh.Vertices[face.A]);
                m.Vertices.Add(mesh.Vertices[face.B]);
                m.Vertices.Add(mesh.Vertices[face.C]);
                m.Faces.AddFace(new MeshFace(i, i + 1, i + 2));
            }

            m.Normals.ComputeNormals();
            m.FaceNormals.ComputeFaceNormals();

            return m;
        }


        public static Vector3d ToRhinoVector(this Vector3 v) => new Vector3d(v.X, v.Y, v.Z);
        public static Point3d ToRhinoPoint(this Vector3 v) => new Point3d(v.X, v.Y, v.Z);

        public static Vector3 ToSystemVector(this Vector3d v) => new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        public static Vector3 ToSystemVector(this Vector3f v) => new Vector3(v.X, v.Y, v.Z);
        public static Vector3 ToSystemVector(this Point3d p) => new Vector3((float)p.X, (float)p.Y, (float)p.Z);
        public static Vector3 ToSystemVector(this Point3f p) => new Vector3(p.X, p.Y, p.Z);


        public static Plane ToRhinoPlane(this Matrix4x4 m)
            => new Plane(
                new Point3d(m.M41, m.M42, m.M43),
                new Vector3d(m.M11, m.M12, m.M13),
                new Vector3d(m.M21, m.M22, m.M23));


        public static Transform ToRhinoTransform(this Matrix4x4 m)
            => new Transform
            {
                M00 = m.M11, M01 = m.M21, M02 = m.M31, M03 = m.M41,
                M10 = m.M12, M11 = m.M22, M12 = m.M32, M13 = m.M42,
                M20 = m.M13, M21 = m.M23, M22 = m.M33, M23 = m.M43,
                M30 = m.M14, M31 = m.M24, M32 = m.M34, M33 = m.M44,
            };


        public static Transform ToRhinoTransform(this Plane p)
            =>new Transform
            {
                M00 = p.XAxis.X, M01 = p.YAxis.X, M02 = p.ZAxis.X, M03 = p.OriginX,
                M10 = p.XAxis.Y, M11 = p.YAxis.Y, M12 = p.ZAxis.Y, M13 = p.OriginY,
                M20 = p.XAxis.Z, M21 = p.YAxis.Z, M22 = p.ZAxis.Z, M23 = p.OriginZ,
                M30 = 0.0      , M31 = 0.0      , M32 = 0.0      , M33 = 1.0,
            };


        public static Matrix4x4 ToMatrix(this Transform t)
            => new Matrix4x4
            {
                M11 = (float)t.M00, M12 = (float)t.M10, M13 = (float)t.M20, M14 = (float)t.M30,
                M21 = (float)t.M01, M22 = (float)t.M11, M23 = (float)t.M21, M24 = (float)t.M31,
                M31 = (float)t.M02, M32 = (float)t.M12, M33 = (float)t.M22, M34 = (float)t.M32,
                M41 = (float)t.M03, M42 = (float)t.M13, M43 = (float)t.M23, M44 = (float)t.M33,
            };


        public static Matrix4x4 ToMatrix(this Plane p)
            => new Matrix4x4
            {
                M11 = (float)p.XAxis.X, M12 = (float)p.XAxis.Y, M13 = (float)p.XAxis.Z, M14 = 0f,
                M21 = (float)p.YAxis.X, M22 = (float)p.YAxis.Y, M23 = (float)p.YAxis.Z, M24 = 0f,
                M31 = (float)p.ZAxis.X, M32 = (float)p.ZAxis.Y, M33 = (float)p.ZAxis.Z, M34 = 0f,
                M41 = (float)p.OriginX, M42 = (float)p.OriginY, M43 = (float)p.OriginZ, M44 = 1f,
            };


        //public static Matrix4x4 ToMatrix(this Rhino.Geometry.Plane p)
        //    => new Matrix4x4
        //    {
        //        M11 = (float)t.M00, M12 = (float)t.M10, M13 = (float)t.M20, M14 = (float)t.M30,
        //        M21 = (float)t.M01, M22 = (float)t.M11, M23 = (float)t.M21, M24 = (float)t.M31,
        //        M31 = (float)t.M02, M32 = (float)t.M12, M33 = (float)t.M22, M34 = (float)t.M32,
        //        M41 = (float)t.M03, M42 = (float)t.M13, M43 = (float)t.M23, M44 = (float)t.M33,
        //    };



        //public static Transform ToRhinoTransformXZ(Matrix4x4 matrix)
        //{
        //    Transform transform = new Transform();
        //    {
        //        transform.M00 = matrix.M11;
        //        transform.M01 = matrix.M31;
        //        transform.M02 = matrix.M21;
        //        transform.M03 = matrix.M41;

        //        transform.M10 = matrix.M13;
        //        transform.M11 = matrix.M33;
        //        transform.M12 = matrix.M23;
        //        transform.M13 = matrix.M43;

        //        transform.M20 = matrix.M12;
        //        transform.M21 = matrix.M32;
        //        transform.M22 = matrix.M22;
        //        transform.M23 = matrix.M42;

        //        transform.M30 = matrix.M14;
        //        transform.M31 = matrix.M34;
        //        transform.M32 = matrix.M24;
        //        transform.M33 = matrix.M44;
        //    }
        //    return transform;
        //}


        //public static Matrix4x4 ToMatrixXZ(Transform transform)
        //{
        //    Matrix4x4 matrix = new Matrix4x4();

        //    matrix.M11 = (float)transform.M00;
        //    matrix.M31 = (float)transform.M01;
        //    matrix.M21 = (float)transform.M02;
        //    matrix.M41 = (float)transform.M03;

        //    matrix.M13 = (float)transform.M10;
        //    matrix.M33 = (float)transform.M11;
        //    matrix.M23 = (float)transform.M12;
        //    matrix.M43 = (float)transform.M13;

        //    matrix.M12 = (float)transform.M20;
        //    matrix.M32 = (float)transform.M21;
        //    matrix.M22 = (float)transform.M22;
        //    matrix.M42 = (float)transform.M23;

        //    matrix.M14 = (float)transform.M30;
        //    matrix.M34 = (float)transform.M31;
        //    matrix.M24 = (float)transform.M32;
        //    matrix.M44 = (float)transform.M33;

        //    return matrix;
        //}
    }
}