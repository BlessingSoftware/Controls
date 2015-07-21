using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BlessingSoftware.Utils
{
	public class BufferedBytes:IDisposable
	{
		public const int DEFAULT_Capacity = 0x40000;
		
		/// <summary>
		/// 
		/// </summary>
		internal int capacity;
		public int Capacity{
			get
			{
				return capacity;
			}
			set{
				if (capacity!=value) {
					ChangeCapacity(value);
					capacity = value;
				}
			}
		}
		
		/// <summary>
		/// 缓冲区长度
		/// </summary>
		internal int length;
		
		/// <summary>
		/// 缓冲区长度
		/// </summary>
		public int Length{get{return length;}}

		/// <summary>
		/// 指示是否已缓冲到了流的末端
		/// </summary>
		internal bool endOfStream;
		/// <summary>
		/// 指示是否已缓冲到了流的末端
		/// </summary>
		public bool EndOfSrteam{get{return endOfStream;}}
		
		/// <summary>
		/// 缓冲区与基础流的偏移量
		/// </summary>
		internal int offset;
//
//		public int Offset{
//			get{return offset;}
//			set{offset=value;}
//		}
		
		byte[] buffer;
		
		internal Stream baseSrteam;
		public Stream BaseStream{get{return baseSrteam;}}
		
		public BufferedBytes():this(DEFAULT_Capacity)
		{
			
		}
		
		public BufferedBytes(int capacity):this(capacity,null)
		{
		}
		
		public BufferedBytes(Stream stream):this(DEFAULT_Capacity,stream)
		{
		}
		
		public BufferedBytes(int capacity, Stream baseSrteam)
		{
			this.capacity = capacity;
			this.buffer = new byte[capacity];
			this.baseSrteam = null;
			this.ChangeStream(baseSrteam);
		}
		
		public void ChangeStream(Stream stream){
            if (stream ==this.baseSrteam)        
                return;
            
			if (baseSrteam != null) {
				baseSrteam.Dispose();
			}
			this.baseSrteam=stream;
			CacheBuffer(0);
		}
		
		public IEnumerable<byte> GetBytesLine(int lineoffset,int count = 0x10){
			if (baseSrteam ==null) {
				yield break;
			}
			int index =lineoffset << 4;
			
			index -=this.offset;
			EnsureBuffer(index);
			for (int i = 0; i < count; i++) {
				if (index >= length)
					yield break;
				yield return buffer[index++];
				
			}
			yield break;
		}
		
		void CacheBuffer(int offset){
			this.offset = offset;
			this.length = 0;
			this.endOfStream = true;
			if (baseSrteam!=null) {
				if (!baseSrteam.CanRead) {
					throw new ArgumentException("StreamNotReadable");
				}
				if (baseSrteam.CanSeek)
					baseSrteam.Seek(0L,SeekOrigin.Begin);
				int k = 1,j = 0x400;
//				endOfStream = true;
				while (k > 0) {
					k = baseSrteam.Read(buffer,offset,j);
					offset=this.length +=k;
					if (this.length == capacity) {
						break;
					}
					if ((j+length)>capacity)
						j=capacity-length;
				}
				this.endOfStream = (k==0);
			}			
		}
		
		void EnsureBuffer(int index){
			if ((index + 0x40) > length) {
				int off=index - capacity / 2;
				if (off<0) 
					off=0;				
				CacheBuffer(off);				            
			}
		}
		
		void ChangeCapacity(int newVal){
			byte[] buffer2= new byte[newVal];
			int len = (newVal < capacity)?newVal:this.length;
			Array.Copy(buffer,buffer2,len);
			this.buffer = buffer2;
		}
		
		public void Dispose()
		{
			buffer = null;
			if (baseSrteam!=null) {
				baseSrteam.Dispose();
			}
			capacity = 0;
			length = 0;
		}
	}
}
