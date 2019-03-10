using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SlamSiteBase;

public partial class Communicator : SlamSiteBase.Communicator
{
    public override PostI UpdateCommunityInfo(string room, string text, string nickName, string lastMessageGuid, int pageNo)
    {
        return null;
    }
}