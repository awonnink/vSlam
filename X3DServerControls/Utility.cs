using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class WebSceneProps : IWebSceneProps
    {
        public WebSceneProps()
        {
            Target = Target._Blank;
            Visitors = 0;
            Favorite = -1;
            History = -1;
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public int Visitors { get; set; }
        public int Favorite { get; set; }
        public int History { get; set; }
        public Target Target { get; set; }
        
    }
    public class Vector2
    {
        public Vector2() { Init(); }
        public Vector2(double x, double y)
        {
            Init();
            X = x; Y = y;
        }
        public Vector2(string vs)
        {
            Init();
            Vector2 v = Vector2.FromString(vs);
            if (v != null)
            {
                X = v.X;
                Y = v.Y;
            }
        }
        void Init()
        {
            X = 0; Y = 0; 
        }
        public double X { get; set; }
        public double Y { get; set; }
        public override string ToString()
        {
            return Vector2.ToString(this);
        }
        public static Vector2 One(double length = 1)
        {
            return length * new Vector2(1, 1);
        }
        public static string ToString(Vector2 v)
        {
            return v == null ? null : string.Format("{0} {1}", v.X, v.Y);
        }
        public static Vector2 FromString(string v)
        {
            if (!string.IsNullOrWhiteSpace(v))
            {
                v = v.Replace("  ", " ").Trim();
                string[] p = v.Split(Convert.ToChar(" "));
                double x = 0;
                double y = 0;
                if (p.Length == 32 && double.TryParse(p[0], out x) && double.TryParse(p[1], out y))
                {
                    return new Vector2(x, y);
                }
            }
            return null;
        }

        public static Vector2 operator *(double x, Vector2 v)
        {
            return v == null ? null : new Vector2(x * v.X, x * v.Y);
        }
        public static Vector2 operator *(Vector2 v, double x)
        {
            return v == null ? null : new Vector2(x * v.X, x * v.Y);
        }
        public static Vector2 operator *(Vector2 v, Vector2 w)
        {
            return v == null || w == null ? null : new Vector2(w.X * v.X, w.Y * v.Y);
        }
        public static Vector2 operator +(Vector2 v, Vector2 w)
        {
            return v == null || w == null ? null : new Vector2(w.X + v.X, w.Y + v.Y);
        }
        public static Vector2 operator -(Vector2 v, Vector2 w)
        {
            return v == null || w == null ? null : new Vector2(v.X - w.X, v.Y - w.Y);
        }
    }

    public class Vector3
    {
        public Vector3() { Init(); }
        public Vector3(double x, double y, double z) {
            Init();
            X = x;Y = y;Z = z;
        }
        public Vector3(double xyz)
        {
            Init();
            X = xyz; Y = xyz; Z = xyz;
        }
        public Vector3(string vs) {
            Init();
            Vector3 v = Vector3.FromString(vs);
            if(v!=null)
            {
                X = v.X;
                Y = v.Y;
                Z = v.Z;
            }
        }
        void Init()
        {
            X = 0; Y = 0; Z = 0;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Length
        {
            get
            {
                return Math.Sqrt(LengthSquared);
            }
        }
        public double LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }
        public static double Distance(Vector3 v1, Vector3 v2)
        {
            Vector3 v3 = v1 - v2;
            return v3.Length;
        }
        public override string ToString()
        {
            return Vector3.ToString(this);
        }
        public static Vector3 One(double length=1)
        {
            return length * new Vector3(1, 1, 1);
        }
        public static string ToString(Vector3 v)
        {
            return v==null?null: string.Format("{0} {1} {2}", v.X, v.Y, v.Z);
        }
        public static Vector3 FromString(string v)
        {
            if (!string.IsNullOrWhiteSpace(v))
            {
                v = v.Replace("  ", " ").Trim();
                string[] p = v.Split(Convert.ToChar(" "));
                double x = 0;
                double y = 0;
                double z = 0;
                if (p.Length == 3 && double.TryParse(p[0], out x) && double.TryParse(p[1], out y) && double.TryParse(p[2], out z))
                {
                    return new Vector3(x, y, z);
                }
            }
            return null;
        }

        public static Vector3 operator * (double x, Vector3 v)
        {
            return v == null ? null : new Vector3(x * v.X, x * v.Y, x * v.Z);
        }
        public static Vector3 operator * (Vector3 v, double x)
        {
            return v == null ? null : new Vector3(x * v.X, x * v.Y, x * v.Z);
        }
        public static Vector3 operator * (Vector3 v, Vector3 w)
        {
            return v == null || w == null ? null : new Vector3(w.X * v.X, w.Y * v.Y, w.Z * v.Z);
        }
        public static Vector3 operator + (Vector3 v, Vector3 w)
        {
            return v == null ||w==null ? null : new Vector3(w.X + v.X, w.Y + v.Y, w.Z + v.Z);
        }
        public static Vector3 operator - (Vector3 v, Vector3 w)
        {
            return v == null || w == null ? null : new Vector3(v.X - w.X, v.Y - w.Y, v.Z - w.Z);
        }
    }
    public class Quaternion
    {
        public Quaternion() { Init(); }
        public Quaternion(double x, double y, double z, double w)
        {
            Init();
            X = x; Y = y; Z = z; W = w;
        }
        public Quaternion(string vs)
        {
            Init();
            Quaternion v = Quaternion.FromString(vs);
            if (v != null)
            {
                X = v.X;
                Y = v.Y;
                Z = v.Z;
                W = v.W;
            }
        }
        void Init()
        {
            X = 0; Y = 0; Z = 0; W = 0;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }
        public override string ToString()
        {
            return Quaternion.ToString(this);
        }
        public static Quaternion One(double length = 1)
        {
            return length * new Quaternion(1, 1, 1, 1);
        }
        public static string ToString(Quaternion v)
        {
            return v == null ? null : string.Format("{0} {1} {2} {3}", v.X, v.Y, v.Z, v.W);
        }
        public static Quaternion FromString(string v)
        {
            if (!string.IsNullOrWhiteSpace(v))
            {
                v = v.Replace("  ", " ").Trim();
                string[] p = v.Split(Convert.ToChar(" "));
                double x = 0;
                double y = 0;
                double z = 0;
                double w = 0;
                if (p.Length == 4 && double.TryParse(p[0], out x) && double.TryParse(p[1], out y) && double.TryParse(p[2], out z) && double.TryParse(p[3], out w))
                {
                    return new Quaternion(x, y, z, w);
                }
            }
            return null;
        }

        public static Quaternion operator *(double x, Quaternion v)
        {
            return v == null ? null : new Quaternion(x * v.X, x * v.Y, x * v.Z, x * v.W);
        }
        public static Quaternion operator *(Quaternion v, double x)
        {
            return v == null ? null : new Quaternion(x * v.X, x * v.Y, x * v.Z, x * v.W);
        }
        public static Quaternion operator * (Quaternion v, Quaternion w)
        {
            return v == null || w == null ? null : new Quaternion(w.X * v.X, w.Y * v.Y, w.Z * v.Z, w.W * v.W);
        }
        public static Quaternion operator +(Quaternion v, Quaternion w)
        {
            return v == null || w == null ? null : new Quaternion(w.X + v.X, w.Y + v.Y, w.Z + v.Z, w.W + v.W);
        }
        public static Quaternion operator -(Quaternion v, Quaternion w)
        {
            return v == null || w == null ? null : new Quaternion(v.X - w.X, v.Y - w.Y, v.Z - w.Z, v.W - w.W);
        }
    }
}
