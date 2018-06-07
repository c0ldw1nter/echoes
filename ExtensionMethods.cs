using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using ModifiedControls;
using System.ArrayExtensions;
using Echoes;
using System.Drawing.Imaging;

namespace System
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> ienum, T o)
        {
            List<T> list = new List<T>();
            list.Add(o);
            return ienum.Except(list);
        }
        public static int IndexOf(this object[] array, object element)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(element))
                {
                    return i;
                }
            }
            return -1;
        } 

        public static Color MixWith(this Color c1, Color c2, float transparency)
        {
            int _r = Math.Min((c1.R + (byte)(c2.R * transparency)), 255);
            int _g = Math.Min((c1.G + (byte)(c2.G * transparency)), 255);
            int _b = Math.Min((c1.B + (byte)(c2.B * transparency)), 255);

            return Color.FromArgb(Convert.ToByte(_r), Convert.ToByte(_g), Convert.ToByte(_b));
        }

        public static Bitmap SetOpacity(this Bitmap originalImage, double opacity)
        {
            if ((originalImage.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
            {
                // Cannot modify an image with indexed colors
                return originalImage;
            }

            Bitmap bmp = (Bitmap)originalImage.Clone();

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            // This code is specific to a bitmap with 32 bits per pixels 
            // (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
            int numBytes = bmp.Width * bmp.Height * 4;
            byte[] argbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += 4)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                // If 100% transparent, skip pixel
                if (argbValues[counter + 4 - 1] == 0)
                    continue;

                int pos = 0;
                pos++; // B value
                pos++; // G value
                pos++; // R value

                argbValues[counter + pos] = (byte)(argbValues[counter + pos] * opacity);
            }

            // Copy the ARGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static void AdjustTextColor(this ModifiedButton btn)
        {
            if (btn.BackColor.R < 127 && btn.BackColor.G < 127 && btn.BackColor.B < 127) 
                btn.ForeColor = Color.White;
            else 
                btn.ForeColor = Color.Black;
        }
        public static Color Lighten(this Color c)
        {
            return c.Lighten(24);
        }

        public static Color Lighten(this Color c, byte lightenValue)
        {
            if (255 - c.R < lightenValue) lightenValue = (byte)(255 - c.R);
            if (255 - c.G < lightenValue) lightenValue = (byte)(255 - c.G);
            if (255 - c.B < lightenValue) lightenValue = (byte)(255 - c.B);
            return Color.FromArgb(c.R + lightenValue, c.G + lightenValue, c.B + lightenValue);
        }

        public static Color Darken(this Color c)
        {
            return c.Darken(24);
        }

        public static Color Darken(this Color c, byte darkenValue)
        {
            byte r=0, g=0, b=0;
            if (c.R > darkenValue) r = (byte)(c.R - darkenValue);
            if (c.G > darkenValue) g = (byte)(c.G - darkenValue);
            if (c.B > darkenValue) b = (byte)(c.B - darkenValue);
            return Color.FromArgb(r,g,b);
        }

        public static Color Invert(this Color c)
        {
            return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
        }

        public static bool IsModRequired(this int num, Modifier mod)
        {
            if ((int)mod >= num) return true;
            return false;
        }

        /*public static string ToModText(this HotkeyData hkd) {
            if (hkd.alt == hkd.shift == hkd.ctrl == false) return "NONE";
            List<string> btns = new List<string>();
            string ret = "";
            if (hkd.ctrl) btns.Add("CTRL");
            if (hkd.alt) btns.Add("ALT");
            if (hkd.shift) btns.Add("SHIFT");
            for (int i = 0; i < btns.Count; i++)
            {
                ret += btns[i];
                if (i < btns.Count - 1)
                {
                    ret += " + ";
                }
            }
            return ret;
        }*/

        public static string ToTime(this int timeint)
        {
            if (timeint < 0) return "00:00";
            string ret = "";
            int temp;
            if (timeint >= 3600)
            {
                temp = timeint / 3600;
                if (temp < 10) ret += "0";
                ret += temp + ":";
                timeint -= temp * 3600;
            }
            temp = timeint / 60;
            if (temp < 10) ret += "0";
            ret += temp + ":";
            timeint -= temp * 60;
            if (timeint < 10) ret += "0";
            ret += timeint;
            return ret;
        }
        public static string ToTime(this double timedb)
        {
            return ToTime((int)timedb);
        }

        public static void SetColors(this Control form) {
            if (Program.mainWindow == null) throw new Exception();

            form.BackColor = Program.mainWindow.backgroundColor;
            foreach (Control ctrl in form.Controls)
            {
                if (ctrl is DataGridView)
                {
                    DataGridView dgv = ctrl as DataGridView;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = Program.mainWindow.controlBackColor.Lighten();
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = Program.mainWindow.controlForeColor;
                    dgv.DefaultCellStyle = Program.mainWindow.defaultCellStyle;
                    dgv.AlternatingRowsDefaultCellStyle = Program.mainWindow.alternatingCellStyle;
                    dgv.BackgroundColor = Program.mainWindow.controlBackColor;
                }
                else if (ctrl is TabPage || ctrl is TabControl)
                {
                    ctrl.BackColor = Program.mainWindow.controlBackColor;
                    ctrl.ForeColor = Program.mainWindow.controlForeColor;
                    SetColors(ctrl);
                }
                else if (ctrl is Label)
                {
                    ctrl.BackColor = Program.mainWindow.backgroundColor;
                    ctrl.ForeColor = Program.mainWindow.controlForeColor;
                }
                else
                {
                    ctrl.BackColor = Program.mainWindow.controlBackColor;
                    ctrl.ForeColor = Program.mainWindow.controlForeColor;
                }
            }
        }

        public static string ToHex(this Color clr)
        {
            return clr.R.ToString("X2") + clr.G.ToString("X2") + clr.B.ToString("X2");
        }

        public static Color ColorFromHex(string hex)
        {
            return Color.FromArgb(Convert.ToByte(hex.Substring(0, 2), 16), Convert.ToByte(hex.Substring(2, 2), 16), Convert.ToByte(hex.Substring(4, 2), 16));
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            if (!String.IsNullOrEmpty(text))
            {
                box.SelectionStart = box.TextLength;
                box.SelectionLength = 0;
                box.SelectionColor = color;
                box.AppendText(text);
                box.SelectionColor = box.ForeColor;
            }
        }

        public static String BytesToString(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public static string ToHex(this byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            if (source == null) return false;
            else if (toCheck == null) return true;
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool ContainsCaseInsensitive(this string str, string match)
        {
            return str.ToLower().Contains(match.ToLower());
        }

        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static string ProperTimeFormat(this TimeSpan span)
        {
            string ret = "";
            if (span.Days > 0)
            {
                ret += span.Days + " days, ";
                span.Subtract(new TimeSpan(span.Days, 0, 0, 0));
            }
            if (span.Hours > 0)
            {
                ret += span.Hours + "h ";
                span.Subtract(new TimeSpan(span.Hours, 0, 0));
            }
            if (span.Minutes > 0)
            {
                ret += span.Minutes + "min ";
                span.Subtract(new TimeSpan(0, span.Minutes, 0));
            }
            if (span.Seconds > 0)
            {
                ret += span.Seconds + "sec";
            }
            return ret;
        }
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }
        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }
    }
    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0) return;
                ArrayTraverse walker = new ArrayTraverse(array);
                do action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
