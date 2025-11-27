using System;
using System.IO;
using UnityEngine;

namespace ES3Internal
{
    internal class ES3PlayerPrefsStream : MemoryStream
    {
        private readonly bool append;
        private bool isDisposed;
        private readonly bool isWriteStream;
        private readonly string path;

        // This constructor should be used for read streams only.
        public ES3PlayerPrefsStream(string path) : base(GetData(path, false))
        {
            this.path = path;
            append = false;
        }

        // This constructor should be used for write streams only.
        public ES3PlayerPrefsStream(string path, int bufferSize, bool append = false) : base(bufferSize)
        {
            this.path = path;
            this.append = append;
            isWriteStream = true;
        }

        private static byte[] GetData(string path, bool isWriteStream)
        {
            if (!PlayerPrefs.HasKey(path))
                throw new FileNotFoundException("File \"" + path + "\" could not be found in PlayerPrefs");
            return Convert.FromBase64String(PlayerPrefs.GetString(path));
        }

        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
                return;
            isDisposed = true;
            if (isWriteStream && Length > 0)
            {
                if (append)
                {
                    // Convert data back to bytes before appending, as appending Base-64 strings directly can corrupt the data.
                    var sourceBytes = Convert.FromBase64String(PlayerPrefs.GetString(path));
                    var appendBytes = ToArray();
                    var finalBytes = new byte[sourceBytes.Length + appendBytes.Length];
                    Buffer.BlockCopy(sourceBytes, 0, finalBytes, 0, sourceBytes.Length);
                    Buffer.BlockCopy(appendBytes, 0, finalBytes, sourceBytes.Length, appendBytes.Length);

                    PlayerPrefs.SetString(path, Convert.ToBase64String(finalBytes));

                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.SetString(path + ES3IO.temporaryFileSuffix, Convert.ToBase64String(ToArray()));
                }

                // Save the timestamp to a separate key.
                PlayerPrefs.SetString("timestamp_" + path, DateTime.UtcNow.Ticks.ToString());
            }

            base.Dispose(disposing);
        }
    }
}