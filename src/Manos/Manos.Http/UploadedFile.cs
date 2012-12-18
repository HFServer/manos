//
// Copyright (C) 2010 Jackson Harper (jackson@manosdemono.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//

//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Manos.Http {

	public interface IUploadedFileCreator {

		UploadedFile Create (string name, string fileRef, long? length);
	}

	public class TempFileUploadedFileCreator : IUploadedFileCreator {

		public UploadedFile Create (string name, string fileRef, long? length)
		{
			string temp_file = Path.GetTempFileName ();
			return new TempFileUploadedFile (name, temp_file, fileRef, length);
		}
	}

	public class InMemoryUploadedFileCreator : IUploadedFileCreator {

		public UploadedFile Create (string name, string fileRef, long? length)
		{
			return new InMemoryUploadedFile (name, fileRef, length);
		}
	}
	
	public abstract class UploadedFile : IDisposable {

	  	 public UploadedFile (string name, string fileRef, long? length)
		 {
			Length = length;
			FileRef = fileRef;
			Name = name;
		 }

		 public abstract string TempFile {
		 	get;
			protected set;
		 }
		
		~UploadedFile ()
		{
			Dispose ();
		}

		public string Name {
		 	get;
			private set;
		}
		
		public string FileRef {
		 	get;
			private set;
		 }

		public abstract void Dispose ();

		public string ContentType {
		 	get;
			set;
		 }

		public long? Length {
		 	get;
			private set;
		}
		
		 public abstract long CurrentLength {
		 	get;
		 }

		 public abstract Stream Contents {
		 	get;
		 }

		public virtual void Finish ()
		{
		}
	  }

	  public class InMemoryUploadedFile : UploadedFile {

		  private MemoryStream stream = new MemoryStream ();

		 public override string TempFile {
		 	get {
				throw new NotImplementedException ("InMemoryUploadedFile is not backed by a temp file.");
			}
			protected set {
				throw new NotImplementedException ("InMemoryUploadedFile is not backed by a temp file.");
			}
		 }

	  	 public InMemoryUploadedFile (string name, string fileRef, long? length) : base (name, fileRef, length)
		 {
		 }
		 
		 public override long CurrentLength {
		 	get {
				return stream.Length;
			}
		 }

		 public override void Dispose () {
			if (Contents != null)
				Contents.Close ();
		 }

		 public override Stream Contents {
		 	get {
				return stream;
			}
		 }

		  public override void Finish ()
		  {
			  stream.Position = 0;
		  }
	  }

	  public class TempFileUploadedFile : UploadedFile {

		  FileStream stream;
		  
	  	 public TempFileUploadedFile (string name, string temp_file, string fileRef, long? length) : base (name, fileRef, length)
		 {
			TempFile = temp_file;
		 }
	  	 
		 public override string TempFile {
		 	get;
			protected set;
		 }

		 public override long CurrentLength {
		 	get {
			    FileInfo f = new FileInfo (TempFile);
			    return f.Length;
			}
		 }

		 public override Stream Contents {
		 	get {
				if (stream == null)
					stream = File.Open (TempFile, FileMode.Open, FileAccess.ReadWrite);
				return stream;
			}
		 }
		 public override void Dispose () {
			if (stream != null) {
				stream = null;
				stream.Close ();
			}
		 }

		 public override void Finish () {
			 stream.Flush ();
			 if (stream != null) {
			 	stream.Close ();
				stream = null;
			 }
		 }
	  }
}

