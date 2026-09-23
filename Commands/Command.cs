using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
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

            TestDrawingBuilder.Build(db);

        }
    }
}
