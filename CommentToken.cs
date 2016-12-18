using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentPrefixVSP
{
    [Serializable]
    public class CommentToken
    {
        public CommentToken()
        {

        }

        public CommentToken(string fileType, string token)
        {
            FileType = fileType;
            Token = token;
        }

        public string FileType { get; set; }
        public string Token { get; set; }
    }
}
