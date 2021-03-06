// C# ExcelWriter class v1.0
// by Serhiy Perevoznyk, 2008

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace appel
{
    /// <summary>
    /// Produces Excel file without using Excel
    /// </summary>
    public class ExcelWriter
    {
        public static void demo() {
            FileStream stream = new FileStream("demo.xls", FileMode.OpenOrCreate);
            ExcelWriter writer = new ExcelWriter(stream);
            writer.BeginWrite();
            writer.WriteCell(0, 0, "ExcelWriter Demo Tiếng Việt - Việt Nam");
            writer.WriteCell(1, 0, "int");
            writer.WriteCell(1, 1, 10);
            writer.WriteCell(2, 0, "double");
            writer.WriteCell(2, 1, 1.5);
            writer.WriteCell(3, 0, "empty");
            writer.WriteCell(3, 1);
            writer.EndWrite();
            stream.Close();
        } 



        private Stream stream;
        private BinaryWriter writer;

        private ushort[] clBegin = { 0x0809, 8, 0, 0x10, 0, 0 };
        private ushort[] clEnd = { 0x0A, 00 };


        private void WriteUshortArray(ushort[] value)
        {
            for (int i = 0; i < value.Length; i++)
                writer.Write(value[i]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public ExcelWriter(Stream stream)
        {
            this.stream = stream;
            writer = new BinaryWriter(stream);
        }

        /// <summary>
        /// Writes the text cell value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="value">The string value.</param>
        public void WriteCell(int row, int col, string value)
        {
            ushort[] clData = { 0x0204, 0, 0, 0, 0, 0 };
            int iLen = value.Length;
            byte[] plainText = Encoding.ASCII.GetBytes(value);
            //byte[] plainText = Encoding.Unicode.GetBytes(value);
            clData[1] = (ushort)(8 + iLen);
            clData[2] = (ushort)row;
            clData[3] = (ushort)col;
            clData[5] = (ushort)iLen;
            WriteUshortArray(clData);
            writer.Write(plainText);
        }

        /// <summary>
        /// Writes the integer cell value.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        /// <param name="value">The value.</param>
        public void WriteCell(int row, int col, int value)
        {
            ushort[] clData = { 0x027E, 10, 0, 0, 0 };
            clData[2] = (ushort)row;
            clData[3] = (ushort)col;
            WriteUshortArray(clData);
            int iValue = (value << 2) | 2;
            writer.Write(iValue);
        }

        /// <summary>
        /// Writes the double cell value.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        /// <param name="value">The value.</param>
        public void WriteCell(int row, int col, double value)
        {
            ushort[] clData = { 0x0203, 14, 0, 0, 0 };
            clData[2] = (ushort)row;
            clData[3] = (ushort)col;
            WriteUshortArray(clData);
            writer.Write(value);
        }

        /// <summary>
        /// Writes the empty cell.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        public void WriteCell(int row, int col)
        {
            ushort[] clData = { 0x0201, 6, 0, 0, 0x17 };
            clData[2] = (ushort)row;
            clData[3] = (ushort)col;
            WriteUshortArray(clData);
        }

        /// <summary>
        /// Must be called once for creating XLS file header
        /// </summary>
        public void BeginWrite()
        {
            WriteUshortArray(clBegin);
        }

        /// <summary>
        /// Ends the writing operation, but do not close the stream
        /// </summary>
        public void EndWrite()
        {
            WriteUshortArray(clEnd);
            writer.Flush();
        }
    }
}
