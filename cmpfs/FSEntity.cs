using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmpfs
{
    public struct FSEntity
    {
        public string Path { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateAccessed { get; set; } 
        public DateTime DateModified { get; set; }
        public FSEntityType Type { get; set; }
        public string Tag { get; set; }
        public FileAttributes Attributes { get; set; }
        public FileAccess SecurityAttributes { get; set; }
        public long Length { get; set; }

        public long GetMetaLength()
        {
            return (Path.Length * 2) + 40 + 4 + (Tag.Length * 2);
        }

        public long GetTotalLength()
        {
            return GetMetaLength() + Length;
        }
    }
}
