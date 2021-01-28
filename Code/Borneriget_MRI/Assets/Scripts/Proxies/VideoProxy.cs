using PureMVC.Patterns.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borneriget.MRI
{
    public class VideoProxy : Proxy
    {
        public new static string NAME = "VideoProxy";

        public VideoProxy(VideoUrls urls) : base(NAME, urls)
        {
        }
    }
}
