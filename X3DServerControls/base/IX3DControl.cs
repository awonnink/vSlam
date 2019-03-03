using System.Collections.Generic;
using System.Text;

namespace SlmControls
{
    public interface IX3DControl
    {
        List<X3DControl> Children { get; set; }
        string TagName { get; set; }

        void AddChild(X3DControl child);
        void AddProperty(string name, object value);
        void Render(StringBuilder sb);
    }
}