using System.IO;

namespace BlessingSoftware.Controls
{
    /// <summary>
    /// 流的处理引擎
    /// </summary>
    public static class StreamEngine
    {
        /// <summary>
        /// 删除流中的一块数据 。
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="position">起始位置</param>
        /// <param name="length">删除的长度</param>
        /// <returns>返回删除是否成功</returns>
        public static bool Delete(Stream stream, int position, int length)
        {
            if (stream == null || position < 0 || length <= 0) return false;
            if (position + length >= stream.Length)
                stream.SetLength(position);
            else
            {
                byte[] vBuffer = new byte[0x1000];
                int i = position;
                int l = 0;
                do
                {
                    stream.Position = i + length;
                    l = stream.Read(vBuffer, 0, vBuffer.Length);
                    stream.Position = i;
                    stream.Write(vBuffer, 0, l);
                    i += l;
                }
                while (l >= vBuffer.Length);
                stream.SetLength(stream.Length - length);
            }
            return true;
        }
        /// <summary>
        /// 处理流之间的复制。
        /// </summary>
        /// <param name="streamFrom">来源流</param>
        /// <param name="streamTo">目标流</param>
        /// <returns>返回复制是否成功</returns>
        public static bool Copy(Stream streamFrom, Stream streamTo)
        {
            if (streamFrom == null || streamTo == null) return false;
            byte[] vBuffer = new byte[0x1000];
            int l;
            while ((l = streamFrom.Read(vBuffer, 0, vBuffer.Length)) > 0)
                streamTo.Write(vBuffer, 0, l);
            return true;
        }
        /// <summary>
        /// 在流的指定位置插入一段数据。
        /// </summary>
        /// <param name="stream">目标流</param>
        /// <param name="position">插入位置</param>
        /// <param name="buffer">数据</param>
        /// <param name="count">数据大小</param>
        /// <returns>返回插入数据是否成功</returns>
        public static bool Insert(Stream stream, int position, byte[] data)
        {
            if (stream == null || data.Length <= 0 || position < 0 ||
                position > stream.Length)
                return false;
            int i = (int)stream.Length;
            byte[] vBuffer = new byte[0x1000];
            stream.SetLength(i + data.Length);
            int l;
            do
            {
                l = position + data.Length <= i - vBuffer.Length ?
                    vBuffer.Length : i - position;
                stream.Position = i - l;
                stream.Read(vBuffer, 0, l);
                stream.Position = i - l + data.Length;
                stream.Write(vBuffer, 0, l);
                i -= l - data.Length;
            } while (l >= vBuffer.Length);
            stream.Position = position;
            stream.Write(data, 0, data.Length);
            return true;
        }
    }

}
