using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {

    struct Color8 {
        public byte R;
        public byte G;
        public byte B;
        public Color8(byte Red, byte Green, byte Blue) {
            R = Red;
            G = Green;
            B = Blue;
        }
    }

    struct Vec4 {
        public float x;
        public float y;
        public float z;
        public float w;
        public Vec4(float x, float y, float z, float w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    struct Matrix4 {
        public Vec4 row1;
        public Vec4 row2;
        public Vec4 row3;
        public Vec4 row4;
        public Matrix4(Vec4 row1, Vec4 row2, Vec4 row3, Vec4 row4) {
            this.row1 = row1;
            this.row2 = row2;
            this.row3 = row3;
            this.row4 = row4;
        }
    }

    class Program {

        public static System.ConsoleColor FromColor(Color8 c) {
            int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0; // Bright bit
            index |= (c.R > 64) ? 4 : 0; // Red bit
            index |= (c.G > 64) ? 2 : 0; // Green bit
            index |= (c.B > 64) ? 1 : 0; // Blue bit
            return (System.ConsoleColor)index;
        }

        static void print(Char c, int x, int y) {
            Console.SetCursorPosition(x, y);
            Console.Write(c);
        }

        static void print(Char c, int x, int y, ConsoleColor color) {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(c);
        }

        static void print(Char c, int x, int y, Color8 color) {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = FromColor(color);
            Console.Write(c);
        }

        static void print(Char c, float x, float y, Color8 color) {
            Console.SetCursorPosition((int)x, (int)y);
            Console.ForegroundColor = FromColor(color);
            Console.Write(c);
        }

        static void render(Color8[,] buf, float[,] zbuff) {
            for(int y = 25; y != 0; --y) {
                for(int x = 80; x != 0; --x) {
                    print('*', x - 1, y - 1, buf[x - 1, y - 1]);
                }
            }
        }

        static void printLine(Color8 color, int x0, int y0, int x1, int y1) {

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;

            int dy = Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;

            int err = (dx > dy ? dx : -dy) / 2;
            int e2;

            for(;;) {

                print('*', x0, y0, color);
                if(x0 == x1 && y0 == y1) {
                    break;
                }
                e2 = err;
                if(e2 > -dx) {
                    err -= dy;
                    x0 += sx;
                }
                if(e2 < dy) {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        static void printLine(Color8[,] buf, float[,] zbuff, int x0, int y0, int x1, int y1, Color8 color) {

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;

            int dy = Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;

            int err = (dx > dy ? dx : -dy) / 2;
            int e2;

            for(;;) {
                if(x0 >= buf.GetLength(0) || y0 >= buf.GetLength(1) || x0 < 0 || y0 < 0)
                    break;

                buf[x0, y0] = color;

                if(x0 == x1 && y0 == y1) {
                    break;
                }
                e2 = err;
                if(e2 > -dx) {
                    err -= dy;
                    x0 += sx;
                }
                if(e2 < dy) {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        static void clearScreen(Color8[,] buf, float[,] zbuff) {
            for(int y = 25; y != 0; --y) {
                for(int x = 80; x != 0; --x) {
                    buf[x - 1, y - 1] = new Color8(0, 0, 0);
                }
            }
        }

        static Matrix4 setProjectionMatrix(float angleOfView, float near, float far) {
            float scale = 1.0f / (float)(Math.Tan(angleOfView * 0.5f * (float)(Math.PI) / 180.0f));
            Vec4 row1 = new Vec4(scale, 0.0f, 0.0f, 0.0f);
            Vec4 row2 = new Vec4(0.0f, scale, 0.0f, 0.0f);
            Vec4 row3 = new Vec4(0.0f, 0.0f, -far / (far - near), -1.0f);
            Vec4 row4 = new Vec4(0.0f, 0.0f, -far * near / (far - near), 0.0f);

            return new Matrix4(row1, row2, row3, row4);
        }

        static Matrix4 setRotationXMatrix(float angle) {

            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            Vec4 row1 = new Vec4(1.0f, 0.0f, 0.0f, 0.0f);
            Vec4 row2 = new Vec4(0.0f, cosTheta, -sinTheta, 0.0f);
            Vec4 row3 = new Vec4(0.0f, sinTheta, cosTheta, 0.0f);
            Vec4 row4 = new Vec4(0.0f, 0.0f, 0.0f, 1.0f);

            return new Matrix4(row1, row2, row3, row4);
        }

        static Matrix4 setRotationYMatrix(float angle) {

            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            Vec4 row1 = new Vec4(cosTheta, 0.0f, sinTheta, 0.0f);
            Vec4 row2 = new Vec4(0.0f, 1.0f, 0.0f, 0.0f);
            Vec4 row3 = new Vec4(-sinTheta, 0.0f, cosTheta, 0.0f);
            Vec4 row4 = new Vec4(0.0f, 0.0f, 0.0f, 1.0f);

            return new Matrix4(row1, row2, row3, row4);
        }

        static Matrix4 setRotationZMatrix(float angle) {

            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);

            Vec4 row1 = new Vec4(cosTheta, -sinTheta, 0.0f, 0.0f);
            Vec4 row2 = new Vec4(sinTheta, cosTheta, 0.0f, 0.0f);
            Vec4 row3 = new Vec4(0.0f, 0.0f, 1.0f, 0.0f);
            Vec4 row4 = new Vec4(0.0f, 0.0f, 0.0f, 1.0f);

            return new Matrix4(row1, row2, row3, row4);
        }

        static Matrix4 setProjectionMatrix4(float angleOfView, float near, float far) {
            float scale = 1.0f / (float)(Math.Tan(angleOfView * 0.5f * (float)(Math.PI) / 180.0f));
            Vec4 row1 = new Vec4(scale, 0.0f, 0.0f, 0.0f);
            Vec4 row2 = new Vec4(0.0f, scale, 0.0f, 0.0f);
            Vec4 row3 = new Vec4(0.0f, 0.0f, -far / (far - near), -far * near / (far - near));
            Vec4 row4 = new Vec4(0.0f, 0.0f, -1.0f, 0.0f);

            return new Matrix4(row1, row2, row3, row4);
        }

        static Vec4 multPointMatrix(Vec4 v, Matrix4 M) {

            Vec4 outRes = new Vec4();

            outRes.x = v.x * M.row1.x + v.y * M.row2.x + v.z * M.row3.x + /* in.z = 1 */ M.row4.x;
            outRes.y = v.x * M.row1.y + v.y * M.row2.y + v.z * M.row3.y + /* in.z = 1 */ M.row4.y;
            outRes.z = v.x * M.row1.z + v.y * M.row2.z + v.z * M.row3.z + /* in.z = 1 */ M.row4.z;
            float w = v.x * M.row1.w + v.y * M.row2.w + v.z * M.row3.w + /* in.z = 1 */ M.row4.w;

            // normalize if w is different than 1 (convert from homogeneous to Cartesian coordinates)
            if(w != 1.0f) {
                outRes.x /= w;
                outRes.y /= w;
                outRes.z /= w;
            }

            return outRes;
        }

        static Matrix4 multMatrix(Matrix4 M1, Matrix4 M2) {

            Vec4 ap1 = M1.row1;
            Vec4 ap2 = M1.row2;
            Vec4 ap3 = M1.row3;
            Vec4 ap4 = M1.row4;

            Vec4 bp1 = M2.row1;
            Vec4 bp2 = M2.row2;
            Vec4 bp3 = M2.row3;
            Vec4 bp4 = M2.row4;

            Vec4 cp1 = new Vec4();
            Vec4 cp2 = new Vec4();
            Vec4 cp3 = new Vec4();
            Vec4 cp4 = new Vec4();

            float a0, a1, a2, a3;

            a0 = ap1.x;
            a1 = ap1.y;
            a2 = ap1.z;
            a3 = ap1.w;

            cp1.x = a0 * bp1.x + a1 * bp2.x + a2 * bp3.x + a3 * bp4.x;
            cp1.y = a0 * bp1.y + a1 * bp2.y + a2 * bp3.y + a3 * bp4.y;
            cp1.z = a0 * bp1.z + a1 * bp2.z + a2 * bp3.z + a3 * bp4.z;
            cp1.w = a0 * bp1.w + a1 * bp2.w + a2 * bp3.w + a3 * bp4.w;

            a0 = ap2.x;
            a1 = ap2.y;
            a2 = ap2.z;
            a3 = ap2.w;

            cp2.x = a0 * bp1.x + a1 * bp2.x + a2 * bp3.x + a3 * bp4.x;
            cp2.y = a0 * bp1.y + a1 * bp2.y + a2 * bp3.y + a3 * bp4.y;
            cp2.z = a0 * bp1.z + a1 * bp2.z + a2 * bp3.z + a3 * bp4.z;
            cp2.w = a0 * bp1.w + a1 * bp2.w + a2 * bp3.w + a3 * bp4.w;

            a0 = ap3.x;
            a1 = ap3.y;
            a2 = ap3.z;
            a3 = ap3.w;

            cp3.x = a0 * bp1.x + a1 * bp2.x + a2 * bp3.x + a3 * bp4.x;
            cp3.y = a0 * bp1.y + a1 * bp2.y + a2 * bp3.y + a3 * bp4.y;
            cp3.z = a0 * bp1.z + a1 * bp2.z + a2 * bp3.z + a3 * bp4.z;
            cp3.w = a0 * bp1.w + a1 * bp2.w + a2 * bp3.w + a3 * bp4.w;

            a0 = ap4.x;
            a1 = ap4.y;
            a2 = ap4.z;
            a3 = ap4.w;

            cp4.x = a0 * bp1.x + a1 * bp2.x + a2 * bp3.x + a3 * bp4.x;
            cp4.y = a0 * bp1.y + a1 * bp2.y + a2 * bp3.y + a3 * bp4.y;
            cp4.z = a0 * bp1.z + a1 * bp2.z + a2 * bp3.z + a3 * bp4.z;
            cp4.w = a0 * bp1.w + a1 * bp2.w + a2 * bp3.w + a3 * bp4.w;

            return new Matrix4(cp1, cp2, cp3, cp4);
        }

        static void Main(string[] args) {

            Color8[,] screen = new Color8[80, 25];
            float[,] zbuffer = new float[80, 25];

            float angle = 0.0f;

            while(true) {
                clearScreen(screen, zbuffer);

                Vec4[] cube = new Vec4[8] {
                    new Vec4(-1.0f,  1.0f, 1.0f, 0.0f),
                    new Vec4( 1.0f,  1.0f, 1.0f, 0.0f),
                    new Vec4( 1.0f, -1.0f, 1.0f, 0.0f),
                    new Vec4(-1.0f, -1.0f, 1.0f, 0.0f),
                    new Vec4(-1.0f,  1.0f, 0.0f, 0.0f),
                    new Vec4( 1.0f,  1.0f, 0.0f, 0.0f),
                    new Vec4( 1.0f, -1.0f, 0.0f, 0.0f),
                    new Vec4(-1.0f, -1.0f, 0.0f, 0.0f)
                };

                Matrix4 model = new Matrix4(
                    new Vec4(0.5f, 0.0f, 0.0f, 0.0f),
                    new Vec4(0.0f, 0.5f, 0.0f, 0.0f),
                    new Vec4(0.0f, 0.0f, 0.75f, 0.0f),
                    new Vec4(0.0f, 0.0f, 0.0f, 1.0f)
                );

                model = multMatrix(model, setRotationXMatrix(angle));
                model = multMatrix(model, setRotationZMatrix(angle));

                Matrix4 view = new Matrix4(
                    new Vec4(1.0f, 0.0f, 0.0f, 0.0f),
                    new Vec4(0.0f, 1.0f, 0.0f, 0.0f),
                    new Vec4(0.0f, 0.0f, 1.0f, 0.0f),
                    new Vec4(0.0f, 0.0f, 2.0f, 1.0f)
                );

                Matrix4 modelView = multMatrix(model, view);

                Matrix4 proj = setProjectionMatrix(60.0f, 0.1f, 1000.0f);

                Vec4 point1 = multPointMatrix(cube[0], modelView);
                point1 = multPointMatrix(point1, proj);
                point1.x = (point1.x + 1.0f) / 2.0f * 80.0f;
                point1.y = (point1.y + 1.0f) / 2.0f * 25.0f;

                Vec4 point2 = multPointMatrix(cube[1], modelView);
                point2 = multPointMatrix(point2, proj);
                point2.x = (point2.x + 1.0f) / 2.0f * 80.0f;
                point2.y = (point2.y + 1.0f) / 2.0f * 25.0f;

                Vec4 point3 = multPointMatrix(cube[2], modelView);
                point3 = multPointMatrix(point3, proj);
                point3.x = (point3.x + 1.0f) / 2.0f * 80.0f;
                point3.y = (point3.y + 1.0f) / 2.0f * 25.0f;

                Vec4 point4 = multPointMatrix(cube[3], modelView);
                point4 = multPointMatrix(point4, proj);
                point4.x = (point4.x + 1.0f) / 2.0f * 80.0f;
                point4.y = (point4.y + 1.0f) / 2.0f * 25.0f;

                Vec4 point5 = multPointMatrix(cube[4], modelView);
                point5 = multPointMatrix(point5, proj);
                point5.x = (point5.x + 1.0f) / 2.0f * 80.0f;
                point5.y = (point5.y + 1.0f) / 2.0f * 25.0f;

                Vec4 point6 = multPointMatrix(cube[5], modelView);
                point6 = multPointMatrix(point6, proj);
                point6.x = (point6.x + 1.0f) / 2.0f * 80.0f;
                point6.y = (point6.y + 1.0f) / 2.0f * 25.0f;

                Vec4 point7 = multPointMatrix(cube[6], modelView);
                point7 = multPointMatrix(point7, proj);
                point7.x = (point7.x + 1.0f) / 2.0f * 80.0f;
                point7.y = (point7.y + 1.0f) / 2.0f * 25.0f;

                Vec4 point8 = multPointMatrix(cube[7], modelView);
                point8 = multPointMatrix(point8, proj);
                point8.x = (point8.x + 1.0f) / 2.0f * 80.0f;
                point8.y = (point8.y + 1.0f) / 2.0f * 25.0f;

                printLine(screen, zbuffer,
                    (int)point1.x,
                    (int)point1.y,
                    (int)point2.x,
                    (int)point2.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point2.x,
                    (int)point2.y,
                    (int)point3.x,
                    (int)point3.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point3.x,
                    (int)point3.y,
                    (int)point4.x,
                    (int)point4.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point4.x,
                    (int)point4.y,
                    (int)point1.x,
                    (int)point1.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point5.x,
                    (int)point5.y,
                    (int)point6.x,
                    (int)point6.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point6.x,
                    (int)point6.y,
                    (int)point7.x,
                    (int)point7.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point7.x,
                    (int)point7.y,
                    (int)point8.x,
                    (int)point8.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point8.x,
                    (int)point8.y,
                    (int)point5.x,
                    (int)point5.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point1.x,
                    (int)point1.y,
                    (int)point5.x,
                    (int)point5.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point2.x,
                    (int)point2.y,
                    (int)point6.x,
                    (int)point6.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point3.x,
                    (int)point3.y,
                    (int)point7.x,
                    (int)point7.y,
                    new Color8(255, 255, 255)
                );

                printLine(screen, zbuffer,
                    (int)point4.x,
                    (int)point4.y,
                    (int)point8.x,
                    (int)point8.y,
                    new Color8(255, 255, 255)
                );

                render(screen, zbuffer);

                angle += (float)(Math.PI / 32.0f);
            }

            Console.ReadKey();
        }
    }
}
