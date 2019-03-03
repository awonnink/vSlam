using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls
{
    public class X3DPage:X3DControl
    {
        public X3DPage()
        {
            //<X3D profile='Immersive' version='3.3' xmlns:xsd='http://www.w3.org/2001/XMLSchema-instance' xsd:noNamespaceSchemaLocation='http://www.web3d.org/specifications/x3d-3.3.xsd' xmlns:slm='http://www.v-slam.org'>
            Head = new X3DHead();
            AddChild(Head);
            TagName = "X3D";
            Profile = "Immersive";
            Version = "3.3";
            Scene = new X3DScene();
            ViewPoint = new X3DViewPoint();
            Scene.AddChild(ViewPoint);
            AddChild(Scene);

        }
        public override void Render(StringBuilder sb)
        {
            AddProperty("profile", Profile);
            AddProperty("version", Profile);
            AddProperty("xmlns:xsd", "http://www.w3.org/2001/XMLSchema-instance");
            AddProperty("xsd:noNamespaceSchemaLocation", "http://www.web3d.org/specifications/x3d-3.3.xsd");
            AddProperty("xmlns:slm", "http://www.v-slam.org");
            sb.Append("<?xml version='1.0' encoding='utf-8'?>\r\n");
            base.Render(sb);
        }
        public X3DHead Head { get; set; }
        public X3DScene Scene;
        public X3DViewPoint ViewPoint;
        public string Profile { get; set; }
        public string Version { get; set; }
    }
}
