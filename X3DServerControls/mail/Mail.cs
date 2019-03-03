using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmControls.mail
{
    public static class Mail
    {
        public static List<MailData> MailDataSet()
        {
            List<MailData> maildata = new List<MailData>();
            maildata.Add(new MailData("john@mira.nl", "mike@oldfield.nl", DateTime.Now.AddDays(-7), 
                "Vis everti animal tamquam ei, et hendrerit contentiones sea",
                @"Et hinc vide tibique duo, diam epicuri percipit sit 
at, duo semper adipisci voluptatum ex. Nec dicam dictas 
euismod id, esse deleniti reprehendunt nam at. Ceteros 
interesset ne mel. Ut quo aliquid complectitur. Dicat evertitur 
philosophia eu quo, electram moderatius his te, 
ut meliore appareat vix."));
            maildata.Add(new MailData("randel.over@mct.ir", "tube@labels.vr", DateTime.Now.AddDays(-9),
                "An omnes alienum facilisi eos",
                @"Et cum hinc everti. Cu amet graece apeirian vim. 
Sea vidit dolorem ex, solet doctus ius et. Et mel everti principes, 
eros indoctum ut sit, principes moderatius ad eam. Mutat nonumes voluptua 
ius an. Mei tation ancillae fabellas ei. Wisi saepe propriae cu est, 
in facer dicunt sea. No augue copiosae quo, an pro legere tincidunt. 
Ad mundi eirmod adipisci vim, ipsum harum scriptorem vel eu. 
Prima prompta omnesque eu pro. Mazim epicuri duo eu.")); return maildata;
        }
        public static X3DTransform GetMailStructure(MailData mailData)
        {
            Vector3 labelColor = new Vector3(0.7, 0.7, 1);
            Vector3 dataColor = new Vector3(0.7, 0.9, 0.9);
            Vector3 fontColor = new Vector3(0,0,0);
            X3DTransform p = new X3DTransform();
            X3DTransform mb = X3DTransform.AddTransFormWithShape(ShapeType.Cube);
            p.AddChild(mb);
            mb.Scale = new Vector3(1, 1.3, 0.001);
            mb.Shape.Appearance.Material.DiffuseColor = new Vector3(0.6, 0.7, 0.9);
            var ypos = 0.58;
            var rh = -0.1;
            //from
            var labelFrom = GetLabel("From", labelColor, LabelSize.small, fontColor);
            p.AddChild(labelFrom);
            labelFrom.Translation = new Vector3(-0.37, ypos, -0.007);
            var dataFrom = GetLabel(mailData.From, dataColor, LabelSize.large, fontColor);
            p.AddChild(dataFrom);
            dataFrom.Translation = new Vector3(0.1, ypos, -0.007);
            //to
            ypos += rh;
            var labelTo = GetLabel("To", labelColor, LabelSize.small, fontColor);
            p.AddChild(labelTo);
            labelTo.Translation = new Vector3(-0.37, ypos, -0.007);
            var dataTo = GetLabel(mailData.To, dataColor, LabelSize.large, fontColor);
            p.AddChild(dataTo);
            dataTo.Translation = new Vector3(0.1, ypos, -0.007);
            //sendDate
            ypos += rh;
            var labelSend = GetLabel("Send", labelColor, LabelSize.small, fontColor);
            p.AddChild(labelSend);
            labelSend.Translation = new Vector3(-0.37, ypos, -0.007);
            var dataSend = GetLabel(mailData.SendDate.ToString("yyyy/MM/dd hh:mm"), dataColor, LabelSize.large, fontColor);
            p.AddChild(dataSend);
            dataSend.Translation = new Vector3(0.1, ypos, -0.007);
            //subject
            ypos += rh;
            var labelSubject = GetLabel("Subject:", labelColor, LabelSize.small, fontColor);
            p.AddChild(labelSubject);
            labelSubject.Translation = new Vector3(-0.37, ypos, -0.007);
            ypos += rh;
            var dataSubject = GetLabel(mailData.Subject, dataColor, LabelSize.xLarge, fontColor);
            p.AddChild(dataSubject);
            dataSubject.Translation = new Vector3(-0.01, ypos, -0.007);
            ypos += rh;
            var dataBody = GetLabel(mailData.Body, dataColor, LabelSize.xxLarge, fontColor);
            p.AddChild(dataBody);
            dataBody.Translation = new Vector3(-0.01, -0.25, -0.007);



            return p;
        }
        static X3DTransform GetLabel(string text, Vector3 backColor, LabelSize labelSize, Vector3 fontColor)
        {
            X3DTransform p = new X3DTransform();
            X3DTransform mb = X3DTransform.AddTransFormWithShape(ShapeType.Cube);
            p.AddChild(mb);
            var xScale = 0.2;
            var yScale = 0.06;
            var txtTrans = 0.12;
            var rectLength = 20;
            switch (labelSize)
            {
                case LabelSize.large:
                    xScale = 0.7;
                    txtTrans = -0.13;
                    break;
                case LabelSize.xLarge:
                    xScale = 0.92;
                    txtTrans = 0;
                    rectLength = 44;
                    yScale = 0.1;
                    break;
                case LabelSize.xxLarge:
                    xScale = 0.92;
                    txtTrans = 0;
                    rectLength = 44;
                    yScale = 0.71;
                    break;
            }
            mb.Scale = new Vector3(xScale, yScale, 0.001);
            mb.Shape.Appearance.Material.DiffuseColor = backColor;
            var labeltxt = X3DTransform.AddTransFormWithShape(ShapeType.Text);
            p.AddChild(labeltxt);
            labeltxt.Translation = new Vector3(txtTrans, 0, -0.012);
            labeltxt.Scale = new Vector3(0.02);
            labeltxt.Shape.Text = text;
            labeltxt.Shape.FontStyle.Justify = Justify.LEFT;
            labeltxt.Shape.RectHeight = 1;
            labeltxt.Shape.RectLength = rectLength;
            labeltxt.Shape.Appearance.Material.DiffuseColor = fontColor;
            //labeltxt.Shape.FontStyle.
            return p;

        }
    }
    public enum LabelSize
    {
        small,
        large,
        xLarge,
        xxLarge
    }
}
