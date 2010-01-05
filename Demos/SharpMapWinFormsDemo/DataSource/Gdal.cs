#region License

/*
 *  The attached / following is part of MapViewer.
 *  
 *  MapViewer is free software © 2009 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/.
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2009
 *  
 */

#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.GdalPreview;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace MapViewer.DataSource
{
    public partial class GdalMask : UserControl, ICreateRasterProvider
    {

        private static readonly List<Type> _gdalPreviewClasses;

        static GdalMask()
        {
            
            List<String> testedAssemblies = new List<string>();
            _gdalPreviewClasses = new List<Type>();

            IEnumerable<Assembly> cdAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in cdAssemblies)
            {
                GetGdalPreviewClasses(assembly);
                testedAssemblies.Add(assembly.FullName);
            }

            foreach (Assembly assembly in cdAssemblies)
            {
                foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
                    TestReferencedAssembly(assemblyName, testedAssemblies);
            }
            //Plugins folder?

        }

        private static void TestReferencedAssembly(AssemblyName assemblyName, ICollection<String> testedAssemblies)
        {
            if (testedAssemblies.Contains(assemblyName.FullName))
                return;

            try
            {
                Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName);
                GetGdalPreviewClasses(assembly);
                testedAssemblies.Add(assembly.FullName);

                foreach (AssemblyName refAssemblyName in assembly.GetReferencedAssemblies())
                    TestReferencedAssembly(refAssemblyName, testedAssemblies);
            }
            catch
            { }

        }

        private static void GetGdalPreviewClasses(Assembly assembly)
        {
            foreach (Type type in  assembly.GetExportedTypes())
            {
                if (type.IsClass && 
                    !type.IsAbstract &&
                    typeof(BaseGdalPreview).IsAssignableFrom(type))
                {
                    _gdalPreviewClasses.Add(type);
                }
            }
        }

        public GdalMask()
        {
            InitializeComponent();
            foreach (Type gdalPreviewClass in _gdalPreviewClasses)
                cboPreviewGenerator.Items.Add(gdalPreviewClass.Name);
            cboPreviewGenerator.SelectedIndex = 0;

            ofd.CheckFileExists = true;
            ofd.Filter = GdalRasterProvider.SupportedFileFormats();
            ofd.RestoreDirectory = true;
            ofd.Multiselect = true;
        }

        private void bBrowse_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    ListViewItem lvi = lvFiles.Items.Add(Path.GetFileName(fileName));
                    lvi.Tag = fileName;
                }
            }
        }

        #region ICreateRasterProvider Members

        public IEnumerable<IRasterProvider> GetRasterProviders()
        {
            GeometryServices gs = new GeometryServices();

            foreach (ListViewItem item in lvFiles.Items)
            {
                BaseGdalPreview pg = (BaseGdalPreview)Activator.CreateInstance(_gdalPreviewClasses[cboPreviewGenerator.SelectedIndex]);
                GdalRasterProvider provider = new GdalRasterProvider((String)item.Tag, pg, gs.DefaultGeometryFactory,
                                                    gs.CoordinateSystemFactory, gs.CoordinateTransformationFactory);
                if (chkTransparentColor.Checked)
                {
                    Color c = btnColor.BackColor;
                    provider.TransparentColor = StyleColor.FromBgra(c.B, c.G, c.R, c.A);
                }
                yield return provider;
            }
        }

        public string ProviderName
        {
            get { return "GdalForSharpMap"; }
        }

        #endregion

        private void btnColor_Click(object sender, EventArgs e)
        {
            cd.Color = btnColor.BackColor;
            if (cd.ShowDialog() == DialogResult.OK)
                btnColor.BackColor = cd.Color;
        }

        private void chkTransparentColor_CheckedChanged(object sender, EventArgs e)
        {
            btnColor.Enabled = chkTransparentColor.Checked;
        }

        private void GdalMask_Load(object sender, EventArgs e)
        {

        }
    }
}