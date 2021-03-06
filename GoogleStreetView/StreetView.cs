﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace GoogleStreetView
{
    internal class StreetView : MapTool
    {

        public static void OpenStreetView(double lat, double lon)
        {
            // Concat geometery (casts as strings) to the url.
            string url = $"https://www.google.com/maps/@?&api=1&map_action=pano&viewpoint={lat.ToString()},{lon.ToString()}";

            // The *Process.Start()* method (from the System.Diagnostics namespace) opens the system's default browser.
            // The method recognizes the string variable as a url/link and opens a broswer window. 
            // No need to specifiy the browser .exe (Easy peasey).
            Process.Start(url);


        }
        public StreetView()
        {
            IsSketchTool = true;
            SketchType = SketchGeometryType.Point;
            SketchOutputMode = SketchOutputMode.Map;
        }

        protected override Task OnToolActivateAsync(bool active)
        {
            return base.OnToolActivateAsync(active);
        }

        // Method returns a  bool Task.  It's a part of the StreetView class that was created with this tool.
        // Async methods called on the UI thread but run on the Main CIM Thread.
        protected override Task<bool> OnSketchCompleteAsync(Geometry geometry)
        {
            // All the Synchronous methods within the scope of *OnSketchCompleteAsync*
            // need to be wrapped in a *QueuedTask* to call and run on the Main CIM Thread (not the UI).
            // QueuedTasks are used to manage threads.
            return QueuedTask.Run(() =>
            {
                double lat;
                double lon;
                if (GeometryEngine.Instance.Project(geometry, SpatialReferences.WGS84) is MapPoint coord)
                {
                    lon = coord.X;
                    lat = coord.Y;
                    try
                    {
                        OpenStreetView(lat, lon);
                    }
                    catch (Exception ex)
                    {
                        string caption = "Failed to get street view";
                        string message = "Process failed. \n\nSave and restart ArcGIS Pro and try process again.\n\n" +
                            "If problem persist, contact your local GIS nerd.";

                        //Using the ArcGIS Pro SDK MessageBox class
                        MessageBox.Show(message, caption);
                    }
                }
                return true;
            });

        }
    }
}
