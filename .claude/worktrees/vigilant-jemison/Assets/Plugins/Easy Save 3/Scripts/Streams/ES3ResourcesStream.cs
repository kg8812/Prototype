using System.IO;
using UnityEngine;

namespace ES3Internal
{
    internal class ES3ResourcesStream : MemoryStream
    {
        // Used when creating 
        public ES3ResourcesStream(string path) : base(GetData(path))
        {
        }

        // Check that data exists by checking stream is not empty.
        public bool Exists => Length > 0;

        private static byte[] GetData(string path)
        {
            var textAsset = Resources.Load(path) as TextAsset;

            // If data doesn't exist in Resources, return an empty byte array.
            if (textAsset == null)
                return new byte[0];

            return textAsset.bytes;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}