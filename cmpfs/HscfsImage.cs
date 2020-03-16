using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmpfs
{
    public class HscfsImage
    {
        public const int MAGIC = 0x10AFECF4;
        public const int VER = 0xA;
        public const int FREE_DTBLOCK_START = 0xFA10C30;
        public const int HDR_START = 0x10AFEC45;

        public List<FSEntity> FileTable;
        public long FilesystemSizeMax { get; set; }
        public string FilesystemLabel { get; set; }
        public CompressionType CmpType { get; set; }
        public bool FSReadOnly { get; set; }

        private FileStream imageStream;
        private BinaryWriter bwriter;
        private BinaryReader breader;

        /// <summary>
        /// Creates a new image
        /// </summary>
        public HscfsImage(string imageName = "filesystem.hscfs")
        {
            FSReadOnly = false;
#if DEBUG
            FSReadOnly = true;
#endif
            CmpType = CompressionType.DEFLATE;
            FilesystemLabel = "Untitled";
            FilesystemSizeMax = 1024 * 1024 * 1024 * 10L; // 10 Gigabytes
            FileTable = new List<FSEntity>();

            imageStream = new FileStream(imageName, FileMode.OpenOrCreate);
            bwriter = new BinaryWriter(imageStream);
            breader = new BinaryReader(imageStream);

            // Load Cmpfs image
            if(imageStream.Length == 0) // Is it empty?
            {
                bwriter.Seek(0, SeekOrigin.Begin);
                bwriter.Write(MAGIC);
                bwriter.Write(VER);
                bwriter.Write(FilesystemSizeMax);
                bwriter.Write(FilesystemLabel);
                bwriter.Write((int)CmpType);
                bwriter.Write(FSReadOnly);
                bwriter.Flush();
            } else
            {
                if (imageStream.Length < 21)
                    throw new Exception("Invalid image header.");
                if (breader.ReadInt32() != MAGIC)
                    throw new Exception("File is not an image.");
                if (breader.ReadInt32() > VER)
                    throw new Exception("Filesystem present on image is too new.");

                FilesystemSizeMax = breader.ReadInt64();
                FilesystemLabel = breader.ReadString();
                CmpType = (CompressionType)breader.ReadInt32();
                FSReadOnly = breader.ReadBoolean();
                FileTable = new List<FSEntity>();

                //TODO read file table
                if (breader.PeekChar() == -1)
                {
                    // No file data to read
                    return;
                }
                
                while(breader.PeekChar() != -1)
                {
                    int hdrStartCode = breader.ReadInt32();
                    if(hdrStartCode == FREE_DTBLOCK_START)
                    {
                        long lengthToAdvance = breader.ReadInt64();
                        imageStream.Seek(lengthToAdvance, SeekOrigin.Current);
                        continue;
                    }
                    if (hdrStartCode != HDR_START)
                        continue;
                    FSEntity e = new FSEntity();
                    e.Attributes = (FileAttributes)breader.ReadInt32();
                    e.SecurityAttributes = (FileAccess)breader.ReadInt32();
                    e.DateAccessed = DateTime.FromBinary(breader.ReadInt64());
                    e.DateCreated = DateTime.FromBinary(breader.ReadInt64());
                    e.DateModified = DateTime.FromBinary(breader.ReadInt64());
                    e.Type = (FSEntityType)breader.ReadInt32();
                    e.Path = breader.ReadString();
                    e.Tag = breader.ReadString();
                    e.Length = breader.ReadInt64();
                    imageStream.Seek(e.Length, SeekOrigin.Current);
                }
            }
        }

        public long GetFSUsage()
        {
            long sz = 12 + (FilesystemLabel.Length * 2); // Include FileSystemSize, compression type, and FS label into total FS length
            foreach (var ent in FileTable)
                sz += ent.GetTotalLength();
            return sz;
        }
    }
}
