using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slam
{
    public interface IPlayerActivatedArgs
    {

    }
    public class PlayerActivatedVoice : IPlayerActivatedArgs
    {
        public bool Interrupt { get; set; }
        public string Text { get; set; }
        public string Voice { get; set; }
    }
}
