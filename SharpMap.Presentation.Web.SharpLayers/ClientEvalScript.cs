using System;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace SharpMap.Presentation.Web.SharpLayers
{
    [Serializable]
    [DefaultProperty("Script")]
    public class ClientEvalScript
    {
        private string clientScript;

        [ClientPropertyName("builderAction")]
        [ExtenderControlProperty]
        public string BuilderAction
        {
            get { return "evaluate"; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        [ClientPropertyName("script")]
        [ExtenderControlProperty]
        public string ClientScript
        {
            get { return clientScript; }
            set { clientScript = value.Trim(); }
        }
    }
}