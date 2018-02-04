using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace Lavie.Infrastructure
{
    public class ResponseFilter : Stream
    {
        private readonly Stream _responseStream;
        private readonly IList<ResponseInsert> _inserts;
        private readonly HttpContextBase _context;
        private List<byte> _buffer;

        public ResponseFilter(Stream responseStream, HttpContextBase context)
        {
            this._responseStream = responseStream;
            this._inserts = new List<ResponseInsert>(5);
            this._context = context;
        }

        public IList<ResponseInsert> Inserts
        {
            get { return _inserts; }
        }

        public override bool CanRead
        {
            get { return _responseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _responseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _responseStream.CanWrite; }
        }

        public override void Flush()
        {
            if (Inserts.Count > 0 && !(_context.AllErrors != null && _context.AllErrors.Length > 0))
            {
                string doc = Encoding.UTF8.GetString(_buffer.ToArray(), 0, _buffer.Count);
                bool modifiedDoc = false;

                foreach (ResponseInsert insert in Inserts)
                    insert.Apply(ref doc, ref modifiedDoc);

                byte[] docBytes = Encoding.UTF8.GetBytes(doc);
                _responseStream.Write(docBytes, 0, docBytes.Length);
            }
            else if (_buffer != null)
            {
                if (_buffer != null)
                    _responseStream.Write(_buffer.ToArray(), 0, _buffer.Count);
            }

            _responseStream.Flush();
        }

        public override long Length
        {
            get { return _responseStream.Length; }
        }

        public override long Position
        {
            get { return _responseStream.Position; }
            set { _responseStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _responseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _responseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _responseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this._buffer == null)
                this._buffer = new List<byte>();

            this._buffer.AddRange(buffer);
        }
    }
}