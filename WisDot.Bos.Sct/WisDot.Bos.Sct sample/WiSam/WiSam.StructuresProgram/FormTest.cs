using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using Wisdot.Bos.WiSam.Core.Domain.Services;
using System.Web.Mvc;

namespace WiSam.StructuresProgram
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }
        /*
         * GMapControl map = (GMapControl)sender;

            if (map.IsMouseOverMarker)
            {
                bool proceed = true;
                WorkConcept wc = null;

                for (int i = startYear; i <= endYear; i++)
                {
                    var o = map.Overlays.Where(el => el.Id.Equals(i.ToString())).First();

                    foreach (var m in o.Markers)
                    {
                        if (m.IsMouseOver && m.Tag is WorkConcept)
                        {
                            wc = (WorkConcept)m.Tag;
                            proceed = false;
                            break;
                        }
                    }

                    if (!proceed)
                    {
                        break;
                    }
                }
            }*/

        private void gMap_Load(object sender, EventArgs e)
        {
            /*
            gMap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMap.Position = new GMap.NET.PointLatLng(8.7832, -124.5085);
            gMap.ShowCenter = true;

            GMapOverlay markers = new GMapOverlay("markers");
            GMapMarker marker = new GMarkerGoogle(
                new PointLatLng(8.5000, -123.5000),
                GMarkerGoogleType.white_small);
            marker.ToolTipText = "Elig";
            marker.Tag = "my tag";
            marker.ToolTipMode = MarkerTooltipMode.Always;
            markers.Markers.Add(marker);

            marker = new GMarkerGoogle(
                new PointLatLng(8.5000, -123.3000),
                GMarkerGoogleType.yellow_small);
            marker.ToolTipText = "QuasiC";
            marker.Tag = "my tag";
            marker.ToolTipMode = MarkerTooltipMode.Always;
            markers.Markers.Add(marker);


            gMap.Overlays.Add(markers);*/
        }

        private void gMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //var me = gMap.Overlays[0].Markers[0].Tag;
            GMapControl map = (GMapControl)sender;
            bool foundMatch = false;

            if (map.IsMouseOverMarker)
            {
                foreach (var o in map.Overlays)
                {
                    foreach (var m in o.Markers)
                    {
                        if (m.IsMouseOver)
                        {
                            MessageBox.Show(m.ToolTipText);
                            break;
                        }
                    }
                }
                /*
                double lat = map.Position.Lat;
                double lng = map.Position.Lng;

                foreach (var o in map.Overlays)
                {
                    foreach (var m in o.Markers)
                    {
                        var latDiff = Math.Abs(m.Position.Lat - lat);
                        var lngDiff = Math.Abs(m.Position.Lng - lng);
                        if (m.Tag.Equals("my tag") && latDiff <= 0.5
                            && lngDiff <= 1.5)
                        {
                            foundMatch = true;
                        }
                    }
                }*/
    }

    MessageBox.Show(foundMatch.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IISTestService serv = new IISTestService();
            serv.GetStandardPlan("32", "-20", "28", "42SS", "yes", "5", "HP", "", "", "");
        }
    }
}
