using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using TestDrawing.Drawing;

namespace TestDrawing.Commands
{
    public class DrawingCommands
    {
        [CommandMethod("TESTDRAWING")]
        public void TestDrawing()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            try
            {
                TestDrawingBuilder.Build(db);
            }
            catch (System.Exception exception)
            {
                doc.Editor.WriteMessage(
                    $"\nTESTDRAWING failed: {exception.GetType().FullName}: {exception.Message}");

                if (exception.InnerException != null)
                {
                    doc.Editor.WriteMessage(
                        $"\nInner exception: {exception.InnerException.GetType().FullName}: " +
                        exception.InnerException.Message);
                }

                doc.Editor.WriteMessage($"\nStack trace:\n{exception}");
                throw;
            }

        }
    }
}
