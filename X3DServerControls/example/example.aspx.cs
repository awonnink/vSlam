using System;
using System.Web.UI;
using SlmControls;

public partial class example : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        X3DPage x3dPage = new X3DPage();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
 
        X3DGroup group = new X3DGroup();
        x3dPage.Scene.AddChild(group);
        X3DTransform parentT = new X3DTransform();
        group.AddChild(parentT);
        X3DTransform worldT = X3DTransform.AddTransFormWithShape(ShapeType.Sphere);
        parentT.AddChild(worldT);
        worldT.Translation = new Vector3(0, 1, 3);
        worldT.Scale = Vector3.One(0.3);
        worldT.Shape.Appearance.Material.DEF = "MaterialLightBlue";
        worldT.Shape.Appearance.Material.DiffuseColor = new Vector3(0.1, 0.5, 1);
        worldT.Shape.Appearance.Material.EmissiveColor = new Vector3(0, 0, 0.2);
        worldT.Shape.Appearance.ImageTexture.Url = "earth-topo.png";
        worldT.Shape.Appearance.Movement.Rotate = new Vector3(0, -1, 0);

        //Render page
        x3dPage.Render(sb);
        Response.Clear();
        Response.ContentType = "text/xml";
        Response.Write(sb.ToString());
        Response.End();
    }
}