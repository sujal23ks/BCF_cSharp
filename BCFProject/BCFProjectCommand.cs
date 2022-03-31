using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace BCFProject
{
    public class BCFProjectCommand : Command
    {
        public BCFProjectCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static BCFProjectCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "AddUserStrings";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //To Select Road Center Lines
            ObjRef[] objrefs;
            Result rc = Rhino.Input.RhinoGet.GetMultipleObjects("Select Cut Layer", false, ObjectType.Curve, out objrefs);
            if (rc != Rhino.Commands.Result.Success)
                return rc;
            if (objrefs == null || objrefs.Length < 1)
                return Rhino.Commands.Result.Failure;

            DateTime today = DateTime.Today;
            string date = today.ToString();
            Result rc1 = Rhino.Input.RhinoGet.GetString("Please Type the deadline ", false, ref date);
            if (rc1 != Rhino.Commands.Result.Success)
                return rc1;

            string Project = null;
            Result rc2 = Rhino.Input.RhinoGet.GetString("Please Type the Project Name ", false, ref Project);
            if (rc2 != Rhino.Commands.Result.Success)
                return rc2;

            string MaterialType = null;
            Result rc3 = Rhino.Input.RhinoGet.GetString("Please Type the Material Type ", false, ref MaterialType);
            if (rc3 != Rhino.Commands.Result.Success)
                return rc3;

            string MaterialTk = null;
            Result rc4 = Rhino.Input.RhinoGet.GetString("Please Type the Material Thickness ", false, ref MaterialTk);
            if (rc4 != Rhino.Commands.Result.Success)
                return rc4;
         
            List<Curve> inCurves = new List<Curve>();
            List<Double> inCurvesUserData = new List<Double>();
            for (int i = 0; i < objrefs.Length; i++)
            {

                objrefs[i].Object().Attributes.SetUserString("deadLine", date);
                objrefs[i].Object().Attributes.SetUserString("projectName", Project);
                objrefs[i].Object().Attributes.SetUserString("materialType", MaterialType);
                objrefs[i].Object().Attributes.SetUserString("materialThickness", MaterialTk);

                var a = objrefs[i].Curve().ToNurbsCurve();

                var tmp = Rhino.Geometry.AreaMassProperties.Compute(a, .001);
                var Area = tmp.Area;

                objrefs[i].Object().Attributes.SetUserString("Area", Area.ToString());

                var layerIndex = doc.Layers.FindByFullPath("Cut", -1);
                Rhino.RhinoDoc thisDoc = Rhino.RhinoDoc.ActiveDoc;
                Rhino.DocObjects.ObjectAttributes myAtt = new Rhino.DocObjects.ObjectAttributes();
                myAtt.LayerIndex = layerIndex;

            }

            return Result.Success;
        }
    }
}
