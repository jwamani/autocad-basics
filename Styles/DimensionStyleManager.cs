using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace TestDrawing.Styles
{
    public static class DimensionStyleManager
    {
        public static ObjectId GetOrCreate(
            Database database,
            Transaction transaction,
            string name,
            ObjectId textStyleId,
            double textHeight,
            double arrowSize,
            Color textColor,
            Color lineColor)
        {
            DimStyleTable dimStyleTable = (DimStyleTable)transaction.GetObject(database.DimStyleTableId, OpenMode.ForRead);

            if (dimStyleTable.Has(name))
            {
                ObjectId existingId = dimStyleTable[name];
                DimStyleTableRecord existing = (DimStyleTableRecord)transaction.GetObject(existingId, OpenMode.ForWrite);

                existing.Dimtxsty = textStyleId;
                existing.Dimtxt = textHeight;
                existing.Dimasz = arrowSize;
                existing.Dimtsz = arrowSize;
                existing.Dimclrt = textColor;
                existing.Dimclrd = lineColor;
                existing.Dimse1 = false;
                existing.Dimse2 = false;
                existing.DimfxlenOn = true;
                existing.Dimfxlen = 100;
                existing.Dimdle = 100;
                existing.Dimexe = 100;
                existing.Dimlwd = LineWeight.ByLayer;

                return existingId;
            }

            DimStyleTableRecord style = new DimStyleTableRecord()
            {
                Name = name,
                Dimtxsty = textStyleId,
                Dimtxt = textHeight,
                Dimasz = arrowSize,
                Dimtsz = arrowSize,
                Dimclrt = textColor,
                Dimlwd = LineWeight.ByLayer,
                DimfxlenOn = true,
                Dimclrd = lineColor,
                Dimse1 = true,
                Dimse2 = true,
            };

            dimStyleTable.UpgradeOpen();

            ObjectId styleId = dimStyleTable.Add(style);
            transaction.AddNewlyCreatedDBObject(style, true);

            return styleId;
        }
    }
}