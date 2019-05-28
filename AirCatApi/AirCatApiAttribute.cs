using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiSdk
{
    public class ApiCommentAttribute : Attribute
    {
        private string apiComment;
        public string ApiComment { get => apiComment; set => apiComment = value; }
        public ApiCommentAttribute(string v)
        {
            this.ApiComment = v;
        }
    }
}
